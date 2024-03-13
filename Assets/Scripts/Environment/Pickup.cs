using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

/// <summary>
/// A abstract script that makes a pickup that disappears on touch.
/// Can be extended to make other types of pickups, like healing or powerups.
/// </summary>
[RequireComponent(typeof(Collider))]
public abstract class Pickup : MonoBehaviour
{
    private void Start()
    {
        var thisCollider = GetComponent<Collider>();
        if (thisCollider.isTrigger == false) throw new Exception("Pickup's collider is not a trigger!");
    }

    public void PickUp(Player player)
    {
        ApplyEffect(player);
        Destroy(this.gameObject);
    }
    
    /// <summary>
    /// The effect of the pickup. Something like healing or buffing the player.
    /// </summary>
    /// <param name="player"></param>
    protected abstract void ApplyEffect(Player player);
}
