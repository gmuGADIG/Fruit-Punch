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
        scoreText.text = FindObjectOfType<PlayerScore>().getScore().ToString();
    }

    void updateRank()
    {
        rankText.text = FindObjectOfType<PlayerScore>().getRank();   
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
