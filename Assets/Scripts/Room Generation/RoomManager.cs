using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class RoomManager : MonoBehaviour {
    public List<GameObject> roomList = new List<GameObject>();

    public void hide(bool hide) {
        for(int i = 1; i < roomList.Count; i++) {
            roomList[i].GetComponent<Room>().hide(hide);
        }
    }
}
