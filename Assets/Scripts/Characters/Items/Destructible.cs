using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    [SerializeField] GameObject spawnOnDeath;
    
    private Health health;

    private void Start()
    {
        health = GetComponent<Health>();
        health.onDeath += DestroyObject;
    }

    void DestroyObject()
    {
        if (spawnOnDeath != null)
        {
            Instantiate(spawnOnDeath, transform.position, transform.rotation, transform.parent);
        }
        Destroy(gameObject);
    }
}
