using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerSelectorSpawner : MonoBehaviour
{
    public GameObject selectorPrefab;
    public ButtonDisplay joinIndicator;

    /// <summary>
    /// called by <see cref="InputLobby.OnPlayerJoin"/>. Instantiates controls for the player the just 
    /// joined and replaces blanks with functional character selectors. 
    /// also handles logic for assigning player 1 and player 2 and giving references for later game use.
    /// </summary>
    /// <param name="context"></param>
    public void ConnectPlayer(JoinContext context)
    {
        // create player and its input
        PlayerInput playerInput = PlayerInput.Instantiate(
            selectorPrefab,
            controlScheme: context.ControlScheme,
            pairWithDevice: context.InputDevice
        );

        var characterSelector = playerInput.gameObject.GetComponent<CharacterSelector>();
        characterSelector.transform.SetParent(this.transform, true);
        characterSelector.Setup(context);

        // adjust or destroy join indicator
        if (CharacterSelector.selectorCount == 2)
            Destroy(joinIndicator.gameObject);
        else
        {
            joinIndicator.transform.SetAsLastSibling();

            var otherSchemes = new List<string> { "controller" };
            if (context.ControlScheme == "keyboardLeft") otherSchemes.Add("keyboardRight");
            if (context.ControlScheme == "keyboardRight") otherSchemes.Add("keyboardLeft");
            
            joinIndicator.keyboardSchemes = otherSchemes.ToArray();
        }
    }
}
