using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreenButtons : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0.0f;
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
}
