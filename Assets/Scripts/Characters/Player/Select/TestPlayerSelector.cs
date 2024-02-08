using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TestPlayerSelector : MonoBehaviour
{
    public GameObject playerOneSelector;
    public GameObject playerTwoSelector;
    public bool playerOneJoined = false;
    public bool playerTwoJoined = false;
    [SerializeField]
    public Sprite[] images;
    private Image img;
    int imgIndex = 0;

    private void Start()
    {
        img = transform.Find("Character Image").GetComponent<Image>();
    }

    void OnUp()
    {
        img.sprite = images[++imgIndex];
    }

    
}
