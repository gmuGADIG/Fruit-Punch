using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemGravity : MonoBehaviour
{

    [Tooltip("The ideal speed at which an item drops to the ground.")]
    public float gravityRate = 9.8f; // 9.8 m/s is IRL gravity so...
    [Tooltip("An array of all active players in a scene.")]
    Player[] player; //Yeah, it does it in update which is a bad spot but what could go wrong?

    [Tooltip("The Belt Character attached to this item.")]
    BeltCharacter bc; //Belt Character Attached to the item.

    // Start is called before the first frame update
    void Start()
    {
        bc = GetComponent<BeltCharacter>(); // Getting the belt char comp for the item
        player = FindObjectsOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        for(int i  = 0; i < player.Length; i++)
        {
            if (!transform.IsChildOf(player[i].transform)) //If child, turn back on your internal position at the transform 
            {
                bc.internalPosition = transform.position;
                bc.enabled = true;
            }

        }


        //Notes: 
        // Transform.position.x = interalPosition.x
        // transform.position.y = interalPosition.z 
        // transform.position.z = interalPosition.y
        // Why would you make it like this?

        // interalPosition(transform.x, transform.z, transform.y)

        if ( bc.internalPosition.z > 0 )
        {

            bc.internalPosition -= new Vector3(transform.position.x, transform.position.z, -gravityRate * Time.deltaTime * transform.position.y);

        }

        if(bc.internalPosition.z < 0)
        {
            bc.internalPosition = new Vector3(transform.position.x, 0, transform.position.y * Time.deltaTime);
        }


    }
}
