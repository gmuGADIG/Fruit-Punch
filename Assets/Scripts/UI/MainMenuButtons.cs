using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    public string playScene;
    public string codexScene;
    public string optionsScene;

    /// <summary>
    /// Start Game -> Goes to Character Select Screen
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene(SwitchScene.controlsMenu);
    }

    /// <summary>
    /// Goes to Options Menu
    /// </summary>
    public void Options()
    {
        SceneManager.LoadSceneAsync(SwitchScene.optionsMenu, LoadSceneMode.Additive);
    }

    /// <summary>
    /// Goes to the Codex
    /// </summary>
    public void Codex()
    {
        SceneManager.LoadSceneAsync(SwitchScene.codex);
    }

    /// <summary>
    /// Goes to Credits Scene
    /// </summary>
    public void Credits()
    {
        SceneManager.LoadScene(SwitchScene.credits);
    }

    /// <summary>
    /// Quits the game
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }
}
