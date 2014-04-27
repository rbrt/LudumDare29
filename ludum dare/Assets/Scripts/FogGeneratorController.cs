using UnityEngine;
using System.Collections;
using System.Linq;

public class FogGeneratorController : MonoBehaviour {

    Fog[] fogGenerators;
    Player player;

    public bool makeFog = true;

    public GameObject fogPrefab;
    float offset = 2f;

	public IEnumerator KickOffGenerators(){
        for(int i = 0; i < 15; i++) {
            Vector3 newPos = new Vector3(transform.position.x + Random.Range(-offset, offset),
                                         transform.position.y + Random.Range(-offset, offset),
                                         transform.position.z);
            var fog = GameObject.Instantiate(fogPrefab, newPos, transform.rotation) as GameObject;
            StartCoroutine(fog.GetComponent<Fog>().AnimateFog());
        }

        if (makeFog){
            while (player == null){
                if(GameObject.Find("PlayerSprite(Clone)") != null){
                    player = GameObject.Find("PlayerSprite(Clone)").GetComponent<Player>();
                }
                yield return null;
            }
            while (true){
                if (makeFog){
                    for (int i = 0; i < 3; i++){
                        Vector3 newPos = new Vector3();
                        if(player.isMoving) {
                            if(player.isHoldingDown) {
                                newPos = new Vector3(transform.position.x + Random.Range(-offset, offset),
                                                     transform.position.y + Random.Range(-offset, -offset*2),
                                                     transform.position.z);
                            }
                            if(player.isHoldingLeft) {
                                newPos = new Vector3(transform.position.x + Random.Range(-offset, -offset*2),
                                                     transform.position.y + Random.Range(-offset, offset),
                                                     transform.position.z);
                            }
                            if(player.isHoldingRight) {
                                newPos = new Vector3(transform.position.x + Random.Range(offset, offset*2),
                                                     transform.position.y + Random.Range(-offset, offset),
                                                     transform.position.z);
                            }
                            if(player.isHoldingUp) {
                                newPos = new Vector3(transform.position.x + Random.Range(-offset, offset),
                                                     transform.position.y + Random.Range(offset, offset*2),
                                                     transform.position.z);
                            }
                        }
                        else{
                            newPos = new Vector3(transform.position.x + Random.Range(-offset, offset),
                                                 transform.position.y + Random.Range(-offset, offset),
                                                 transform.position.z);
                        }
                        var fog = GameObject.Instantiate(fogPrefab, newPos, transform.rotation) as GameObject;
                        StartCoroutine(fog.GetComponent<Fog>().AnimateFog());
                    }

                    if (!player.isMoving){
                        yield return new WaitForSeconds(Random.Range(.3f, .7f));
                    }
                    else{
                        yield return new WaitForSeconds(Random.Range(.08f, .4f));
                    }
                }
                else{
                    yield return null;
                }
            }
        }
    }
}
