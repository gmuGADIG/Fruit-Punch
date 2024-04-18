using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Manages MultiplePlayers
/// </summary>
public class PostUIPlayerManager : MonoBehaviour
{
    GameObject[] players;

    public GameObject resultUI;
    // Start is called before the first frame update
    void Awake()
    {
        players = GameObject.FindGameObjectsWithTag("Player");

        for(int i = players.Length- 1; i >= 0 ; i--)
        {
            PostLevelResults temp = Instantiate(resultUI, transform).GetComponent<PostLevelResults>();
            temp.setPlayer(players[i]);
            Debug.Log("Sent player " + i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
