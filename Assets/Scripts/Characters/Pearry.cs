using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pearry : MonoBehaviour
{

    public BeltCharacter player;

    //public BeltCharacter enemy;

    public HurtBox hurtBox;

    public bool pearry;

    //damage to be inflicted on enemy
    public int DamageAbsorbed;

    private Enemy enemy;
    //Using player input to capture C input
    /*
    PlayerInput.Instantiate(
        playerPrefab,
        controlScheme: "KeyboardLeft",
        pairWithDevice: Keyboard.current
        );
    */

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //bool isKeyDown = Input.GetKey(KeyCode.C);

        //add cooldown
        //if (isKeyDown) {
        //    pearry = true;
        //    print("Pearry active");
        //    InflictRetalitoryDamage();
        //}
        //else {
        //    pearry = false;
        //}

        /*
        var pearryAction = input.actions["gameplay/Pearry"];
        if (pearryAction.wasPreformedThisFrame())
        {
            print("Pearry active");
        }
        */
    }

    void InflictRetalitoryDamage() 
    {
        enemy.Hurt(DamageAbsorbed);

    }

    //Need Projectile object
    void reflectProjectile() 
    {

    }
}
