using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class TestPlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    public Vector2 moveDir = Vector2.zero;
    public InputActionReference strike;

    PlayerInput input;

    void Start() {
        TryGetComponent(out input);
    }

    void Update() {
        var jumpAction = input.actions["gameplay/Jump"];
        if (jumpAction.WasPerformedThisFrame()) {
            Debug.Log("Player jumped!");
        }
    }

    void FixedUpdate() {
        var leftRight = input.actions["gameplay/Left/Right"];
        var upDown = input.actions["gameplay/Up/Down"];
        var moveDir = Vector3.ClampMagnitude(new Vector3(
            leftRight.ReadValue<float>(),
            upDown.ReadValue<float>(),
            0
        ), 1);

        transform.position += moveDir * Time.fixedDeltaTime * 2f * moveSpeed;
    }

    public void OnStrike() {
        Debug.Log("Strike Pressed!");
    }

    public void OnDebugRebindControl() {
        Debug.Log("DebugRebindControl Pressed!");

        InputConfigManager.StartRebinding(
            input,
            strike.action.id,
            () => Debug.Log("Rebinding complete!")
        );
    }
}
