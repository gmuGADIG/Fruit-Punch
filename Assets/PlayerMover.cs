using System.Linq;
using UnityEngine;

public class PlayerMover : MonoBehaviour {
    [SerializeField] Transform Position1;
    [SerializeField] Transform Position2;
    [SerializeField] Transform BossPosition;

    public void Start() {
        Debug.Assert(Position1 != null);
        Debug.Assert(Position2 != null);
        Debug.Assert(BossPosition != null);
    }

    public void MovePlayers() {
        var player1 = FindObjectsOfType<Player>().Where(p => p.PlayerNum == 1).FirstOrDefault();
        var player2 = FindObjectsOfType<Player>().Where(p => p.PlayerNum == 2).FirstOrDefault();
        var boss = FindObjectOfType<Boss>();

        if (player1 != null) {
            player1.transform.position = Position1.position;
            player1.transform.rotation = Position1.rotation;
        }

        if (player2 != null) {
            player2.transform.position = Position2.position;
            player2.transform.rotation = Position2.rotation;
        }

        if (boss != null) {
            boss.transform.position = BossPosition.position;
            boss.transform.rotation = BossPosition.rotation;
        }
    }
}
