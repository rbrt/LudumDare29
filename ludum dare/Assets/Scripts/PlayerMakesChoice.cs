using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerMakesChoice : MonoBehaviour {

    public Camera camera;
    public GameObject enemyPrefab;

    public Transform playerDeathLocation,
                     targetDeathLocation;

    public SpriteRenderer spriteRenderer;
    public Sprite itsYou,
                  vengeance,
                  whatHaveIDone;


    GameObject player,
               targetEnemy;

    List<GameObject> enemies;

    public Fade fade;

    public GameObject LineRendererObject;
    List<LineRenderer> lineRenderers;

    void Start(){
        lineRenderers = LineRendererObject.GetComponentsInChildren<LineRenderer>().ToList();
        lineRenderers.ForEach(x => {
            x.sortingLayerID =2;
        });
        ResetPosition();
    }
    

    public IEnumerator StartSequence(bool correctTarget, GameObject p, GameObject e, List<GameObject> es){
        MapLoader mapLoader = GameObject.Find("Map Loader").GetComponent<MapLoader>();
        player = p;
        targetEnemy = e;
        enemies = es;

        Camera.main.GetComponent<CameraFollowsPlayer>().follow = false;

        var cameraPos = camera.transform.position;
        Vector3 moveToPos = cameraPos;
        Vector3 force = new Vector3();
        Vector3 force2 = new Vector3();
        Vector3 force3 = new Vector3();
        moveToPos.z += 2;

        Vector3 enemyPosition = targetDeathLocation.position;
        Vector3 playerPosition = playerDeathLocation.position;

        GameObject.Find("FogGenerators").GetComponent<FogGeneratorController>().makeFog = false;

        // Move player and enemy into position, and zoom in
        while (Vector3.Distance(camera.transform.position, moveToPos) > .2f){
            camera.transform.position = Vector3.SmoothDamp(camera.transform.position, moveToPos, ref force, .5f);
            yield return null;
        }


        while(Vector3.Distance(player.transform.position, playerPosition) > .1f ||
              Vector3.Distance(targetEnemy.transform.position, enemyPosition) > .1f) {
                  player.transform.position = Vector3.SmoothDamp(player.transform.position, playerPosition, ref force2, .5f);
                  targetEnemy.transform.position = Vector3.SmoothDamp(targetEnemy.transform.position, enemyPosition, ref force3, .5f);
            yield return null;
        }

        yield return StartCoroutine(DisplayDialogue(correctTarget));

        // Kill Knight
        yield return StartCoroutine(targetEnemy.GetComponent<Enemy>().Die());

        yield return StartCoroutine(fade.FadeIn());

        // Set all enemies to be able to move again and
        // have them come kill you
        enemies.ForEach(enemy => {
            if (enemy != targetEnemy){
                enemy.GetComponent<Enemy>().canMove = true;
                enemy.GetComponent<Enemy>().GoKillPlayer(player);
            }
        });

        Camera.main.GetComponent<CameraFollowsPlayer>().follow = true;

        bool far = true;
        while (far){
            far = false;
            enemies.Where(x => x != targetEnemy).ToList().ForEach(x => {
                if (x.GetComponent<Enemy>().FarFromPlayer()){
                    far = true;
                }
            });

            yield return null;
        }

        yield return StartCoroutine(FadeEnemies(enemies));
        

        GameObject.Find("Fade").GetComponent<MeshRenderer>().sortingLayerID = 2;

        // slashes
        yield return StartCoroutine(DrawSlashes());
        var col = player.GetComponent<Player>().spriteRenderer.material.color;
        col.a = 1;
        player.GetComponent<Player>().spriteRenderer.material.color = col;
        player.GetComponent<Player>().SetPlayerDead();

        for(int i = enemies.Count-1; i >= 0; i-- ){
            if (enemies[i] != targetEnemy){
                Destroy(enemies[i]);
            }
        }

            yield return StartCoroutine(fade.FadeOut());

        
        force = new Vector3();
        while(Vector3.Distance(camera.transform.position, cameraPos) > .2f) {
            camera.transform.position = Vector3.SmoothDamp(camera.transform.position, cameraPos, ref force, .5f);
            yield return null;
        }

        GameObject.Find("FogGenerators").GetComponent<FogGeneratorController>().makeFog = true;
    }

    void ResetPosition(){
        lineRenderers.ForEach(x => {
            x.SetPosition(0, Vector3.zero);
            x.SetPosition(1, Vector3.zero);
            x.SetPosition(2, Vector3.zero);
            x.SetPosition(3, Vector3.zero);
        });
    }

    IEnumerator FadeEnemies(List<GameObject> enemies){
        bool done = false;
        while (!done){
            done = true;
            for (int i = 0; i < enemies.Count; i++){
                var col = enemies[i].GetComponent<Enemy>().spriteRenderer.material.color;
                if (enemies[i] != targetEnemy && col.a > 0f){
                    done = false;
                    col.a -= .05f;
                    enemies[i].GetComponent<Enemy>().spriteRenderer.material.color = col;
                }
            }
            var color = player.GetComponent<Player>().spriteRenderer.material.color;
            if (color.a > 0){
                done = false;
                color.a -= .05f;
                player.GetComponent<Player>().spriteRenderer.material.color = color;
            }
            yield return null;
        }
    }

    IEnumerator DisplayDialogue(bool correctChoice){
        spriteRenderer.sortingLayerID = 2;
        Vector3 speechPos = playerDeathLocation.position;
        Vector3 speechScale = transform.localScale;
        speechScale.x = .5f;
        speechScale.y = .5f;

        transform.localScale = speechScale;

        speechPos.y += .65f;
        speechPos.x -= .25f;
        speechPos.z = 2.6f;
        transform.position = speechPos;
        spriteRenderer.sprite = vengeance;
        yield return new WaitForSeconds(2);

        speechPos = targetDeathLocation.position;
        speechPos.y += .65f;
        speechPos.x -= .6f;
        speechPos.z = 2.6f;
        transform.position = speechPos;
        
        if (correctChoice){
            spriteRenderer.sprite = itsYou;
        }
        else{
            spriteRenderer.sprite = whatHaveIDone;
        }
        yield return new WaitForSeconds(2);
        spriteRenderer.enabled = false;
    }

    IEnumerator DrawSlashes(){
        for (int i = 0; i < lineRenderers.Count; i++){
            StartCoroutine(DrawSlash(lineRenderers[i]));
            yield return new WaitForSeconds(.1f);
        }
        yield return new WaitForSeconds(1f);
        StartCoroutine(fadeSlashes());
    }

    IEnumerator fadeSlashes(){
        for(int i = 0; i < lineRenderers.Count; i++) {
            lineRenderers[i].enabled = false;
            yield return null;
        }
    }

    IEnumerator DrawSlash(LineRenderer lineRenderer){
        float upperBound = .5f;
        float lowerBound = 0;

        int xSign = 1,
            ySign = 1;
        if (Random.Range(0,100) > 50){
            xSign = -1;
        }
        if (Random.Range(0,100) > 50){
            ySign = -1;
        }

        Vector3 playerPos = player.transform.position;
        Vector3 start = new Vector3(playerPos.x + Random.Range(lowerBound, upperBound) * xSign,
                                    playerPos.y + Random.Range(lowerBound, upperBound) * ySign,
                                    camera.transform.position.z + .5f);

        xSign *= -1;
        ySign = 1;
        if(Random.Range(0, 100) > 50) {
            ySign = -1;
        }

        Vector3 end = new Vector3(playerPos.x + Random.Range(lowerBound, upperBound) * xSign,
                                  playerPos.y + Random.Range(lowerBound, upperBound) * ySign,
                                  camera.transform.position.z + .5f);

        Vector3 mid1 = Vector3.Lerp(start, end, .3f);
        Vector3 mid2 = Vector3.Lerp(start, end, .7f);
        Vector3[] current = {start,
                             start,
                             start,
                             start};

        lineRenderer.SetPosition(0, current[0]);
        lineRenderer.SetPosition(1, current[1]);
        lineRenderer.SetPosition(2, current[2]);
        lineRenderer.SetPosition(3, current[3]);

        while (Vector3.Distance(current[0], end) > .05f){
            current[1] = Vector3.Lerp(current[1], mid1, .5f);
            current[2] = Vector3.Lerp(current[2], mid2, .5f);
            current[3] = Vector3.Lerp(current[3], end, .5f);

            lineRenderer.SetPosition(1, current[1]);
            lineRenderer.SetPosition(2, current[2]);
            lineRenderer.SetPosition(3, current[3]);
            yield return null;
        }
    }

}
