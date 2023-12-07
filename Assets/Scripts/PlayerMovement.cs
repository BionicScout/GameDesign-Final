using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public FloorTile playerTile;
    public RoomGeneration roomGen;
    public FloorGrid floor;
    public int playerInstrument; //-1 no instrument, 0 Guitar, 1 Wind Pipes, 2 Harp, 3 Flute
    public bool playerHasHeal;
    public bool playerHasCrank;
    public bool playerHasTeleport;
    public float crankPerMove;
    public List<FloorTile> enemyTiles = new List<FloorTile>();
    public TextMeshProUGUI instruTxt;
    public TextMeshProUGUI healTxt;
    public TextMeshProUGUI crankTxt;
    public TextMeshProUGUI teleportTxt;
    public float timeSinceMove;
    public float timeDelay;
    public int damage;
    public int healAmt;
    public int crankReduceAmt;
    public Room spawnRoom;


    public void Awake()
    {
        instruTxt.gameObject.SetActive(false);
        healTxt.gameObject.SetActive(false);
        crankTxt.gameObject.SetActive(false);
        teleportTxt.gameObject.SetActive(false);
        timeSinceMove = timeDelay;

        playerInstrument = -1;
    }

    /***** User Input *****/

    private void Update() {
        moveKeys();
        useItem();
    }

    public void moveKeys() {
        //Movemet

        //If a movment key is being held down, build up Time
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) {
            timeSinceMove += Time.deltaTime;
        }

        //If time is built up, move charcter and reset time
        if(timeSinceMove > timeDelay) {
            timeSinceMove = 0;

            if(Input.GetKey(KeyCode.W)) {
                MoveIfAvialable(0 , 1 , 0);
            }
            else if(Input.GetKey(KeyCode.A)) {
                MoveIfAvialable(-1 , 0 , 1);
            }
            else if(Input.GetKey(KeyCode.S)) {
                MoveIfAvialable(0 , -1 , 2);
            }
            else if(Input.GetKey(KeyCode.D)) {
                MoveIfAvialable(1 , 0 , 3);
            }
        }

        //If key goes up, set move time to time Delay
        if(Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D)) {
            timeSinceMove = timeDelay;
        }


        /*
        if(Input.GetKey(KeyCode.W)) {
            timeSinceMove += Time.deltaTime;
            if(timeSinceMove > timeDelay) {
                MoveIfAvialable(0 , 1 , 0);
                timeSinceMove = 0;
            }
        }
        if(Input.GetKeyUp(KeyCode.W)) {
            timeSinceMove = timeDelay;
        }

        if(Input.GetKey(KeyCode.A)) {
            timeSinceMove += Time.deltaTime;
            if(timeSinceMove > timeDelay) {
                MoveIfAvialable(-1 , 0 , 1);
                timeSinceMove = 0;
            }
        }
        if(Input.GetKeyUp(KeyCode.A)) {
            timeSinceMove = timeDelay;
        }

        if(Input.GetKey(KeyCode.S)) {
            timeSinceMove += Time.deltaTime;
            if(timeSinceMove > timeDelay) {
                MoveIfAvialable(0 , -1 , 2);
                timeSinceMove = 0;
            }
        }
        if(Input.GetKeyUp(KeyCode.S)) {
            timeSinceMove = timeDelay;
        }

        if(Input.GetKey(KeyCode.D)) {
            timeSinceMove += Time.deltaTime;
            if(timeSinceMove > timeDelay) {
                MoveIfAvialable(1 , 0 , 3);
                timeSinceMove = 0;
            }
        }
        if(Input.GetKeyUp(KeyCode.D)) {
            timeSinceMove = timeDelay;
        }*/
    }

    public void useItem() {
        if((Input.GetKeyDown(KeyCode.E) && playerHasHeal)) {
            MainManager.instance.Heal(healAmt);
            playerHasHeal = false;
            healTxt.gameObject.SetActive(false);

            AudioManager.instance.Play("PlayerHeal"); // Use Item
        }
        if((Input.GetKeyDown(KeyCode.Q)) && playerHasCrank) {
            MainManager.instance.removeCrank(crankReduceAmt);
            playerHasCrank = false;
            crankTxt.gameObject.SetActive(false);

            AudioManager.instance.Play("Crank"); // Use Item
        }
        if((Input.GetKeyDown(KeyCode.F)) /*&& playerHasTeleport*/) {
            playerHasTeleport = false;
            teleportTxt.gameObject.SetActive(false);





            //Update Current Tile
            playerTile.removePlayer();

            playerTile.floorGrid.GetComponent<Room>().hasPlayer = false;
            playerTile.floorGrid.GetComponent<Room>().hide(true);

            //Swap Tiles
            playerTile = spawnRoom.floor.grid[1, 1];

            //Update New Tile
            playerTile.addPlayer(2);

            playerTile.floorGrid.GetComponent<Room>().hide(false);
            playerTile.floorGrid.GetComponent<Room>().hasPlayer = true;
            AudioManager.instance.Play("Move"); //Move Sound



            setCamera();








            AudioManager.instance.Play("Teleport"); // Use Item
        }
    }

    /***** Player Movement *****/


    void MoveIfAvialable(int xMove, int yMove, int playerDirection) //PlayerDirection = 0 Up, 1 Left, 2 Down, 3 Right 
    {
        //Get Direction of movement
        Vector2Int potentialCoord = new Vector2Int(playerTile.gridPos.x , playerTile.gridPos.y) + getDirection(playerDirection);
        floor = playerTile.floorGrid;

        //Check if the player move goes of the board
        bool offBoard = movingOffBoard(potentialCoord);

        if (offBoard && floor.grid[playerTile.gridPos.x, playerTile.gridPos.y].doorRefrence != null) {
            betweenRoomMovement(playerDirection);
        }
        else if (offBoard)
            return;
        else {
            inRoomMovement(playerDirection, potentialCoord);
        }

        MainManager.instance.addCrank(crankPerMove);

        //Pick up instrument
        if(playerTile.hasItem)
            pickUpTile();

        //gets the camera and set it position to the players
        setCamera();

        moveEnemies();
    }

    public void betweenRoomMovement(int playerDirection) {
        //Update Current Tile
        playerTile.removePlayer();

        playerTile.floorGrid.GetComponent<Room>().hasPlayer = false;
        playerTile.floorGrid.GetComponent<Room>().hide(true);

        //Swap Tiles
        playerTile = floor.grid[playerTile.gridPos.x , playerTile.gridPos.y].doorRefrence;

        //Update New Tile
        playerTile.addPlayer(playerDirection);

        playerTile.floorGrid.GetComponent<Room>().hide(false);
        playerTile.floorGrid.GetComponent<Room>().hasPlayer = true;
        AudioManager.instance.Play("Move"); //Move Sound
    }

    public void inRoomMovement(int playerDirection, Vector2Int potentialCoord) {
        //Return instrument to oswald
        if(floor.grid[potentialCoord.x , potentialCoord.y].hasOswald && playerInstrument != -1) {
            playerTile.InstrumentSound(playerInstrument);
            playerInstrument = -1;

            instruTxt.gameObject.SetActive(false);
            MainManager.instance.addScore(1);
        }
        //Player Attack Enemy
        if(playerInstrument != -1 && floor.grid[potentialCoord.x , potentialCoord.y].enemy != -1) {
            playerTile.InstrumentSound(playerInstrument);
            floor.grid[potentialCoord.x , potentialCoord.y].removeEnemy();
            Debug.Log("Instumentert " + playerInstrument);
        }
        if(playerInstrument == -1 && floor.grid[potentialCoord.x , potentialCoord.y].enemy != -1) {
            MainManager.instance.takeDamage(damage);
            AudioManager.instance.Play("PlayerHurt"); // Hurt Sound
            Debug.Log("Instumentert " + playerInstrument);
        }

        //If no enemy, move player
        if(floor.grid[potentialCoord.x , potentialCoord.y].enemy == -1) {
            //Update Current Tile
            playerTile.removePlayer();

            //Swap Tiles
            playerTile = floor.grid[potentialCoord.x , potentialCoord.y];

            //Update New Tile
            playerTile.addPlayer(playerDirection);

            AudioManager.instance.Play("Move"); //Move Sound
        }
    }

    /***** PLAYER MOVEMENT UTILITY *****/

    public void pickUpTile() {
        if(playerTile.instrument != -1) {
            playerInstrument = playerTile.instrument;
            playerTile.InstrumentSound(playerInstrument);
            playerTile.removeInstrument();

            instruTxt.gameObject.SetActive(true);
            playerTile.InstrumentSound(playerInstrument);
        }

        //0 Heal Potion, 1 Crank Potion, 2 Hourglass
        else if(playerTile.item == 0 && playerHasHeal == false) {
            playerTile.removeItem();
            playerHasHeal = true;

            Debug.Log("Player got a health potion");
            healTxt.gameObject.SetActive(true);

            AudioManager.instance.Play("Item"); //Pick Up
        }
        else if(playerTile.item == 1 && playerHasCrank == false) {
            playerTile.removeItem();
            playerHasCrank = true;

            Debug.Log("Player got a crank potion");
            crankTxt.gameObject.SetActive(true);

            AudioManager.instance.Play("Item"); // Pick Up
        }
        else if(playerTile.item == 2 && playerHasTeleport == false) {
            playerTile.removeItem();
            playerHasTeleport = true;
            
            Debug.Log("Player got a Teleport item");
            teleportTxt.gameObject.SetActive(true);




            AudioManager.instance.Play("Item"); //Pick Up
        }
    }

    public void setCamera()
    {
        GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
        cam.transform.position = new Vector3(playerTile.transform.position.x, playerTile.transform.position.y, -10);
    }

    /***** ENEMY MOVEMENT *****/

    public void moveEnemies() {
        for(int i = 0; i < enemyTiles.Count; i++) {
            //if enemey is in a room that is not discover, don't move
            if(enemyTiles[i].floorGrid.GetComponent<Room>().isHidden || !enemyTiles[i].floorGrid.GetComponent<Room>().hasPlayer) {
                continue;
            }

            Debug.Log("Enemy Moves");

            //Get Direction of snake
            Vector2Int vec; 

            Vector2Int playerVec = playerTile.gridPos;

            Vector2Int distanceVec = playerVec - enemyTiles[i].gridPos;


            if(0 == distanceVec.y) {
                vec = pickDirection();
            }
            else if(Mathf.Abs(distanceVec.x) > Mathf.Abs(distanceVec.y)) {
                vec = new Vector2Int(distanceVec.x / Mathf.Abs(distanceVec.x),0);
            }
            else {
                vec = new Vector2Int(0, distanceVec.y / Mathf.Abs(distanceVec.y));
            }


            if(Random.Range(0, 100) > 35) {
                vec = pickDirection();
            }



            int potentialX = enemyTiles[i].gridPos.x + vec.x;
            int potentialY = enemyTiles[i].gridPos.y + vec.y;
            FloorGrid floor = enemyTiles[i].floorGrid;


            bool offBoard = false;
            if(potentialX < 0 || potentialX >= floor.width) {
                offBoard = true;
            }
            if(potentialY < 0 || potentialY >= floor.height) {
                offBoard = true;
            }


            if(offBoard)
                return;
            else {
                //checks if there is a player on potential tile
                if(floor.grid[potentialX , potentialY].playerDir == -1) {
                    int enemyType = enemyTiles[i].enemy;

                    enemyTiles[i].removeEnemy();
                    enemyTiles[i] = floor.grid[potentialX , potentialY];
                    enemyTiles[i].addEnemy(enemyType);

                    for(int j = 0; j < 4; j++) {
                        Vector2Int dir = getDirection(j);
                        offBoard = movingOffBoard(dir);

                        if(offBoard)
                            continue;

                        if(floor.grid[potentialX + dir.x , potentialY + dir.y].playerDir != -1) {
                            MainManager.instance.takeDamage(damage);
                            AudioManager.instance.Play("PlayerHurt"); // Hurt Sound
                        }
                    }
                }
            }

        }
    }

    /***** UTILITY *****/

    public Vector2Int pickDirection() {
        int[,] baseDirections = { { 0 , 1 }, { -1 , 0 } , { 0 , -1 } , { 1 , 0 } }; //[0 Up, 1 Left, 2 Down, 3 Right   ,   0 x, 1 y]
        int direction = Random.Range(0 , 4) % 4;


        return new Vector2Int(baseDirections[direction , 0] , baseDirections[direction , 1]);
    }

    public Vector2Int getDirection(int id) {
        int[,] baseDirections = { { 0 , 1 }, { -1 , 0 } , { 0 , -1 } , { 1 , 0 } }; //[0 Up, 1 Left, 2 Down, 3 Right   ,   0 x, 1 y]
        return new Vector2Int(baseDirections[id , 0] , baseDirections[id, 1]);
    }

    public bool movingOffBoard(Vector2Int tile) {
        bool offBoard = false;
        if(tile.x < 0 || tile.x >= floor.width) {
            offBoard = true;
        }
        if(tile.y < 0 || tile.y >= floor.height) {
            offBoard = true;
        }

        return offBoard;
    }
}