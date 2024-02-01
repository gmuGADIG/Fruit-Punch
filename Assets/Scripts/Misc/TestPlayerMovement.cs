using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class TestPlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    public Vector2 moveDir = Vector2.zero;
    private InputAction move;
    private InputAction fire;

    PlayerInput input;

    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent(out input);
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
}
