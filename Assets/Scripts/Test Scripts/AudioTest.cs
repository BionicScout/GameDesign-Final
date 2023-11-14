using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTest : MonoBehaviour {
    void Update() {
        if(Input.GetKeyDown(KeyCode.T)) {
            AudioManager.instance.Play("Move");
        }
        if(Input.GetKeyDown(KeyCode.P)) {
            AudioManager.instance.Play("Knife");
        }

    }
}
