using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMarker : MonoBehaviour
{
    [SerializeField] new bool enabled = false;
    public static DebugMarker Instantiate(GameObject prefab, Vector3 position, Color color) {
        Instantiate(prefab, position, Quaternion.identity)
            .GetComponentOrError(out DebugMarker result);
        result.GetComponentInChildrenOrError(out SpriteRenderer sprite);
        sprite.color = color;

        return result;
    }

    void Start()
    {
        if (!Application.isEditor) {
            // DebugMarker is explictly for debugging
            Destroy(gameObject);

            // https://docs.unity3d.com/Manual/LogFiles.html
            Debug.LogWarning("DebugMarker instanced in build.");
        }

        if (!enabled) {
            Destroy(gameObject);
        }
    }

}
