using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ItemDrops : MonoBehaviour
{
    private Health health;

    /// <summary>
    /// The chance for a pickup to drop
    /// </summary>
    [SerializeField]
    private float percentChance = 0.25f;

    /// <summary>
    /// The pickup being dropped
    /// </summary>
    [SerializeField]
    private GameObject pickup;

    private void Start()
    {
        health = GetComponent<Health>();
        health.onDeath += dropPickup;
    }

    /// <summary>
    /// rolls the chance for the object to drop a pickup before destroying the object
    /// </summary>
    void dropPickup()
    {
        if (Random.value <= percentChance)
        {
            Instantiate(pickup, transform.position, Quaternion.identity);
        }
    }
}
