using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawns : MonoBehaviour
{

    public int enemyCountTotal = 1;

    public int burstSpawnCount = 1;

    public float spawnRate = 2.0f;

    private enum AuraType{Red, Blue, Yellow, Green};


    public GameObject testEnemy;


    private float timer;

    private int spawnsRemaining;

    public void SpawnEnemy()
    {

        GameObject enemy = Instantiate(testEnemy, transform.position, Quaternion.identity);

        spawnsRemaining -= 1;
    }


    // Start is called before the first frame update
    void Start()
    {

        timer = spawnRate;
        spawnsRemaining = enemyCountTotal;
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnsRemaining <= 0)
        {
            return;
        }
        if (timer > 0.0f)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            SpawnEnemy();

            timer = spawnRate;
        }
    }
}
