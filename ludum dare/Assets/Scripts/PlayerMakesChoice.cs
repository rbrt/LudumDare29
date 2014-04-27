using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerMakesChoice : MonoBehaviour {

    public Camera camera;
    public GameObject enemyPrefab;

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

        var cameraPos = camera.transform.position;
        Vector3 moveToPos = cameraPos;
        Vector3 force = new Vector3();
        moveToPos.z += 2;

        GameObject.Find("FogGenerators").GetComponent<FogGeneratorController>().makeFog = false;

        while (Vector3.Distance(camera.transform.position, moveToPos) > .2f){
            camera.transform.position = Vector3.SmoothDamp(camera.transform.position, moveToPos, ref force, .5f);
            yield return null;
        }

        // if correct target, play right anim

        // otherwise other anim

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

        GameObject.Find("Fade").GetComponent<MeshRenderer>().sortingLayerID = 2;

        // slashes
        yield return StartCoroutine(DrawSlashes());

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
