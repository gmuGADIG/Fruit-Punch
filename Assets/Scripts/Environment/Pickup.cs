using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

/// <summary>
/// A script that makes a healing pickup that disappears on touch.
/// Can be modified to make other types of pickups, like a temp buff.
/// </summary>
public class Pickup : MonoBehaviour
{
    /// <summary>
    /// The amount of health to be healed.
    /// </summary>
    [SerializeField]
    private int healthRestore = 20;

    [ReadOnlyInInspector]
    private BeltCharacter beltCharacter;

    private List<BeltCharacter> collisions;

    private void Start()
    {
        beltCharacter = gameObject.GetComponent<BeltCharacter>();
    }

    private void Update()
    {
        collisions = beltCharacter.GetOverlappingBeltCharacters(gameObject.GetComponent<Collider2D>(), Physics2D.AllLayers);
        foreach(BeltCharacter i in collisions)
        {
            Health health = i.GetComponent<Health>();
            if (health != null && i.tag == "Player")
            {
                health.Heal(healthRestore);
                Destroy(gameObject);
            }
        }
    }
}
