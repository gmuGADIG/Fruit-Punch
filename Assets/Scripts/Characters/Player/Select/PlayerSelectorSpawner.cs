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
    public List<ButtonDisplay> joinIndicators;

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
        if (joinIndicators.Count > 0)
        {
            Destroy(joinIndicators[0].gameObject);
            joinIndicators.RemoveAt(0);
            if (joinIndicators.Count > 0)
            {
                joinIndicators[0].transform.SetAsLastSibling();

                var otherSchemes = new List<string> { "controller" };
                if (context.ControlScheme == "keyboardLeft") otherSchemes.Add("keyboardRight");
                if (context.ControlScheme == "keyboardRight") otherSchemes.Add("keyboardLeft");

                joinIndicators[0].keyboardSchemes = otherSchemes.ToArray();
            }
        }

    }
}
