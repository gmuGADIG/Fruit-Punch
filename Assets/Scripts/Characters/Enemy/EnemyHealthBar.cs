using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Slider healthBar;

    Health enemyHealth;

    void Start()
    {
        enemyHealth = GetComponentInParent<Health>();
        enemyHealth.onHealthChange += UpdateHealthbar;
        //healthBar.maxValue = enemyHealth.MaxHealth;
        //healthBar.minValue = 0;
    }

    void Update() {
        transform.rotation = Quaternion.identity;
    }

    void UpdateHealthbar(HealthChange change)
    {
        healthBar.value = change.newHealthValue / enemyHealth.MaxHealth * healthBar.maxValue;
        if (healthBar.value <= 0)
        {
            //Destroy(enemyHealth.gameObject);
        }
    }
}
