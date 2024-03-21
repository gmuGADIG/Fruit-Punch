using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// This component allows an item or an enemy to be grabbed by a player. See also: <c>Grabber.cs</c> <br/>
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Grabbable : MonoBehaviour
{
    [Tooltip("This the weight of the gameObject. --Do you even lift enough to grab this?--")]
    public float weight;

    Rigidbody rb;
    
    void Start()
    {
        this.GetComponentOrError(out rb);
    }

    public event Action onGrab;
    public event Action onThrow;
    public event Action onForceRelease;

    public void Grab()
    {
        onGrab?.Invoke();
        rb.isKinematic = true;
    }

    public void Release()
    {
        onThrow?.Invoke();
        rb.isKinematic = false;
    }
    
    public void ForceRelease()
    {
        onForceRelease?.Invoke();
        rb.isKinematic = false;
    }
}
