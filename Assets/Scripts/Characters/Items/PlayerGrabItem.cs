using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrabItem : MonoBehaviour
{

    #region Vars

    #region Public Vars
    [Tooltip("Boolen to deteremine if a player currently has an item.")]
    public bool hasItem;

    [Tooltip("How hard an item will be thrown by the player. This var is a float.")]
    public float throwForce = 5;
    [Tooltip("Player test object speed. Currently this var is an int.")]
    public int playerSpeed = 5;
    #endregion

    #region Private Vars
    [Tooltip("The belt character component for the character.")]
    BeltCharacter bc;
    [Tooltip("This is the 2D collider of the character.")]
    Collider2D col;

    [Tooltip("This is the list of overlapping items that the player is currently standing over. Only items within this list will be able to be grabbed. This list is also only updated when the player presses E or the Interact key.")]
    List<BeltCharacter> overlappingItems;
    #endregion

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        bc = GetComponent<BeltCharacter>(); // Getting the belt char comp for the player
        col = GetComponent<Collider2D>(); // Getting the player's collider. 
        hasItem = false; // Setting hasItem to false, as the character doesn't start with an item in hand. 
    }

    // Update is called once per frame
    void Update()
    {
        #region PlayerTESTMovement TO DELETE
        //Test Movement for the player test object. Remove when no longer needed.
        if (Input.GetKey(KeyCode.A))
        {
            bc.internalPosition += new Vector3(-playerSpeed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.D)) 
        {
            bc.internalPosition += new Vector3(playerSpeed * Time.deltaTime, 0, 0);
        }
        #endregion

        #region Interact System
        //Current way items can be picked up and dropped. This should be changed when able.
        if (Input.GetKeyDown(KeyCode.E) && hasItem == false)
        {
            GetItem();
        }

        if (Input.GetKeyDown(KeyCode.Space) && hasItem == true)
        {
            ThrowItem();
        }
        #endregion

    }

    #region Custom Functions

    #region Get Item Function
    /// <summary>
    /// This function determines how a player will be able to grab an item. Items will only be added to the list if the player collider overlaps with the enemy collider. Function will disable the Items BeltCharacter and make it a child of the player.
    /// </summary>
    public void GetItem()
    {
        overlappingItems = bc.GetOverlappingBeltCharacters(col, LayerMask.GetMask("Item"));
        for(int i = 0; i < overlappingItems.Count; i++)
        {
            BeltCharacter currentBC = overlappingItems[i];
            if(hasItem == false)
            {
                currentBC.GetComponent<BeltCharacter>().enabled = false;
                currentBC.transform.parent = transform;
                hasItem = true;
            }

        }
        
    }
    #endregion

    #region Throw Item Function
    /// <summary>
    /// This function will throw an item away from the player. Function reenables the Belt Character for the child and adds to the transform of the item with throwForce. Finally, item is detached from the parent gameobject.
    /// </summary>
    public void ThrowItem()
    {
        GetComponentInChildren<BeltCharacter>().enabled = true;
        GetComponentInChildren<Transform>().position += new Vector3(throwForce, throwForce, 0);
        
        transform.DetachChildren();
        hasItem = false;
    }
    #endregion

    #endregion

}
