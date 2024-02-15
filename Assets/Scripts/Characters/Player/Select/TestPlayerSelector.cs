using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Character
{
    Apple,
    Banana,
    Orange
}

public class TestPlayerSelector : MonoBehaviour
{
    
    [SerializeField]
    public Sprite[] images;
    private Image characterImage;
    private int numCharacters;
    private Character character;

    public bool characterSelected;

    private void Start()
    {
        characterImage = transform.Find("Character Image").gameObject.GetComponent<Image>();
        numCharacters = Character.GetNames(typeof(Character)).Length;
        if (numCharacters != images.Length)
            Debug.LogWarning("TestPlayerSelector.cs: The player enum and images provided do not have equivilent length.");
        
        character = Character.Apple;
        characterImage.sprite = images[0];
    }

    void OnUp()
    {
        if (characterSelected)
            return;

        character++;
        if ((int)character >= numCharacters)
            character = (Character)0;
        characterImage.sprite = images[(int)character];
    }

    void OnDown()
    {
        if (characterSelected)
            return;

        character--;
        if (character < 0)
            character = (Character)(numCharacters - 1);
        characterImage.sprite = images[(int)character];
    }

    void OnConfirm()
    {
        if (characterSelected)
        {
            
        }
        else
        {
            characterSelected = true;
        }
    }

    void OnBack()
    {
        Debug.Log("Back");
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
