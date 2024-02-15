using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSpawner : MonoBehaviour
{

    public float spawnDelay = 2.0f;
    public int numEnemy = 10;
    public int numAura = 0;
    public int enemyOnScreen = 3;
    public int auraOnScreen = 1;
    public float enemyScalingMultiplier = 1.0f;

    //boolean optionalWaves; //no idea what this is supposed to be, leaving it in a comment for now

    public List<EnemySpawns> enemySpawns = new List<EnemySpawns>();



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
