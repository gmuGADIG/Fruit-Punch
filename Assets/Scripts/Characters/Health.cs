using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Our current goal is to create a generalized script describing enemy health.

Methods:
Enemy Health, (track with a public variable in GUI - maybe serialize?)
Enemy Damaged, (function damage - effect enemy health)
Enemy Dead, (within enemy damaged function)

Max Health, Current Health, Decrease and Increase health functions.

Magic of Events Implemented by Justin from designed code. 

*/
public class Health : MonoBehaviour
{
    public AuraType vulnerableTypes;
    
    [SerializeField]
    private float maxHealth = 100;

    public float MaxHealth => maxHealth;

#if UNITY_EDITOR
    [ReadOnlyInInspector]
#endif
    [SerializeField]
    private float currentHealth;
    public float CurrentHealth => currentHealth;
    /// <summary>
    /// Invoked when this character's health reaches zero. <br/>
    /// (run after onHealthChange and onHurt)
    /// </summary>
    public event Action onDeath;

    /// <summary>
    /// Invoked with the amount of damage and knockback whenever hurt. <br/>
    /// (run after onHealthChange and before onDeath)
    /// </summary>
    public event Action<DamageInfo> onHurt;

    /// <summary>
    /// Invoked with the new health whenever it's changed, positively or negatively. <br/>
    /// (run before onHurt and onDeath)
    /// </summary>
    public event Action<HealthChange> onHealthChange;

    public event Action<DamageInfo> onDamageImmune;

    public event Action<AuraType> onAuraChange;
    
    void Start()
    {
        currentHealth = maxHealth;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<HurtBox>(out var hurtBox))
        { // TODO: should we be handle hurting in the hurtbox?
            var hitsThisLayer = ((1 << this.gameObject.layer) & hurtBox.hitLayers) > 0;
            if (hitsThisLayer) {
                var info = hurtBox.GetDamageInfo();
                this.Damage(info);
                if (IsVulnerableTo(info.aura)) {
                    hurtBox.onHurt?.Invoke(info);
                }
            }
        }
    }

    /// <summary>
    /// Attempts to damage the character, decreasing its health towards zero (no lower). <br/>
    /// Factors in the aura of the attack, and ignoring it's damage if this character isn't vulnerable. <br/>
    /// Doesn't do anything with knockback; that's up the player or enemy script. <br/>
    /// (onHealthChange, onHurt, and onDeath are invoked if applicable)
    /// </summary>
    public void Damage(DamageInfo info)
    {
        if (this.currentHealth <= 0) return; // don't die twice. probably gonna be convenient later.
        if (!IsVulnerableTo(info.aura))
        {
            onDamageImmune?.Invoke(info);
            return;
        }
        
        currentHealth = Mathf.MoveTowards(currentHealth, 0, info.damage);
        AuraBreak();
        
        print($"{gameObject.name}'s health lowered to {currentHealth}");
        
        onHealthChange?.Invoke(new HealthChange(currentHealth));
        onHurt?.Invoke(info);
        
        
        if (currentHealth <= 0) Die();
    }

    /// <summary>
    /// Returns true if this character is vulnerable to an attack's aura.
    /// </summary>
    public bool IsVulnerableTo(AuraType attackingAura)
    {
        var effectiveAuras = this.vulnerableTypes & attackingAura;
        return effectiveAuras != 0;
    }

    /// <summary>
    /// Heals the character, increasing its health towards the maximum (no higher). <br/>
    /// (onHealthChange is invoked with the new health value)
    /// </summary>
    /// <param name="amount"></param>
    public void Heal(float amount)
    {
        currentHealth = Mathf.MoveTowards(currentHealth, maxHealth, amount);
        onHealthChange?.Invoke(new HealthChange(currentHealth));
    }

    private void Die()
    {
        onDeath?.Invoke();
    }

    public bool HasAura()
    {
        return vulnerableTypes.IsSpecial();
    }

    /// <summary>
    /// When hit by an effective aura, the enemy becomes vulnerable to all damage types.
    /// </summary>
    public void AuraBreak()
    {
        this.vulnerableTypes = AuraType.Everything;
        onAuraChange?.Invoke(vulnerableTypes);
    }
}

public struct HealthChange
{
    public float newHealthValue;
    public HealthChange(float newHealthValue)
    {
        this.newHealthValue = newHealthValue;
    }
}
