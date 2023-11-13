using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour {
    public static SceneSwitcher instance;

    public static string currentScene;
    public static bool firstMapLoad = true;

    void Awake() {
        if(instance == null)
            instance = this;
        else {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        currentScene = SceneManager.GetActiveScene().name;
        //Debug.Log(currentScene);
    }

    public void A_ExitButton() {
        Application.Quit();
    }

    public void A_LoadScene(string sceneName) {
    //Switch Scene
        SceneManager.LoadScene(sceneName);
        currentScene = sceneName;
    }




    public string GetCurrentScene() {
        return currentScene;
    }


}
