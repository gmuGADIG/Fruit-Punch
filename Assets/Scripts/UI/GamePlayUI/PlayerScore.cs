using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    public event Action updateScoreAndRank;

    [SerializeField]
    private int playerScore;

    [System.Serializable]
    public struct rankLevels
    {
        public string rankName;
        public int pointThreshold;

    }
    [Tooltip("Top rank at index 0, descending Order")]
    public rankLevels[] rankList;
    public void addScore(int points)
    {
        playerScore += points;
        updateScoreAndRank();
    }

    public void subtractScore(int points)
    {
        playerScore -= points;
        if(playerScore < 0) { playerScore = 0; }
        updateScoreAndRank();
    }

    public int getScore()
    {
        return playerScore;
    }

    public string getRank()
    {
        string tempRank = null;
        for(int i = rankList.Length - 1; i >= 0; i--)
        {
            if(playerScore > rankList[i].pointThreshold) { tempRank = rankList[i].rankName;}
        }
        return tempRank;
    }

    // Start is called before the first frame update
    void Start()
    {
        updateScoreAndRank();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
