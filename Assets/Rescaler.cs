using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions {
    public static void SetGlobalScale (this Transform transform, Vector3 globalScale) {
        transform.localScale = Vector3.one;
        transform.localScale = new Vector3 (globalScale.x/transform.lossyScale.x, globalScale.y/transform.lossyScale.y, globalScale.z/transform.lossyScale.z);
    }

}

[ExecuteAlways]
public class Rescaler : MonoBehaviour {
    [SerializeField] Vector3 goalScale = Vector3.one;
    void Update() {
        transform.SetGlobalScale(goalScale);
    }
}
