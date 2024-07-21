using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScorePersister : MonoBehaviour
{
    public int[][] playerLevelScores = { new int[] { 0, 0, 0 }, new int[] { 0, 0, 0 } };
    int currentLevelIndex = 0;

    public static ScorePersister Instance { get; private set; }

    public void Clear() 
    {
        Destroy(gameObject);
    }

    void Awake() {
        if (Instance != null) { 
            Destroy(gameObject);
        } else {

            transform.parent = null; // unity wont let you DontDestroyOnLoad something that has a parent
            GameSceneManager.OnStageIndexChange += (int index) => currentLevelIndex = index;
            DontDestroyOnLoad(gameObject);

            Instance = this;
        }
    }

    public void HydrateScore(PlayerScore score) {
        var player = score.GetComponent<Player>();
        var playerIndex = player.PlayerNum - 1;

        score.SetScore(playerLevelScores[playerIndex][currentLevelIndex], false);
        score.OnUpdateScoreAndRank += _animate => {
            playerLevelScores[playerIndex][currentLevelIndex] = score.GetScore();
        };
    }
}
