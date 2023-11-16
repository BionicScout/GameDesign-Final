using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public FloorTile playerTile;
    public bool playerHasInstru;
    public float crankPerMove;
    public List<FloorTile> enemyTiles = new List<FloorTile>();


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            MoveIfAvialable(0, 1);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            MoveIfAvialable(-1, 0);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            MoveIfAvialable(0, -1);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            MoveIfAvialable(1, 0);
        }
    }

    void MoveIfAvialable(int xMove, int yMove)
    {
        int potentialX = playerTile.floorCord[0] + xMove;
        int potentialY = playerTile.floorCord[1] + yMove;
        FloorGrid floor = playerTile.floorGrid;

        if (playerTile.HasInstru)
        {
            playerHasInstru = true;
            playerTile.transform.GetChild(2).gameObject.SetActive(false);
            MainManager.instance.addScore(1);
        }

        bool offBoard = false;
        if (potentialX < 0 || potentialX >= floor.width)
        {
            offBoard = true;
        }
        if (potentialY < 0 || potentialY >= floor.height)
        {
            offBoard = true;
        }

        if (offBoard && floor.grid[playerTile.floorCord[0], playerTile.floorCord[1]].doorRefrence != null)
        {
            ///checks if there is an enemy
            if (floor.grid[playerTile.floorCord[0], playerTile.floorCord[1]].doorRefrence.HasEnemy == false)
            {
                playerTile.hasPlayer = false;
                playerTile.transform.GetChild(1).gameObject.SetActive(false);
                playerTile = floor.grid[playerTile.floorCord[0], playerTile.floorCord[1]].doorRefrence;
                playerTile.hasPlayer = true;
                playerTile.transform.GetChild(1).gameObject.SetActive(true);
            }
            if (playerHasInstru == true && (floor.grid[playerTile.floorCord[0], playerTile.floorCord[1]].doorRefrence.HasEnemy == false))
            {
                floor.grid[playerTile.floorCord[0], playerTile.floorCord[1]].doorRefrence.transform.GetChild(3).gameObject.SetActive(false);
            }
            if ((floor.grid[playerTile.floorCord[0], playerTile.floorCord[1]].doorRefrence.HasEnemy == false) && playerHasInstru == false)
            {
                SceneSwitcher.instance.A_LoadScene("Fail-Death");
            }

        }
        else if (offBoard)
            return;
        else
        {
            ///checks if there is an enemy
            if (floor.grid[potentialX, potentialY].hasOswald && playerHasInstru)
            {
                playerHasInstru = false;
                MainManager.instance.addScore(1);
            }
            if (floor.grid[potentialX, potentialY].HasEnemy == false)
            {
                playerTile.hasPlayer = false;
                playerTile.transform.GetChild(1).gameObject.SetActive(false);
                playerTile = floor.grid[potentialX, potentialY];
                playerTile.hasPlayer = true;
                playerTile.transform.GetChild(1).gameObject.SetActive(true);
            }
            if (playerHasInstru == true && floor.grid[potentialX, potentialY].HasEnemy == true)
            {
                floor.grid[potentialX, potentialY].transform.GetChild(3).gameObject.SetActive(false);
            }
            if (floor.grid[potentialX, potentialY].HasEnemy == true && playerHasInstru == false)
            {
                SceneSwitcher.instance.A_LoadScene("Fail-Death");
            }

        }

        MainManager.instance.addCrank(crankPerMove);

        //gets the camera and set it position to the players
        setCamera();

        moveEnemies();
    }

    public void setCamera()
    {
        GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
        cam.transform.position = new Vector3(playerTile.transform.position.x, playerTile.transform.position.y, -10);
    }

    public void moveEnemies()
    {
        for (int i = 0; i < enemyTiles.Count; i++)
        {
            int potentialX = playerTile.floorCord[0];
            int potentialY = playerTile.floorCord[1];
            FloorGrid floor = playerTile.floorGrid;

            bool offBoard = false;
            if (potentialX < 0 || potentialX >= floor.width)
            {
                offBoard = true;
            }
            if (potentialY < 0 || potentialY >= floor.height)
            {
                offBoard = true;
            }
            if (offBoard)
                return;
            else
            {
                ///checks if there is a player
                if (floor.grid[potentialX, potentialY].hasPlayer == false)
                {
                    playerTile.HasEnemy = false;
                    playerTile.transform.GetChild(3).gameObject.SetActive(false);
                    playerTile = floor.grid[potentialX, potentialY];
                    playerTile.HasEnemy = true;
                    playerTile.transform.GetChild(3).gameObject.SetActive(true);
                }
                if (floor.grid[potentialX, potentialY].HasEnemy == true && playerHasInstru == false)
                {
                    SceneSwitcher.instance.A_LoadScene("Fail-Death");
                }

            }
        }
    }

}




