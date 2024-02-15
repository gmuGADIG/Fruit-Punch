using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/*   This Code is a Piece of Garbage   */

public class PlayerGrab : MonoBehaviour
{
    public CircleCollider2D playerCollider;
    public bool emptyHands; //TODO use listener

    private void OnCollisionEnter(Collision otherCollision){                //Checking touch
        Pickup pickup = otherCollision.gameObject.GetComponent<Pickup>();   //Defining Pickup of Pickup Type (Pickup Script)
        if (pickup == null) {return;}                                       //No pickup, no grab
        if (Input.GetKeyDown(KeyCode.Q) && emptyHands)                      //Once grab key is known, change "Q"
        {
            //pickup.picktheFuckUp;
        }
    }

}
