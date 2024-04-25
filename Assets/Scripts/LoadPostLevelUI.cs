using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class LoadPostLevelUI : MonoBehaviour
{
    public string PostUI;

    public string nextLevelName; //holds to pass to Load next level button on Post UI Screen

    PlayableDirector transition;
    private void Start()
    {
        transition = GetComponent<PlayableDirector>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            transition.Play();
            Invoke("loadPostUI", 0.8f);
        }
    }

    public void loadPostUI()
    {
        SceneManager.LoadSceneAsync(PostUI, LoadSceneMode.Additive);
    }

}
