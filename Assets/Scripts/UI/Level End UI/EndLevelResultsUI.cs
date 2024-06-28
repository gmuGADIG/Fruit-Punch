using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevelResultsUI : MonoBehaviour
{
    [SerializeField] GameObject[] elementsToHide;
    [SerializeField] GameObject menu;
    [SerializeField] GameObject continueButton;
    [SerializeField] float continueButtonShowDelay = 5f;
    [SerializeField] float menuShowDelay = 2f;

    private void Awake()
    {
        menu.SetActive(false);
        continueButton.SetActive(false);
    }

    public void ShowMenu()
    {
        var playerScores = FindObjectsOfType<PlayerScore>();
        foreach (var score in playerScores)
        {
            score.StopScoreCountdown();
        }
        StartCoroutine(ShowMenuCoroutine());
    }

    IEnumerator ShowMenuCoroutine()
    {
        yield return new WaitForSeconds(menuShowDelay);
        for (int i = 0; i < elementsToHide.Length; i++)
        {
            elementsToHide[i].SetActive(false);
        }
        menu.SetActive(true);
        var scoreResults = GetComponentsInChildren<PlayerScoreResultsUI>();
        foreach (var result in scoreResults)
        {
            result.DisplayResults();
        }
        StartCoroutine(ShowContinueButton());
    }

    IEnumerator ShowContinueButton()
    {
        yield return new WaitForSeconds(continueButtonShowDelay);
        continueButton.SetActive(true);
    }

    public void Continue()
    {
        FindAnyObjectByType<GameSceneManager>().GoToNextScene();
    }
}
