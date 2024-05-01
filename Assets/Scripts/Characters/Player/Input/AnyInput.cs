using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnyInput : MonoBehaviour
{
    public InputActionReference action;

    [SerializeField] GameObject inputSensorPrefab;

    public Action<InputAction.CallbackContext> performed;
    public bool triggered => action.action.triggered;

    void Start() {
        foreach (Gamepad gamepad in Gamepad.all) {
            var playerInput = PlayerInput.Instantiate(
                inputSensorPrefab, 
                controlScheme: "Controller",
                pairWithDevice: gamepad
            );
            playerInput.PlayerInputActionOfActionId(action.action.id).performed += (c) => performed?.Invoke(c);
        }
        
        foreach (string scheme in new string[] { "keyboardLeft", "keyboardRight" }) {
            var playerInput = PlayerInput.Instantiate(
                inputSensorPrefab, 
                controlScheme: scheme,
                pairWithDevice: Keyboard.current
            );
            playerInput.PlayerInputActionOfActionId(action.action.id).performed += (c) => performed?.Invoke(c);
        }
    }
}
