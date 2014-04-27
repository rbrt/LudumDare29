using UnityEngine;
using System.Collections;
using System.Linq;

public class TargetDisplay : MonoBehaviour {


    public Sprite targetSprite;

    GameObject targetObject;
    Vector3 originalScale;

    void Start(){
        Screen.showCursor = false;
        StartCoroutine(WaitForTarget());
    }

    IEnumerator WaitForTarget(){
        bool found = false;

        while (!found){
            var childNames = from child in GetComponentsInChildren<Transform>() select child.name;
            if (childNames.Any(x => x.ToLower().Contains("clone"))){
                targetObject = GetComponentsInChildren<Transform>().FirstOrDefault(x => x.name.ToLower().Contains("clone")).gameObject;
                originalScale = targetObject.transform.localScale;
                found = true;
            }
            else{
                yield return new WaitForSeconds(1);
            }
        }

        while (true){
            yield return StartCoroutine(Pulse());
        }
    }

    IEnumerator Pulse(){
        Vector3 targetScale;
        Vector3 force = new Vector3();
        if (Vector3.Distance(targetObject.transform.localScale, originalScale) > .1f){
            targetScale = originalScale;
        }
        else{
            targetScale = originalScale * .85f;
        }

        while (Vector3.Distance(targetObject.transform.localScale, targetScale) > .1f){
            targetObject.transform.localScale = Vector3.SmoothDamp(targetObject.transform.localScale, targetScale, ref force, .5f);
            yield return null;
        }
    }
}
