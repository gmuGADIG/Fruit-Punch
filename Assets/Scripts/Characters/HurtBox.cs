using System;
using System.Collections.Generic;
using UnityEngine;
using static Utils;

public struct HurtContext {
    public Health ThingTakingDamage;
    public DamageInfo DamageInfo;
}

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
    public Vector2 knockback;
    [SerializeField] private AuraType _aura;

    public AuraType aura {
        get => _aura;
        set {
            _aura = value;
            previousHits.Clear();
        }
    }

    private List<BeltCharacter> previousHits = new();

    // TODO
    public event Action<HurtContext> OnHurt;

    private void Start()
    {
        this.GetComponentOrError(out collider);
        Assert(beltCharacter != null);
    }

    private void OnEnable()
    {
        previousHits.Clear();
    }

    private void Update()
    {
        // handle collisions
        var hits = beltCharacter.GetOverlappingBeltCharacters(collider, hitLayers);
        foreach (var hit in hits)
        {
            if (previousHits.Contains(hit)) continue;
            // todo: handle collision here
            if (hit.TryGetComponent<Health>(out var thingTakingDamage))
            {
                print($"Aura: {aura}");

                // Makes the thingTakingDamage take damage based on the aura, 
                // damage and knockback (set in the hurtbox)
                var damageInfo = new DamageInfo(damage, knockback, aura);
                thingTakingDamage.Damage(damageInfo); 
                OnHurt?.Invoke(new HurtContext {
                    ThingTakingDamage = thingTakingDamage,
                    DamageInfo = damageInfo
                });
            }
        }   
        previousHits = hits;
    }
}
