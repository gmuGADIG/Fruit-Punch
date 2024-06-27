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

    [Tooltip("Is this enemy a splitter? If not leave false.")]
    public bool isSplitter = false;
    [Tooltip("What kind of enemies can spawn out of a this enemy if it is a splitter?")]
    public List<GameObject> enemyTypes;


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
        Destroy(enemyAI);
        rb.velocity = Vector3.zero;

        //Add Death anim trigger here
        rb.AddForce(knockBackAngle * knockBackForce, ForceMode.Impulse);

        Debug.Log("EnemyDied!");
        Invoke("DeathPart2", knockBackTime);
        
    }
    private void DeathPart2()
    {
        SplitterDeath();
        rb.velocity = Vector3.zero;
        Destroy(gameObject, timeToDeath);
    }

    private void OnEnable()
    {
        myHealth.onDeath += DeathSequence;
    }


    /// <summary>
    /// If an enemy is seen as a splitter, it will spawn 2 more enemies on death, which are picked randomly from a list of enemies that can be spawned.
    /// </summary>
    private void SplitterDeath()
    {
        if(isSplitter == true)
        {
            for(int i = 0; i < 2; i++)
            {
                int ranEnemy = Random.Range(0, enemyTypes.Count);
                GameObject newEnemy = Instantiate(enemyTypes[ranEnemy], gameObject.transform);
                newEnemy.transform.parent = null;
            }

        }
    }

}
