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
    [SerializeField] TMP_Text hpText;
    [SerializeField] Image hpFill;
    [SerializeField] Image characterPortrait;
    [SerializeField] Sprite[] portraits;

    float maxHealth;

    
    // Attaches the health bar to the player's health and adjusts all necessary UI to fit the player.
    // Called by PlayerHealthBarHolder
    public void Setup(Player player)
    {
        var health = player.GetComponent<Health>();
        this.maxHealth = health.MaxHealth;
        health.onHealthChange += (h) => UpdateHealth(h.newHealthValue);
        UpdateHealth(health.MaxHealth);

        characterPortrait.sprite = portraits[(int)player.playerCharacter];
    }

    void UpdateHealth(float newHealth)
    {
        hpText.text = newHealth + "/" + maxHealth;
        hpFill.fillAmount = newHealth / maxHealth;
    }
}
