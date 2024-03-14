using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsBackButton : MonoBehaviour
{
    public void backButton()
    {
        string lastScene = FindObjectOfType<SaveOpenSceneNames>().getLastSceneName();
        Destroy(FindObjectOfType<SaveOpenSceneNames>().gameObject);
        SceneManager.LoadScene(lastScene);  
    }
}
