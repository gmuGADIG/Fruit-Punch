using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class ScorePersister : MonoBehaviour {
    int[] scores = {0, 0};

    public static ScorePersister Instance { get; private set; }

    public void Clear() {
        Debug.Log("Clearing score!", this);
        Destroy(gameObject);
    }

    void Awake() {
        if (Instance != null) { 
            Debug.Log("Destroying self!");
            Destroy(gameObject);
        } else {
            Debug.Log("Registering self!");

            transform.parent = null; // unity wont let you DontDestroyOnLoad something that has a parent
            DontDestroyOnLoad(gameObject);

            Instance = this;
        }
    }

    public void HydrateScore(PlayerScore score) {
        var player = score.GetComponent<Player>();
        var index = player.PlayerNum - 1;

        Debug.Log($"score.SetScore({scores[index]})", this);
        score.SetScore(scores[index], false);
        score.OnUpdateScoreAndRank += _animate => {
            scores[index] = score.GetScore();
            Debug.Log($"scores[{index}] = {score.GetScore()}", this);
        };
    }
}
