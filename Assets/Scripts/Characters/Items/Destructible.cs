using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    Health health;

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
        health.onDeath += DestroyObject;
    }

    void DestroyObject()
    {
        dropPickup();
        Destroy(gameObject);
    }

    /// <summary>
    /// rolls the chance for the object to drop a pickup before destroying the object
    /// </summary>
    public void dropPickup()
    {
        if (Random.value <= percentChance)
        {
            Instantiate(pickup, transform.position, Quaternion.identity);
        }
    }
}
