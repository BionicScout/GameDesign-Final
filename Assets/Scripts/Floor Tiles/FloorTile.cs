using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTile : MonoBehaviour {
    public bool hasPlayer;
    public bool HasInstru;
    public bool HasEnemy;
    public bool hasOswald;

    public FloorGrid floorGrid;
    public int[] floorCord;

    public FloorTile doorRefrence = null;
    public int doorRefrenceDir = -1;

    public void hide(bool hide) {
        transform.GetChild(0).gameObject.SetActive(!hide); //Hide tile
        transform.GetChild(1).gameObject.SetActive(!hide);
        transform.GetChild(2).gameObject.SetActive(!hide);
        transform.GetChild(3).gameObject.SetActive(!hide);
        transform.GetChild(4).gameObject.SetActive(!hide);

        //if(!hide) {

        //    transform.GetChild(1).gameObject.SetActive(!hide && hasPlayer);
        //    transform.GetChild(2).gameObject.SetActive(!hide && HasInstru);
        //    transform.GetChild(3).gameObject.SetActive(!hide && HasEnemy);
        //    transform.GetChild(4).gameObject.SetActive(!hide && hasOswald);
        //}
    }
}
