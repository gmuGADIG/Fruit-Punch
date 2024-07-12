using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] playerPrefabs;
    [SerializeField] Transform playerOneSpawnPoint;
    [SerializeField] Transform playerTwoSpawnPoint;

    // During development, it's helpful to start directly from a scene instead of the main menu.
    // When the character-select scene is skipped, a single player with these values is spawned.
    [Header("Debug")]
    [SerializeField] string debugControlScheme;
    [SerializeField] Character debugCharacter;
    [SerializeField] InputDevice debugInputDevice;
    
    private void Start()
    {
        var playerCount = PlayerInfo.PlayerCount();
        if (playerCount == 0) // debug spawn 
        {
            if (Application.isEditor == false) Debug.LogError("Debug spawn should not be used in a build!");
            var playerGO = SpawnPlayer(debugCharacter, debugControlScheme, debugInputDevice, playerOneSpawnPoint);
            Player player = playerGO.GetComponent<Player>();
            player.PlayerNum = 1;
            PlayerHealthBarHolder.SetHealthBar(0, player);
            PlayerHealthBarHolder.SetPlayerTwoVisible(false);
            PlayerScoreHolder.SetupPlayerScore(0, player);
            PlayerScoreHolder.SetPlayerTwoVisible(false);
        }
        else // normal spawn
        { 
            for (int i = 0; i < PlayerInfo.PlayerCount(); i++)
            {
                var character = PlayerInfo.characters[i];
                var controlScheme = PlayerInfo.playerControlSchemes[i];
                var inputDevice = PlayerInfo.playerInputDevices[i];
                var spawnPoint = (i == 0) ? playerOneSpawnPoint : playerTwoSpawnPoint;
                
                var playerGO = SpawnPlayer(character, controlScheme, inputDevice, spawnPoint);

                // trigger UI
                var player = playerGO.GetComponent<Player>();
                PlayerHealthBarHolder.SetHealthBar(i, player);
                PlayerScoreHolder.SetupPlayerScore(i, player);
                player.PlayerNum = i + 1;
            }
            
            PlayerHealthBarHolder.SetPlayerTwoVisible(PlayerInfo.PlayerCount() == 2);
            PlayerScoreHolder.SetPlayerTwoVisible(PlayerInfo.PlayerCount() == 2);
        }
    }

    public GameObject SpawnPlayer(Character character, string controlScheme, InputDevice inputDevice, Transform spawnPoint)
    {
        var player = PlayerInput.Instantiate(
            prefab: playerPrefabs[(int) character],
            controlScheme: controlScheme,
            pairWithDevice: inputDevice
        ).gameObject;
        
        player.transform.position = spawnPoint.position;

        return player;
    }

}
