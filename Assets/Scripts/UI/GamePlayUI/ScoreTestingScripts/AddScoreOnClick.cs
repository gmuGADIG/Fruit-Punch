using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddScoreOnClick : MonoBehaviour
{
    PlayerScore playerScore;
    public int points = 5;
    private void OnMouseDown()
    {
        playerScore.addScore(points);
    }
    // Start is called before the first frame update
    void Start()
    {
        playerScore = FindObjectOfType<PlayerScore>();
    }

}
