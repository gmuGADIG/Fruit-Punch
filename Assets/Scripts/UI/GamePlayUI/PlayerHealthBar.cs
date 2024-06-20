using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerHealthBar : MonoBehaviour
{
    //public PlayerHealth playerHealth;
    [SerializeField] TMP_Text healthUI;
    [SerializeField] Image maxHealthBar;
    [SerializeField] Image currentHealthBar;

    float maxHealth;

    [SerializeField]
    private Sprite[] CharacterPortraits;
    
    // Attaches the health bar to the player's health and adjusts all necessary UI to fit the player.
    // Called by PlayerHealthBarHolder
    public void Setup(Player player)
    {
        var health = player.GetComponent<Health>();
        this.maxHealth = health.MaxHealth;
        health.onHealthChange += UIUpdate;
    }

    void UIUpdate(HealthChange healthChange)
    {
        healthUI.text = healthChange.newHealthValue + "/" + maxHealth;
        currentHealthBar.fillAmount = healthChange.newHealthValue / maxHealth;
    }
}
