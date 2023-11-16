using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTile : MonoBehaviour {
    public bool hasPlayer;
    public bool HasInstru;
    public bool HasEnemy;

    public FloorGrid floorGrid;
    public int[] floorCord;

    public FloorTile doorRefrence = null;
    public int doorRefrenceDir = -1;
}
