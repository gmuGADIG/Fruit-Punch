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

    /// <summary>
    /// Invoked when this hurt box tries to hurt something, but is pearried.
    /// </summary>
    public Action<DamageInfo> onPearried;

    private HashSet<Health> hurtThisCycle = new();

    public bool HasHurtThisCycle(Health health) {
        if (GetComponentInParent<Player>() != null) return false;

        if (hurtThisCycle.Contains(health)) {
            return true;
        } 

        hurtThisCycle.Add(health);
        return false;
    }
    
    void OnEnable() {
        hurtThisCycle.Clear();
    }
    
    void Start()
    {
        Assert(hitLayers != 0);
    }

    public DamageInfo GetDamageInfo()
    {
        Vector2 knockback = transform.right * this.knockback;
        return new DamageInfo(gameObject, this.damage, knockback, this.aura);
    }

    public void SetAura(AuraType aura)
    {
        this.aura = aura;
    }
}
