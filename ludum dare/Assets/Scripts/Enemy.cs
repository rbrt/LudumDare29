﻿using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public MapLoader mapLoader;

    int x,
        y;

    enum Behaviours {Walk, Turn, Stand};
    Behaviours currentBehaviour;

    enum Directions { Up, Down, Left, Right };
    Directions currentDirection;

    int actionCount = 0;
    bool isBehaving = false;

	// Use this for initialization
	void Start () {
        currentBehaviour = Behaviours.Walk; // (Behaviours)Random.Range(0, 2);
        currentDirection = (Directions)Random.Range(0, 3);
        actionCount = Random.Range(0, 5);
	}

    public void SetMyCoords(int newX, int newY){
        x = newX;
        y = newY;
    }

	// Update is called once per frame
	void Update () {
        if (!isBehaving){
            StartCoroutine(AdvanceBehaviour());
            isBehaving = true;
        }

        Debug.Log(currentBehaviour);
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

    IEnumerator AdvanceBehaviour(){
        if (actionCount <= 0){
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
            }
        }
        
        isBehaving = false;
    }

    IEnumerator Walk(){
        // if up - y++
        // if left x--
        // if right x++
        // if down y--

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

    }

    IEnumerator moveToSquare(Vector3 square, int newX, int newY){
        Vector3 force = new Vector3();

        while (Vector3.Distance(transform.position, square) > .1f){
            transform.position = Vector3.SmoothDamp(transform.position, square, ref force, .7f);
            yield return null;
        }

        actionCount--;
        x = newX;
        y = newY;
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
        yield break;
    }
}