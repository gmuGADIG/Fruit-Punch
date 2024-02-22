using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Character
{
    None=-1,
    Apple=0,
    Banana,
    Orange
}

public class TestPlayerSelector : MonoBehaviour
{
    public static CharacterSelectorManager manager;

    [SerializeField]
    public Sprite[] images;
    private Image characterImage;
    private int numCharacters;
    
    public Character character;
    public bool characterSelected;

    private void Start()
    {
        manager = GameObject.Find("Manager").GetComponent<CharacterSelectorManager>();
        characterImage = transform.Find("Character Image").gameObject.GetComponent<Image>();
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
                Debug.Log("Characters Selected");
            }
        }
        else
        {
            //Character other = manager.GetOtherCharacter();
            //if (other == Character.None || (Character)other!=character)
                characterSelected = true;
        }
    }

    void OnBack()
    {
        if (characterSelected)
        {
            characterSelected = false;
        }
        else
        {
            //FIXME: temporary solution because the edge cases of character leave would be terrible.
            //Uncreate Character
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }




}
