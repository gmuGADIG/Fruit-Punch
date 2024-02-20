using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

/// <summary>
/// A abstract script that makes a pickup that disappears on touch.
/// Can be extended to make other types of pickups, like healing or powerups.
/// </summary>
public abstract class Pickup : MonoBehaviour
{
    [ReadOnlyInInspector]
    private BeltCharacter beltCharacter;

    /// <summary>
    /// A list of all objects that collides with the pickup
    /// </summary>
    private List<BeltCharacter> collisions;

    private void Start()
    {
        beltCharacter = gameObject.GetComponent<BeltCharacter>();
    }

    /// <summary>
    /// Checks if pickup is colliding with the player before applying the pickup effect
    /// </summary>
    private void Update()
    {
        collisions = beltCharacter.GetOverlappingBeltCharacters(gameObject.GetComponent<Collider2D>(), Physics2D.AllLayers);
        foreach(BeltCharacter i in collisions)
        {
            GameObject obj = i.gameObject;
            if (obj != null && i.GetComponent<Player>())
            {
                Effect(obj);
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// The effect of the pickup. Something like healing or buffing the player.
    /// </summary>
    /// <param name="player"></param>
    public abstract void Effect(GameObject player);
}
