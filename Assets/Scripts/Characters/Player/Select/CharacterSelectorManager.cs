using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectorManager : MonoBehaviour
{
    public CharacterSelector playerOneSelector;
    public CharacterSelector playerTwoSelector;

    /// <summary>
    /// checks if all players joined have selected a character.
    /// </summary>
    /// <returns>true if all player have selected a charcter.</returns>
    public bool GetPlayersReady()
    {
        if (playerOneSelector != null)
        {
            if (playerTwoSelector != null) //two players
            {
                return playerOneSelector.characterSelected && playerTwoSelector.characterSelected; //return true if both have selected characters
            }
            else //one player
            {
                return playerOneSelector.characterSelected; //return true if player has selected
            }
        }
        else
            return false;
    }

    /// <summary>
    /// used to ensure a player cannnot select a character that has already been selected.
    /// </summary>
    /// <returns>the <see cref="Character"/> of the other player. returns <see cref="Character.None"/> if no players have selectecd a character</returns>
    public Character GetOtherCharacter()
    {
        if (playerOneSelector.characterSelected)
        {
            return playerOneSelector.character;
        }
        else if (playerTwoSelector.characterSelected)
        {
            return playerTwoSelector.character;
        }
        return Character.None;
    }

}
