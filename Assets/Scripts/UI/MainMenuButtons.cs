using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    public string OptionsScene;

    /// <summary>
    /// Start Game -> Goes to Character Select Screen
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    /// <summary>
    /// Goes to Options Menu
    /// </summary>
    public void Options()
    {
        SceneManager.LoadSceneAsync(OptionsScene, LoadSceneMode.Additive);
    }
    /// <summary>
    /// Quits the game
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }
}
