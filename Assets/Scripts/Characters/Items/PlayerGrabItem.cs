using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrabItem : MonoBehaviour
{
    public string itemTag;
    public bool hasItem;
    public GameObject currentItem;

    float throwForce = 5;
    int playerSpeed = 5;

    BeltCharacter bc;
    Collider2D col;

    // Start is called before the first frame update
    void Start()
    {
        bc = GetComponent<BeltCharacter>();
        hasItem = false;
    }

    // Update is called once per frame
    void Update()
    {
        #region PlayerTESTMovement TO DELETE
        if (Input.GetKey(KeyCode.A))
        {
            bc.internalPosition += new Vector3(-playerSpeed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.D)) 
        {
            bc.internalPosition += new Vector3(playerSpeed * Time.deltaTime, 0, 0);
        }
        #endregion

        if(Input.GetKey(KeyCode.E) && hasItem)
        {
            currentItem.GetComponent<Rigidbody2D>().AddForce(currentItem.transform.forward * Time.deltaTime * throwForce);
        }

        bc.GetOverlappingBeltCharacters(col, LayerMask.GetMask("Item"));

    }

    public void GetItem()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            //Get Current Item
            //Foreach something
        }
    }


    public void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Hitting E");
            if (other.gameObject.CompareTag(itemTag))
            {
                Debug.Log("Got Item");
                currentItem = other.gameObject;
                currentItem.transform.parent = this.transform;
                hasItem = true;
            }
        }
    }

}
