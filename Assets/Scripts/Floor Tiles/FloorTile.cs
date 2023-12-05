using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTile : MonoBehaviour {
    public int playerDir = -1; //-1 no player, 0 Up, 1 Left, 2 Down, 3 Right 
    public int instrument = -1; //-1 no instrument, 0 Guitar, 1 Wind Pipes, 2 Harp, 3 Flute
    public int enemy = -1; //-1 no enemy, 0 snake
    public int item = -1; //-1 no item, 0 Heal Potion, 1 Crank Potion, 2 Hourglass
    public bool hasOswald = false;
    public bool hasItem = false; // True if any of the above are true or > -1

    public FloorGrid floorGrid;
    public Vector2Int gridPos;

    public FloorTile doorRefrence = null;
    public int doorRefrenceDir = -1;

    public void set(int w, int h, FloorGrid parent) {
        floorGrid = parent;
        gridPos = new Vector2Int(w , h);
    }

    public void hide(bool hide) {
        transform.GetChild(0).gameObject.SetActive(!hide); //Hide tile
        transform.GetChild(1).gameObject.SetActive(!hide); //Hide Player
        transform.GetChild(2).gameObject.SetActive(!hide); //Hide Instrument
        transform.GetChild(3).gameObject.SetActive(!hide); //Hide Enemies
        transform.GetChild(4).gameObject.SetActive(!hide); //Hide Items

        transform.GetChild(5).gameObject.SetActive(!hide && hasOswald); //Hide Oswald
    }

    //Add Sprites
    public void addPlayer(int id) {
        playerDir = id;
        transform.GetChild(1).GetChild(playerDir).gameObject.SetActive(true);

        hasItem = true;
    }

    public void addInstrument(int id) {
        instrument = id;
        transform.GetChild(2).GetChild(instrument).gameObject.SetActive(true);

        hasItem = true;
    }

    public void addEnemy(int id) {
        enemy = id;
        transform.GetChild(3).GetChild(enemy).gameObject.SetActive(true);

        hasItem = true;
    }

    public void addItem(int id) {
        item = id;
        transform.GetChild(4).GetChild(item).gameObject.SetActive(true);

        hasItem = true;
    }

    //Remove Sprites
    public void removePlayer() {
        transform.GetChild(1).GetChild(playerDir).gameObject.SetActive(false);
        playerDir = -1;

        hasItem = false;
    }

    public void removeInstrument() {
        transform.GetChild(2).GetChild(instrument).gameObject.SetActive(false);
        instrument = -1;

        hasItem = false;
    }

    public void removeEnemy() {
        transform.GetChild(3).GetChild(enemy).gameObject.SetActive(false);
        enemy = -1;

        hasItem = false;
    }

    public void removeItem() {
        transform.GetChild(4).GetChild(item).gameObject.SetActive(false);
        item = -1;

        hasItem = false;
    }
}
