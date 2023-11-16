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
            //MainManager.instance.addScore(1);
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
                playerTile.floorGrid.GetComponent<Room>().hide(false);
            }
            if (playerHasInstru == true && (floor.grid[playerTile.floorCord[0], playerTile.floorCord[1]].doorRefrence.HasEnemy == false))
            {
                floor.grid[playerTile.floorCord[0], playerTile.floorCord[1]].doorRefrence.transform.GetChild(3).gameObject.SetActive(false);
            }
            if ((floor.grid[playerTile.floorCord[0], playerTile.floorCord[1]].doorRefrence.HasEnemy == true) && playerHasInstru == false)
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

            //if (enemyTiles[i].transform.GetChild(3).gameObject.active)
            //{
            //    return;
            //}

            Vector2 vec = pickDirection();
            int potentialX = enemyTiles[i].floorCord[0] + (int)vec.x;
            int potentialY = enemyTiles[i].floorCord[1] + (int)vec.y;
            FloorGrid floor = enemyTiles[i].floorGrid;


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
                ///checks if there is a player on potential tile
                if (floor.grid[potentialX, potentialY].HasEnemy == false)
                {
                    enemyTiles[i].hasPlayer = false;
                    enemyTiles[i].transform.GetChild(3).gameObject.SetActive(false);
                    enemyTiles[i] = floor.grid[potentialX, potentialY];
                    enemyTiles[i].hasPlayer = true;
                    enemyTiles[i].transform.GetChild(3).gameObject.SetActive(true);
                }

                //if (playerHasInstru == true && floor.grid[potentialX, potentialY].HasEnemy == true)
                //{
                //   floor.grid[potentialX, potentialY].transform.GetChild(3).gameObject.SetActive(false);
                //}

                //if (floor.grid[potentialX, potentialY].HasEnemy == true && playerHasInstru == false)
                //{
                //    SceneSwitcher.instance.A_LoadScene("Fail-Death");
                //}
            }

        }
    }
    public Vector2 pickDirection()
    {
        int[,] baseDirections = { { -1, 0 }, { 0, -1 }, { 1, 0 }, { 0, 1 } }; //[0 Up, 1 Left, 2 Down, 3 Right   ,   0 x, 1 y]
        int direction = Random.Range(0, 4) % 4;


        return new Vector2(baseDirections[direction, 0], baseDirections[direction, 1]);
    }
}




