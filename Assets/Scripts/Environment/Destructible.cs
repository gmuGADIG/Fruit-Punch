using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    // Required actions:
    // Attach 2D collider component
    // Assign player and damgePerHit with values in inspector
    // Attach Health and BeltCharacter scripts to the object to be destroyed
    // Must set a Max Health value in health script and Vulnerable Types to Strike

    public GameObject player;
    public float damagePerHit;

    private BeltCharacter beltCharacter;
    private Health health;
    private List<BeltCharacter> collisions;

    /// <summary>
    /// The chance of the object dropping a pickup
    /// </summary>
    [SerializeField]
    private float percentChance = 0.25f;

    /// <summary>
    /// The pickup that will be dropped
    /// </summary>
    [SerializeField]
    private GameObject pickup;


    // Start is called before the first frame update
    void Start()
    {
        // Getting BeltCharacter script from this game object in order to use GetOverlappingBeltCharacters function for collision
        beltCharacter = gameObject.GetComponent<BeltCharacter>();

        // Getting this objects health
        health = gameObject.GetComponent<Health>();

        // subscribing obstacleDestroyed() to health scripts onDeath Action
        health.onDeath += obstacleDestroyed;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if obstacle is colliding with anything
        collisions = beltCharacter.GetOverlappingBeltCharacters(gameObject.GetComponent<Collider2D>(), Physics2D.AllLayers);
        foreach (BeltCharacter i in collisions)
        {
            // If the collision is with the player then destroy this gameobject
            // If we want the obstacle to be destroyed only when player attackes it edit the If statement below
            // ie: adding "&& player.attackScriptName.isAttacking"
            if ( i.tag == "Player")
            {
                // Instantiate obstacle being destroyed animations here

                // Does damage to the object this script is attached to with zero knockback and a strike type
                health.Damage(new DamageInfo(damagePerHit, Vector2.zero, AuraType.Strike));
            }
        }
    }

    /// <summary>
    /// rolls the chance for the object to drop a pickup before destroying the object
    /// </summary>
    public void obstacleDestroyed()
    {
        if (Random.value < percentChance)
        {
            Instantiate(pickup, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
    
}
