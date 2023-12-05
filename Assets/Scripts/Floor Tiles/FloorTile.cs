using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTile : MonoBehaviour {
    public bool hasPlayer;
    public bool HasInstru;
    public bool HasEnemy;
    public bool hasOswald;
    public bool hasHealPotion;
    public bool hasCrankPotion;
    public bool hasTeleport;

    public FloorGrid floorGrid;
    public int[] floorCord;

    public FloorTile doorRefrence = null;
    public int doorRefrenceDir = -1;

    public Vector2Int gridPos;

    public void set(int w, int h, FloorGrid parent) {
        floorCord = new int[2];
        floorCord[0] = w;
        floorCord[1] = h;
        floorGrid = parent;
        gridPos = new Vector2Int(w , h);
    }

    public void hide(bool hide) {
        transform.GetChild(0).gameObject.SetActive(!hide); //Hide tile
        transform.GetChild(1).gameObject.SetActive(!hide);
        transform.GetChild(2).gameObject.SetActive(!hide);
        transform.GetChild(3).gameObject.SetActive(!hide);
        transform.GetChild(4).gameObject.SetActive(!hide);
        transform.GetChild(5).gameObject.SetActive(!hide);
        transform.GetChild(6).gameObject.SetActive(!hide);
        transform.GetChild(7).gameObject.SetActive(!hide);

        if(!hide) {

            transform.GetChild(1).gameObject.SetActive(!hide && hasPlayer);
            transform.GetChild(2).gameObject.SetActive(!hide && HasInstru);
            transform.GetChild(3).gameObject.SetActive(!hide && HasEnemy);
            transform.GetChild(4).gameObject.SetActive(!hide && hasOswald);
            transform.GetChild(5).gameObject.SetActive(!hide && hasHealPotion);
            transform.GetChild(6).gameObject.SetActive(!hide && hasCrankPotion);
            transform.GetChild(7).gameObject.SetActive(!hide && hasTeleport);
        }
    }
}
