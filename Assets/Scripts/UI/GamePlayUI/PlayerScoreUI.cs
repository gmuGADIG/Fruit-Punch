using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerScoreUI : MonoBehaviour
{
    [SerializeField] TMP_Text scoreText;

    PlayerScore playerScore;

    public void Setup(Player player)
    {
        playerScore = player.GetComponent<PlayerScore>();
        playerScore.OnUpdateScoreAndRank += UpdateScore;
        scoreText.text = playerScore.GetScore().ToString();
    }

    private void UpdateScore()
    {
        scoreText.text = playerScore.GetScore().ToString();
    }
}
