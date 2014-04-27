using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public MapLoader mapLoader;

    public SpriteRenderer spriteRenderer;
    public Sprite[] frontAnimations;

    enum Directions { Up, Down, Left, Right };
    Directions currentDirection;

    public int x,
               y;

    public bool isMoving = false;

    public bool isHoldingDown = false,
                isHoldingUp = false,
                isHoldingRight = false,
                isHoldingLeft = false,
                isAttacking = false,
                canMove = true,
                isWalking = false;

    public void SetMyCoords(int newX, int newY){
        x = newX;
        y = newY;
    }

	// Use this for initialization
	void Start () {
        currentDirection = Directions.Up;
	}
	
	// Update is called once per frame
	void Update () {
        if (canMove){
            // Up
	        if (Input.GetKeyDown(KeyCode.W)){
                currentDirection = Directions.Up;
                isHoldingUp = true;
            }
            // Left
            else if(Input.GetKeyDown(KeyCode.A)) {
                currentDirection = Directions.Left;
                isHoldingLeft = true;
            }
            // Right
            else if(Input.GetKeyDown(KeyCode.D)) {
                currentDirection = Directions.Right;
                isHoldingRight = true;
            }
            // Down
            else if(Input.GetKeyDown(KeyCode.S)) {
                currentDirection = Directions.Down;
                isHoldingDown = true;
            }

            // Up
            if(Input.GetKeyUp(KeyCode.W)) {
                isHoldingUp = false;
            }
            // Left
            else if(Input.GetKeyUp(KeyCode.A)) {
                isHoldingLeft = false;
            }
            // Right
            else if(Input.GetKeyUp(KeyCode.D)) {
                isHoldingRight = false;
            }
            // Down
            else if(Input.GetKeyUp(KeyCode.S)) {
                isHoldingDown = false;
            }

            if (Input.GetKey(KeyCode.Space)){
                if (!isAttacking){
                    isAttacking = true;
                    Attack();
                }
            }

            if (!isMoving){
                if (isHoldingUp && CheckIfCanMove(x, y+1)){
                    StartCoroutine(moveToSquare(mapLoader.MapCoords[x,y+1].position, x, y+1));
                }
                else if(isHoldingLeft && CheckIfCanMove(x-1, y)) {
                    StartCoroutine(moveToSquare(mapLoader.MapCoords[x - 1, y].position, x - 1, y));
                }
                else if(isHoldingRight && CheckIfCanMove(x+1, y)) {
                    StartCoroutine(moveToSquare(mapLoader.MapCoords[x + 1, y].position, x + 1, y));
                }
                else if(isHoldingDown && CheckIfCanMove(x, y - 1)) {
                    StartCoroutine(moveToSquare(mapLoader.MapCoords[x, y - 1].position, x, y - 1));
                }
            }
            
        }
	}

    void Attack(){
        if (currentDirection == Directions.Up){
           StartCoroutine( CheckIfCorrectEnemy(x, y + 1));
        }
        else if(currentDirection == Directions.Down) {
            StartCoroutine(CheckIfCorrectEnemy(x, y - 1));
        }
        else if(currentDirection == Directions.Left) {
            StartCoroutine(CheckIfCorrectEnemy(x - 1, y));
        }
        else if(currentDirection == Directions.Right) {
            StartCoroutine(CheckIfCorrectEnemy(x + 1, y));
        }
        isAttacking = false;
    }

    bool CheckIfCanMove(int x, int y) {
        try {
            if(mapLoader.MapCoords[x, y].isOccupied || mapLoader.MapCoords[x, y].isWall) {
                return false;
            }
        }
        catch(System.IndexOutOfRangeException e) {
            return false;
        }

        return true;
    }

    IEnumerator CheckIfCorrectEnemy(int x, int y){
        canMove = false;
        if (mapLoader.MapCoords[x,y].isOccupied){
            yield return StartCoroutine(mapLoader.CheckEnemies(x,y));
        }

        canMove = true;
    }

    IEnumerator moveToSquare(Vector3 square, int newX, int newY) {
        isMoving = true;
        isWalking = true;
        StartCoroutine(AnimateWalk());
        Vector3 force = new Vector3();

        while(Vector3.Distance(transform.position, square) > .01f) {
            transform.position = Vector3.SmoothDamp(transform.position, square, ref force, .1f);
            yield return null;
        }
        transform.position = square;

        mapLoader.MapCoords[x, y].isOccupied = false;

        x = newX;
        y = newY;

        mapLoader.MapCoords[x, y].isOccupied = true;
        isMoving = false;
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

            yield return new WaitForSeconds(.25f);
        }
        spriteRenderer.sprite = frontAnimations[0];
    }
}
