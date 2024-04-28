using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricWaterScript : MonoBehaviour
{
    public float damageAmount = 2f;
    public float timeBetweenShocks = 1f;

    float timer;
    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Shock(collision.gameObject); //shocks player when they enter
            timer = Time.time + timeBetweenShocks;
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.tag == "Player" && timer < Time.time) //shock player at intervels if they stay
        {
            Shock(collision.gameObject);
            timer = Time.time + timeBetweenShocks;    
        }
    }

    void Shock(GameObject player)
    {
        DamageInfo shockInfo = new DamageInfo(damageAmount, Vector2.zero, AuraType.Normal);
        player.GetComponent<Health>().Damage(shockInfo);
    }
}
