using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{

    public PlayerHealthBarSpawner barSpawner;
    [SerializeField]
    private GameObject[] playerPrefabs;
    [SerializeField]
    private GameObject healthBarCanvasObject;
    public Transform playerOneSpawnPoint;
    public Transform playerTeoSpawnPoint;

    private void Awake()
    {
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

        GameObject pOne = null;
        GameObject pTwo = null;
        switch (GameManager.gameManager.PlayerCount())
        {
            
            case 0:
                Debug.LogError("gameManager.PlayerCount() was 0");
                break;
            case 1:
                pOne = SpawnPlayer(GameManager.gameManager.playerOne);
                barSpawner.SpawnHealthBar(GameManager.gameManager.playerOne, pOne);
                break;
            case 2:
                pOne = SpawnPlayer(GameManager.gameManager.playerOne);
                barSpawner.SpawnHealthBar(GameManager.gameManager.playerOne, pOne).transform.SetAsFirstSibling();
                pTwo = SpawnPlayer(GameManager.gameManager.playerTwo);
                barSpawner.SpawnHealthBar(GameManager.gameManager.playerTwo, pTwo);
                break;
            default:
                Debug.LogError("gameManager.PlayerCount() return a value that was not 0,1, or 2");
                break;
        }

    }

    public GameObject SpawnPlayer(Character character)
    {
        GameObject player = Instantiate<GameObject>(playerPrefabs[(int)character]);

        //bar.gameObject.transform.parent = healthBarCanvasObject.transform;
        player.transform.localScale = playerPrefabs[(int)character].transform.localScale;

        return player;
    }

}
