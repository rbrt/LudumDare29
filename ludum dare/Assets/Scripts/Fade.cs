using UnityEngine;
using System.Collections;

public class Fade : MonoBehaviour {

    Material mat;

	void Start () {
        mat = GetComponent<MeshRenderer>().material;
	}

    public IEnumerator FadeIn(){
        while (mat.color.a < 255){
            var color = mat.color;
            color.a += 5;
            mat.color = color;
            yield return null;
        }
    }

    public IEnumerator FadeOut(){
        while(mat.color.a > 0) {
            var color = mat.color;
            color.a -= 5;
            mat.color = color;
            yield return null;
        }
    }
	
}
