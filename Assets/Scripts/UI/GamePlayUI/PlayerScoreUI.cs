using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerScoreUI : MonoBehaviour
{
    [SerializeField] TMP_Text scoreText;
    [SerializeField] float updateInterval = .1f;
    [SerializeField] Color defaultColor;
    [SerializeField] Color losingPointsColor;
    [SerializeField] Color gainingPointsColor;

    PlayerScore playerScore;
    int targetScore;
    int currentScore;

    public void Setup(Player player)
    {
        playerScore = player.GetComponent<PlayerScore>();
        playerScore.OnUpdateScoreAndRank += UpdateScore;
        scoreText.color = defaultColor;
        scoreText.text = playerScore.GetScore().ToString() + " pts";
        currentScore = playerScore.GetScore();
    }

    private void UpdateScore(bool animate)
    {
        targetScore = playerScore.GetScore();
        if (gameObject.activeInHierarchy)
        {
            if (animate) {
                StartCoroutine(UpdateScoreCoroutine());
            } else {
                currentScore = targetScore;
                scoreText.text = currentScore.ToString() + " pts";
            }
        }
    }

    IEnumerator UpdateScoreCoroutine()
    {
        if (currentScore > targetScore)
        {
            scoreText.color = losingPointsColor;
        }
        else
        {
            scoreText.color = gainingPointsColor;
        }
        while (currentScore != targetScore)
        {
            yield return new WaitForSeconds(updateInterval);
            if (currentScore > targetScore)
            {
                currentScore -= 1;
            }
            else if (currentScore < targetScore)
            {
                currentScore += 1;
            }
            scoreText.text = currentScore.ToString() + " pts";
        }
        scoreText.color = defaultColor;
    }
}
