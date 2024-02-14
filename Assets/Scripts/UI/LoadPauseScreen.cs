using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadPauseScreen : MonoBehaviour
{
    public string pauseScreen;
    
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && Time.timeScale != 0.0f) //Update when input system gets finished
        {
            SceneManager.LoadSceneAsync(pauseScreen, LoadSceneMode.Additive);
        }
    }
}
