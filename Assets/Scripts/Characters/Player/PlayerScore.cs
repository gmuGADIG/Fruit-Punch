using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    public static int pointsPerKill = 10;
    public static int pointsPerPearry = 5;
    public static float pointsPerDamageTakenMultiplier = .5f;
    public static int pointsLostOnDeath = 50;
    public static float timePointLossInterval = 5f; // player loses point every x seconds

    public event Action OnUpdateScoreAndRank;

    [SerializeField]
    private int playerScore;

    [System.Serializable]
    public struct RankLevels
    {
        public string rankName;
        public int pointThreshold;
        public Sprite rankImage;
    }

    [Tooltip("Top rank at index 0, descending Order")]
    public RankLevels[] rankList;

    void Start()
    {
        Health health = GetComponent<Health>();
        health.OnPearry += () => AddScore(pointsPerPearry);
        health.onHurt += (DamageInfo damage) => SubtractScore((int)(damage.damage * pointsPerDamageTakenMultiplier));
        health.onDeath += () => SubtractScore(pointsLostOnDeath);
        StartCoroutine(TimeScoreSubtraction());
    }

    public void AddScore(int points)
    {
        playerScore += points;
        OnUpdateScoreAndRank?.Invoke();
    }

    public void SubtractScore(int points)
    {
        playerScore -= points;
        if(playerScore < 0) { playerScore = 0; }
        OnUpdateScoreAndRank?.Invoke();
    }

    public int GetScore()
    {
        return playerScore;
    }

    /// <summary>
    /// returns rank (string) based on current score
    /// </summary>
    /// <returns></returns>
    public string GetRank()
    {
        string tempRank = null;
        for(int i = rankList.Length - 1; i >= 0; i--)
        {
            if(playerScore > rankList[i].pointThreshold) { tempRank = rankList[i].rankName;}
        }
        return tempRank;
    }

    public Sprite GetRankImage()
    {
        Sprite tempRankSprite = null;
        for (int i = rankList.Length - 1; i >= 0; i--)
        {
            if (playerScore > rankList[i].pointThreshold) { tempRankSprite = rankList[i].rankImage; }
        }
        return tempRankSprite;
    }

    public void StopScoreCountdown()
    {
        StopCoroutine(TimeScoreSubtraction());
    }

    IEnumerator TimeScoreSubtraction()
    {
        while (true)
        {
            yield return new WaitForSeconds(timePointLossInterval);
            SubtractScore(1);
        }
    }

}
