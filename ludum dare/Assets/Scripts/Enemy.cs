using UnityEngine;
using System.Collections;
using System.Linq;

public class Enemy : MonoBehaviour {

    public MapLoader mapLoader;
    GameObject playerCharacter;

    public SpriteRenderer spriteRenderer;

    public Sprite[] frontAnimations;
    public Sprite[] deathAnimations;

    public bool isTarget = false,
                canMove = true;

    public int x,
               y;

    enum Behaviours {Walk, Turn, Stand, Kill};
    Behaviours currentBehaviour;

    enum Directions { Up, Down, Left, Right };
    Directions currentDirection;

    int actionCount = 0;
    bool isBehaving = false,
         inKillPosition = false,
         isWalking = false;

	// Use this for initialization
	void Start () {
        spriteRenderer.sprite = frontAnimations[0];

        currentBehaviour = (Behaviours)Random.Range(0, 2);
        currentDirection = (Directions)Random.Range(0, 3);
        actionCount = Random.Range(0, 5);

        if (!mapLoader){
            mapLoader = GameObject.Find("Map Loader").GetComponent<MapLoader>();
        }

        playerCharacter = GameObject.Find("PlayerSprite(Clone)");
       
	}

    public void SetMyCoords(int newX, int newY){
        x = newX;
        y = newY;
    }

    public void SetMeToKill(){
        currentBehaviour = Behaviours.Kill;
    }

	// Update is called once per frame
	void Update () {
        if (canMove){
            if (!isBehaving){
                StartCoroutine(AdvanceBehaviour());
                isBehaving = true;
            }
        }
        else{
            actionCount = 0;
        }
	}

    bool CheckIfCanMove(int x, int y){
        try{
            if (mapLoader.MapCoords[x,y].isOccupied || mapLoader.MapCoords[x,y].isWall){
                return false;
            }
        }
        catch (System.IndexOutOfRangeException e){
            return false;
        }

        return true;
    }

    public void GoKillPlayer(GameObject player){
        player = playerCharacter;
        currentBehaviour = Behaviours.Kill;
        actionCount = 1;
    }

    IEnumerator AdvanceBehaviour(){
        if (actionCount <= 0 && currentBehaviour != Behaviours.Kill){
            actionCount = Random.Range(1, 5);
            currentBehaviour = (Behaviours)Random.Range(0, 2);
        }
        
        while (actionCount > 0){
            switch (currentBehaviour){
                case Behaviours.Walk: 
                    yield return StartCoroutine(Walk());
                    break;
                case Behaviours.Turn:
                    yield return StartCoroutine(Turn());
                    break;
                case Behaviours.Stand: 
                    yield return StartCoroutine(Stand());
                    break;
                case Behaviours.Kill:
                    yield return StartCoroutine(Kill());
                    break;
            }
        }
        
        isBehaving = false;
    }

    IEnumerator Walk(){
        // if up - y++
        // if left x--
        // if right x++
        // if down y--

        isWalking = true;
        StartCoroutine(AnimateWalk());

        if (currentDirection == Directions.Up && CheckIfCanMove(x, y + 1)){
            
            yield return StartCoroutine(moveToSquare(mapLoader.MapCoords[x, y+1].position,
                                                     x,
                                                     y+1));
        }
        else if(currentDirection == Directions.Down && CheckIfCanMove(x, y - 1)){
            yield return StartCoroutine(moveToSquare(mapLoader.MapCoords[x, y - 1].position,
                                        x,
                                        y-1));
        }
        else if(currentDirection == Directions.Left && CheckIfCanMove(x - 1, y)){
            yield return StartCoroutine(moveToSquare(mapLoader.MapCoords[x-1, y].position,
                                        x-1,
                                        y));
        }
        else if(currentDirection == Directions.Right && CheckIfCanMove(x + 1, y)){
            yield return StartCoroutine(moveToSquare(mapLoader.MapCoords[x+1, y].position,
                                        x+1,
                                        y));
        }
        else{
            currentBehaviour = Behaviours.Turn;
        }
        isWalking = false;
    }

    IEnumerator AnimateWalk(){
        int lastFrame = 0;

        while(isWalking) {
            if (lastFrame < frontAnimations.Length){
                spriteRenderer.sprite = frontAnimations[lastFrame];
                lastFrame++;
            }
            else{
                lastFrame = 0;
            }

            yield return new WaitForSeconds(.5f);
        }
        spriteRenderer.sprite = frontAnimations[0];
    }

    IEnumerator moveToSquare(Vector3 square, int newX, int newY){
        Vector3 force = new Vector3();

        while (Vector3.Distance(transform.position, square) > .1f){
            transform.position = Vector3.SmoothDamp(transform.position, square, ref force, .5f);
            yield return null;
        }

        actionCount--;
        mapLoader.MapCoords[x, y].isOccupied = false;

        x = newX;
        y = newY;

        mapLoader.MapCoords[x, y].isOccupied = true;
    }

    IEnumerator Turn(){
        int dir = 0;
        
        if ((int)currentDirection == 0){
            dir = Random.Range(1, 3);
        }
        else if ((int)currentDirection == 3){
            dir = Random.Range(0, 2);
        }
        else{
            dir = Random.Range(0, 100) > 50 ? 0 : 3;
        }
        currentDirection = (Directions)dir;
        
        // Rotate Sprite

        actionCount = 0;
        yield break;
    }

    IEnumerator Stand(){
        while(actionCount > 0){
            yield return new WaitForSeconds(1.0f);
            actionCount--;
        }
    }

    IEnumerator Kill(){
        Vector3 force = new Vector3();
        int offsetX = Random.Range(-1, 1);
        int offsetY = Random.Range(-1, 1);
        // run at player

        var pos = playerCharacter.transform.position;
        while (Vector3.Distance(transform.position, pos) > .5f){
            pos = playerCharacter.transform.position;
            pos.x += offsetX;
            pos.y += offsetY;

            transform.position = Vector3.SmoothDamp(transform.position,
                                                    pos,
                                                    ref force,
                                                    1f);
            yield return null;
        }

        //start doing attack animation



        inKillPosition = true;


    }

    public IEnumerator Die(){
        var components = GetComponentsInChildren<Transform>().Where(x => !x.gameObject.name.Equals("renderer")).ToArray();
        for(int i = 0; i < components.Count(); i++ ){
            if (components[i].gameObject.name.Equals("EnemySprite(Clone)")){
            }
            else{
                Destroy(components[i].gameObject);
            }
            
        }

            for(int i = 0; i < deathAnimations.Length; i++) {
                spriteRenderer.sprite = deathAnimations[i];
                if(i > 3) {
                    yield return new WaitForSeconds(.09f);
                }
                else {
                    yield return new WaitForSeconds(.2f);
                }
            }
    }

    public bool FarFromPlayer(){
        return !inKillPosition;
    }

}
