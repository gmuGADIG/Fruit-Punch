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

    public void OnStrike() {
        Debug.Log("Strike Pressed!");
    }

    public void OnDebugRebindControl() {
        Debug.Log("DebugRebindControl Pressed!");

        var actionIter = input.actions.actionMaps.Aggregate(
            new List<InputAction>().AsEnumerable(),
            (aux, next) => aux.Concat(next).Concat(next)
        );
        var action = actionIter
            .Where(action => action.id == strike.action.id)
            .First();

        action.Disable();
        action.PerformInteractiveRebinding()
            // .OnApplyBinding((operation, bindingPath) => {
            //     var arr = InputConfigManager.Instance.Rebindings;
            //     arr.Add(new Rebinding {
            //         Device = input.devices[0],
            //         ControlScheme = input.currentControlScheme,
            //         ActionId = action.id,
            //         BindingPath = bindingPath
            //     });
            // })
            .OnComplete(operation => {
                Debug.Log("Rebinding complete!");

                action.Enable();
                var bindingPath = action.bindings[0].effectivePath;
                var arr = InputConfigManager.Instance.Rebindings;
                arr.Add(new Rebinding {
                    Device = input.devices[0],
                    ControlScheme = input.currentControlScheme,
                    ActionId = action.id,
                    BindingPath = bindingPath
                });

                operation.Dispose(); // manual memory management: c# is not a real language
            })
            .Start();
    }
}
