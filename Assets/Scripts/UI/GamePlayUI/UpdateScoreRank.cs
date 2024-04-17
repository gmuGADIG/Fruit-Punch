using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateScoreRank : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI rankText;

    public PlayerScore myPlayerScore;

    private void OnEnable()
    {
        myPlayerScore.updateScoreAndRank += updateScore;
        myPlayerScore.updateScoreAndRank += updateRank;
    }

    void updateScore()
    {
        scoreText.text = myPlayerScore.getScore().ToString();
    }

    void updateRank()
    {
        rankText.text = myPlayerScore.getRank();   
    }

}
