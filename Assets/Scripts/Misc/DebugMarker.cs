using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMarker : MonoBehaviour
{
    void Start()
    {
        if (!Application.isEditor) {
            // DebugMarker is explictly for debugging
            Destroy(gameObject);

            // https://docs.unity3d.com/Manual/LogFiles.html
            Debug.LogWarning("DebugMarker instanced in build.");
        }
    }

}
