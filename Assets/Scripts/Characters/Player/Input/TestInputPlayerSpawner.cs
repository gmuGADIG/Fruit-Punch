using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestInputPlayerSpawner : MonoBehaviour
{
    public GameObject prefab;

    public void SpawnPlayer(JoinContext context) {
        PlayerInput.Instantiate(
            prefab,
            controlScheme: context.ControlScheme,
            pairWithDevice: context.InputDevice
        );
    }
}
