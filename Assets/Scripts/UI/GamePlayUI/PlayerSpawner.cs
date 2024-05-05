using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class PlayerSpawner : MonoBehaviour
{

    public PlayerHealthBarSpawner barSpawner;
    [SerializeField]
    private GameObject[] playerPrefabs;
    [SerializeField]
    private GameObject healthBarCanvasObject;
    public Transform playerOneSpawnPoint;
    public Transform playerTwoSpawnPoint;

    private void Start()
    {
        //game manger doesnt exist
        if (GameManager.gameManager == null)
        {
            Debug.LogError("Game Manager NOT FOUND / NOT INSTANCEATED");
            return;
        }

        if (healthBarCanvasObject == null)
        {
            Debug.LogWarning("HealthBarCanvasObject NOT ASSIGNED");
            healthBarCanvasObject = GameObject.Find("Canvas").transform.Find("Health Bars").gameObject;
            if (healthBarCanvasObject == null)
                Debug.LogError("HealthBarCanvasObject NOT FOUND AND NOT ASSIGNED");
        }
        
        for (int i = 0; i < GameManager.gameManager.PlayerCount(); i++)
        {
            SpawnPlayer(GameManager.gameManager.characters[i], i);
        }
    }

    public GameObject SpawnPlayer(Character character, int playerIndex)
    {
        var player = PlayerInput.Instantiate(
            playerPrefabs[(int)character],
            controlScheme: GameManager.gameManager.playerControlSchemes[playerIndex],
            pairWithDevice: GameManager.gameManager.playerInputDevices[playerIndex]
        ).gameObject;

        //bar.gameObject.transform.parent = healthBarCanvasObject.transform;
        
        player.transform.localScale = playerPrefabs[(int)character].transform.localScale;
        var spawnPoint = (playerIndex == 0) ? playerOneSpawnPoint : playerTwoSpawnPoint;
        player.transform.position = spawnPoint.position;

        return player;
    }

}
