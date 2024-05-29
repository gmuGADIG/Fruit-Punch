using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    private int delayCount;
    [Tooltip("Number of frames to wait after unpause before allowing pausing")]
    [SerializeField] int frameDelay;

    private void Start() {
        delayCount = frameDelay;
    }

    private void Update() {
        if (Time.timeScale != 0 && delayCount > 0) {
            delayCount--;
        }
    }

    /// <summary>
    /// Called by players to pause the game
    /// </summary>
    public void Pause() {
        if (delayCount == 0) {;
            Time.timeScale = 0;
            delayCount = frameDelay;
            SceneManager.LoadSceneAsync(SwitchScene.pauseMenu, LoadSceneMode.Additive);
        }
    }
}
