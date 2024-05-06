using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Assigns the canvas's "render camera" field to the main camera. <br/>
/// Motivation: this allows the canvas to be in its own prefab without having to manually assign to camera of every instance.
/// </summary>
[ExecuteAlways]
public class CanvasCameraFetcher : MonoBehaviour
{
    void OnValidate()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
    }

    void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
    }
}
