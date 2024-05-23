using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ScreenSpawner : MonoBehaviour
{
    #region Editor Fields
    /// <summary>
    /// Contains data about how each enemy should spawn
    /// </summary>
    [System.Serializable]
    public struct EnemySpawnData
    {
        public GameObject enemyPrefab;
        public AuraType aura;
        public EnemySpawns spawnpoint;
    }

    [Tooltip("Time between an enemy dying and another enemy entering the play area.")]
    public float spawnDelay = 2.0f;
    [Tooltip("Number of enemies to spawn (1 player)")]
    public int numEnemy = 10;
    [Tooltip("Number of aura enemies to spawn (1 player)")]
    public int numAura = 0;
    [Tooltip("Maximum number of enemies allowed on screen at once")]
    public int enemyOnScreen = 3;
    [Tooltip("Maximum number of aura enemies allowed on screen at once")]
    public int auraOnScreen = 1;
    [Tooltip("Multiplier for enemy scaling when two players are in game")]
    public float enemyScalingMultiplier = 1.0f;

    [Tooltip("Spawn as if two players are in game")]
    [SerializeField]
    private bool spawnAsTwoPlayer = false;

    /// <summary>
    /// List of enemies to spawn that is editable before spawning starts.
    /// </summary>
    [SerializeField]
    private List<EnemySpawnData> enemiesToSpawn;

    
    /// <summary>
    /// Event that is called when the final enemy is defeated.
    /// </summary>
    public UnityEvent onWaveComplete;
    #endregion

    #region Private Fields
    /// <summary>
    /// Property to mark if two player for debugging.
    /// </summary>
    private bool twoPlayer => spawnAsTwoPlayer;

    /// <summary>
    /// The number of enemies that need to be spawned before stopping spawns.
    /// </summary>
    private int EnemyCountToSpawn
    {
        get => twoPlayer ? (int)(enemyScalingMultiplier * numEnemy) : numEnemy;
    }

    /// <summary>
    /// The number of aura enemies that need to be spawned before stopping spawns.
    /// </summary>
    private int AuraCountToSpawn
    {
        get => twoPlayer ? (int)(enemyScalingMultiplier * numAura) : numAura;
    }

    /// <summary>
    /// The queue of non-aura enemies to spawn from.
    /// </summary>
    private Queue<EnemySpawnData> normalEnemySpawnQueue;

    /// <summary>
    /// The queue of aura enemies to spawn from.
    /// </summary>
    private Queue<EnemySpawnData> auraEnemySpawnQueue;

    /// <summary>
    /// Manager for what enemies are on screen.
    /// </summary>
    private HashSet<GameObject> enemiesOnScreen = new();

    /// <summary>
    /// Number of aura enemies currently on screen.
    /// </summary>
    private int auraEnemiesOnScreen;


    /// <summary>
    /// Number of spawnable enemies left.
    /// </summary>
    private int enemiesLeft;

    /// <summary>
    /// Number of aura enemies left.
    /// </summary>
    private int auraEnemiesLeft;

    /// <summary>
    /// If a timer is active for a new spawn.
    /// </summary>
    private bool isCurrentlySpawning;

    private bool spawningComplete => enemiesLeft <= 0 && auraEnemiesLeft <= 0 && enemiesOnScreen.Count == 0;

    private float spawnTimer;


    #endregion
    private void Awake()
    {
        enemiesOnScreen = new HashSet<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isCurrentlySpawning)
        {
            if(spawnTimer > 0)
            {
                spawnTimer -= Time.deltaTime;
            } else
            {
                spawnTimer += spawnDelay;
                DoSpawn();
            }
        }
    }
    private void LoadSpawnQueue()
    {
        normalEnemySpawnQueue = new Queue<EnemySpawnData>();
        auraEnemySpawnQueue = new Queue<EnemySpawnData>();
        
        for (int i = 0; i < 1000; i++)
        {
            var enemyData = enemiesToSpawn[i % enemiesToSpawn.Count];
            if (enemyData.aura.IsSpecial())
            {
                if (auraEnemySpawnQueue.Count >= numAura) continue;
                auraEnemySpawnQueue.Enqueue(enemyData);
            }
            else
            {
                if (normalEnemySpawnQueue.Count >= EnemyCountToSpawn) continue;
                normalEnemySpawnQueue.Enqueue(enemyData);
            }
            
            if (normalEnemySpawnQueue.Count + auraEnemySpawnQueue.Count >= EnemyCountToSpawn)
                return;
        }
        
        // if execution reaches here, the loop got to 1000, which should never happen
        throw new Exception("LoadSpawnQueue failsafe was used!");
    }
    public void StartSpawning()
    {
        Debug.Log("Spawning Enemies...");
        enemiesLeft = EnemyCountToSpawn;
        auraEnemiesLeft = AuraCountToSpawn;
        enemiesOnScreen.Clear();
        LoadSpawnQueue();
        for (int i = 0; i < enemyOnScreen; i++)
        {
            DoSpawn();
        }
    }

    private EnemySpawnData PickSpawnData()
    {
        //if we can spawn an aura enemy, do so
        if(auraEnemiesLeft > 0 && auraEnemiesOnScreen < auraOnScreen && auraEnemySpawnQueue.Count > 0)
        {
            return auraEnemySpawnQueue.Dequeue();
        }
        //if we can spawn a normal enemy, do so
        if(enemiesLeft > 0 && enemiesOnScreen.Count < enemyOnScreen && normalEnemySpawnQueue.Count > 0)
        {
            return normalEnemySpawnQueue.Dequeue();
        }

        return default;
    }


    private void DoSpawn()
    {
        EnemySpawnData spawnData = PickSpawnData();
        if(spawnData.Equals(default(EnemySpawnData)))
        {
            // Both queues are empty, we are done spawning
            return;
        }
        
        GameObject instance = spawnData.spawnpoint.SpawnEnemy(spawnData.enemyPrefab, spawnData.aura);
        enemiesOnScreen.Add(instance);
        
        enemiesLeft--;

        Health healthInstance = instance.GetComponent<Health>();
        healthInstance.onDeath += () => OnEnemyDeath(instance);
        if(healthInstance.HasAura())
        {
            auraEnemiesOnScreen++;
            auraEnemiesLeft--;
        }
    }

    /// <summary>
    /// Callback function that runs when a enemies dies. Starts a spawn check.
    /// </summary>
    /// <param name="enemyThatDied"></param>
    private void OnEnemyDeath(GameObject enemyThatDied)
    {
        Debug.Log($"Enemy {enemyThatDied.name} has died.");
        Health healthInstance = enemyThatDied.GetComponent<Health>();
        if(healthInstance.HasAura())
        {
            auraEnemiesOnScreen--;
        }
        enemiesOnScreen.Remove(enemyThatDied);
        if (spawningComplete)
        {
            onWaveComplete?.Invoke();
            Debug.Log("Wave Complete");
        } else if(!isCurrentlySpawning)
        {
            isCurrentlySpawning = true;
            spawnTimer = spawnDelay;
        }
    }
}
