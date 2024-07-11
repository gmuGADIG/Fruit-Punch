using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevelResultsUI : MonoBehaviour
{
    [SerializeField] GameObject[] elementsToHide;
    [SerializeField] GameObject menu;
    [SerializeField] GameObject continueButton;
    [SerializeField] float continueButtonShowDelay = 10f;
    [SerializeField] float menuShowDelay = 2f;

    [SerializeField] GameObject vegetableMusicPlayer;
    [SerializeField] GameObject rottenMusicPlayer;
    [SerializeField] GameObject bogMusicPlayer;
    [SerializeField] GameObject ripeMusicPlayer;
    [SerializeField] GameObject freshMusicPlayer;

    private void Awake()
    {
        menu.SetActive(false);
        continueButton.SetActive(false);
    }

    public void ShowMenu()
    {
        var playerScores = FindObjectsOfType<PlayerScore>();

        ScorePersister.Instance.Clear(); // reset the scores after showing

        foreach (var score in playerScores)
        {
            score.StopScoreCountdown();
        }

        StartCoroutine(ShowMenuCoroutine());
    }

    IEnumerator ShowMenuCoroutine()
    {
        yield return new WaitForSeconds(menuShowDelay);

        var rank = FindObjectsOfType<PlayerScore>()
            .OrderBy(score => -score.GetScore()) // orderby is ascending
            .First()
            .GetRank();

        try {
            #pragma warning disable CS8509
            GameObject musicPlayer = rank switch {
                "Fresh" => freshMusicPlayer,
                "Ripe" => ripeMusicPlayer,
                "Bog-Standard" => bogMusicPlayer,
                "Rotten" => rottenMusicPlayer,
                "Vegetable" => vegetableMusicPlayer
            };
            musicPlayer.SetActive(true);
        } catch (System.Runtime.CompilerServices.SwitchExpressionException) {
            Debug.LogError("rank = " + rank);
            throw;
        }

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
