using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayerSelectorSpawner : MonoBehaviour
{
    public GameObject prefab;
    public GameObject blankPOneSelector;
    public GameObject blankPTwoSelector;
    /// <summary>
    /// game obejct in scene that has <see cref="CharacterSelectorManager"/>.
    /// </summary>
    public GameObject manager;
    public void ConnectPlayer(JoinContext context)
    {

        PlayerInput p = PlayerInput.Instantiate(
            prefab,
            controlScheme: context.ControlScheme,
            pairWithDevice: context.InputDevice
        );

        if(GameManager.gameManager == null)
            GameManager.gameManager = new GameManager();


        if (blankPOneSelector != null)
        {
            p.gameObject.transform.parent = GameObject.Find("Canvas").transform;
            p.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(-250, -100, 0);
            Destroy(blankPOneSelector);
            manager.GetComponent<CharacterSelectorManager>().playerOneSelector = p.gameObject;
            GameManager.gameManager.playerOneInputDevice = context.InputDevice; //p.devices[0]
            GameManager.gameManager.playerOneControlScheme = context.ControlScheme; // p.currentControlScheme
        }
        else if (blankPTwoSelector != null)
        {
            p.gameObject.transform.parent = GameObject.Find("Canvas").transform;
            p.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(250, -100, 0);
            Destroy(blankPTwoSelector);
            manager.GetComponent<CharacterSelectorManager>().playerTwoSelector = p.gameObject;
            GameManager.gameManager.playerTwoInputDevice = context.InputDevice;
            GameManager.gameManager.playerTwoControlScheme = context.ControlScheme;
        }
        
    }
}
