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

    /// <summary>
    /// finds the health script and uses its values
    /// </summary>
    void Awake()
    {
        healthScript = gameObject.GetComponent<Health>();
    }

    /// <summary>
    /// divides current health and max health and return the percentage(float)
    /// </summary>
    /// <param name="playerHealth"></param>
    /// <param name="maxPlayerHealth"></param>
    /// <returns></returns>
    private float CalculateSliderPercentage(int playerHealth, int maxPlayerHealth)
    {
        return (float)playerHealth / maxPlayerHealth;
    }
    
    /// <summary>
    /// updates the ui element to be accurate
    /// </summary>
    public void UpdateHealthBar()
    {
        healthSlider.GetComponent<Slider>().value = CalculateSliderPercentage(healthScript.currentHealth, healthScript.maxHealth);
        healthBarText.text = "HP " + healthScript.currentHealth + " / " + healthScript.maxHealth;
    }
}
