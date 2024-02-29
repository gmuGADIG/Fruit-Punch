using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    public static bool AreEnemiesPresent { get; private set; }

    void OnTriggerEnter(Collider other)
    {
        // Check if the collided object has the Enemy script component attached
        if (other.GetComponent<Enemy>() != null)
        {
            AreEnemiesPresent = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Check if the collided object has the Enemy script component attached
        if (other.GetComponent<Enemy>() != null)
        {
            AreEnemiesPresent = false;
        }
    }
}
