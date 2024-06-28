using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeOutUI : MonoBehaviour
{
    [SerializeField] Image fadeImage;
    [SerializeField] float fadeSpeed = .1f;

    string nextScene;

    public void FadeOutToNextScene(string nextScene)
    {
        this.nextScene = nextScene;
        StartCoroutine(FadeOutCoroutine());
    }

    IEnumerator FadeOutCoroutine()
    {
        while (fadeImage.color.a < 1)
        {
            yield return null;
            Color c = fadeImage.color;
            c.a = Mathf.Clamp(c.a + fadeSpeed * Time.deltaTime, 0, 1);
            fadeImage.color = c;
        }
        SceneManager.LoadScene(nextScene);
    }
}
