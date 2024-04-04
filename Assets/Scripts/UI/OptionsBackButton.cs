using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsBackButton : MonoBehaviour
{
    /// <summary>
    /// Unloads the Options Screen
    /// </summary>
    public void backButton()
    {
        SceneManager.UnloadSceneAsync("OptionsMenu");

    }
}
