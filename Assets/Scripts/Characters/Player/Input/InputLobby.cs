using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// Data about the input of the player joining.
/// </summary>
public struct JoinContext {
    /// <summary>
    /// The control scheme the player is using. 
    /// (i.e. controller / left side of keyboard / right side of keyboard)
    /// </summary>
    public string ControlScheme;

    /// <summary>
    /// The input device the player is using.
    /// </summary>
    public InputDevice InputDevice;
}

/// <summary>
/// Detects input for when the players want to join and sends an event when a 
/// player tries to join.
/// </summary>
public class InputLobby : MonoBehaviour
{
    public GameObject InputSensorPrefab;

    /// <summary>
    /// Invoked when the player is trying to join the game.
    /// </summary>
    public UnityEvent<JoinContext> OnPlayerJoin = new();
    HashSet<object> usedInputs = new();

    void Start()
    {
        // TODO: Detect gamepads added after Start
        foreach (Gamepad gamepad in Gamepad.all) {
            var playerInput = PlayerInput.Instantiate(
                InputSensorPrefab, 
                controlScheme: "Controller",
                pairWithDevice: gamepad
            );

            playerInput.GetComponent<InputSensor>().jumped += () => {
                if (!usedInputs.Contains(gamepad)) {
                    usedInputs.Add(gamepad);
                    OnPlayerJoin.Invoke(new JoinContext {
                        ControlScheme = "Controller",
                        InputDevice = gamepad
                    });
                }
            };
        }
        
        foreach (string scheme in new[] { "keyboardLeft", "keyboardRight" }) {
            var playerInput = PlayerInput.Instantiate(
                InputSensorPrefab, 
                controlScheme: scheme,
                pairWithDevice: Keyboard.current
            );

            playerInput.GetComponent<InputSensor>().jumped += () => {
                if (!usedInputs.Contains(scheme)) {
                    usedInputs.Add(scheme);
                    OnPlayerJoin.Invoke(new JoinContext {
                        ControlScheme = scheme,
                        InputDevice = Keyboard.current
                    });
                }
            };
        }
    }
}
