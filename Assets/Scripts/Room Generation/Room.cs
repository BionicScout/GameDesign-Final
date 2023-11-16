using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {
    public FloorGrid floor;

    private void Start() {
        floor = GetComponent<FloorGrid>();
    }

    public void hide(bool hide) {
        floor.hide(hide);
        transform.GetChild(1).gameObject.SetActive(!hide);
    }
}
