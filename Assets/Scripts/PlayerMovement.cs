using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public FloorTile playerTile;
    public bool playerHasInstru;
    public bool playerHasHeal;
    public bool playerHasCrank;
    public bool playerHasTeleport;
    public float crankPerMove;
    public List<FloorTile> enemyTiles = new List<FloorTile>();
    public HealthBar healthBar;
    public TextMeshProUGUI instruTxt;
    public TextMeshProUGUI healTxt;
    public TextMeshProUGUI crankTxt;
    public TextMeshProUGUI teleportTxt;
    public float timeSinceMove;
    public float timeDelay;

    public void Awake()
    {
        instruTxt.gameObject.SetActive(false);
        healTxt.gameObject.SetActive(false);
        crankTxt.gameObject.SetActive(false);
        teleportTxt.gameObject.SetActive(false);
        timeSinceMove = 0;
    }


    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            timeSinceMove += Time.deltaTime;
            if(timeSinceMove > timeDelay)
            {
                MoveIfAvialable(0, 1);
                timeSinceMove = 0;
            }
        }
        if(Input.GetKeyUp(KeyCode.W)) 
        {
            timeSinceMove = timeDelay;
        }

        if (Input.GetKey(KeyCode.A))
        {
            timeSinceMove += Time.deltaTime;
            if (timeSinceMove > timeDelay)
            {
                MoveIfAvialable(-1, 0);
                timeSinceMove = 0;
            }
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            timeSinceMove = timeDelay;
        }

        if (Input.GetKey(KeyCode.S))
        {
            timeSinceMove += Time.deltaTime;
            if (timeSinceMove > timeDelay)
            {
                MoveIfAvialable(0, -1);
                timeSinceMove = 0;
            }
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            timeSinceMove = timeDelay;
        }

        if (Input.GetKey(KeyCode.D))
        {
            timeSinceMove += Time.deltaTime;
            if (timeSinceMove > timeDelay)
            {
                MoveIfAvialable(1, 0);
                timeSinceMove = 0;
            }
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            timeSinceMove = timeDelay;
        }
    }


    void MoveIfAvialable(int xMove, int yMove)
    {
        
        int potentialX = playerTile.floorCord[0] + xMove;
        int potentialY = playerTile.floorCord[1] + yMove;
        FloorGrid floor = playerTile.floorGrid;

        if (playerTile.HasInstru)
        {
            playerTile.transform.GetChild(2).gameObject.SetActive(false);
            playerHasInstru = true;
            instruTxt.gameObject.SetActive(true);
            //MainManager.instance.addScore(1);
        }
        //checks if tile has a potion
        if (playerTile.hasHealPotion && playerHasHeal == false)
        {
            playerTile.transform.GetChild(6).gameObject.SetActive(false);
            playerHasHeal = true;
            Debug.Log("Player got a health potion");
            healTxt.gameObject.SetActive(true);
            //MainManager.instance.addScore(1);
        }
        if (playerTile.hasCrankPotion && playerHasCrank == false)
        {
            playerTile.transform.GetChild(7).gameObject.SetActive(false);
            playerHasCrank = true;
            Debug.Log("Player got a crank potion");
            crankTxt.gameObject.SetActive(true);
            //MainManager.instance.addScore(1);
        }
        //check if tile has a teleport item
        if (playerTile.hasTeleport && playerHasTeleport == false)
        {
            playerTile.transform.GetChild(5).gameObject.SetActive(false);
            playerHasTeleport = true;
            Debug.Log("Player got a Teleport item");
            teleportTxt.gameObject.SetActive(true);
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
                floor.gameObject.GetComponent<Room>().hasPlayer = false;
                playerTile.hasPlayer = true;
                playerTile.transform.GetChild(1).gameObject.SetActive(true);
                playerTile.floorGrid.GetComponent<Room>().hide(false);
                floor.gameObject.GetComponent<Room>().hasPlayer = true;
            }
            if (playerHasInstru == true && (floor.grid[playerTile.floorCord[0], playerTile.floorCord[1]].doorRefrence.HasEnemy == false))
            {
                floor.grid[playerTile.floorCord[0], playerTile.floorCord[1]].doorRefrence.transform.GetChild(3).gameObject.SetActive(false);
            }
            if ((floor.grid[playerTile.floorCord[0], playerTile.floorCord[1]].doorRefrence.HasEnemy == true) && playerHasInstru == false)
            {
                healthBar.SetHealth(1);
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
                instruTxt.gameObject.SetActive(false);
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
                healthBar.SetHealth(1);
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
        for (int i = 0; i < enemyTiles.Count; i++) {
        //if enemey is in a room that is not discover, don't move
            if(enemyTiles[i].floorGrid.GetComponent<Room>().isHidden) {
                continue;
            }

        //Get Direction of snake
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
                if (floor.grid[potentialX, potentialY].hasPlayer == false)
                {
                    enemyTiles[i].HasEnemy = false;
                    enemyTiles[i].transform.GetChild(3).gameObject.SetActive(false);
                    enemyTiles[i] = floor.grid[potentialX, potentialY];
                    enemyTiles[i].HasEnemy = true;
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

    public void snakeMovement() {

    }

    public void DijkstraSnakeMovement() {
        /*
        function dijkstra(G, S)
            for each vertex V in G
                distance[V] <- infinite
                previous[V] <- NULL
                If V != S, add V to Priority Queue Q
            distance[S] <- 0

            while Q IS NOT EMPTY
                U <- Extract MIN from Q
                for each unvisited neighbour V of U
                    tempDistance <- distance[U] + edge_weight(U, V)
                    if tempDistance < distance[V]
                        distance[V] <- tempDistance
                        previous[V] <- U
            return distance[], previous[]
         */
    }

    public Vector2 pickDirection()
    {
        int[,] baseDirections = { { -1, 0 }, { 0, -1 }, { 1, 0 }, { 0, 1 } }; //[0 Up, 1 Left, 2 Down, 3 Right   ,   0 x, 1 y]
        int direction = Random.Range(0, 4) % 4;


        return new Vector2(baseDirections[direction, 0], baseDirections[direction, 1]);
    }
}




