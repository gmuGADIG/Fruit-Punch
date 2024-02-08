using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayerSelectorSpawner : MonoBehaviour
{
    public GameObject prefab;
    public void ConnectPlayer(JoinContext context)
    {
        PlayerInput.Instantiate(
            prefab,
            controlScheme: context.ControlScheme,
            pairWithDevice: context.InputDevice
        );
    }
}
