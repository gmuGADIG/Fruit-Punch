using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    private int delayCount;
    [Tooltip("Number of frames to wait after unpause before allowing pausing")]
    [SerializeField] int frameDelay;

    Stack<string> menuStack = new Stack<string>();
    public bool Paused { get => menuStack.Count > 0; }
    public void PushSubmenu(string scene)
    {
       menuStack.Push(scene);
       var loadOperation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
    }

    public void OnBack()
    {
        if (menuStack.Count > 0)
        {
            var topMenu = menuStack.Pop();
            SceneManager.UnloadSceneAsync(topMenu);
        }
        if (menuStack.Count == 0)
        {
            Unpause();
        }
    }
    private void Unpause()
    {
        Time.timeScale = 1.0f;
    }
    private void Start() {
        delayCount = frameDelay;
    }

    private void Update() {
        if (Time.timeScale != 0 && delayCount > 0) {
            delayCount--;
        }
    }

    /// <summary>
    /// Called by the player when the pause button is pressed.
    /// </summary>
    public void OnPause()
    {
        if (Paused)
        {
            OnBack();
        } else
        {
            Pause();
        }
    }

    /// <summary>
    /// Called by players to pause the game
    /// </summary>
    public void Pause() {
        if (delayCount == 0) {;
            Time.timeScale = 0;
            delayCount = frameDelay;
            PushSubmenu(SwitchScene.pauseMenu);
        }
    }
}
