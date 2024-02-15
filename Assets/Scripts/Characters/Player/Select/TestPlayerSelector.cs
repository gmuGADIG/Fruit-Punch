using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TestPlayerSelector : MonoBehaviour
{
    enum Character
    {
        Apple,
        Banana,
        Orange
    }
    
    public GameObject playerOneSelector;
    public GameObject playerTwoSelector;
    public bool playerOneJoined = false;
    public bool playerTwoJoined = false;
    [SerializeField]
    public Sprite[] images;
    private Image characterImage;
    int numCharacters;
    Character character;

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
        character++;
        if ((int)character >= numCharacters)
            character = (Character)0;
        characterImage.sprite = images[(int)character];
    }

    void OnDown()
    {
        character--;
        if (character < 0)
            character = (Character)(numCharacters - 1);
        characterImage.sprite = images[(int)character];
    }

    void OnConfirm()
    {
        Debug.Log("Confirm");
    }


}
