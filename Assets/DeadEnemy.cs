using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Marker component for dead enemies. Just place an animation on this and call DestroySelf when ready.
/// </summary>
public class DeadEnemy : MonoBehaviour {
    [Tooltip("If true, the screen will remain locked while the enemy is dead.")]
    [SerializeField] bool shouldPersistWave = false;

    public bool ShouldPersistWave { get => shouldPersistWave; }

    void DestroySelf() { 
        Destroy(gameObject); 
    }
}
