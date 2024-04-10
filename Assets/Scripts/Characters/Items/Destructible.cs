using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    Health health;

    private void Start()
    {
        health = GetComponent<Health>();
        health.onDeath += DestroyObject;
    }

    void DestroyObject()
    {
        Destroy(gameObject);
    }
}
