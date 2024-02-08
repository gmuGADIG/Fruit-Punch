using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public struct Rebinding {
    public InputDevice Device;
    public string ControlScheme;
    public Guid ActionId;
    public string BindingPath;
}

public class InputConfigManager : MonoBehaviour
{
    public static InputConfigManager Instance;

    public List<Rebinding> Rebindings = new();

    public void ApplyRebindings() {
        var playerInputs = FindObjectsOfType<PlayerInput>();
        foreach (var rebinding in Rebindings) {
            var input = playerInputs
                .Where(p => p.currentControlScheme == rebinding.ControlScheme)
                .Where(p => p.devices.Contains(rebinding.Device))
                .First();
            
            if (input == null) {
                Debug.LogWarning("Found rebinding not associated to player input.");
                continue;
            }

            var actionIter = input.actions.actionMaps.Aggregate(
                new List<InputAction>().AsEnumerable(),
                (aux, next) => aux.Concat(next).Concat(next)
            );
            var action = actionIter
                .Where(action => action.id == rebinding.ActionId)
                .First();

            if (action == null) {
                Debug.LogError("Found rebinding not associated to an action.");
                continue;
            }

            action.ChangeBindingWithGroup(rebinding.ControlScheme)
                .WithPath(rebinding.BindingPath);
        }
    }

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Rebinds an action for a player input. <br/>
    /// This function will wait for user input and rebind the action to said 
    /// user input. <br/>
    /// This will also create a record of the rebind in the 
    /// InputConfigManager's Rebindings array for the device the player input 
    /// is using. It will call afterRebinding after the action is rebound.
    /// </summary>
    /// <param name="input">The PlayerInput in question.</param>
    /// <param name="actionId">The action to rebind.</param>
    /// <param name="afterRebinding">Callback for after the action has been rebound.</param>
    public static void StartRebinding(
            PlayerInput input, 
            Guid actionId,
            Action? afterRebinding
    ) {
        var action = input.PlayerInputActionOfActionId(actionId);
        action.Disable();
        action.PerformInteractiveRebinding()
            .OnComplete(operation => {
                action.Enable();
                // note: all of the bindings for this action were set
                // to the new binding path, hence why action.bindings[0] works
                var bindingPath = action.bindings[0].effectivePath;
                Instance.Rebindings.Add(new Rebinding {
                    Device = input.devices[0],
                    ControlScheme = input.currentControlScheme,
                    ActionId = action.id,
                    BindingPath = bindingPath
                });

                operation.Dispose(); // manual memory management: c# is not a real language

                afterRebinding?.Invoke();
            })
            .Start();
    }
}
