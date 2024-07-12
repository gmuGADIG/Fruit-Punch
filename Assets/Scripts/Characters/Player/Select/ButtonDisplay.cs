using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Displays an image of a key in one or more control styles. <br/>
/// If multiple styles, will cycle between sprites. 
/// </summary>
public class ButtonDisplay : MonoBehaviour
{
    public Sprite keyboardLeftImage;
    public Sprite keyboardRightImage;
    public Sprite controllerImage;

    public string[] keyboardSchemes = { "keyboardLeft", "keyboardRight", "controller" };
    
    Image image;
    int index;

    List<Sprite> images = new();
    
    void Start()
    {
        image = gameObject.GetComponentInChildren<Image>();

        if (keyboardSchemes.Contains("keyboardLeft")) images.Add(keyboardLeftImage);
        if (keyboardSchemes.Contains("keyboardRight")) images.Add(keyboardRightImage);
        if (keyboardSchemes.Contains("controller")) images.Add(controllerImage);
    }

    void Update()
    {
        Debug.Log("hai :3", this);

        var imageIndex = (int)Time.time % images.Count;
        image.sprite = images[imageIndex];

        Debug.Log("images.Count = " + images.Count, this);
        Debug.Log("imageIndex = " + imageIndex, this);
        Debug.Log("image.sprite = " + image.sprite, this);
    }


}
