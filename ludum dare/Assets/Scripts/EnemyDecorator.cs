using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyDecorator : MonoBehaviour {

    List<GameObject> enemies;

    public GameObject targetIndicator;


    public void AdornEnemies(List<GameObject> enemyList){
        enemies = enemyList;
        int targetIndex = Random.Range(0, enemies.Count);

        enemies.ForEach(enemy => {
            if (enemies.IndexOf(enemy) == targetIndex){
                enemy.GetComponent<Enemy>().isTarget = true;
            }

            AdornEnemy(enemy);
        });
    }

    // Attach a bunch of shit to enemy at random. If it's the target, store the list somewhere
    // so it can be displayed to player.
    public void AdornEnemy(GameObject enemy){
        if (enemy.GetComponent<Enemy>().isTarget){
            var pos = enemy.transform.position;
            pos.z -= 1;
            var indicator = GameObject.Instantiate(targetIndicator, pos, enemy.transform.rotation) as GameObject;
            Destroy(indicator.GetComponent<Player>());
            indicator.transform.localScale = new Vector3(.3f, .3f, .3f);
            indicator.transform.parent = enemy.transform;
            
        }
    }


}
