using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private int health;
    [SerializeField]
    private int maxHealth = 100;

    public int Health
    {
        get { return health; }
    }

    public int MaxHealth
    {
        get { return maxHealth; }
    }



    public void Start()
    {
        health = maxHealth;
    }

    public void removeHealth(int damage){
        health -= damage;
    }

      public void addHealth(int heal){
        health += heal;
    }

    
    // Update is called once per frame
    void Update()
    {
        //when a player's health drops below 0 they die
        if(health <= 0){
            health = 0;
            print("you died :3");
        }


    }
}
