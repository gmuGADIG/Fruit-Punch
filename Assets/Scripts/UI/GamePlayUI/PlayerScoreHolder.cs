using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScoreHolder : MonoBehaviour
{
    [SerializeField] Transform[] playerScoreUI;

    static PlayerScoreHolder instance;

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Setup score UI for a single player (index=0 for player one, index=1 for player two). <br/>
    /// Called by PlayerSpawner.cs when creating the players.
    /// </summary>
    public static void SetupPlayerScore(int index, Player player)
    {
        var scoreUI = instance.playerScoreUI[index].GetComponent<PlayerScoreUI>();
        scoreUI.Setup(player);
    }

    public static void SetPlayerTwoVisible(bool visible)
    {
        instance.playerScoreUI[1].gameObject.SetActive(visible);
    }
}
