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
/// </summary>
public class HurtBox : MonoBehaviour
{
    [Tooltip("Only these layers will be considered when dealing damage. Generally, player hurtboxes will hit the `Enemy` layer and enemies will hit `Player` layer.")]
    public LayerMask hitLayers;
    
    [Tooltip("How much damage is dealt when an enemy is hit (assuming vulnerable aura)")]
    public float damage;

    [Tooltip("How much knockback this hurtbox does.")]
    public float knockback = .1f;

    //[Tooltip("For dealing knockback, the hurtbox checks if the parent is flipped in the x-axis (negative scale).")]
    //public Transform parentTransform;
    
    [Tooltip("This attack only hurts enemies vulnerable to this Aura. For enemy hurtboxes, use the type `Enemy Atk`.")]
    [SerializeField] private AuraType aura;
    
    /// <summary>
    /// Invoked when this hurt box hurts something.
    /// </summary>
    public Action<DamageInfo> onHurt;
    
    void Start()
    {
        //Assert(aura != 0);
        //Assert(parentTransform != null);
        Assert(hitLayers != 0);
    }

    public DamageInfo GetDamageInfo()
    {
        Vector2 knockback = Vector2.right * this.knockback;
        return new DamageInfo(this.damage, knockback, this.aura);
    }
}
