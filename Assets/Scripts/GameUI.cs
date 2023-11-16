using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameUI : MonoBehaviour {
    public GameObject UI_Obj;
    UnityEngine.UI.Slider slider;
    TMP_Text text;

    private void Start() {
        slider = UI_Obj.transform.GetChild(0).GetComponent<UnityEngine.UI.Slider>();
        text = UI_Obj.transform.GetChild(1).GetComponent<TMP_Text>();
    }

    public void updateCrank(float currentCrank) {
        slider.value = currentCrank/MainManager.instance.maxCrankiness;
    }

    public void updateScore(int score) {
        text.text = "Score: " + score + "/" + MainManager.instance.maxScore;
    }
}
