using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Lets the play grab any nearby grabbables (items or enemies). <br/>
/// Requires a trigger Collider component.
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

    void Update()
    {
        // keep rotation fixed
        transform.eulerAngles = new Vector3(0, 0, 0);
    }


    /// <summary>
    /// Throws the currently held grabbable left or right (depending on facingLeft).
    /// </summary>
    /// <param name="facingLeft"></param>
    public void ThrowItem(bool facingLeft)
    {
        var throwDir = new Vector3(1, 1f, 0);
        if (facingLeft) throwDir.x *= -1;
        var item = GetGrabbedItem();
        item.Throw();
        item.transform.SetParent(null);
        item.onForceRelease.RemoveListener(ForceReleaseCallback);
        item.GetComponent<Rigidbody>().AddForce(throwDir.normalized * throwForce);
    }

    
    /// <summary>
    /// Attempts to grab nearby grabbables. Returns false if nothing can be grabbed. <br/>
    /// Otherwise, re-parents the grabbable and invokes its relevant events.
    /// </summary>
    /// <returns></returns>
    public bool GrabItem()
    {
        if (currentOverlaps.Count == 0) return false;

        var item = currentOverlaps[0];

        // If the item has a health component and its aura isn't vulnerable to throw attacks, cancel grab.
        // todo: this also cancels the grab even if another vulnerable enemy is within range. might be worth a fix.
        if (item.TryGetComponent<Health>(out var health))
        {
            if (!health.IsVulnerableTo(AuraType.Throw)) return false;
            if (health.CurrentHealth <= 0) return false;
        }
        item.Grab();
        item.transform.SetParent(this.transform);
        item.transform.position = this.transform.position;
        item.onForceRelease.AddListener(ForceReleaseCallback);
        return true;
    }

    /// <summary>
    /// Callback function subscribed to each grabbed item's onForceRelease event. <br/>
    /// This function releases the item (calls item.Release and un-parents it) and invokes the grabber's onForceRelease event.
    /// </summary>
    void ForceReleaseCallback()
    {
        var item = GetGrabbedItem();
        item.transform.SetParent(null);
        item.onForceRelease.RemoveListener(ForceReleaseCallback);
        onForceRelease?.Invoke();
    }

    void OnTriggerEnter(Collider other)
    {
        var grabbable = other.GetComponentInParent<Grabbable>();
        if (grabbable != null && grabbable.enabled)
        {
            currentOverlaps.Add(grabbable);
            currentOverlaps = currentOverlaps.OrderBy(g => Vector3.Distance(this.transform.position, g.transform.position)).ToList();
            grabbable.InGrabbingRange();

            grabbable.disabled += () => {
                if (currentOverlaps.Remove(grabbable)) {
                    grabbable.OutOfGrabbingRange();
                }
            };
        }
    }   

    void OnTriggerExit(Collider other)
    {
        var grabbable = other.GetComponentInParent<Grabbable>();
        if (grabbable != null && grabbable.enabled)
        {
            grabbable.OutOfGrabbingRange();
            currentOverlaps.Remove(grabbable);
        }
    }
}
