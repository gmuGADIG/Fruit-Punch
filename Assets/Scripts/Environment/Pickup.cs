using System.Collections;
using System.Collections.Generic;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Health health = collision.GetComponent<Health>();
        if(health != null)
        {
            health.Heal(healthRestore);
            Destroy(gameObject);
        }
    }
}
