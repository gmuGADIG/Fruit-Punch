using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour {
    [SerializeField] Transform Position1;
    [SerializeField] Transform Position2;

    public void MovePlayers() {
        var player1 = FindObjectsOfType<Player>().Where(p => p.PlayerNum == 1).FirstOrDefault();
        var player2 = FindObjectsOfType<Player>().Where(p => p.PlayerNum == 2).FirstOrDefault();

        if (player1 != null) {
            player1.transform.position = Position1.position;
        }

        if (player2 != null) {
            player2.transform.position = Position2.position;
        }
    }
}
