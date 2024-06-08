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
        myPlayerScore.OnUpdateScoreAndRank += updateScore;
        myPlayerScore.OnUpdateScoreAndRank += updateRank;
    }

    void updateScore()
    {
        scoreText.text = myPlayerScore.GetScore().ToString();
    }

    void updateRank()
    {
        rankText.text = myPlayerScore.GetRank();   
    }

}
