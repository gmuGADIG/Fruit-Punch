using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// Lets the play grab any nearby grabbables (items or enemies). <br/>
/// Requires a trigger Collider component.
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Grabber : MonoBehaviour
{
    public event Action OnForceRelease;
    
    [Tooltip("The force (not speed) applied to thrown objects. Heavier objects will be slower.")]
    [SerializeField] float throwForce;

    [Tooltip("Multiplier for the amount of damage done on landing.")]
    [SerializeField] float throwDamageMultiplier = 1.0f;

    [Tooltip("The radius of which the player has the ability to grab an object.")]
    [SerializeField] float grabRadius = 1f;
    
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
        item.GetComponent<Grabbable>().damageOnLandingMultiplier = throwDamageMultiplier;
    }

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
            
            // Perform a shape cast
            // Find all the grabbables in the shape cast
            // Pick the one closest to the player
            Collider[] hits = Physics.OverlapSphere(transform.position, grabRadius);
            Grabbable grabbed = hits
                .Select(hit => hit.GetComponent<Grabbable>())
                .Where(hit => hit != null)
                .Where(hit => hit.enabled)
                .OrderBy(grab => Vector3.Distance(this.transform.position, grab.transform.position))
                .FirstOrDefault();

            if (grabbed == null) {
                // bail early, because we didn't find anything to grab
                return false;
            }

            // item = currentOverlaps[0];

            //Checks for Health component
            if (grabbed.TryGetComponent(out Health health))
            {
                // Don't grab things that can't be grabbed
                if (!health.IsVulnerableTo(AuraType.Throw)) return false;
                // Don't grab dead things
                if (health.CurrentHealth <= 0) return false;
            }

            grabbed.Grab();
            grabbed.transform.SetParent(this.transform);
            grabbed.transform.position = this.transform.position;
            grabbed.onForceRelease.AddListener(ForceReleaseCallback);
            SoundManager.Instance.PlaySoundAtPosition("Pickup", this.transform.position);

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
        OnForceRelease?.Invoke();
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Grabbable grabbable)) {
            grabbable.InGrabbingRange(this);

            grabbable.Disabled += () => {
                grabbable.OutOfGrabbingRange(this);
            };
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.TryGetComponent(out Grabbable grabbable)) {
            grabbable.OutOfGrabbingRange(this);
        }
    }
}
