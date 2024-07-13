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
    public float spawnDelay = 0.2f;
    [Tooltip("Number of enemies to spawn (1 player)")]
    public int numEnemy = 10;
    [Tooltip("Maximum number of enemies allowed on screen at once")]
    public int enemyOnScreen = 3;
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
    /// Event that is called when the first enemy is spawned.
    /// </summary>
    public UnityEvent onWaveStart;
    
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
    /// The queue of non-aura enemies to spawn from.
    /// </summary>
    private Queue<EnemySpawnData> enemySpawnQueue = new();

    bool spawningActivated = false;
    private float spawnTimer;


    #endregion

    void Start()
    {
        Health.OnAnyDeath += OnAnyDeath;
        LoadSpawnQueue();
    }

    void OnDestroy()
    {
        Health.OnAnyDeath -= OnAnyDeath;
    }

    // Update is called once per frame
    void Update()
    {
        if (!spawningActivated) return;
        
        if(spawnTimer > 0)
        {
            spawnTimer -= Time.deltaTime;
        } else
        {
            spawnTimer += spawnDelay;
            DoSpawn();
        } 
    }
    private void LoadSpawnQueue()
    {
        for (int i = 0; i < EnemyCountToSpawn; i++)
        {
            var enemyData = enemiesToSpawn[i % enemiesToSpawn.Count];
            enemySpawnQueue.Enqueue(enemyData);
        }
    }
    
    public void StartSpawning()
    {
        Debug.Log("Spawning Enemies...");
        spawningActivated = true;
        onWaveStart.Invoke();
        for (int i = 0; i < enemyOnScreen; i++)
        {
            DoSpawn();
        }
    }

    int CountEnemies()
    {
        var enemies = FindObjectsOfType<Enemy>().Concat<MonoBehaviour>(FindObjectsOfType<Boss>());
        var persistingEnemies = 
            enemies.Where(e => e.GetComponent<Health>().CurrentHealth != 0)
            .Concat<MonoBehaviour>(FindObjectsOfType<DeadEnemy>().Where(de => de.ShouldPersistWave));
        
        return persistingEnemies.Count();
    }

    private void DoSpawn()
    {
        if (enemySpawnQueue.Count == 0) return; // nothing left to spawn
        if (CountEnemies() >= enemyOnScreen) return; // too many enemies already on-screen
        
        var spawnData = enemySpawnQueue.Dequeue();
        spawnData.spawnpoint.SpawnEnemy(spawnData.enemyPrefab, spawnData.aura);
    }

    /// <summary>
    /// Callback function that runs when a enemies dies. Starts a spawn check.
    /// </summary>
    private void OnAnyDeath(GameObject thingThatDied)
    {
        if (thingThatDied.GetComponent<Enemy>() == null && thingThatDied.GetComponent<Boss>() == null) return;
        CheckForWaveClear();
    }

    private void CheckForWaveClear()
    {
        if (!spawningActivated) return;
        
        IEnumerator Coroutine() {
            yield return null; // we wait a frame here since spawning the dead enemy takes a frame
            var doneSpawning = enemySpawnQueue.Count == 0;
            var allDead = CountEnemies() == 0;

            if (doneSpawning && allDead) {
                onWaveComplete?.Invoke();
                Debug.Log("Wave Complete");
                Destroy(this.gameObject);
            }
        }

        StartCoroutine(Coroutine());
    }
}
