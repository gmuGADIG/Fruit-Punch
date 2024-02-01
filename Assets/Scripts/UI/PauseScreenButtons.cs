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
        if(Input.GetKeyUp(KeyCode.Escape))
        { Resume(); }
    }
    public void Resume() //closes pause screen resumes game
    {
        Time.timeScale = 1.0f;
        SceneManager.UnloadSceneAsync("PauseScreen");
        Debug.Log("PauseScreen unloaded");
    }

    public void RestartLevel() //restarts level
    {
        for(int i = 0; i < SceneManager.sceneCount; i++)
        {
            if(SceneManager.GetSceneAt(i).name != "PauseScreen")
            {
                SceneManager.LoadSceneAsync(SceneManager.GetSceneAt(i).name);
            }
            Time.timeScale = 1.0f;
            SceneManager.UnloadSceneAsync("PauseScreen");
            Debug.Log("RestartedLevel");
        }
    }

    public void MainMenu() //returns to mainMenu
    {
        SceneManager.LoadScene(mainMenuScene);
        Debug.Log("Return to Main");
    }
    public void QuitGame() //Quits the game
    {
        //add script to save game
        Application.Quit();
        Debug.Log("Quit Game");
    }

    public void OptionsMenu() //Opens options menu
    {
        Debug.Log("Open Options Menu");
    }
}
