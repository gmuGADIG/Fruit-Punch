using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubtractScoreOnClick : MonoBehaviour
{
    PlayerScore playerScore;
    public int points = 5;
    private void OnMouseDown()
    {
        playerScore.subtractScore(points);
    }
    // Start is called before the first frame update
    void Start()
    {
        playerScore = FindObjectOfType<PlayerScore>();
    }
}
