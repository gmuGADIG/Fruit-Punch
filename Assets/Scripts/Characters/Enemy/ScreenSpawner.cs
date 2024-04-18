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
        public Enemy enemy;
        public AuraType aura;
        public EnemySpawns spawnpoint;
    }

    [Tooltip("Time between an enemy dying and another enemy entering the play area.")]
    public float spawnDelay = 2.0f;
    public int numEnemy = 10;
    public int numAura = 0;
    [Tooltip("Maximum number of enemies allowed on screen at once")]
    public int enemyOnScreen = 3;
    [Tooltip("Maximum number of aura enemies allowed on screen at once")]
    public int auraOnScreen = 1;
    public float enemyScalingMultiplier = 1.0f;

    /// <summary>
    /// List of enemies to spawn that is editable before spawning starts.
    /// </summary>
    [SerializeField]
    private List<EnemySpawnData> enemiesToSpawn;

    /// <summary>
    /// Event that is called when the max number of enemies are spawned.
    /// </summary>
    public UnityEvent onWaveComplete;
    #endregion

    #region Private Fields
    /// <summary>
    /// Property to mark if two player for debugging.
    /// </summary>
    private bool twoPlayer = true;

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
    /// The queue of enemies to spawn from.
    /// </summary>
    private Queue<EnemySpawnData> enemySpawnQueue;

    /// <summary>
    /// Manager for what enemies are on screen.
    /// </summary>
    private HashSet<Enemy> enemiesOnScreen;
    
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

    private float spawnTimer;


    #endregion
    private void Awake()
    {
        enemiesOnScreen = new HashSet<Enemy>();
    }
    // Start is called before the first frame update

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
                if(ShouldSpawn())
                {
                    DoSpawn();
                }
            }
        }
    }
    private void LoadSpawnQueue()
    {
        enemySpawnQueue = new Queue<EnemySpawnData>(enemiesToSpawn);

    }
    public void StartSpawning()
    {
        Debug.Log("Spawning Enemies...");
        enemiesLeft = EnemyCountToSpawn;
        auraEnemiesLeft = numAura;
        enemiesOnScreen.Clear();
        LoadSpawnQueue();
        while (ShouldSpawn())
        {
            DoSpawn();
        }
    }

    /// <summary>
    /// Checks if spawning an enemy would exceed the max on screen values.
    /// </summary>
    /// <returns>True if spawning the next enemy is okay</returns>
    private bool ShouldSpawn()
    {
        if(enemiesOnScreen.Count >= enemyOnScreen)
        {
            return false;
        }
        EnemySpawnData data = enemySpawnQueue.Peek();
        if(data.aura.IsSpecial() && auraEnemiesOnScreen >= auraOnScreen)
        {
            return false;
        }     
        return true;
    }
    private void DoSpawn()
    {
        EnemySpawnData spawnData = enemySpawnQueue.Dequeue();
        
        Enemy instance = spawnData.spawnpoint.SpawnEnemy(spawnData.enemy, spawnData.aura);
        enemiesOnScreen.Add(instance);
        
        enemiesLeft--;

        Health healthInstance = instance.GetComponent<Health>();
        healthInstance.onDeath += () => OnEnemyDeath(instance);
        if(healthInstance.HasAura())
        {
            auraEnemiesOnScreen++;
            auraEnemiesLeft--;
        }

        if(enemySpawnQueue.Count == 0) {
            LoadSpawnQueue();
        }
    }

    /// <summary>
    /// Callback function that runs when a enemies dies. Starts a spawn check.
    /// </summary>
    /// <param name="enemyThatDied"></param>
    private void OnEnemyDeath(Enemy enemyThatDied)
    {
        Health healthInstance = enemyThatDied.GetComponent<Health>();
        if(healthInstance.HasAura())
        {
            auraEnemiesOnScreen--;
        }
        enemiesOnScreen.Remove(enemyThatDied);
        enemyThatDied.GetComponent<Health>().onDeath -= () => OnEnemyDeath(enemyThatDied);
        //TODO Enemies should probably find a way to end themselves.
        Destroy(enemyThatDied.gameObject);
        if(!isCurrentlySpawning)
        {
            isCurrentlySpawning = true;
            spawnTimer = spawnDelay;
        }
    }
}
