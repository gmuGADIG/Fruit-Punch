using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class ScorePersister : MonoBehaviour {
    int[] scores = {0, 0};

    public static ScorePersister Instance { get; private set; }

    public void Clear() {
        Destroy(gameObject);
    }

    void Awake() {
        if (Instance != null) { 
            Destroy(gameObject);
        } else {

            transform.parent = null; // unity wont let you DontDestroyOnLoad something that has a parent
            DontDestroyOnLoad(gameObject);

            Instance = this;
        }
    }

    public void HydrateScore(PlayerScore score) {
        var player = score.GetComponent<Player>();
        var index = player.PlayerNum - 1;

        score.SetScore(scores[index], false);
        score.OnUpdateScoreAndRank += _animate => {
            scores[index] = score.GetScore();
        };
    }
}
