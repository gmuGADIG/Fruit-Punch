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

    private void Awake()
    {
        StartCoroutine(DelayStartUp(1f));
    }

    public IEnumerator DelayStartUp(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        StartUp();
    }

    public void StartUp()
    {
        //game manger doesnt exist
        if (GameManager.gameManager == null)
        {
            Debug.LogError("Game Manager NOT FOUND / NOT INSTANCEATED");
            return;
        }

        //unassigned public var
        if (healthBarCanvasObject == null)
        {
            Debug.LogWarning("HealthBarCanvasObject NOT ASSIGNED");
            healthBarCanvasObject = GameObject.Find("Canvas").transform.Find("Health Bars").gameObject;
            if (healthBarCanvasObject == null)
                Debug.LogError("HealthBarCanvasObject NOT FOUND AND NOT ASSIGNED");
        }

        GameObject pOne = null;
        GameObject pTwo = null;

        switch (GameManager.gameManager.PlayerCount())
        {

            case 0:
                Debug.LogError("gameManager.PlayerCount() was 0");
                break;
            case 1:
                pOne = SpawnPlayer(GameManager.gameManager.playerOne, true);
                barSpawner.SpawnHealthBar(GameManager.gameManager.playerOne, pOne);
                break;
            case 2:
                pOne = SpawnPlayer(GameManager.gameManager.playerOne, true);
                barSpawner.SpawnHealthBar(GameManager.gameManager.playerOne, pOne).transform.SetAsFirstSibling();
                pTwo = SpawnPlayer(GameManager.gameManager.playerTwo, false);
                barSpawner.SpawnHealthBar(GameManager.gameManager.playerTwo, pTwo);
                break;
            default:
                Debug.LogError("gameManager.PlayerCount() return a value that was not 0,1, or 2");
                break;
        }
    }

    public GameObject SpawnPlayer(Character character, bool isPlayerOne)
    {
        GameObject player;
        
        if (isPlayerOne)
        {
            player = PlayerInput.Instantiate(
                playerPrefabs[(int)character],
                controlScheme: GameManager.gameManager.playerOneControlScheme,
                pairWithDevice: GameManager.gameManager.playerOneInputDevice
            ).gameObject;
        }
        else 
        {
            player = PlayerInput.Instantiate(
                playerPrefabs[(int)character],
                controlScheme: GameManager.gameManager.playerTwoControlScheme,
                pairWithDevice: GameManager.gameManager.playerTwoInputDevice
            ).gameObject;
        }

        //bar.gameObject.transform.parent = healthBarCanvasObject.transform;
        
        player.transform.localScale = playerPrefabs[(int)character].transform.localScale;
        if (isPlayerOne)
            player.transform.position = playerOneSpawnPoint.transform.position;
        else
            player.transform.position = playerTwoSpawnPoint.transform.position;

        return player;
    }

}
