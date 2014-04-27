using UnityEngine;
using System.Collections;

public class MenuObject : MonoBehaviour {

    public Sprite mainMenuSprite,
                  instructionSprite,
                  winSprite,
                  failSprite;


    public Sprite endSprite;

    SpriteRenderer spriteRenderer;

    enum MenuStates {Main, Instructions, Off, End}
    MenuStates currentState;

    bool blocked = false;

	// Use this for initialization
	void Start () {
        currentState = MenuStates.Main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.renderer.sortingLayerID = 3;
	}
	
    public IEnumerator SetEndSprite(bool success){
        Debug.Log("fired coroutine");
        blocked = true;
        if (success){
            endSprite = winSprite;
        }
        else{
            endSprite = failSprite;
        }
        currentState = MenuStates.End;
        spriteRenderer.sprite = endSprite;
        yield return StartCoroutine(TransitionIn());
        blocked = false;
    }

	// Update is called once per frame
	void Update () {
	    if (currentState == MenuStates.Main){
            if (!blocked){
                spriteRenderer.sprite = mainMenuSprite;
            }
            if (Input.GetKeyDown(KeyCode.Return)){
                if (!blocked){
                    blocked = true;
                    StartCoroutine(TransitionState(MenuStates.Instructions, instructionSprite));
                }
            }
        }
        else if (currentState == MenuStates.Instructions){
            if(Input.GetKeyDown(KeyCode.Return)) {
                currentState = MenuStates.Off;
                GameObject.Find("Map Loader").GetComponent<MapLoader>().StartGame();
                StartCoroutine(TransitionOut());
            }
        }
        else if(currentState == MenuStates.Off) {

        }
        else if(currentState == MenuStates.End) {
            if (Input.GetKeyDown(KeyCode.Return) && !blocked){
                blocked = true;
                Application.LoadLevel("Game");
            }
        }
	}

    IEnumerator TransitionState(MenuStates state, Sprite sprite){

        yield return StartCoroutine(TransitionOut());

        spriteRenderer.sprite = sprite;

        yield return StartCoroutine(TransitionIn());

        currentState = state;
        blocked = false;
    }

    IEnumerator TransitionOut(){
        while(spriteRenderer.renderer.material.color.a > 0) {
            var col = spriteRenderer.renderer.material.color;
            col.a -= .09f;
            spriteRenderer.renderer.material.color = col;
            yield return null;
        }
    }

    IEnumerator TransitionIn(){
        while(spriteRenderer.renderer.material.color.a < 1) {
            var col = spriteRenderer.renderer.material.color;
            col.a += .01f;
            spriteRenderer.renderer.material.color = col;
            yield return null;
        }
    }
}
