using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Testing : MonoBehaviour {
    public RoomGeneration roomGen;

    private void Start() {
        //grid.generateEmpty();

        //GameObject playerObject = new GameObject();
        //playerObject.AddComponent<PlayerMovement>();
        //playerObject.GetComponent<PlayerMovement>().playerTile = grid.grid[0 , 0];
        //grid.grid[0 , 0].hasPlayer = true;
        //grid.grid[0, 0].transform.GetChild(0).gameObject.SetActive(true);

        roomGen.generate();
        roomGen.generatetiles();

      


    }


}
