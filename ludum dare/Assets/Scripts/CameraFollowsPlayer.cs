using UnityEngine;
using System.Collections;

public class CameraFollowsPlayer : MonoBehaviour {

    public GameObject player;
    Vector3 force;

	void Start () {
        StartCoroutine(FindPlayer());
	}
	
	void Update () {
        if (player != null){
            Vector3 pos = transform.position;
            pos = Vector3.SmoothDamp(pos, player.transform.position, ref force, .4f);
            pos.z = transform.position.z;
            transform.position = pos;
        }
	}

    IEnumerator FindPlayer(){
        while (GameObject.Find("PlayerSprite(Clone)") == null){
            yield return null;
        }

        player = GameObject.Find("PlayerSprite(Clone)");
    }
}
