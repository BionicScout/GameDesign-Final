using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour {
    //Oswald
    float crankiness;
    public int maxCrankiness;

    int score;
    public int maxScore;

    public int curHealth;
    public int maxHealth = 10;
    public HealthBar healthBar;

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
        curHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
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

    public void takeDamage(int dmg)
    {
        if(dmg >= curHealth)
        {
            SceneSwitcher.instance.A_LoadScene("Fail-Death");
        }
        curHealth -= dmg;
        healthBar.SetHealth(curHealth);
    }

}
