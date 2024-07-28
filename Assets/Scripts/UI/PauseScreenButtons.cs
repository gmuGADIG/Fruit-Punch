using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class PauseScreenButtons : MonoBehaviour
{
    public static event Action OnLevelRestart;

    public string OptionsScene;
    private PauseManager pauseManager;

    private void Start()
    {
        pauseManager = GameObject.Find("PauseManager").GetComponent<PauseManager>();
    }

    /// <summary>
    /// Closes pause screen, Resumes game
    /// </summary>
    public void Resume() 
    {
        pauseManager.OnBack();
    }

    /// <summary>
    /// Restarts Level
    /// </summary>
    public void RestartLevel()
    {
        Scene currScene = SceneManager.GetActiveScene();
        Time.timeScale = 1.0f;
        OnLevelRestart?.Invoke();
        SceneManager.LoadScene(currScene.buildIndex);
    }
    /// <summary>
    /// Returns to Main menu
    /// </summary>
    public void MainMenu()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SwitchScene.mainMenu);
    }
    /// <summary>
    /// Quits the Game
    /// </summary>
    public void QuitGame() 
    {
        //add script to save game
        Application.Quit();
    }

    /// <summary>
    /// Opens Option menu (Options menu currently doesn't exist)
    /// </summary>
    public void OptionsMenu() 
    {
        pauseManager.PushSubmenu(OptionsScene);
    }
}
