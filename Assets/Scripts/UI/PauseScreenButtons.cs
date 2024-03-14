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
    // Start is called before the first frame update
    void Start()
    {

        Time.timeScale = 0.0f;
        
        
    }

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
        Debug.Log("PauseScreen unloaded");
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
        SceneManager.LoadScene(0);
        Debug.Log("Return to Main");
    }
    /// <summary>
    /// Quits the Game
    /// </summary>
    public void QuitGame() 
    {
        //add script to save game
        Application.Quit();
        Debug.Log("Quit Game");
    }

    /// <summary>
    /// Opens Option menu (Options menu currently doesn't exist)
    /// </summary>
    public void OptionsMenu() //Change when options menu get created
    {
        SceneManager.LoadScene(OptionsScene);
        Debug.Log("Open Options Menu");
    }
}
