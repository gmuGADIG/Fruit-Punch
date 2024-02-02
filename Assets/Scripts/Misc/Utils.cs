using System;
using UnityEngine;

public static class Utils
{
    /// <summary>
    /// Throws an error if the given condition is not true. <br/>
    /// Unlike Debug.Assert or Assert.IsTrue, this is not just in debug mode; it will run always.
    /// </summary>
    public static void Assert(bool condition)
    {
        if (!condition) throw new Exception("Assertion failed!");
    }

    /// <summary>
    /// Similar to TryGetComponent, except an error will be thrown if no such component exists.
    /// </summary>
    public static void GetComponentOrError<T>(this MonoBehaviour obj, out T result)
    {
        if (obj.TryGetComponent(out result)) return;
        else throw new Exception($"`{obj.name}` is missing component of type `{typeof(T).Name}`!");
    }

    /// <summary>
    /// Returns a Rect with world space coordinates of the edges of the camera's view.
    /// Throws an error if the camera is not orthographic.
    /// </summary>
    public static Rect OrthographicBoundingRect(this Camera cam)
    {
        if (cam.orthographic == false) throw new Exception("Camera must be orthographic to get 2d bounds.");
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = cam.orthographicSize * 2;
        float cameraWidth = cameraHeight * screenAspect;
        return new Rect(cam.transform.position.x - cameraWidth / 2, cam.transform.position.y - cameraHeight / 2, cameraWidth, cameraHeight);
    }
}
