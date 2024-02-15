using System.Collections.Generic;
using UnityEngine;
using static Utils;

/// <summary>
/// Creates a hurt box, dealing damage to players or enemies when they collide with the attached Collider2D. <br/>
/// The given <c>beltCharacter</c> is used for z-position checking. It may be attached to the same object or a (grand)parent. 
/// </summary>
public class HurtBox : MonoBehaviour
{
    private new Collider2D collider;
    public BeltCharacter beltCharacter;
    public LayerMask hitLayers;
    public float damage;

    private List<BeltCharacter> previousHits = new();

    private void Start()
    {
        this.GetComponentOrError(out collider);
        Assert(beltCharacter != null);
    }

    private void Update()
    {
        // handle collisions
        var hits = beltCharacter.GetOverlappingBeltCharacters(collider, hitLayers);
        foreach (var hit in hits)
        {
            if (previousHits.Contains(hit)) continue;
            // todo: handle collision here
            if (hit.TryGetComponent<Enemy>(out var enemy)) enemy.Hurt(this.damage);
        }
        previousHits = hits;
    }
}
