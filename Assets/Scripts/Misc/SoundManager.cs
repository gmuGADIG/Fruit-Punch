using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [RequireComponent(typeof(PlayerInput))]

    // Start is called before the first frame update
    void Start()
    {
        NormalUpdate();
    }

    // Update is called once per frame
    void NormalUpdate()
    {
        // Print statements to be used as placeholders for sounds
        if (playerInput.actions["gameplay/Jump"].triggered)
        {
            print("Jump sound");
        }
        
        if (playerInput.actions["gameplay/Strike"].triggered)
        {
            print("Strike sound");
        }

        if (playerInput.actions["gameplay/Pearry"].triggered)
        {
            print("Pearry sound");
        }
        
        if (playerInput.actions["gameplay/Interact"].triggered)
        {
            if (grabber.GrabItem()) print("Grabbing sound");
        } 
    }
}
