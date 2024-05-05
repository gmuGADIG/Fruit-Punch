using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

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
        SceneManager.LoadScene(SwitchScene.characterSelect);
    }
    /// <summary>
    /// Goes to Options Menu
    /// </summary>
    public void Options()
    {
        SceneManager.LoadSceneAsync(SwitchScene.optionsMenu);
    }
    /// <summary>
    /// Goes to the Codex
    /// </summary>
    public void Codex()
    {
        SceneManager.LoadSceneAsync(SwitchScene.codex);
    }
    /// <summary>
    /// Quits the game
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }
}
