using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputTest : MonoBehaviour
{
    public GameObject SensorPrefab;
    public GameObject PlayerPrefab;

    HashSet<object> usedInputs = new();

    void Start()
    {
        // TODO: Detect gamepads added after Start
        foreach (Gamepad gamepad in Gamepad.all) {
            var playerInput = PlayerInput.Instantiate(
                SensorPrefab, 
                controlScheme: "Controller",
                pairWithDevice: gamepad
            );

            playerInput.GetComponent<InputSensor>().jumped += () => {
                if (!usedInputs.Contains(gamepad)) {
                    usedInputs.Add(gamepad);
                    PlayerInput.Instantiate(
                        PlayerPrefab,
                        controlScheme: "Controller",
                        pairWithDevice: gamepad
                    );
                }
            };
        }

        foreach (string scheme in new string[] {"KeyboardLeft", "KeyboardRight"}) {
            var playerInput = PlayerInput.Instantiate(
                SensorPrefab, 
                controlScheme: scheme,
                pairWithDevice: Keyboard.current
            );

            playerInput.GetComponent<InputSensor>().jumped += () => {
                if (!usedInputs.Contains(scheme)) {
                    usedInputs.Add(scheme);
                    PlayerInput.Instantiate(
                        PlayerPrefab,
                        controlScheme: scheme,
                        pairWithDevice: Keyboard.current
                    );
                }
            };
        }
    }
}
