using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class FloorGrid : MonoBehaviour{
    public int height, width;
    public int size;
    public FloorTile[,] grid; //[height, width]
    public GameObject tile;
    List<FloorTile> doorTiles;

    public void set(int h , int w , int s) {
        height = h;
        width = w;
        grid = new FloorTile[h , w];
        size = s;
    }

    //Create Room with all Room Tiles
    public GameObject generateEmpty(GameObject parent) {
        grid = new FloorTile[width , height];

        for(int w = 0; w < width; w++) {
            for(int h = 0; h < height; h++) {
                //Instantiate tile and adjust properties
                float middle = Mathf.FloorToInt(width / 2f) + (((width+1)%2) * -0.5f);

                GameObject obj = Instantiate(tile , new Vector3((w * size) - middle , (h * size) - middle , 1) + parent.transform.position, Quaternion.identity);
                obj.transform.SetParent(parent.transform);
                obj.transform.localScale *= size;

                //Add Floor
                obj.AddComponent<FloorTile>();
                grid[w , h] = obj.GetComponent<FloorTile>();
                grid[w , h].set(w, h, this);
            }
        }

        return parent;
    }

    //Links two doors tiles
    public void addDoor(int direction , Material doorMat , FloorGrid adjRoomFloor) {
        //Get the tiles connecting the foors
        FloorTile doorTile = addDoor(direction, doorMat);
        FloorTile adjDoor = adjRoomFloor.getdoorTile((direction + 2)%4);

        //Set each tile to refrence the other
        doorTile.doorRefrence = adjDoor;
        doorTile.doorRefrenceDir = direction;

        adjDoor.doorRefrence = doorTile;
        adjDoor.doorRefrenceDir = (direction + 2) % 4;
    }

    //Adds door to tile and returns that tile
    public FloorTile addDoor(int direction, Material doorMat) {

        FloorTile doorTile = getdoorTile(direction);

        Renderer doorEdge = doorTile.transform.GetChild(0).GetChild(direction).gameObject.GetComponent<Renderer>();
        doorEdge.material = doorMat;
        doorTile.transform.GetChild(0).GetChild(direction).position += new Vector3(0, 0, -2);

        return doorTile;
    }

    //Get tile connected to a door based of direction
    public FloorTile getdoorTile(int direction) {
        FloorTile doorTile;
        int middle = Mathf.FloorToInt(width / 2f);

        if(direction == 0) //Up
            doorTile = grid[middle , height - 1];
        else if(direction == 1) //Left
            doorTile = grid[0 , middle];
        else if(direction == 2) //Down
            doorTile = grid[middle , 0];
        else //Right
            doorTile = grid[height - 1 , middle];

        return doorTile;
    }

    //Spawn player in bottom left of a room
    public void addPlayer() {
        grid[0, 0].hasPlayer = true;
        grid[0,0].transform.GetChild(1).gameObject.SetActive(true);
        FindObjectOfType<PlayerMovement>().playerTile = grid[0, 0];
        FindObjectOfType<PlayerMovement>().setCamera();
    }

    //Spawn Oswald in top right of room
    public void addOswald() {
        grid[width - 4 , height - 4].hasOswald = true;
        grid[width - 4 , height - 4].transform.GetChild(6).gameObject.SetActive(true);
    }

    //Get Random tile in this room that is not in front of a door
    public FloorTile GetRandTile()
    {
        //Get Random Tile
        FloorTile temp = grid[Random.Range(0, width), Random.Range(0, height)];

        //If on a potential door tile, pick another
        if(temp.doorRefrenceDir != -1) {
            temp = GetRandTile();
        }

        return temp;
    }

    //Hide All tiles
    public void hide(bool hide) {
        for(int w = 0; w < width; w++) {
            for(int h = 0; h < height; h++) {
                grid[h , w].hide(hide);
            }
        }
    }
}