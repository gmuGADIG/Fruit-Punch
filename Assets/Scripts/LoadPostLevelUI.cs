using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadPostLevelUI : MonoBehaviour
{
    public string PostUI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            SceneManager.LoadSceneAsync(PostUI, LoadSceneMode.Additive);
        }
    }

}
