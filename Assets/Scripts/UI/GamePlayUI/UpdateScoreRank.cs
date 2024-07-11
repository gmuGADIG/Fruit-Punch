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

    void updateScore(bool _animate)
    {
        scoreText.text = myPlayerScore.GetScore().ToString();
    }

    void updateRank(bool _animate)
    {
        rankText.text = myPlayerScore.GetRank();   
    }

}
