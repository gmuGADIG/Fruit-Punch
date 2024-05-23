using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.InputSystem.LowLevel;

/// <summary>
/// This component allows an item or an enemy to be grabbed by a player. See also: <c>Grabber.cs</c> <br/>
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Grabbable : MonoBehaviour
{
    [Tooltip("May be null. Otherwise, this item is enabled when a player is in grabbing range, and disabled when walking away.")]
    [SerializeField] GameObject grabIndicator;
    /// <summary>
    /// Amount of Grabbers currently in grabbing range. grabIndicator should be visible when this != 0.
    /// </summary>
    HashSet<Grabber> grabbers = new();
    int grabRangeCount => grabbers.Count();
    
    Rigidbody rb;

    NavMeshAgent agent;

    public bool currentlyGrabbed { get; private set; } = false;

    public event Action disabled;


    [Tooltip("The hurtbox to be used when throwing an object")]
    [SerializeField] HurtBox throwingHurtBox;

    void Start()
    {
        Utils.Assert(throwingHurtBox != null);
        throwingHurtBox.gameObject.SetActive(false); // make sure it's off

        this.GetComponentOrError(out rb);
        agent = GetComponent<NavMeshAgent>();
    }

    public UnityEvent onGrab;
    public UnityEvent onThrow;
    public UnityEvent onForceRelease;

    public void Grab()
    {
        if (grabIndicator != null) grabIndicator.SetActive(false);
        currentlyGrabbed = true;
        onGrab?.Invoke();
        rb.isKinematic = true;
        if (agent != null)
            agent.enabled = false;
    }

    public void Throw()
    {
        currentlyGrabbed = false;
        onThrow?.Invoke();
        rb.isKinematic = false;
        if(agent != null)
            agent.enabled = true;

        throwingHurtBox.gameObject.SetActive(true);
    }

    public void ForceRelease()
    {
        currentlyGrabbed = false;
        onForceRelease?.Invoke();
        rb.isKinematic = false;
        if(agent != null)
            agent.enabled = true;
    }

    /// <summary>
    /// Indicate that there is a player that is in range to pick up the grabbable.
    /// Turns on the grabbable indicator.
    /// <paramref name="grabber"/> is used to uniquely identify the player that got in range.
    /// </summary>
    public void InGrabbingRange(Grabber grabber)
    {
        grabbers.Add(grabber);
        if (grabIndicator != null) grabIndicator.SetActive(true);
    }

    /// <summary>
    /// Indicate that a player left the range to pick up the grabbable.
    /// Turns off the grabbable indicator when all players leave.
    /// <paramref name="grabber"/> is used to uniquely identify the player that left.
    /// </summary>
    public void OutOfGrabbingRange(Grabber grabber)
    {
        grabbers.Remove(grabber);
        if (grabRangeCount == 0 && grabIndicator != null) grabIndicator.SetActive(false);
        Utils.Assert(grabRangeCount >= 0);
    }

    void OnDisable() {
        disabled?.Invoke();
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("World")) {
            throwingHurtBox.gameObject.SetActive(false);
        }
    }
}
