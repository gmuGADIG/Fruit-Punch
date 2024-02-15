using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Just detects when the jump button has been pressed and invokes an event saying it has
/// </summary>
[RequireComponent(typeof(PlayerInput))]
public class InputSensor : MonoBehaviour
{
    public Action jumped;
    void OnJump() {
        jumped?.Invoke();
    }
}
