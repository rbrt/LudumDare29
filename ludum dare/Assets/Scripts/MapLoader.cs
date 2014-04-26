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

    mapCoordinates[,] mapCoords;

    public mapCoordinates[,] MapCoords{
        get { return mapCoords; }
    }

    float tileOffset = .32f;

    List<SpriteRenderer> map;

    GameObject playerCharacter;

    List<GameObject> enemies;

	void Start () {
        GameObject mapRoot = new GameObject("Map Root");
        map = new List<SpriteRenderer>();
        enemies = new List<GameObject>();
        var mapText = File.ReadAllLines(filePath + "map1.txt");
        int charCount = mapText[0].Length;
        mapCoords = new mapCoordinates[charCount, mapText.Length];
        float x = 0,
              y = 0;

        int countX = 0, countY = 0;
        Vector3 difference = mapRoot.transform.position - new Vector3(-2f, -.12f, 2f);

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

        Debug.Log(difference);

        mapRoot.transform.position = new Vector3(-2f, -.12f, 2f);
        enemies.ToList().ForEach(enemy => {
            enemy.transform.parent = null;
        });

        

        playerCharacter.transform.parent = null;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}