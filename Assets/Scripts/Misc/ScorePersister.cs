using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScorePersister : MonoBehaviour
{
    // first index = player index, second index = level index
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
            GameSceneManager.OnStageIndexChange += (int index) => SetLevelIndex(index);
            PauseScreenButtons.OnLevelRestart += ResetLevelScore;
            DontDestroyOnLoad(gameObject);

            Instance = this;
        }
    }

    void SetLevelIndex(int index)
    {
        currentLevelIndex = index;
    }

    public void ResetLevelScore()
    {
        playerLevelScores[0][currentLevelIndex] = 0;
        playerLevelScores[1][currentLevelIndex] = 0;
    }

    public void HydrateScore(PlayerScore score) {
        var player = score.GetComponent<Player>();
        var playerIndex = player.PlayerNum - 1;

        score.SetScore(playerLevelScores[playerIndex][currentLevelIndex], false);
        score.OnUpdateScoreAndRank += _animate => {
            playerLevelScores[playerIndex][currentLevelIndex] = score.GetScore();
        };
    }

    private void OnDestroy()
    {
        PauseScreenButtons.OnLevelRestart -= ResetLevelScore;
        GameSceneManager.OnStageIndexChange -= (int index) => SetLevelIndex(index);
    }
}
