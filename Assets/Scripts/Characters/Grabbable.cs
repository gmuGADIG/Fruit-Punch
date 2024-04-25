using System;
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
    int grabRangeCount = 0;
    
    Rigidbody rb;

    NavMeshAgent agent;

    public bool currentlyGrabbed { get; private set; } = false;

    public event Action disabled;

    void Start()
    {
        this.GetComponentOrError(out rb);
        if (GetComponent<NavMeshAgent>() != null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        else
        {
            return;
        }
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
    }

    public void ForceRelease()
    {
        currentlyGrabbed = false;
        onForceRelease?.Invoke();
        rb.isKinematic = false;
        if(agent != null)
            agent.enabled = true;
    }

    public void InGrabbingRange()
    {
        grabRangeCount += 1;
        if (grabIndicator != null) grabIndicator.SetActive(true);
    }

    public void OutOfGrabbingRange()
    {
        grabRangeCount -= 1;
        if (grabRangeCount == 0 && grabIndicator != null) grabIndicator.SetActive(false);
        Utils.Assert(grabRangeCount >= 0);
    }

    void OnDisable() {
        disabled?.Invoke();
    }
}
