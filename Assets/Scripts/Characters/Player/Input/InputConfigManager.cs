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
}
