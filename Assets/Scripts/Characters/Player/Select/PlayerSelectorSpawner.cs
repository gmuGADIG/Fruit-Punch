using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerSelectorSpawner : MonoBehaviour
{
    public GameObject prefab;
    public GameObject blankPOneSelector;
    public GameObject blankPTwoSelector;
    /// <summary>
    /// game obejct in scene that has <see cref="CharacterSelectorManager"/>.
    /// </summary>
    public GameObject manager;

    /// <summary>
    /// called by <see cref="InputLobby.OnPlayerJoin"/>. Instaites controls for the player the just joined and replaces blanks with fucntional character selectors. 
    /// also handles logic for assigning player 1 and player 2 and giving refrences for later game use.
    /// </summary>
    /// <param name="context"></param>
    public void ConnectPlayer(JoinContext context)
    {

        PlayerInput p = PlayerInput.Instantiate(
            prefab,
            controlScheme: context.ControlScheme,
            pairWithDevice: context.InputDevice
        );

        //redudant? checks if gameManager singleton exists, if not it creates it. but i dont think this would work.
        if(GameManager.gameManager == null)
            GameManager.gameManager = new GameManager();

        p.gameObject.transform.Find("Button Image").GetComponent<ButtonConfirm>().SetButton(context.ControlScheme);

        //if no player has joined yet, current player joins as player 1.
        if (blankPOneSelector != null)
        {
            //use blanks positions for the new selector
            p.gameObject.transform.parent = GameObject.Find("Canvas").transform;
            p.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(-250, -100, 0);
            //destroy blank
            Destroy(blankPOneSelector);

            ///set refrences for other objects that need the info (<see cref="CharacterSelectorManager.playerOneSelector"/>, <see cref="GameManager.playerOneInputDevice"/>, etc.)
            manager.GetComponent<CharacterSelectorManager>().playerOneSelector = p.gameObject.GetComponent<CharacterSelector>();
            p.GetComponent<CharacterSelector>().isPlayerOne = true;
            GameManager.gameManager.playerOneInputDevice = context.InputDevice; 
            GameManager.gameManager.playerOneControlScheme = context.ControlScheme; 
        }
        else if (blankPTwoSelector != null) //a player had joined already, current player joins as player 2.
        {
            //use blanks positions for the new selector
            p.gameObject.transform.parent = GameObject.Find("Canvas").transform;
            p.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(250, -100, 0);

            //destroy blank
            Destroy(blankPTwoSelector);

            ///set refrences for other objects that need the info (<see cref="CharacterSelectorManager.playerTwoSelector"/>, <see cref="GameManager.playerTwoInputDevice"/>, etc.)
            manager.GetComponent<CharacterSelectorManager>().playerTwoSelector = p.gameObject.GetComponent<CharacterSelector>();
            p.GetComponent<CharacterSelector>().isPlayerOne = false;
            GameManager.gameManager.playerTwoInputDevice = context.InputDevice;
            GameManager.gameManager.playerTwoControlScheme = context.ControlScheme;
        }
        
    }
}
