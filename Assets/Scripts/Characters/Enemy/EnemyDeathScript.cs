using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyDeathScript : MonoBehaviour
{
    public Vector3 knockBackAngle = new Vector3(1.0f, 0.0f, 0.5f); // future Randomize or base off of last movements
    public float knockBackForce = 2.0f;
    [Tooltip("How long enemy is falling")]
    public float knockBackTime = 0.3f;
    [Tooltip("How long till Enemy destroy after knock back ends")]
    public float timeToDeath = 0.8f;

    Health myHealth;
    Enemy enemyAI;
    Rigidbody rb;
    // Start is called before the first frame update
    void Awake()
    {
        myHealth = GetComponent<Health>();
        enemyAI = GetComponent<Enemy>();
        rb = GetComponent<Rigidbody>();

    }



    /// <summary>
    /// Initiates the Death animations.
    /// </summary>
    private void DeathSequence()
    {
        enemyAI.enabled = false;
        rb.velocity = Vector3.zero;

        //Add Death anim trigger here
        rb.AddForce(knockBackAngle * knockBackForce, ForceMode.Impulse);

        Debug.Log("EnemyDied!");
        Invoke("DeathPart2", knockBackTime);
        
    }
    private void DeathPart2()
    {
        rb.velocity = Vector3.zero;
        Destroy(gameObject, timeToDeath);
    }

    private void OnEnable()
    {
        myHealth.onDeath += DeathSequence;
    }
}
