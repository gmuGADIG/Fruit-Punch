using System;
using UnityEngine;

/// <summary>
/// When the player attacks an enemy, only certain attacks (or auras) will land depending on the enemy. <br/>
/// This type is a flag, so masks are possible. An enemy might be vulnerable to all or none of them.
/// </summary>
[Flags]
public enum AuraType
{
    Strike   = 1 << 0,
    Throw    = 1 << 1,
    JumpAtk  = 1 << 2,
    Pearry   = 1 << 3,
    EnemyAtk = 1 << 4,
    
    Normal = Strike | Throw | JumpAtk | Pearry,
    Everything = Strike | Throw | JumpAtk | Pearry | EnemyAtk
}

public struct DamageInfo
{
    public GameObject source;
    public float damage;
    public Vector2 knockback;
    public AuraType aura;

    public DamageInfo(GameObject source, float damage, Vector2 knockback, AuraType aura)
    {
        this.source = source;
        this.damage = damage;
        this.knockback = knockback;
        this.aura = aura;
    }
}