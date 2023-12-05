using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    public FloorTile playerTile;
    public RoomGeneration roomGen;
    public FloorGrid floor;

    public bool playerHasInstru;
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
        timeSinceMove = 0;
    }


    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            timeSinceMove += Time.deltaTime;
            if(timeSinceMove > timeDelay)
            {
                MoveIfAvialable(0, 1, 3);
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
                MoveIfAvialable(-1, 0, 2);
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
                MoveIfAvialable(0, -1, 4);
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
                MoveIfAvialable(1, 0, 1);
                timeSinceMove = 0;
            }
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            timeSinceMove = timeDelay;
        }
        if((Input.GetKeyDown(KeyCode.E) && playerHasHeal))
        {
            MainManager.instance.Heal(healAmt);
            playerHasHeal = false;
            healTxt.gameObject.SetActive(false);

        }
        if((Input.GetKeyDown(KeyCode.Q)) && playerHasCrank)
        {
            MainManager.instance.removeCrank(crankReduceAmt);
            playerHasCrank = false;
            crankTxt.gameObject.SetActive(false);

        }
        if ((Input.GetKeyDown(KeyCode.F)) && playerHasTeleport)
        {
            playerHasTeleport = false;
            teleportTxt.gameObject.SetActive(false);

            roomManager.roomList[1].GetComponent<Room>().hide(false);
            transform.GetChild(2).GetChild(1).gameObject.SetActive(true);

            playerTile.transform.GetChild(10).gameObject.SetActive(false);
            playerTile.transform.GetChild(11).gameObject.SetActive(false);
            playerTile.transform.GetChild(12).gameObject.SetActive(false);
            playerTile.transform.GetChild(13).gameObject.SetActive(false);

        }
    }
    public void Teleport()
    {

    }


    void MoveIfAvialable(int xMove, int yMove, int pSprite)
    {
        
        int potentialX = playerTile.floorCord[0] + xMove;
        int potentialY = playerTile.floorCord[1] + yMove;
        floor = playerTile.floorGrid;

        if (playerTile.HasInstru)
        {
            playerTile.transform.GetChild(10).gameObject.SetActive(false);
            playerTile.transform.GetChild(11).gameObject.SetActive(false);
            playerTile.transform.GetChild(12).gameObject.SetActive(false);
            playerTile.transform.GetChild(13).gameObject.SetActive(false);
            playerHasInstru = true;
            instruTxt.gameObject.SetActive(true);
            //MainManager.instance.addScore(1);
        }
        //checks if tile has a potion
        if (playerTile.hasHealPotion && playerHasHeal == false)
        {
            playerTile.transform.GetChild(7).gameObject.SetActive(false);
            playerHasHeal = true;
            Debug.Log("Player got a health potion");
            healTxt.gameObject.SetActive(true);
            //MainManager.instance.addScore(1);
        }
        if (playerTile.hasCrankPotion && playerHasCrank == false)
        {
            playerTile.transform.GetChild(8).gameObject.SetActive(false);
            playerHasCrank = true;
            Debug.Log("Player got a crank potion");
            crankTxt.gameObject.SetActive(true);
            //MainManager.instance.addScore(1);
        }
        //check if tile has a teleport item
        if (playerTile.hasTeleport && playerHasTeleport == false)
        {
            playerTile.transform.GetChild(9).gameObject.SetActive(false);
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
                playerTile.transform.GetChild(2).gameObject.SetActive(false);
                playerTile.transform.GetChild(3).gameObject.SetActive(false);
                playerTile.transform.GetChild(4).gameObject.SetActive(false);
                playerTile.floorGrid.GetComponent<Room>().hasPlayer = false;
                playerTile.floorGrid.GetComponent<Room>().hide(true);

                playerTile = floor.grid[playerTile.floorCord[0], playerTile.floorCord[1]].doorRefrence;

                playerTile.hasPlayer = true;
                playerTile.transform.GetChild(pSprite).gameObject.SetActive(true);
                playerTile.floorGrid.GetComponent<Room>().hide(false);
                playerTile.floorGrid.GetComponent<Room>().hasPlayer = true;
            }
            if (playerHasInstru == true && (floor.grid[playerTile.floorCord[0], playerTile.floorCord[1]].doorRefrence.HasEnemy == false))
            {
                floor.grid[playerTile.floorCord[0], playerTile.floorCord[1]].doorRefrence.transform.GetChild(5).gameObject.SetActive(false);
            }
            if((floor.grid[playerTile.floorCord[0] , playerTile.floorCord[1]].doorRefrence.HasEnemy == true) && playerHasInstru == false) {
                MainManager.instance.takeDamage(damage);
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
                playerTile.transform.GetChild(2).gameObject.SetActive(false);
                playerTile.transform.GetChild(3).gameObject.SetActive(false);
                playerTile.transform.GetChild(4).gameObject.SetActive(false);
                playerTile = floor.grid[potentialX, potentialY];
                playerTile.hasPlayer = true;
                playerTile.transform.GetChild(pSprite).gameObject.SetActive(true);
            }
            if (playerHasInstru == true && floor.grid[potentialX, potentialY].HasEnemy == true)
            {
                floor.grid[potentialX, potentialY].transform.GetChild(5).gameObject.SetActive(false);
            }
            if (floor.grid[potentialX, potentialY].HasEnemy == true && playerHasInstru == false)
            {
                MainManager.instance.takeDamage(damage);
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



            int potentialX = enemyTiles[i].floorCord[0] + vec.x;
            int potentialY = enemyTiles[i].floorCord[1] + vec.y;
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
                ///checks if there is a player on potential tile
                if(floor.grid[potentialX , potentialY].hasPlayer == false) {
                    enemyTiles[i].HasEnemy = false;
                    enemyTiles[i].transform.GetChild(5).gameObject.SetActive(false);
                    enemyTiles[i] = floor.grid[potentialX , potentialY];
                    enemyTiles[i].HasEnemy = true;
                    enemyTiles[i].transform.GetChild(5).gameObject.SetActive(true);

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

    struct DijkstraTile {
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
         */


    }


    public Vector2Int pickDirection()
    {
        int[,] baseDirections = { { -1, 0 }, { 0, -1 }, { 1, 0 }, { 0, 1 } }; //[0 Up, 1 Left, 2 Down, 3 Right   ,   0 x, 1 y]
        int direction = Random.Range(0, 4) % 4;


        return new Vector2Int(baseDirections[direction, 0], baseDirections[direction, 1]);
    }
}




