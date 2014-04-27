using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class MapLoader : MonoBehaviour {

    public GameObject     playerSprite,
                          enemySprite;


    public SpriteRenderer[] floorsprites;
    public SpriteRenderer[] wallsprites;

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

	public void StartGame () {
        GameObject mapRoot = new GameObject("Map Root");
        GameObject.Find("TargetDisplay").GetComponent<SpriteRenderer>().enabled = true;

        GameObject.Find("FogGenerators").GetComponent<FogGeneratorController>().makeFog = true;
        StartCoroutine(GameObject.Find("FogGenerators").GetComponent<FogGeneratorController>().KickOffGenerators());
        map = new List<SpriteRenderer>();
        allNodes = new List<Node>();
        enemies = new List<GameObject>();
        var mapText = mapstring.Split('\n').ToList();
        int charCount = mapText[0].Length;
        mapCoords = new mapCoordinates[charCount, mapText.Count];
        float x = 0,
              y = 0;

        int countX = 0, countY = 0;
        Vector3 difference = mapRoot.transform.position - new Vector3(-2f, -.12f, 3f);

        

        mapText.ToList().ForEach(line => {
            line.ToList().ForEach(character => {
                
                int val = Random.Range(0,100);
                
                // 1 = wall
                if (character == '1'){
                    int index = Random.Range(0, wallsprites.Length - 1);
                    var newSprite = GameObject.Instantiate(wallsprites[index], new Vector3(x, y, 0), transform.rotation) as SpriteRenderer;
                    newSprite.transform.parent = mapRoot.transform;
                    map.Add(newSprite);

                    mapCoords[countX, countY] = new mapCoordinates(new Vector3(x, y, 0), true, false);
                }
                // 2 = floor
                else if (character == '0'){
                    int index = Random.Range(0, floorsprites.Length-1);
                    var newSprite = GameObject.Instantiate(floorsprites[index], new Vector3(x, y, 0), transform.rotation) as SpriteRenderer;
                    if (val < 25){
                        newSprite.transform.Rotate(Vector3.forward, 90);
                    }
                    else if (val < 50){
                        newSprite.transform.Rotate(Vector3.forward, 180);
                    }
                    else if (val < 75){
                        newSprite.transform.Rotate(Vector3.forward, 270);
                    }
                    newSprite.transform.parent = mapRoot.transform;
                    map.Add(newSprite);

                    mapCoords[countX, countY] = new mapCoordinates(new Vector3(x, y, 0), false, false);
                }
                // @ = player
                else if(character == '@') {
                    int index = Random.Range(0, floorsprites.Length - 1);
                    var newSprite = GameObject.Instantiate(floorsprites[index], new Vector3(x, y, 0), transform.rotation) as SpriteRenderer;
                    newSprite.transform.parent = mapRoot.transform;
                    map.Add(newSprite);
                    var player = GameObject.Instantiate(playerSprite, new Vector3(x, y, 0), transform.rotation) as GameObject;
                    player.transform.parent = mapRoot.transform;
                    player.GetComponent<Player>().SetMyCoords(countX, countY);
                    player.GetComponent<Player>().mapLoader = this;
                    playerCharacter = player.gameObject;

                    mapCoords[countX, countY] = new mapCoordinates(new Vector3(x, y, 0), false, true);
                }
                // 2 == enemy
                else if(character == '2') {
                    int index = Random.Range(0, floorsprites.Length-1);
                    var newSprite = GameObject.Instantiate(floorsprites[index], new Vector3(x, y, 0), transform.rotation) as SpriteRenderer;
                    newSprite.transform.parent = mapRoot.transform;
                    map.Add(newSprite);

                    var enemy = GameObject.Instantiate(enemySprite, new Vector3(x, y, 0), transform.rotation) as GameObject;
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

        xLength = mapText.Count;
        yLength = charCount;

        for(int i = 0; i < charCount; i++){
            for (int j = 0; j < mapText.Count; j++){
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


    string mapstring = @"111111111111111111111111111111111111111111111111111111111111111\n
100000000000100000000000000000000000000000000000000000000000001\n
100000@00000100000000000000000000000000000020000000000000000001\n
100000200000100000000000000000000000000000000000000001000000001\n
100000000000111111111111111111111111111100000000000001000200001\n
111110001111100000000000000000000001100000000000000001000000001\n
100000000000100000000000000000000001100000000000000001000000001\n
100000000000100000000002000000000001100000000000000001000000001\n
100000000000100000000000000000000001100000000000000001000000001\n
100000000000000000000000000000000001100000000000000001000000001\n
100000000000000002000000000000000001100000000000000001000000001\n
100000000000000000000010000000000001100000020000000001000000001\n
100000000000000000000010000000000001100000000000000001000000001\n
111111111110000000000010222000000001100000000111111111111111111\n
100000000010000000000010000000000000000000000100000000000000001\n
100000000010000000000010000000020000000000000100000000000000001\n
100001111110200002000011111110000000000000000100000000000000001\n
100000000000000000000010000000000000000000000100000000000000001\n
100000000000000000000010000000000000000000000100000000000000001\n
100000000000000000000010000000000000000000000100000000000000001\n
100000000000000000000010000002000020000000000100000000000000001\n
100000000100000000000000000000000000000001000100000000200000001\n
100000000000000000000000000000000000000000000100000000000000001\n
100000222000000000000000000000000000000000000100000111100000001\n
100000000000000000000000000000000000000000000100000111100000001\n
100000000111111111111111111111111110000000000100000111100000001\n
100000000111111111111111111111111110000000000100000111100000001\n
100000000110000000000000000000000000000000000100000000000000001\n
100000000110000000000000000000000000000000000000000000000000001\n
100000000110000000002000000000000000000000000020000000000000001\n
100000000110000000000000000000000002000000000000000000000000001\n
100000000110000000000000000000000000000000000000000000000000001\n
100000000110000000000000000000000000000001000000000000000000001\n
100000000000000000000022111020000000000000000000000000000000001\n
100000000000000000000000000000000000000000000000000000000000001\n
111111111111111111111111111111111111111111111111111111111111111\n";

}
