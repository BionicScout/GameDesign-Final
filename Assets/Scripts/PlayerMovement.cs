using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public FloorTile playerTile;

    private void Update() {
        if(Input.GetKeyDown(KeyCode.W)) {
            MoveIfAvialable(0, 1);
        }
        if(Input.GetKeyDown(KeyCode.A)) {
            MoveIfAvialable(-1, 0);
        }
        if(Input.GetKeyDown(KeyCode.S)) {
            MoveIfAvialable(0, -1);
        }
        if(Input.GetKeyDown(KeyCode.D)) {
            MoveIfAvialable(1, 0);
        }
    }

    void MoveIfAvialable(int xMove, int yMove) {
        int potentialX = playerTile.floorCord[0] + xMove;
        int potentialY = playerTile.floorCord[1] + yMove;
        FloorGrid floor = playerTile.floorGrid;

        Debug.Log("INPUT");

        if(potentialX < 0 || potentialX >= floor.width) {
            return;
        }
        if(potentialY < 0 || potentialY >= floor.height) {
            return;
        }

        Debug.Log("Moving");

        playerTile.hasPlayer = false;
        playerTile.transform.GetChild(0).gameObject.SetActive(false);
        playerTile = floor.grid[potentialX , potentialY];
        playerTile.hasPlayer = true;
        playerTile.transform.GetChild(0).gameObject.SetActive(true);
    }

}
