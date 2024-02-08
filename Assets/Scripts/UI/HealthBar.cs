using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class HealthBar : MonoBehaviour
{


    private Health healthScript;

    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthBarText;

    void Awake()
    {
        healthScript = gameObject.GetComponent<Health>();
    }

    private float CalculateSliderPercentage(int playerHealth, int maxPlayerHealth)
    {
        return (float)playerHealth / maxPlayerHealth;
    }

    public void UpdateHealthBar()
    {
        healthSlider.GetComponent<Slider>().value = CalculateSliderPercentage(healthScript.health, healthScript.maxHealth);
        healthBarText.text = "HP " + healthScript.health + " / " + healthScript.maxHealth;
    }
}
