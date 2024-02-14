using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TestInputPlayerSpawner : MonoBehaviour
{
    public GameObject prefab;

    public void SpawnPlayer(JoinContext context) {
        PlayerInput.Instantiate(
            prefab,
            controlScheme: context.ControlScheme,
            pairWithDevice: context.InputDevice
        );

        InputConfigManager.Instance.ApplyRebindings();
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Delete)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
