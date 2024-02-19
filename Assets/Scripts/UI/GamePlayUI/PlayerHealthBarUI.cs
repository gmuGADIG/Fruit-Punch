using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerHealthBarUI : MonoBehaviour
{
    //public PlayerHealth playerHealth;
    public TMP_Text healthUI;
    public Image maxHealthBar;
    public Image currentHealthBar;

    [SerializeField]
    private Health playerHealth;

    // Start is called before the first frame update
    void Start()
    {
        playerHealth.onHealthChange += UIUpdate;
    }

    void UIUpdate(HealthChange currentHealth)
    {
        healthUI.text = currentHealth.newHealthValue + "/" + playerHealth.MaxHealth;
        currentHealthBar.fillAmount = currentHealth.newHealthValue / playerHealth.MaxHealth;
    }
}
