using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreenButtons : MonoBehaviour
{
    public string mainMenuScene;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0.0f;
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape)) //Replace when input system commpleted
        { Resume(); }
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
        SceneManager.LoadScene(mainMenuScene);
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
        Debug.Log("Open Options Menu");
    }
}
