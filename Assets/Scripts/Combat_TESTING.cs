using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Combat_TESTING : MonoBehaviour {
    public FloorGrid grid;

    
    private void Start() {
        grid.generateEmpty();

        GameObject playerObject = new GameObject();
        playerObject.AddComponent<PlayerMovement>();
        playerObject.GetComponent<PlayerMovement>().playerTile = grid.grid[0 , 0];
        grid.grid[0 , 0].hasPlayer = true;
        grid.grid[0 , 0].transform.GetChild(0).gameObject.SetActive(true);
    }

}
