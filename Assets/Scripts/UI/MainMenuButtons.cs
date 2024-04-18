using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    [SerializeField]
    public string SelectScene;
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
    /// Quits the game
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }
}
