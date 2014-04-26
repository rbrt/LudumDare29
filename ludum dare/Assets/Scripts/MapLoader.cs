using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class MapLoader : MonoBehaviour {

    public SpriteRenderer wallSprite,
                          floorSprite,
                          playerSprite,
                          enemySprite;

    string filePath = "Assets" + Path.DirectorySeparatorChar + "Maps" + Path.DirectorySeparatorChar;

    public struct mapCoordinates{
        public Vector3 position;
        public bool isWall;
        public bool isOccupied;

        public mapCoordinates(Vector3 pos, bool wall, bool occupied){
            position = pos;
            isWall = wall;
            isOccupied = occupied;
        }
    }

    public struct Node {
        public int x;
        public int y;

        public Node(int one, int two) {
            x = one;
            y = two;
        }

    }

    mapCoordinates[,] mapCoords;

    int xLength,
        yLength;

    public mapCoordinates[,] MapCoords{
        get { return mapCoords; }
    }

    float tileOffset = .32f;

    List<SpriteRenderer> map;

    public GameObject playerCharacter;

    List<GameObject> enemies;
    List<Node> allNodes;

	void Start () {
        GameObject mapRoot = new GameObject("Map Root");
        map = new List<SpriteRenderer>();
        allNodes = new List<Node>();
        enemies = new List<GameObject>();
        var mapText = File.ReadAllLines(filePath + "map1.txt");
        int charCount = mapText[0].Length;
        mapCoords = new mapCoordinates[charCount, mapText.Length];
        float x = 0,
              y = 0;

        int countX = 0, countY = 0;
        Vector3 difference = mapRoot.transform.position - new Vector3(-2f, -.12f, 3f);

        mapText.ToList().ForEach(line => {
            line.ToList().ForEach(character => {
                // 1 = wall
                if (character == '1'){
                    var newSprite = GameObject.Instantiate(wallSprite, new Vector3(x, y, 0), transform.rotation) as SpriteRenderer;
                    newSprite.transform.parent = mapRoot.transform;
                    map.Add(newSprite);

                    try {
                        mapCoords[countX, countY] = new mapCoordinates(new Vector3(x, y, 0), true, false);
                    }
                    catch (System.IndexOutOfRangeException e){
                        Debug.Log(mapCoords.Length + " " + countX + " " + countY);
                    }
                }
                // 2 = floor
                else if (character == '0'){
                    var newSprite = GameObject.Instantiate(floorSprite, new Vector3(x, y, 0), transform.rotation) as SpriteRenderer;
                    newSprite.transform.parent = mapRoot.transform;
                    map.Add(newSprite);

                    mapCoords[countX, countY] = new mapCoordinates(new Vector3(x, y, 0), false, false);
                }
                // @ = player
                else if(character == '@') {
                    var newSprite = GameObject.Instantiate(floorSprite, new Vector3(x, y, 0), transform.rotation) as SpriteRenderer;
                    newSprite.transform.parent = mapRoot.transform;
                    map.Add(newSprite);
                    var player = GameObject.Instantiate(playerSprite, new Vector3(x, y, 0), transform.rotation) as SpriteRenderer;
                    player.transform.parent = mapRoot.transform;
                    player.GetComponent<Player>().SetMyCoords(countX, countY);
                    player.GetComponent<Player>().mapLoader = this;
                    playerCharacter = player.gameObject;

                    mapCoords[countX, countY] = new mapCoordinates(new Vector3(x, y, 0), false, true);
                }
                // 2 == enemy
                else if(character == '2') {
                    var newSprite = GameObject.Instantiate(floorSprite, new Vector3(x, y, 0), transform.rotation) as SpriteRenderer;
                    newSprite.transform.parent = mapRoot.transform;
                    map.Add(newSprite);

                    var enemy = GameObject.Instantiate(enemySprite, new Vector3(x, y, 0), transform.rotation) as SpriteRenderer;
                    enemy.transform.parent = mapRoot.transform;
                    enemy.GetComponent<Enemy>().SetMyCoords(countX, countY);
                    enemy.GetComponent<Enemy>().mapLoader = this;
                    enemies.Add(enemy.gameObject);

                    mapCoords[countX, countY] = new mapCoordinates(new Vector3(x, y, 0), false, true);
                }

                mapCoords[countX, countY].position -= difference;
                
                x += tileOffset;
                countX++;
            });
            y += tileOffset;
            x = 0;
            countX = 0;
            countY++;
        });

        mapRoot.transform.position = new Vector3(-2f, -.12f, 3f);
        enemies.ToList().ForEach(enemy => {
            enemy.transform.parent = null;
        });

        xLength = mapText.Length;
        yLength = charCount;

        for(int i = 0; i < charCount; i++){
            for (int j = 0; j < mapText.Length; j++){
                if (!mapCoords[i,j].isWall){
                    allNodes.Add(new Node(i, j));
                }
            }
        }

        playerCharacter.transform.parent = null;

        GameObject.Find("EnemyDecorator").GetComponent<EnemyDecorator>().AdornEnemies(enemies);
	}

    public IEnumerator CheckEnemies(int x, int y){
        GameObject foundEnemy = null;
        enemies.ForEach(enemy => {
            var current = enemy.GetComponent<Enemy>();
            current.canMove = false;
            if (current.x == x && current.y == y){
                Debug.Log(current.x + " " + x);
                foundEnemy = enemy;
            }
        });

        Debug.Log(foundEnemy);
        yield return new WaitForSeconds(10);
        // Kill found enemy


        // Set all enemies to be able to move again and
        // have them come kill you
        enemies.ForEach(enemy => {
            enemy.GetComponent<Enemy>().canMove = true;
            enemy.GetComponent<Enemy>().GoKillPlayer(playerCharacter);
        });

        if (foundEnemy.GetComponent<Enemy>().isTarget){
            Debug.Log("Win!");
        }
        else{
            Debug.Log("Lose!");
        }

        yield break;
    }

   

    public int[] GetNextSquareToPlayer(int x, int y){
        List<Node> nodesInPath = new List<Node>();
        allNodes.ForEach(node => {
            if (!mapCoords[node.x, node.y].isOccupied){
                nodesInPath.Add(node);
            }
        });


        var nextNode = TryNodes(new Node(x, y));
        int[] result = {nextNode.x, nextNode.y};
        return result;
    }

    public Node TryNodes(Node current){
        Node[] nodesToTry = { new Node(current.x - 1, current.y),
                              new Node(current.x, current.y-1),
                              new Node(current.x - 1, current.y - 1),
                              new Node(current.x + 1, current.y),
                              new Node(current.x + 1, current.y + 1),
                              new Node(current.x, current.y+1),
                              new Node(current.x + 1, current.y-1),
                              new Node(current.x -1 , current.y + 1)};


        if (current.x < 0 || current.x > xLength || current.y < 0 || current.y > yLength){
            return new Node(-1,-1);
        }

        if (mapCoords[current.x, current.y].isOccupied && !mapCoords[current.x, current.y].isWall 
            && playerCharacter.GetComponent<Player>().x == current.x && playerCharacter.GetComponent<Player>().y == current.y){
                return current;
        }
        else{
            for (int i = 0; i < nodesToTry.Length; i++){
                Node nextNode = TryNodes(nodesToTry[i]);
                if (nextNode.x != -1){
                    return nextNode;
                }
            }
        }

        return new Node(-1, -1);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
