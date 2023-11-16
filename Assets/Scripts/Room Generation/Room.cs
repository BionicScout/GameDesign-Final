using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {
    public FloorGrid floor;

    private void Start() {
        floor = GetComponent<FloorGrid>();
    }
}
