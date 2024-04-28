using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PostLevelUIButtons : MonoBehaviour
{
    public string mainMenuScene;
    
    public void MainMenuButtons()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(FindObjectOfType<LoadPostLevelUI>().nextLevelName); //Gets next level name from the level loader
    }
}
