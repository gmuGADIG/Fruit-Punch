using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveOpenSceneNames : MonoBehaviour
{
    string LastSceneName;
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    void Start()
    {
        LastSceneName = SceneManager.GetActiveScene().name;
    }

    public string getLastSceneName()
    {
        return LastSceneName;
    }

    
}
