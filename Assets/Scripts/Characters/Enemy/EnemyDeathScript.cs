using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyDeathScript : MonoBehaviour
{
    // public Vector3 knockBackAngle = new Vector3(1.0f, 0.0f, 0.5f); // future Randomize or base off of last movements
    // public float knockBackForce = 2.0f;
    // [Tooltip("How long enemy is falling")]
    // public float knockBackTime = 0.3f;
    // [Tooltip("How long till Enemy destroy after knock back ends")]
    // public float timeToDeath = 0.8f;

    [SerializeField] GameObject deadEnemyPrefab;

    Health myHealth;
    Rigidbody rb;
    // Start is called before the first frame update
    void Awake()
    {
        myHealth = GetComponent<Health>();
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Initiates the Death animations.
    /// </summary>
    private void DeathSequence() {
        if (!(GetComponent<ExploderEnemy>()?.IsExploding ?? false)) {
            Instantiate(deadEnemyPrefab, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }

    private void OnEnable()
    {
        myHealth.onDeath += DeathSequence;
    }

}
