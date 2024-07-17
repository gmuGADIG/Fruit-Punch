using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] string targetScene;

    public void ChangeScene()
    {
        SceneManager.LoadScene(targetScene);
    }
}