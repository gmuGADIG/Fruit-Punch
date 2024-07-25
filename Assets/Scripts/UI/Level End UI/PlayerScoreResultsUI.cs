using System.Linq;
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
    [SerializeField] float initialDelay = 2;
    [SerializeField] float scoreCountUpDuration = 8f;

    #nullable enable
    PlayerScore? playerScore;

    private void Start()
    {
        playerScore = FindObjectsOfType<Player>()
            .Where(p => p.PlayerNum == playerNum)
            .FirstOrDefault()?
            .GetComponent<PlayerScore>();

        if (playerScore == null) {
            Destroy(gameObject);
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
        yield return new WaitForSeconds(initialDelay); // initial delay
        scoreNumber.gameObject.SetActive(true);
        scoreNumber.text = "0";
        float startingScore = 0;
        int target = playerScore.GetScore();
        float velocity = target / scoreCountUpDuration;

        float timeRemaining = scoreCountUpDuration;
        while (timeRemaining > 0f)
        {
            startingScore = Mathf.MoveTowards(
                    startingScore, 
                    target, 
                    velocity * Time.deltaTime
            );
            scoreNumber.text = ((int)startingScore).ToString();

            timeRemaining -= Time.deltaTime;

            yield return null;
        }

        startingScore = target;
        scoreNumber.text = ((int)startingScore).ToString();

        ranking.gameObject.SetActive(true);
        ranking.text = playerScore.GetRank();
    }
}
