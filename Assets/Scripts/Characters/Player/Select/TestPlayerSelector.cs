using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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
    int numCharacters;
    Character character;

    byte selectState;

    private void Start()
    {
        selectState = 0;
        characterImage = transform.Find("Character Image").gameObject.GetComponent<Image>();
        numCharacters = Character.GetNames(typeof(Character)).Length;
        if (numCharacters != images.Length)
            Debug.LogWarning("TestPlayerSelector.cs: The player enum and images provided do not have equivilent length.");
        
        character = Character.Apple;
        characterImage.sprite = images[0];
    }

    void OnUp()
    {
        if (selectState!=0)
            return;

        character++;
        if ((int)character >= numCharacters)
            character = (Character)0;
        characterImage.sprite = images[(int)character];
    }

    void OnDown()
    {
        if (selectState!=0)
            return;

        character--;
        if (character < 0)
            character = (Character)(numCharacters - 1);
        characterImage.sprite = images[(int)character];
    }

    void OnConfirm()
    {
        if (selectState<2)
        {
            selectState++;
        }
    }

    void OnBack()
    {
        if (selectState >= 0)
        {
            selectState--;
        }
        else
        {
            //FIXME: temporary solution because the edge cases of character leave would be terrible.
            //Uncreate Character
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }




}
