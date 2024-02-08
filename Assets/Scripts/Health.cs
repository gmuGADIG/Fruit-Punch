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

    [SerializeField]
    public float maxHealth = 100;
    [ReadOnlyInInspector]
    private float currentHealth;

    private bool isPlayer;

    /// <summary>
    /// Invoked when this character's health reaches zero.
    /// </summary>
    private event Action onDeath;

    /// <summary>
    /// Invoked with the amount of damage and knockback whenever hurt.
    /// </summary>
    private event Action<float, Vector2> onHurt;

    /// <summary>
    /// Invoked when the player health is changed to update the health bar.
    /// </summary>
    private event Action onPlayerHealth;
    
    void Start()
    {
        currentHealth = maxHealth;
        if(this.tag == "Player"){
            isPlayer = true;
        } else
        {
            isPlayer = false;
        }
    }
    
    /// <summary>
    /// Damages the character, removing it's health. Knockback should be handled in a separate script.
    /// </summary>
    public void Damage(float amount, Vector2 knockback)
    {
        if(currentHealth - amount <= 0)
        {
            currentHealth = 0;
        }else
        {
            currentHealth -= amount;
        }
        if (isPlayer)
        {
            onPlayerHealth?.Invoke();
        }
        onHurt.Invoke(amount, knockback);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isPlayer)
        {
            onPlayerHealth?.Invoke();
        }
        currentHealth = Mathf.MoveTowards(currentHealth, maxHealth, amount);
    }

    private void Die()
    {
        onDeath?.Invoke();
    }

}
