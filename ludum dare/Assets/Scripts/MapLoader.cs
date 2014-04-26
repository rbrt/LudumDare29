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

    float tileOffset = .32f;

    List<SpriteRenderer> map;

    GameObject playerCharacter;

    List<GameObject> enemies;

	void Start () {
        GameObject mapRoot = new GameObject("Map Root");
        map = new List<SpriteRenderer>();
        enemies = new List<GameObject>();
        var mapText = File.ReadAllLines(filePath + "map1.txt");
        float x = 0,
              y = 0;

        mapText.ToList().ForEach(line => {
            line.ToList().ForEach(character => {
                // 1 = wall
                if (character == '1'){
                    var newSprite = GameObject.Instantiate(wallSprite, new Vector3(x, y, 0), transform.rotation) as SpriteRenderer;
                    newSprite.transform.parent = mapRoot.transform;
                    map.Add(newSprite);
                }
                // 2 = floor
                else if (character == '0'){
                    var newSprite = GameObject.Instantiate(floorSprite, new Vector3(x, y, 0), transform.rotation) as SpriteRenderer;
                    newSprite.transform.parent = mapRoot.transform;
                    map.Add(newSprite);
                }
                // @ = player
                else if(character == '@') {
                    var newSprite = GameObject.Instantiate(floorSprite, new Vector3(x, y, 0), transform.rotation) as SpriteRenderer;
                    newSprite.transform.parent = mapRoot.transform;
                    map.Add(newSprite);
                    var player = GameObject.Instantiate(playerSprite, new Vector3(x, y, 0), transform.rotation) as SpriteRenderer;
                    player.transform.parent = mapRoot.transform;
                    playerCharacter = player.gameObject;
                }
                // 2 == enemy
                else if(character == '2') {
                    var newSprite = GameObject.Instantiate(floorSprite, new Vector3(x, y, 0), transform.rotation) as SpriteRenderer;
                    newSprite.transform.parent = mapRoot.transform;
                    map.Add(newSprite);
                    var enemy = GameObject.Instantiate(enemySprite, new Vector3(x, y, 0), transform.rotation) as SpriteRenderer;
                    enemy.transform.parent = mapRoot.transform;
                    enemies.Add(enemy.gameObject);
                }
                x += tileOffset;
            });
            y += tileOffset;
            x = 0;
        });

        mapRoot.transform.position = new Vector3(-2f, -.12f, 2f);
        enemies.ToList().ForEach(enemy => enemy.transform.parent = null);
        playerCharacter.transform.parent = null;
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
