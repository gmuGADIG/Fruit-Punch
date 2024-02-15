using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarUI : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public TMP_Text healthUI;
    public Image maxHealthBar;
    public Image currentHealthBar;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        int health = playerHealth.Health;
        int maxHealth = playerHealth.MaxHealth;
        healthUI.text = health.ToString() + "/" + maxHealth;
        currentHealthBar.fillAmount = (float) health / maxHealth;
    }
}
