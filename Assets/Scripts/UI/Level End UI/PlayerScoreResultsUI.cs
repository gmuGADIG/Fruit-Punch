using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerScoreResultsUI : MonoBehaviour
{
    [SerializeField] TMP_Text scoreNumber;
    [SerializeField] TMP_Text ranking;
    [SerializeField] int playerNum = 0;
    [Header("Animation Params")]
    [SerializeField] float initialDelay = 1;
    [SerializeField] float scoreCountUpDelay = .1f;
    [SerializeField] float rankingDelay = 2f;

    PlayerScore playerScore;

    private void Start()
    {
        Player[] players = FindObjectsOfType<Player>();
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].PlayerNum == playerNum)
            {
                playerScore = players[i].GetComponent<PlayerScore>();
            }
        }
        scoreNumber.gameObject.SetActive(false);
        ranking.gameObject.SetActive(false);
    }

    public void DisplayResults()
    {
        StartCoroutine(DisplayCoroutine());
    }

    IEnumerator DisplayCoroutine()
    {
        yield return new WaitForSeconds(1); // initial delay
        scoreNumber.gameObject.SetActive(true);
        scoreNumber.text = "0";
        int startingScore = 0;
        if (playerScore != null)
        {
            while (startingScore != playerScore.GetScore())
            {
                startingScore += 1;
                yield return new WaitForSeconds(scoreCountUpDelay);
                scoreNumber.text = startingScore.ToString();
            }
            yield return new WaitForSeconds(rankingDelay);
            ranking.gameObject.SetActive(true);
            ranking.text = playerScore.GetRank();
        }
    }
}
