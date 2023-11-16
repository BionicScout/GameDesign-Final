using System.Collections.Generic;
using UnityEngine;

public class FloorGrid : MonoBehaviour{
    public int height, width;
    public int size;
    public FloorTile[,] grid; //[height, width]
    public GameObject tile;
    List<FloorTile> doorTiles;

    public FloorGrid(int h, int w, int s) {
        height = h;
        width = w;
        grid = new FloorTile[h, w];
    }

    public void set(int h , int w , int s) {
        height = h;
        width = w;
        grid = new FloorTile[h , w];
        size = s;
    }

    public void generateEmpty() {
        GameObject parent = new GameObject("Grid");
        grid = new FloorTile[width , height];

        for(int w  = 0; w < width; w++) {
            for(int h = 0; h < height; h++) {
                GameObject obj = Instantiate(tile, new Vector3(w * size , h * size , 0), Quaternion.identity);
                obj.transform.SetParent(parent.transform);
                obj.transform.localScale *= size;

                obj.AddComponent<FloorTile>();
                grid[w, h] = obj.GetComponent<FloorTile>();
                grid[w , h].floorCord = new int[2];
                grid[w , h].floorCord[0] = w;
                grid[w , h].floorCord[1] = h;
                grid[w , h].floorGrid = this;
            }
        }
    }

    public GameObject generateEmpty(GameObject parent) {
        grid = new FloorTile[width , height];

        for(int w = 0; w < width; w++) {
            for(int h = 0; h < height; h++) {
                float middle = Mathf.FloorToInt(width / 2f) + (((width+1)%2) * -0.5f);

                GameObject obj = Instantiate(tile , new Vector3((w * size) - middle , (h * size) - middle , 1) + parent.transform.position, Quaternion.identity);
                obj.transform.SetParent(parent.transform);
                obj.transform.localScale *= size;

                obj.AddComponent<FloorTile>();
                grid[w , h] = obj.GetComponent<FloorTile>();
                grid[w , h].floorCord = new int[2];
                grid[w , h].floorCord[0] = w;
                grid[w , h].floorCord[1] = h;
                grid[w , h].floorGrid = this;
            }
        }

        return parent;
    }

    public void addDoor(int direction , Material doorMat , FloorGrid adjRoomFloor) {
        FloorTile doorTile = addDoor(direction, doorMat);
        FloorTile adjDoor = adjRoomFloor.getdoorTile((direction + 2)%4);

        doorTile.doorRefrence = adjDoor;
        doorTile.doorRefrenceDir = direction;

        adjDoor.doorRefrence = doorTile;
        adjDoor.doorRefrenceDir = (direction + 2) % 4;

        Debug.Log("REFRENCE ADDED");
    }

    public FloorTile addDoor(int direction, Material doorMat) {

        FloorTile doorTile = getdoorTile(direction);

        Renderer doorEdge = doorTile.transform.GetChild(0).GetChild(direction).gameObject.GetComponent<Renderer>();
        doorEdge.material = doorMat;
        doorTile.transform.GetChild(0).GetChild(direction).position += new Vector3(0, 0, -2);

        return doorTile;
    }

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

    public void addPlayer() {
        grid[0, 0].hasPlayer = true;
        grid[0,0].transform.GetChild(1).gameObject.SetActive(true);
        FindObjectOfType<PlayerMovement>().playerTile = grid[0, 0];
    }
}