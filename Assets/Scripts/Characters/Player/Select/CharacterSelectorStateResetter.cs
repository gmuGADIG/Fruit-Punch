using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectorStateResetter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
        
        CharacterSelector.selectorCount = 0;
        CharacterSelector.selectorsLockedIn = 0;
        CharacterSelector.selectedCharacters = new();
    }
}
