using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum Character
{
    None=-1,
    Apple=0,
    Banana,
    Watermelon,
    Grapes
}

public class CharacterSelector : MonoBehaviour
{
    [Header("Stats")]
    // index = (int) Character
    [SerializeField] StatsInfo[] stats;
    
    [FormerlySerializedAs("images")]
    [Header("Sprites")]
    //images used for each character that the player can select, index = (int)Character
    [SerializeField] Sprite[] characterPreviews;

    [SerializeField] Sprite starFull;
    [SerializeField] Sprite starEmpty;

    [Header("Child References")]
    [SerializeField] Image characterImage;
    [SerializeField] TextMeshProUGUI text;

    [SerializeField] Transform healthStars;
    [SerializeField] Transform damageStars;
    [SerializeField] Transform speedStars;
    [SerializeField] ButtonDisplay buttonDisplay;

    int playerIndex;
    Character character;
    bool characterSelected;
    int numCharacters;

    public static int selectorCount = 0;
    public static int selectorsLockedIn = 0;
    public static event Action<CharacterSelector> onSelectCharacter;
    public static event Action<CharacterSelector> onUnselectCharacter;
    public static HashSet<Character> selectedCharacters = new();

    private void Start()
    {
        // get names of characters
        numCharacters = Enum.GetNames(typeof(Character)).Length-1;
        Utils.Assert(numCharacters == characterPreviews.Length);

        // set default character
        character = (Character) playerIndex;
        UpdateDisplay();
    }

    void OnDestroy()
    {
        selectorCount -= 1;
    }

    /// <summary>
    /// Change to next character (or does nothing if already selected).
    /// </summary>
    void OnRight()
    {
        if (characterSelected)
            return;

        character++;

        //loops back if character index would be out of bounds 
        if ((int)character >= numCharacters)
            character = (Character) 0;
        
        UpdateDisplay();
    }
    
    /// <summary>
    /// Change to the previous character (or does nothing if already selected).
    /// </summary>
    void OnLeft()
    {
        if (characterSelected)
            return;

        character--;
        // loops back if character index would be out of bounds 
        if (character < 0)
            character = (Character)(numCharacters-1);

        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        // set character sprite
        characterImage.sprite = characterPreviews[(int) character];
        
        // display stats
        var characterStats = stats[(int) character];
        SetStars(healthStars, characterStats.health);
        SetStars(damageStars, characterStats.damage);
        SetStars(speedStars, characterStats.speed);
    }

    void SetStars(Transform parent, StatValue statsValue)
    {
        for (int i = 0; i < 3; i++)
        {
            var starImage = parent.GetChild(i).GetComponent<Image>();
            var filled = i < (int)statsValue;
            starImage.sprite = filled ? starFull : starEmpty;
        }
    }

    public void Setup(JoinContext joinContext)
    {
        playerIndex = selectorCount;
        selectorCount += 1;
        
        PlayerInfo.playerControlSchemes[playerIndex] = joinContext.ControlScheme;
        PlayerInfo.playerInputDevices[playerIndex] = joinContext.InputDevice;
        buttonDisplay.keyboardSchemes = new[] { joinContext.ControlScheme };
    }
    

    /// <summary>
    /// on first press it locks in the character selected, on second press, if all players are ready it loads the next scene.
    /// </summary>
    void OnConfirm()
    {
        if (characterSelected) // if a character has been chosen
        {
            if (selectorsLockedIn == selectorCount) // Start Game
            {
                Debug.Log("Starting game!");
                SceneManager.LoadScene(SwitchScene.level1_1);
            }
            else Debug.Log($"Not everyone's selected! ({selectorsLockedIn} out of {selectorCount} locked in)");
        }
        else // if a character hadn't been chosen, store the selected character
        {
            if (selectedCharacters.Contains(this.character))
            {
                print("Already been selected!");
            }
            else
            {
                print("Character selected.");
                CommitSelection();
            }
        }
    }

    void CommitSelection()
    {
        selectorsLockedIn += 1;
        selectedCharacters.Add(this.character);
        
        PlayerInfo.characters[playerIndex] = character;
        text.text = "Press\r\n\r\n\r\nto start";
        characterSelected = true;
        
        onSelectCharacter?.Invoke(this);
    }

    void UndoSelection()
    {
        selectorsLockedIn -= 1;
        selectedCharacters.Remove(this.character);
        
        characterSelected = false;
        text.text = "Press\r\n\r\n\r\nto select";
        
        onUnselectCharacter?.Invoke(this);
    }

    /// <summary>
    /// unselects the character a player had previously selected. if no characters had been selected it goes to the main menu.
    /// </summary>
    void OnBack()
    {
        if (characterSelected)
        {
            UndoSelection();
        }
        else
        {
            SceneManager.LoadScene(SwitchScene.mainMenu);
        }
    }




}
