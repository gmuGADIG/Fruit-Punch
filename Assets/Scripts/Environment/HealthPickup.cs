using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

/// <summary>
/// A script that extends the Pickup script. It makes a healing pickup that disappears on touch.
/// </summary>
public class HealthPickup: Pickup
{
    /// <summary>
    /// The amount of health to be healed.
    /// </summary>
    [SerializeField]
    private int healthRestore = 20;

    /// <summary>
    /// Accesses player's Health script and heals them
    /// </summary>
    /// <param name="player"></param>
    public override void Effect(GameObject player)
    {
        player.GetComponent<Health>().Heal(healthRestore);
    }
}