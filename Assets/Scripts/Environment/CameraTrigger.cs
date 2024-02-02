using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Holds a UnityEvent that triggers when this object is inside the camera bounds.
/// </summary>
public class CameraTrigger : MonoBehaviour
{
    [Tooltip("Triggers when this object is inside the camera bounds.")]
    public UnityEvent OnTrigger;

    private bool hasFired = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!hasFired && InCameraBounds(Camera.main))
        {
            OnTrigger.Invoke();
            hasFired = true;
        }
    }

    /// <summary>
    /// Returns true if this trigger is within the camera's view bounds.
    /// </summary>
    private bool InCameraBounds(Camera cam)
    {
        if (cam == null) return false;
        return cam.OrthographicBoundingRect().Contains(transform.position);
    }
}
