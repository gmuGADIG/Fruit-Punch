using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public static CharacterSelectorManager manager;

    //images used for each character that the player can select, index = (int)Character
    [SerializeField]
    public Sprite[] images;
    [SerializeField]
    public StatsInfo[] stats;

    [SerializeField]
    public Sprite starFull;
    [SerializeField]
    public Sprite starEmpty;

    [SerializeField]
    private Image characterImage;
    [SerializeField]
    private TextMeshProUGUI text;
    private int numCharacters;
    
    [SerializeField]
    public Transform healthStars;
    [SerializeField]
    public Transform damageStars;
    [SerializeField]
    public Transform speedStars;
    [SerializeField]
    public GameObject buttonImage;

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
        SetStars();
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
        SetStars();

    }

    public void SetStars()
    {
        switch (stats[(int)character].health)
        {
            case StatValue.Low:
                healthStars.transform.GetChild(0).GetComponent<Image>().sprite = starFull;
                healthStars.transform.GetChild(1).GetComponent<Image>().sprite = starEmpty;
                healthStars.transform.GetChild(2).GetComponent<Image>().sprite = starEmpty;
                break;
            case StatValue.Mid:
                healthStars.transform.GetChild(0).GetComponent<Image>().sprite = starFull;
                healthStars.transform.GetChild(1).GetComponent<Image>().sprite = starFull;
                healthStars.transform.GetChild(2).GetComponent<Image>().sprite = starEmpty;
                break;
            case StatValue.High:
                healthStars.transform.GetChild(0).GetComponent<Image>().sprite = starFull;
                healthStars.transform.GetChild(1).GetComponent<Image>().sprite = starFull;
                healthStars.transform.GetChild(2).GetComponent<Image>().sprite = starFull;
                break;
            default:
                break;
        }

        switch (stats[(int)character].damage)
        {
            case StatValue.Low:
                damageStars.transform.GetChild(0).GetComponent<Image>().sprite = starFull;
                damageStars.transform.GetChild(1).GetComponent<Image>().sprite = starEmpty;
                damageStars.transform.GetChild(2).GetComponent<Image>().sprite = starEmpty;
                break;
            case StatValue.Mid:
                damageStars.transform.GetChild(0).GetComponent<Image>().sprite = starFull;
                damageStars.transform.GetChild(1).GetComponent<Image>().sprite = starFull;
                damageStars.transform.GetChild(2).GetComponent<Image>().sprite = starEmpty;
                break;
            case StatValue.High:
                damageStars.transform.GetChild(0).GetComponent<Image>().sprite = starFull;
                damageStars.transform.GetChild(1).GetComponent<Image>().sprite = starFull;
                damageStars.transform.GetChild(2).GetComponent<Image>().sprite = starFull;
                break;
            default:
                break;
        }

        switch (stats[(int)character].speed)
        {
            case StatValue.Low:
                speedStars.transform.GetChild(0).GetComponent<Image>().sprite = starFull;
                speedStars.transform.GetChild(1).GetComponent<Image>().sprite = starEmpty;
                speedStars.transform.GetChild(2).GetComponent<Image>().sprite = starEmpty;
                break;
            case StatValue.Mid:
                speedStars.transform.GetChild(0).GetComponent<Image>().sprite = starFull;
                speedStars.transform.GetChild(1).GetComponent<Image>().sprite = starFull;
                speedStars.transform.GetChild(2).GetComponent<Image>().sprite = starEmpty;
                break;
            case StatValue.High:
                speedStars.transform.GetChild(0).GetComponent<Image>().sprite = starFull;
                speedStars.transform.GetChild(1).GetComponent<Image>().sprite = starFull;
                speedStars.transform.GetChild(2).GetComponent<Image>().sprite = starFull;
                break;
            default:
                break;
        }

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
        SetStars();
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
                SceneManager.LoadScene(SwitchScene.switchScene.PostCharSelect); 
                Debug.Log("Characters Selected");
            }
        }
        else //if a character hadn't been chosen, store the selected character
        {
            Character other = manager.GetOtherCharacter();
            if (other == Character.None || other != character) //checks if no character has been selected or if the character that has been slected isnt the one this player is trying to select.
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
