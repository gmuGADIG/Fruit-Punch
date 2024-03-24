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
    float defaultY;

    void Start()
    {
        defaultY = transform.localPosition.y;
    }

    void Update()
    {
        var pos = transform.localPosition;
        pos.y = defaultY + Mathf.Sin(Time.time * 2 * Mathf.PI * rate) * floatDistance;
        transform.localPosition = pos;
    }
}
