using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectorManager : MonoBehaviour
{
    public GameObject playerOneSelector;
    public GameObject playerTwoSelector;

    public bool GetPlayersReady()
    {
        if (playerOneSelector != null)
        {
            if (playerTwoSelector != null) //two players
            {
                return playerOneSelector.GetComponent<CharacterSelector>().characterSelected && playerTwoSelector.GetComponent<CharacterSelector>().characterSelected; //return true if both have selected characters
            }
            else //one player
            {
                return playerOneSelector.GetComponent<CharacterSelector>().characterSelected; //return true if player has selected
            }
        }
        else
            return false;
    }

    public Character GetOtherCharacter()
    {
        if (playerOneSelector.GetComponent<CharacterSelector>().characterSelected)
        {
            return playerOneSelector.GetComponent<CharacterSelector>().character;
        }
        else if (playerTwoSelector.GetComponent<CharacterSelector>().characterSelected)
        {
            return playerTwoSelector.GetComponent<CharacterSelector>().character;
        }
        return Character.None;
    }

}
