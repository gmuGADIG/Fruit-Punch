using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Grabbable : MonoBehaviour
{
    public event Action onGrab;
    public event Action OnThrow;
    public event Action onForceRelease;

    public void Grab()
    {
        onGrab?.Invoke();
    }

    public void Release()
    {
        OnThrow?.Invoke();
    }
    
    public void ForceRelease()
    {
        onForceRelease?.Invoke();
    }
}
