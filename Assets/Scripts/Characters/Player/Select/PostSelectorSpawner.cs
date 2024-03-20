using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PostSelectorSpawner : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject playerOne = null;
    public GameObject playerTwo = null;
    private void Start()
    {
        if (GameManager.gameManager == null)
            GameManager.gameManager = new GameManager();

        if (GameManager.gameManager != null)
        {
            Debug.Log("not Null");
            if (GameManager.gameManager.playerOne != Character.None)
            {
                PlayerInput p = PlayerInput.Instantiate(
                playerPrefab,
                controlScheme: GameManager.gameManager.playerOneControlScheme,
                pairWithDevice: GameManager.gameManager.playerOneInputDevice
                );
            }

            if (GameManager.gameManager.playerTwo != Character.None)
            {
                PlayerInput p = PlayerInput.Instantiate(
                playerPrefab,
                controlScheme: GameManager.gameManager.playerTwoControlScheme,
                pairWithDevice: GameManager.gameManager.playerTwoInputDevice
                );
            }

        }
    }
}
