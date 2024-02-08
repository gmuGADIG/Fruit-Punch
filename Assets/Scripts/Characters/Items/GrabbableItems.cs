using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



public class GrabbableItems : MonoBehaviour
{

    /*
     * Player object needs to hold the item
     *  Becomes Child of player.
     *  Player use key should player Animation.
     *  
     *  Item Stats to keep:
     *      Prefab?
     *      Damage
     *      Durability?
     *      Recharge Time? - Might be player attack speed.
    */

    public ItemScriptable item;

    [Tooltip("Belt Character Script")]
    BeltCharacter beltChar;
    [Tooltip("2D Collider")]
    Collider2D col;

    // Start is called before the first frame update
    void Start()
    {
        beltChar = GetComponent<BeltCharacter>();
        col = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }



}
