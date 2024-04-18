using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputTest : MonoBehaviour
{
    AnyInput anyInput;
    void Start() {
        anyInput = GetComponent<AnyInput>();
        anyInput.performed += (_c) => Debug.Log("performed");
    }

    void Update() {
        if (anyInput.triggered) Debug.Log("triggered");
    }
}
