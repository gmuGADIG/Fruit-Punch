using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    [SerializeField]
    public string SelectScene;
    [SerializeField] 
    public string CodexScene;
    [SerializeField]
    public string OptionsScene;

    /// <summary>
    /// Start Game -> Goes to Character Select Screen
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene(SwitchScene.switchScene.CharacterSelect);
    }
    /// <summary>
    /// Goes to Options Menu
    /// </summary>
    public void Options()
    {
        SceneManager.LoadSceneAsync(SwitchScene.switchScene.Options, LoadSceneMode.Additive);
    }
    /// <summary>
    /// Goes to the Codex
    /// </summary>
    public void Codex()
    {
        SceneManager.LoadScene(SwitchScene.switchScene.Codex);
    }
    /// <summary>
    /// Quits the game
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }
}
