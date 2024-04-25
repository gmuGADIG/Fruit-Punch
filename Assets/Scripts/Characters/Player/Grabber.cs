using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

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

    [Tooltip("The radius of which the player has the ability to grab an object.")]
    [SerializeField] float grabRadius = 1f;
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
        OnRelease(); // Sometimes its called sometimes its not, and when it is't it brings Unity to it's knees. 
    }

    public Grabbable item;

    public float grabCoolDown = 2f;
    public float currentTime;

    /// <summary>
    /// Attempts to grab nearby grabbables. Returns false if nothing can be grabbed. <br/>
    /// Otherwise, re-parents the grabbable and invokes its relevant events.
    /// </summary>
    /// <returns></returns>
    public bool GrabItem()
    {
        if (currentTime <= 0.0f) {
            //currentOverlaps.Clear();
            Collider[] grabs = Physics.OverlapSphere(transform.position, grabRadius);

            if (grabs != null && grabs.Length > 0)
            {
                for (int i = 0; i < grabs.Length; i++)
                {
                    if (grabs[i].TryGetComponent<Grabbable>(out var action))
                    {
                        currentOverlaps.Add(action);
                        currentOverlaps = currentOverlaps.OrderBy(g => Vector3.Distance(this.transform.position, g.transform.position)).ToList();
                        action.InGrabbingRange();
                        break;
                    }
                }
            }


            if (currentOverlaps.Count == 0) return false;

            item = currentOverlaps[0];

            //Checks for Health component
            if (item.GetComponent<Health>())
            {
                //Checks if enemies are vulnerable throw attacks, if they are not then the grap is canceled
                if (!item.gameObject.GetComponent<Health>().IsVulnerableTo(AuraType.Throw)) return false;
                if (health.CurrentHealth <= 0) return false;
            }

            item.Grab();
            item.transform.SetParent(this.transform);
            item.transform.position = this.transform.position;
            item.onForceRelease.AddListener(ForceReleaseCallback);


            /*if (item != null)
            {
                item.OutOfGrabbingRange();
                currentOverlaps.Remove(item);
            }*/
            return true;
        }
        else
        {
            currentTime -= Time.deltaTime;
            return false;
        }
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


    public void OnRelease()
    {
        item.OutOfGrabbingRange();
        currentOverlaps.Remove(item);
        
        item = null;
        currentOverlaps.Clear();
    }

    /*
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
    */
    
}
