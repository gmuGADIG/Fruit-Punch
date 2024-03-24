using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Makes an object softly float up and down.
/// </summary>
public class Floater : MonoBehaviour
{
    [Tooltip("How far up and down the object can move. Distance from top to bottom is double this value.")]
    [SerializeField] float floatDistance = .1f;
    [Tooltip("How fast it floats up and down, in cycles per second.")]
    [SerializeField] float rate = 1f;
    [SerializeField] FloatDirection direction = FloatDirection.UpAndDown;
    Vector3 defaultPos;
    
    enum FloatDirection {UpAndDown, SideToSide}

    void Start()
    {
        defaultPos = transform.localPosition;
    }

    void Update()
    {
        var vec = direction == FloatDirection.UpAndDown ? Vector3.up : Vector3.right;
        vec *= floatDistance;
        transform.localPosition = defaultPos + vec * Mathf.Sin(Time.time * 2 * Mathf.PI * rate);
    }
}
