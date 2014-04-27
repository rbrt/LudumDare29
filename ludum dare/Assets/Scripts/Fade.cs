using UnityEngine;
using System.Collections;
using System.Linq;

public class Fade : MonoBehaviour {

    Material mat;

	void Start () {
        mat = GetComponent<MeshRenderer>().material;
        var edge = Camera.main.camera.GetComponentsInChildren<Transform>().Where(x => x.name.Equals("edge")).First();
        edge.GetComponent<MeshRenderer>().sortingLayerID = 4;

	}

    public IEnumerator FadeIn(){
        while (mat.color.a < 1){
            var color = mat.color;
            color.a += .005f;
            mat.color = color;
            yield return null;
        }
    }

    public IEnumerator FadeOut(){
        while(mat.color.a > 0.2f) {
            var color = mat.color;
            color.a -= .005f;
            mat.color = color;
            yield return null;
        }
    }
	
}
