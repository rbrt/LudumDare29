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

        public bool markedForAI;

        public mapCoordinates(Vector3 pos, bool wall, bool occupied){
            position = pos;
            isWall = wall;
            isOccupied = occupied;
            markedForAI = false;
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

    public int xLength,
               yLength;

    public mapCoordinates[,] MapCoords{
        get { return mapCoords; }
    }

    float tileOffset = .32f;

    List<SpriteRenderer> map;

    public GameObject playerCharacter;

    public List<GameObject> enemies;
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

        // Kill found enemy
        yield return StartCoroutine(GameObject.Find("PlayerMakesChoice").GetComponent<PlayerMakesChoice>().StartSequence(foundEnemy.GetComponent<Enemy>().isTarget,
                                    playerCharacter,
                                    foundEnemy,
                                    enemies));

        if (foundEnemy.GetComponent<Enemy>().isTarget){
            Debug.Log("Win!");
        }
        else{
            Debug.Log("Lose!");
        }

        yield break;
    }

	
	// Update is called once per frame
	void Update () {
	
	}
}
