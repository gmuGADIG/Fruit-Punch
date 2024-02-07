using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadPauseScreen : MonoBehaviour
{
    public string pauseScreen;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && Time.timeScale != 0.0f)
        {
            SceneManager.LoadSceneAsync(pauseScreen, LoadSceneMode.Additive);
        }
    }
}
