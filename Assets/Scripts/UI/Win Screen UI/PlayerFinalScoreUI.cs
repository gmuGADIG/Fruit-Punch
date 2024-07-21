using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class PlayerFinalScoreUI : MonoBehaviour
{
    [SerializeField] TMP_Text[] levelScoreTexts;
    [SerializeField] TMP_Text finalScoreText;
    //[SerializeField] TMP_Text rankingText;

    [SerializeField] int playerIndex = 0;
    [Header("Animation Params")]
    [SerializeField] float initialDelay = 2;
    [SerializeField] float scoreCountUpDuration = 8f;

    ScorePersister playerScores;

    private void Start()
    {
        playerScores = ScorePersister.Instance;

        for (int i = 0; i < levelScoreTexts.Length; i++)
        {
            levelScoreTexts[i].gameObject.SetActive(false);
        }
        finalScoreText.gameObject.SetActive(false);
        //rankingText.gameObject.SetActive(false);
        StartCoroutine(DisplayCoroutine());
    }

    IEnumerator DisplayCoroutine()
    {
        yield return new WaitForSeconds(initialDelay); // initial delay
        for (int levelIndex = 0; levelIndex < levelScoreTexts.Length; levelIndex++)
        {
            levelScoreTexts[levelIndex].gameObject.SetActive(true);
            levelScoreTexts[levelIndex].text = "0";
            float startingScore = 0;
            int targetScore = playerScores.playerLevelScores[playerIndex][levelIndex];
            float velocity = targetScore / scoreCountUpDuration;

            float timeRemaining = scoreCountUpDuration;
            while (timeRemaining > 0f)
            {
                startingScore = Mathf.MoveTowards(
                        startingScore,
                        targetScore,
                        velocity * Time.deltaTime
                );
                levelScoreTexts[levelIndex].text = ((int)startingScore).ToString();

                timeRemaining -= Time.deltaTime;

                yield return null;
            }

            startingScore = targetScore;
            levelScoreTexts[levelIndex].text = ((int)startingScore).ToString();
        }

        finalScoreText.gameObject.SetActive(true);
        int finalScore = 0;
        for (int i = 0; i < 3; i++)
        {
            finalScore += playerScores.playerLevelScores[playerIndex][i];
        }
        finalScoreText.text = finalScore.ToString();
        //rankingText.gameObject.SetActive(true);
        //rankingText.text = playerScore.GetRank();
        playerScores.Clear();
    }
}
