using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCooldown : MonoBehaviour
{
    public float cooldownTime = 3; 

    private float nextFireTime = 0;

    // Update is called once per frame
    void Update()
    {
        if(Time.time > nextFireTime)
        {
            if(Input.GetMouseButtonDown(0))
            {
                print("Ability used, cooldown initiated");
                nextFireTime = Time.time + cooldownTime;
            }
        }
    }
}
