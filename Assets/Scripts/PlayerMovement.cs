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
    public int playerInstrument; //-1 no instrument, 0 Guitar, 2 Wind Pipes, 3 Harp, 4 Flute
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


    public void Awake()
    {
        instruTxt.gameObject.SetActive(false);
        healTxt.gameObject.SetActive(false);
        crankTxt.gameObject.SetActive(false);
        teleportTxt.gameObject.SetActive(false);
        timeSinceMove = timeDelay;
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

            timeSinceMove = 0;
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

            AudioManager.instance.Play("SOUND_EFFECT_NEEDED"); // Use Item
        }
        if((Input.GetKeyDown(KeyCode.Q)) && playerHasCrank) {
            MainManager.instance.removeCrank(crankReduceAmt);
            playerHasCrank = false;
            crankTxt.gameObject.SetActive(false);

            AudioManager.instance.Play("SOUND_EFFECT_NEEDED"); // Use Item
        }
        if((Input.GetKeyDown(KeyCode.F)) && playerHasTeleport) {
            playerHasTeleport = false;
            teleportTxt.gameObject.SetActive(false);

            AudioManager.instance.Play("SOUND_EFFECT_NEEDED"); // Use Item
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

        //Check for enemy
        bool hasEnemy = floor.grid[playerTile.gridPos.x , playerTile.gridPos.y].doorRefrence.enemy != -1;

        if(playerInstrument != -1 && hasEnemy) {
            floor.grid[playerTile.gridPos.x , playerTile.gridPos.y].doorRefrence.transform.GetChild(5).gameObject.SetActive(false);
            playerTile.InstrumentSound(playerInstrument);
        }
        if(playerInstrument == -1 && hasEnemy) {
            MainManager.instance.takeDamage(damage);
            AudioManager.instance.Play("SOUND_EFFECT_NEEDED"); // Hurt Sound
        }
    }

    public void inRoomMovement(int playerDirection, Vector2Int potentialCoord) {
        //Return instrument to oswald
        if(floor.grid[potentialCoord.x , potentialCoord.y].hasOswald && playerInstrument != -1) {
            playerTile.InstrumentSound(playerInstrument);
            playerInstrument = -1;

            instruTxt.gameObject.SetActive(false);
            MainManager.instance.addScore(1);
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
        //Player Attack Enemy
        if(playerInstrument != -1 && floor.grid[potentialCoord.x , potentialCoord.y].enemy != -1) {
            floor.grid[potentialCoord.x , potentialCoord.y].transform.GetChild(5).gameObject.SetActive(false);
            playerTile.InstrumentSound(playerInstrument);
        }
        if(playerInstrument == -1 && floor.grid[potentialCoord.x , potentialCoord.y].enemy != -1) {
            MainManager.instance.takeDamage(damage);
            AudioManager.instance.Play("SOUND_EFFECT_NEEDED"); // Hurt Sound
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

            AudioManager.instance.Play("SOUND_EFFECT_NEEDED"); //Pick Up
        }
        else if(playerTile.item == 1 && playerHasCrank == false) {
            playerTile.removeItem();
            playerHasCrank = true;

            Debug.Log("Player got a crank potion");
            crankTxt.gameObject.SetActive(true);

            AudioManager.instance.Play("SOUND_EFFECT_NEEDED"); // Pick Up
        }
        else if(playerTile.item == 2 && playerHasTeleport == false) {
            playerTile.removeItem();
            playerHasTeleport = true;
            
            Debug.Log("Player got a Teleport item");
            teleportTxt.gameObject.SetActive(true);

            AudioManager.instance.Play("SOUND_EFFECT_NEEDED"); //Pick Up
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

            Debug.Log("Snake Moves");

            //Get Direction of snake
            Vector2Int vec; //= DijkstraSnakeMovement(enemyTiles[i].floorGrid , enemyTiles[i].gridPos);
            //Debug.Log("Snake: " + enemyTiles[i].gridPos + "\nMove To: " + vec);


            Vector2Int playerVec = playerTile.gridPos;

            Vector2Int distanceVec = playerVec - enemyTiles[i].gridPos;

            if(Mathf.Abs(distanceVec.x) > Mathf.Abs(distanceVec.y)) {
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
                }

                //if (playerHasInstru == true && floor.grid[potentialX, potentialY].HasEnemy == true)
                //{
                //   floor.grid[potentialX, potentialY].transform.GetChild(5).gameObject.SetActive(false);
                //}

                //if (floor.grid[potentialX, potentialY].HasEnemy == true && playerHasInstru == false)
                //{
                //    MainManager.instance.takeDamage(1);
                //    //SceneSwitcher.instance.A_LoadScene("Fail-Death");
                //}
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

    /***** OTHER *****/

    /*struct DijkstraTile {
        public int x;
        public int y;

        public bool visted;
        public bool hasPlayer;

        public int distance;

        public int previousX;
        public int previousY;
    }

    public Vector2Int DijkstraSnakeMovement(FloorGrid floorGrid , Vector2Int snakePos) {
        Vector2Int[] baseDirections = { new Vector2Int( -1 , 0) , new Vector2Int ( 0 , -1 ) , new Vector2Int ( 1 , 0 ) , new Vector2Int ( 0 , 1 ) };
        DijkstraTile[,] tiles = new DijkstraTile[floorGrid.grid.GetLength(0) , floorGrid.grid.GetLength(1)];
        Vector2Int playCord = new Vector2Int();

        //Set all tiles to -1 and snake tile to 0
        for(int x = 0; x < tiles.GetLength(0); x++) {
            for(int y = 0; y < tiles.GetLength(1); y++) {
                DijkstraTile tile = new DijkstraTile();

                tile.x = x;
                tile.y = y;

                tile.visted = false;
                tile.hasPlayer = floorGrid.grid[x , y].hasPlayer;
                if(tile.hasPlayer) { playCord = new Vector2Int(); }

                tile.distance = -1;

                tile.previousX = -1;
                tile.previousY = -1;

                tiles[x , y] = tile;
            }
        }

        tiles[snakePos.x , snakePos.y].distance = 0;

        //Loop through all unvisted nodes
        Queue<DijkstraTile> unvistedTiles = new Queue<DijkstraTile>();
        unvistedTiles.Enqueue(tiles[snakePos.x , snakePos.y]);

        int failSafe = 150;

        while(unvistedTiles.Count > 0) {
            //Get Observed Tile
            DijkstraTile temp2 = unvistedTiles.Dequeue();
            DijkstraTile currentTile = tiles[temp2.x , temp2.y];

            //Check all adjasint tiles
            Debug.Log("Current Tile: " + currentTile.x + "    " + currentTile.y + "    " + currentTile.visted);
            for(int i = 0; i < baseDirections.GetLength(0); i++) {
                //Debug.Log("Dir: " + baseDirections[i]);
                Vector2Int direction = new Vector2Int(baseDirections[i].x + currentTile.x , baseDirections[i].y + currentTile.y);

                //If Wall, no Tile to check
                if(direction.x < 0 || direction.x >= tiles.GetLength(0) || direction.y < 0 || direction.y >= tiles.GetLength(1))
                    continue;

                //If Unvisted, update and add to list, if visted, skip
                DijkstraTile adjTile = tiles[direction.x , direction.y];

                if(adjTile.visted)
                    continue;

                //Update Tile Distance if Needed
                int distanceUsingCurrent = currentTile.distance + 1;

                if(distanceUsingCurrent > adjTile.distance || adjTile.distance == -1) {
                    adjTile.distance = distanceUsingCurrent;
                    adjTile.previousX = currentTile.x;
                    adjTile.previousY = currentTile.y;

                    //Debug.Log("AdjTile: " + adjTile.x + "    " + adjTile.y);
                }
                //else if(distanceUsingCurrent == adjTile.distance) { //Distances are the same, so pick a random tile to be prvious
                //    int prevTile = Random.Range(0 , 100);

                //    if(prevTile >= 50) { //50% chace to switch previous tile to the current 
                //        adjTile.previousX = currentTile.x;
                //        adjTile.previousY = currentTile.y;
                //    }
                //}

                tiles[adjTile.x, adjTile.y] = adjTile;

                bool check = true;
                foreach(DijkstraTile t in unvistedTiles) {
                    if((t.x == adjTile.x && t.y == adjTile.y)) {
                        check = false; break;
                    }
                        
                }

                if(!check && !adjTile.visted)
                    unvistedTiles.Enqueue(adjTile);
            }

            Debug.Log("Current Tile Previous: " + currentTile.previousX + "   " + currentTile.previousY);

            currentTile.visted = true;
            tiles[currentTile.x, currentTile.y] = currentTile;

            if(currentTile.hasPlayer) {
                Debug.Log("This tile is player");
                break;
            }

            failSafe--;

            if(failSafe <= 0) {
                Debug.Log("Fail safe!!!!!!");
                break;
            }
        }

        //Get PLayer Route
        DijkstraTile temp = tiles[playCord.x , playCord.y];
        Stack<DijkstraTile> path = new Stack<DijkstraTile>();

        Debug.Log("SNAKE -----------------------------------------------------");

        Debug.Log("PLayer: " + temp.previousX + "   " + temp.previousY);


        Debug.Log("Path: " + path.Count);

        for(int i = 0; i < 20; i++) {
            path.Push(temp);
            Debug.Log("next Tile: " + temp.previousX + "   " + temp.previousY);
            temp = tiles[temp.previousX , temp.previousY];



        }

        //while(temp.previousX != -1) { //Pushes tiles until snake tile
        //    path.Push(temp);
        //    temp = tiles[temp.previousX , temp.previousY];
        //    Debug.Log("Path: " + path.Count);
        //}


        //Debug.Log("Path: " + path.Count);
        ////path.Pop();
        //temp = path.Pop();
        return new Vector2Int(temp.x , temp.y);

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
         * /
    }*/
}