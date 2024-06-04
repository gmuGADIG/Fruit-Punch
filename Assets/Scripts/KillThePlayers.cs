using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillThePlayers : MonoBehaviour
{
    //This is a testing script for killing the player for test reasons.
    Health[] playersHealth;
    // Start is called before the first frame update
    void Start()
    {
        playersHealth = FindObjectsOfType<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        DamageInfo damageInfo = new DamageInfo(gameObject, 100, Vector2.right, AuraType.Strike);

        if (Input.GetKeyDown(KeyCode.J))
        {
            playersHealth[0].Damage(damageInfo);
            Debug.Log("Player 1 Should be damaged");
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            playersHealth[1].Damage(damageInfo);
            Debug.Log("Player 2 Should be damaged");
        }
    }
}
