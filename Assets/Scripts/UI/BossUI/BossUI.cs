using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles boss title animation and health bar. Should be added to all boss scenes, and the boss should have a BossUITrigger component.
/// </summary>
public class BossUI : MonoBehaviour
{
    public static BossUI instance;

    [SerializeField] TMP_Text bossNameDisplay;
    [SerializeField] Image hpFill;
    [SerializeField] Image hpFillDelay;

    Health bossHealth;

    BossUI()
    {
        if (instance != null) Debug.LogWarning("Found two BossUIs existing at the same time!");
        instance = this;
    }

    void Update()
    {
        hpFillDelay.fillAmount = Mathf.MoveTowards(hpFillDelay.fillAmount, hpFill.fillAmount, Time.deltaTime * .1f);
    }

    public void Setup(string bossName, Health health)
    {
        gameObject.SetActive(true);
        bossHealth = health;
        bossNameDisplay.text = bossName;
        bossHealth.onHurt += OnBossHurt;
    }

    void OnBossHurt(DamageInfo damageInfo)
    {
        hpFill.fillAmount = bossHealth.CurrentHealth / bossHealth.MaxHealth;
    }
}
