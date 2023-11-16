using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour {
    //Oswald
    float crankiness;
    public int maxCrankiness;

    int score;
    public int maxScore;

    public GameUI UI; 

    public static MainManager instance;
    void Awake() {
        if(instance == null) {
            instance = this;
            Debug.Log("INSTANCE MADE");
        }
        else {
            Destroy(gameObject);
            return;
        }
    }

    public void addCrank(float num) {
        crankiness += num;

        if(crankiness >= maxCrankiness) {
            SceneSwitcher.instance.A_LoadScene("Fail-Cranky");         
        }
        UI.updateCrank(crankiness);
    }

    public float getCrank() {
        return crankiness;
    }

    public void addScore(int num) {
        score += num;

        if(score >= maxScore) {
            SceneSwitcher.instance.A_LoadScene("Win");
        }
        UI.updateScore(score);
    }

    public int getScore() {
        return score;
    }
}
