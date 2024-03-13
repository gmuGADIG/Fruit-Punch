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
                return playerOneSelector.GetComponent<TestPlayerSelector>().characterSelected && playerTwoSelector.GetComponent<TestPlayerSelector>().characterSelected; //return true if both have selected characters
            }
            else //one player
            {
                return playerOneSelector.GetComponent<TestPlayerSelector>().characterSelected; //return true if player has selected
            }
        }
        else
            return false;
    }

    public Character GetOtherCharacter()
    {
        if (playerOneSelector.GetComponent<TestPlayerSelector>().characterSelected)
        {
            return playerOneSelector.GetComponent<TestPlayerSelector>().character;
        }
        else if (playerTwoSelector.GetComponent<TestPlayerSelector>().characterSelected)
        {
            return playerTwoSelector.GetComponent<TestPlayerSelector>().character;
        }
        return Character.None;
    }

}
