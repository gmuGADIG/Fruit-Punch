using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Lets the play grab any nearby grabbables (items or enemies).
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Grabber : MonoBehaviour
{
    public event Action onForceRelease;
    
    [Tooltip("The force (not speed) applied to thrown objects. Heavier objects will be slower.")]
    [SerializeField] float throwForce;
    
    List<Grabbable> currentOverlaps = new();
    
    public bool IsGrabbing => this.transform.childCount > 0;
    Grabbable GetGrabbedItem() => this.transform.GetChild(0).GetComponent<Grabbable>();

    /// <summary>
    /// Throws the currently held grabbable in the direction of throwDir.
    /// </summary>
    /// <param name="throwDir"></param>
    public void ThrowItem(Vector3 throwDir)
    {
        var item = GetGrabbedItem();
        item.transform.SetParent(null);
        item.onForceRelease -= ForceReleaseCallback;
        item.GetComponent<Rigidbody>().AddForce(throwDir.normalized * throwForce);
        item.Release();
    }

    
    /// <summary>
    /// Attempts to grab nearby grabbables. Returns false if nothing can be grabbed. <br/>
    /// Otherwise, re-parents the grabbable and invokes its relevant events.
    /// </summary>
    /// <returns></returns>
    public bool GrabItem()
    {
        // TODO: player can only grab enemies if the aura allows. this is not yet checked.
        if (currentOverlaps.Count == 0) return false;
        
        var item = currentOverlaps[0];
        item.transform.SetParent(this.transform);
        item.GetComponent<Rigidbody>().velocity = Vector3.zero;
        item.onForceRelease += ForceReleaseCallback;
        item.Grab();
        return true;
    }

    /// <summary>
    /// Callback function subscribed to each grabbed item's onForceRelease event. <br/>
    /// This function simply invokes 
    /// </summary>
    void ForceReleaseCallback()
    {
        onForceRelease?.Invoke();
    }

    void OnTriggerEnter(Collider other)
    {
        var grabbable = other.GetComponentInParent<Grabbable>();
        if (grabbable != null) currentOverlaps.Add(grabbable);
    }   

    void OnTriggerExit(Collider other)
    {
        var grabbable = other.GetComponentInParent<Grabbable>();
        if (grabbable != null) currentOverlaps.Remove(grabbable);
    }
}
