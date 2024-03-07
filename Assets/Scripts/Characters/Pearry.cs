using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
// This may need to be added to the player inputs later
public class Pearry : MonoBehaviour
{

    public BeltCharacter player;

    public HurtBox hurtBox;
    //true if player is in a pearry, false otherwise
    public bool pearry;
    //The amount of time the player is pearrying for (Default is 0.08 sec)
    public float pearryTime = 0.08f;
    //The current cooldown for the pearry
    private float cooldown = 0;
    //The customizable cooldown that gets set to the cooldown timer (Default is 0.5 sec)
    public float cooldownTime = 0.5f;

    public Vector2 knockback = new Vector2(1, 1);

    //damage to be inflicted on enemy
    public int DamageAbsorbed = 10;

    private Enemy enemy;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool isKeyDown = Input.GetKey(KeyCode.Q);

        if (isKeyDown && cooldown <= 0) {
            pearry = true;
            print("Pearry started");
            InflictRetalitoryDamage();
            cooldown = cooldownTime;
        }

        if (pearry) {
            print("Is pearrying");
            pearryTime -= Time.deltaTime;
            if(pearryTime <= 0)
            {
                pearry = false;
                pearryTime = 0.08f;
                print("Ended pearry");
            }
        }
        cooldown -= 1 * Time.deltaTime;
    }

    void InflictRetalitoryDamage() 
    {
        //Need the script where enemies damage players
        //enemy.Hurt(DamageAbsorbed, knockback);

    }

    //Need Projectile object
    void reflectProjectile() 
    {

    }
}
