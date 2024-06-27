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

        thisCollider.enabled = false;
        transform.localScale = Vector3.zero;
        StartCoroutine(Appear());

        IEnumerator Appear()
        {
            var startPosition = transform.position;
            var t = 0f;
            while (t < 1f)
            {
                // some arbitrary equation to make it pop in nicely
                transform.localScale = Vector3.one * (2.5f * t - 1.5f * Mathf.Pow(t, 3));
                transform.position = startPosition + Vector3.up * (2.5f * (t - Mathf.Pow(t, 2)));

                t += Time.deltaTime * 2f;
                yield return new WaitForEndOfFrame();
            }
            
            thisCollider.enabled = true;
        }
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
