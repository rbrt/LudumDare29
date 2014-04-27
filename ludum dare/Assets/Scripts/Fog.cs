using UnityEngine;
using System.Collections;

public class Fog : MonoBehaviour {
    public Sprite[] fogSprites;
    public SpriteRenderer spriteRenderer;

    float offset = .1f;

    
    public IEnumerator AnimateFog(){
        while (true){
            yield return StartCoroutine(MakeFog());
        }
    }

    IEnumerator MakeFog(){
        Quaternion originalRotation = transform.rotation;
        Vector3 originalScale = transform.localScale;
        Vector3 originalPos = transform.localPosition;
        Vector3 force1 = new Vector3(),
                force2 = new Vector3();

        var color = spriteRenderer.color;
        color.a = 0;
        spriteRenderer.color = color;

        transform.localScale = new Vector3(Random.Range(.2f, .8f),
                                           Random.Range(.2f, .8f), 
                                           transform.localScale.z);
        
        transform.localPosition = new Vector3(originalPos.x + Random.Range(-offset, offset),
                                              originalPos.y + Random.Range(-offset, offset),
                                              originalPos.z);
        
        transform.Rotate(0, 0, Random.Range(0,180));

        spriteRenderer.sprite = fogSprites[Random.Range(0, fogSprites.Length - 1)];


        while (color.a < .8f){
            if(Vector3.Distance(originalScale, transform.localScale) > .1f) {
                transform.localScale = Vector3.SmoothDamp(transform.localScale, originalScale, ref force1, 1f);
            }
            else {
                originalScale = new Vector3(Random.Range(.2f, .8f),
                                            Random.Range(.2f, .8f),
                                            transform.localScale.z);
            }

            if(Vector3.Distance(originalPos, transform.localPosition) > .1f) {
                transform.localPosition = Vector3.SmoothDamp(transform.localPosition, originalPos, ref force2, 2f);
            }
            else {
                originalPos = new Vector3(transform.localPosition.x + Random.Range(-offset, offset),
                                          transform.localPosition.y + Random.Range(-offset, offset),
                                          transform.localPosition.z);
            }

            if(Quaternion.Angle(originalRotation, transform.rotation) > 0) {
                transform.Rotate(0, 0, -.25f);
            }

            color.a += .05f;
            spriteRenderer.color = color;
            yield return new WaitForSeconds(.025f);
        }


        while (color.a >= 0){
            if(Vector3.Distance(originalScale, transform.localScale) > .1f) {
                transform.localScale = Vector3.SmoothDamp(transform.localScale, originalScale, ref force1, 1f);
            }
            else {
                originalScale = new Vector3(Random.Range(.2f, .8f),
                                            Random.Range(.2f, .8f),
                                            transform.localScale.z);
            }

            if(Vector3.Distance(originalPos, transform.localPosition) > .1f) {
                transform.localPosition = Vector3.SmoothDamp(transform.localPosition, originalPos, ref force2, 2f);
            }
            else {
                originalPos = new Vector3(transform.localPosition.x + Random.Range(-offset, offset),
                                          transform.localPosition.y + Random.Range(-offset, offset),
                                          transform.localPosition.z);
            }

            if(Quaternion.Angle(originalRotation, transform.rotation) > 0) {
                transform.Rotate(0, 0, -.25f);
            }

            color.a-=.005f;
            spriteRenderer.color = color;
            yield return new WaitForSeconds(.025f);
        
        }

        Destroy(gameObject);
    }

}
