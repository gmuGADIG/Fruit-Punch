using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubtractScoreOnClick : MonoBehaviour
{
    public PlayerScore playerScore;
    public int points = 5;
    private void OnMouseDown()
    {
        playerScore.SubtractScore(points);
        Debug.Log("Subtracted");
    }

}
