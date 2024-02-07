using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lateral_Player_Movement : MonoBehaviour
{

    public float speed = 10f;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Get only horizontal input, add it onto the existing X position without touching any other axis.
        float horizontalInput = Input.GetAxis("Horizontal");
        transform.position+= new Vector3(horizontalInput*speed*Time.deltaTime,0,0);

    }
}
