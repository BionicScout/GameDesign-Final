using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public string mainMenu = "MainMenu";
    public void Menu()
    {
        SceneManager.LoadScene(mainMenu);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT");
        Application.Quit();

    }
}
