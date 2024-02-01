using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int health = 100;
    [SerializeField] private int maxHealth = 100;

    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthBarText;

    void Start()
    {
        healthSlider.GetComponent<Slider>().value = CalculateSliderPercentage(health, maxHealth);
        healthBarText.text = "HP " + health + " / " + maxHealth;
    }

    void Update()
    {
        if (health > 0)
        {
            healthSlider.GetComponent<Slider>().value = CalculateSliderPercentage(health, maxHealth);
            healthBarText.text = "HP " + health + " / " + maxHealth;
        } else
        {
            health = 0;
            print("you died :3");
        }
    }

    private float CalculateSliderPercentage(int playerHealth, int maxPlayerHealth)
    {
        return (float)playerHealth / maxPlayerHealth;
    }

    public void removeHealth(int damage)
    {
        health -= damage;
    }

    public void addHealth(int heal)
    {
        health += heal;
    }
}
