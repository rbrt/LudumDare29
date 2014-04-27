using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyDecorator : MonoBehaviour {

    List<GameObject> enemies;

    public GameObject targetIndicator;

    public Sprite[] capes,
                    robes,
                    banners;

    int[] targetPermutation;

    public void AdornEnemies(List<GameObject> enemyList){
        enemies = enemyList;
        int targetIndex = Random.Range(0, enemies.Count);

        targetPermutation = new int[3];
        targetPermutation[0] = Random.Range(0, capes.Length+2);
        targetPermutation[1] = Random.Range(0, robes.Length+2);
        targetPermutation[2] = Random.Range(0, banners.Length+2);

        enemies.ForEach(enemy => {
            if (enemies.IndexOf(enemy) == targetIndex){
                enemy.GetComponent<Enemy>().isTarget = true;
            }

            StartCoroutine(AdornEnemy(enemy));
        });
    }

    // Attach a bunch of shit to enemy at random. If it's the target, store the list somewhere
    // so it can be displayed to player.
    public IEnumerator AdornEnemy(GameObject enemy){

        var robeRenderer = CreateNewGameObjectWithRenderer("robe", enemy.transform);
        var bannerRenderer = CreateNewGameObjectWithRenderer("banner", enemy.transform);
        var capeRenderer = CreateNewGameObjectWithRenderer("cape", enemy.transform);
        

        if (enemy.GetComponent<Enemy>().isTarget){
            if (targetPermutation[0] >= capes.Length){
            }
            else{
                robeRenderer.sprite = robes[targetPermutation[0]];
                robeRenderer.sortingLayerID = 3;
            }
            if(targetPermutation[1] >= capes.Length) {
            }
            else {
                capeRenderer.sprite = capes[targetPermutation[1]];
                capeRenderer.sortingLayerID = 1;
            }
            if(targetPermutation[2] >= banners.Length) {
            }
            else {
                bannerRenderer.sprite = banners[targetPermutation[2]];
                var pos = bannerRenderer.transform.position;
                pos.y += .4f;
                bannerRenderer.transform.position = pos;
                bannerRenderer.sortingLayerID = 1;
            }

            var display = GameObject.Find("TargetDisplay");
            GameObject copy = GameObject.Instantiate(enemy, display.transform.position, transform.rotation) as GameObject;
            Destroy(copy.GetComponent<Enemy>());
            copy.GetComponentsInChildren<SpriteRenderer>().ToList().ForEach(x => {
                if (x.name.ToLower().Contains("banner") || x.name.ToLower().Contains("cape")){
                    x.sortingLayerID = 5;
                }
                else{
                    x.sortingLayerID = 6;
                }
            });
            copy.transform.parent = display.transform;
            var position = copy.transform.position;
            position.y += .4f;
            copy.transform.position = position;

            copy.transform.localScale = copy.transform.localScale * 1.6f;
        }
        else{
            int[] permutation = {Random.Range(0, robes.Length+2),
                                 Random.Range(0, capes.Length+2),
                                 Random.Range(0, banners.Length+2)};

            while (permutation[0] == targetPermutation[0] && permutation[1] == targetPermutation[1] && 
                   permutation[2] == targetPermutation[2]){
                        permutation[0] = Random.Range(0, robes.Length+2);
                        permutation[1] = Random.Range(0, capes.Length+2);
                        permutation[2] = Random.Range(0, banners.Length+2);
                                       
                        yield return null;
            }

            if(permutation[0] >= capes.Length) {
            }
            else {
                robeRenderer.sprite = robes[permutation[0]];
                robeRenderer.sortingLayerID = 3;
            }
            if(permutation[1] >= capes.Length) {
            }
            else {
                capeRenderer.sprite = capes[permutation[1]];
                capeRenderer.sortingLayerID = 1;
            }
            if(permutation[2] >= banners.Length) {
            }
            else {
                bannerRenderer.sprite = banners[permutation[2]];
                var pos = bannerRenderer.transform.position;
                pos.y += .4f;
                bannerRenderer.transform.position = pos;
                bannerRenderer.sortingLayerID = 1;
            }
        
        }
    }

    SpriteRenderer CreateNewGameObjectWithRenderer(string name, Transform enemy){
        GameObject robe = new GameObject(name);
        var robeRenderer = robe.AddComponent<SpriteRenderer>();
        robe.transform.parent = enemy;
        var pos = enemy.position;
        pos.y += .2f;
        robe.transform.position = pos;
        return robeRenderer;
    }


}
