using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LoadPauseScreen : MonoBehaviour
{

    public string pauseScreen;
    /// <summary>
    /// Pause key hit, pause game
    /// </summary>
    public void OnPause()
    {
        if( Time.timeScale != 0)
        {
            SceneManager.LoadSceneAsync(pauseScreen, LoadSceneMode.Additive);
        }

    }
}
