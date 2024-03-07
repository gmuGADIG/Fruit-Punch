using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// This component allows an item or an enemy to be grabbed by a player. See also: <c>Grabber.cs</c> <br/>
/// It requires a trigger collider attached a child (or itself). Generally, it should have an additional non-trigger collider on itself for physics. 
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Grabbable : MonoBehaviour
{
    Rigidbody rb;
    
    void Start()
    {
        this.GetComponentOrError(out rb);
        
        var hasTrigger = GetComponentsInChildren<Collider>().Any(c => c.isTrigger);
        if (!hasTrigger) Debug.LogError("Grabbable does not have any trigger! Add a trigger collider to it or a child.");
    }

    public event Action onGrab;
    public event Action OnThrow;
    public event Action onForceRelease;

    public void Grab()
    {
        onGrab?.Invoke();
        rb.isKinematic = true;
    }

    public void Release()
    {
        OnThrow?.Invoke();
        rb.isKinematic = false;
    }
    
    public void ForceRelease()
    {
        onForceRelease?.Invoke();
        rb.isKinematic = false;
    }
}