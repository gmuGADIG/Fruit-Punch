using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    public int playerScore;

    public struct rankLevels
    {
        public string rankName;
        public int maxPoints;
        public int minPoints;
    }

    public rankLevels[] rankList;
    public void addScore(int points)
    {
        playerScore += points;
    }

    public void subtractScore(int points)
    {
        playerScore -= points;
    }

    public int getScore()
    {
        return playerScore;
    }

    public string getRank()
    {
        return null;
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
