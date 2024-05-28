using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class PauseScreenButtons : MonoBehaviour
{

    public string OptionsScene;

    /// <summary>
    /// Pause key hit, Resume game
    /// </summary>
    public void OnPause()
    {
        Resume();
    }

    /// <summary>
    /// Closes pause screen, Resumes game
    /// </summary>
    public void Resume() 
    {
        Time.timeScale = 1.0f;
        SceneManager.UnloadSceneAsync("PauseScreen");
    }

    /// <summary>
    /// Restarts Level
    /// </summary>
    public void RestartLevel()
    {
        Scene currScene = SceneManager.GetActiveScene();
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(currScene.buildIndex);
    }
    /// <summary>
    /// Returns to Main menu
    /// </summary>
    public void MainMenu()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
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
        SceneManager.LoadSceneAsync(OptionsScene, LoadSceneMode.Additive);
    }
}
