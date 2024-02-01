using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestInputPlayerSpawner : MonoBehaviour
{
    public GameObject prefab;

    void Start()
    {
        PlayerInput.Instantiate(
            prefab, 
            controlScheme: "KeyboardLeft", 
            pairWithDevice: Keyboard.current
        );

        PlayerInput.Instantiate(
            prefab, 
            controlScheme: "KeyboardRight", 
            pairWithDevice: Keyboard.current
        );
    }
}
