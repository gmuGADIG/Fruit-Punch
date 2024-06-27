using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddScoreOnClick : MonoBehaviour
{
    public PlayerScore playerScore;
    public int points = 5;
    private void OnMouseDown()
    {
        playerScore.AddScore(points);
        Debug.Log("Added");
    }
}
