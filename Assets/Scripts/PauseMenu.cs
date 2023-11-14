using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public string mainMenu = "MainMenu";
    public GameObject pauseMenu;
    public bool isPaused;
    void Start()
    {
       pauseMenu.SetActive(false);
    }
    void Update()
    {
       if(Input.GetKeyDown(KeyCode.Escape))
       {
            PauseGame();
       } 
    }
    public void PauseGame()
    {
        if (isPaused)
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
        }
        else
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
        }
    }
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
