using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Character
{
    None=-1,
    Apple=0,
    Banana,
    Carrot
}

public class TestPlayerSelector : MonoBehaviour
{
    public static CharacterSelectorManager manager;

    //images used for each character that the player can select, index = (int)Character
    [SerializeField]
    public Sprite[] images;
    private Image characterImage;
    private TextMeshProUGUI text;
    private int numCharacters;
    
    public Character character;
    public bool characterSelected;
    public bool isPlayerOne;

    private void Start()
    {
        //trys to finds manager if manager isnt assigned
        if (manager == null)
            manager = GameObject.Find("Manager").GetComponent<CharacterSelectorManager>();

        //trys to finds characterImage if characterImage isnt assigned
        if (characterImage == null)
            characterImage = transform.Find("Character Image").gameObject.GetComponent<Image>();
        
        //trys to finds text if text isnt assigned
        if(text == null)
            text = transform.Find("Confirm Text").gameObject.GetComponent<TextMeshProUGUI>();

        //
        numCharacters = Character.GetNames(typeof(Character)).Length-1;
        if (numCharacters != images.Length)
            Debug.LogWarning("TestPlayerSelector.cs: The player enum and images provided do not have equivilent length.");
        
        character = Character.Apple;
        characterImage.sprite = images[0];
    }

    /// <summary>
    /// changes the character that would be selected
    /// </summary>
    void OnRight()
    {
        if (characterSelected)
            return;

        character++;

        //loops back if character index would be out of bounds 
        if ((int)character >= numCharacters)
            character = (Character)0;
        characterImage.sprite = images[(int)character];
    }

    /// <summary>
    /// changes the character that would be selected
    /// </summary>
    void OnLeft()
    {
        if (characterSelected)
            return;

        character--;
        //loops back if character index would be out of bounds 
        if (character < 0)
            character = (Character)(numCharacters-1);
        characterImage.sprite = images[(int)character];
    }

    /// <summary>
    /// on first press it locks in the character selected, on second press, if all players are ready it loads the next scene.
    /// </summary>
    void OnConfirm()
    {
        if (characterSelected) //if a character has been chosen
        {
            //check if players have selected thier characters
            if (manager.GetPlayersReady())
            {
                //Start Game
                SceneManager.LoadScene(GameManager.gameManager.postCharacterSelectorScene); 
                Debug.Log("Characters Selected");
            }
        }
        else //if a character hadn't been chosen, store the selected character
        {
            if (isPlayerOne)
                GameManager.gameManager.playerOne = character;
            else
                GameManager.gameManager.playerTwo = character;

            //updates text for selector
            text.text = "Press\r\n\r\n\r\nto start";
            characterSelected = true;
        }
    }

    /// <summary>
    /// unselects the character a player had preiously selected. if no characters had been selected it goes to the previous scene. 
    /// IMPORANT: previous scene may not be the right scene. 
    /// </summary>
    void OnBack()
    {
        if (characterSelected)
        {
            characterSelected = false;
            text.text = "Press\r\n\r\n\r\nto select";
            if (isPlayerOne)
                GameManager.gameManager.playerOne = Character.None;
            else
                GameManager.gameManager.playerTwo = Character.None;
        }
        else
        {
            //FIXME: temporary solution because the edge cases of character leave would be terrible.
            //Uncreate Character
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }




}
