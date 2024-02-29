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
        manager = GameObject.Find("Manager").GetComponent<CharacterSelectorManager>();
        characterImage = transform.Find("Character Image").gameObject.GetComponent<Image>();
        text = transform.Find("Confirm Text").gameObject.GetComponent<TextMeshProUGUI>();
        numCharacters = Character.GetNames(typeof(Character)).Length-1;
        if (numCharacters != images.Length)
            Debug.LogWarning("TestPlayerSelector.cs: The player enum and images provided do not have equivilent length.");
        
        character = Character.Apple;
        characterImage.sprite = images[0];
    }

    void OnRight()
    {
        if (characterSelected)
            return;

        character++;
        if ((int)character >= numCharacters)
            character = (Character)0;
        characterImage.sprite = images[(int)character];
    }

    void OnLeft()
    {
        if (characterSelected)
            return;

        character--;
        if (character < 0)
            character = (Character)(numCharacters-1);
        characterImage.sprite = images[(int)character];
    }

    void OnConfirm()
    {
        if (characterSelected)
        {
            
            if (manager.GetPlayersReady())
            {
                //Start Game
                SceneManager.LoadScene("PostCharacterSelector");
                Debug.Log("Characters Selected");
            }
        }
        else
        {
            if (isPlayerOne)
                GameManager.gameManager.playerOne = character;
            else
                GameManager.gameManager.playerTwo = character;
            //Character other = manager.GetOtherCharacter();
            //if (other == Character.None || (Character)other!=character)
            text.text = "Press\r\n\r\n\r\nto start";
            characterSelected = true;
        }
    }

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
