using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathScript : MonoBehaviour
{
    Health myHealth;
    // Start is called before the first frame update
    void Start()
    {
        myHealth = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void DeathSequence()
    {
        Debug.Log("EnemyDied!");
    }

    private void OnEnable()
    {
        myHealth.onDeath += DeathSequence;
    }
}
