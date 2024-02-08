using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    private Rigidbody rigidBody;
    private float jumpAmount;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown(KeyCode.UpArrow))
        {
            rigidBody.AddForce(Vector2.up * jumpAmount, ForceMode2D.Impulse);
        }
    }
}
