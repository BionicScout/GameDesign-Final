using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Testing : MonoBehaviour {
    public FloorGrid grid;
    public RoomGeneration roomGen;

    private void Start() {
        //grid.generateEmpty();

        //GameObject playerObject = new GameObject();
        //playerObject.AddComponent<PlayerMovement>();
        //playerObject.GetComponent<PlayerMovement>().playerTile = grid.grid[0 , 0];
        //grid.grid[0 , 0].hasPlayer = true;
        //grid.grid[0, 0].transform.GetChild(0).gameObject.SetActive(true);

        roomGen.generate();

        

        GameObject parent = new GameObject("Grid");
        grid.grid = new FloorTile[grid.width , grid.height];

        for(int w = 0; w < grid.width; w++) {
            for(int h = 0; h < grid.height; h++) {
                if(roomGen.roomLayout[w, h] != -1) {
                    GameObject obj = Instantiate(grid.tile , new Vector3((w - 11) * grid.size, (h - 11) * grid.size, 0) , Quaternion.identity);
                    obj.transform.SetParent(parent.transform);
                    obj.transform.localScale *= grid.size;

                    obj.AddComponent<FloorTile>();
                    grid.grid[w , h] = obj.GetComponent<FloorTile>();
                    grid.grid[w , h].floorCord = new int[2];
                    grid.grid[w , h].floorCord[0] = w;
                    grid.grid[w , h].floorCord[1] = h;
                    grid.grid[w , h].floorGrid = grid;
                }
            }
        }


    }


}
