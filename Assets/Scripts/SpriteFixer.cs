using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When attached along with a sprite renderer, makes it display correctly to a rotated camera. <br/>
/// Since the rotated camera will compress the sprite, this calculates how much it needs to stretch to cancel it. <br/>
/// This was chosen over a simple billboard to create a better experience in the scene view. <br/>
/// Only runs in the inspector.
/// </summary>
[ExecuteInEditMode]
public class SpriteFixer : MonoBehaviour
{
    void OnValidate()
    {
        if (Camera.main == null) return; // sometimes happens when the project loads
        
        var xRotation = transform.eulerAngles.x;
        var rotationFromCam = xRotation - Camera.main.transform.eulerAngles.x;
        var adjustedScale = Mathf.Abs(1 / Mathf.Cos(Mathf.Deg2Rad * rotationFromCam));
        transform.localScale = new Vector3(1, adjustedScale, 1);
    }
}
