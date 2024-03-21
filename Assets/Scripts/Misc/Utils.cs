using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

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
    /// Similar to TryGetComponent, except an error will be thrown if no such component exists.
    /// </summary>
    public static void GetComponentOrError<T>(this GameObject obj, out T result)
    {
        if (obj.TryGetComponent(out result)) return;
        else throw new Exception($"`{obj.name}` is missing component of type `{typeof(T).Name}`!");
    }

    /// <summary>
    /// Similar to GetComponentInChildren, except an error will be thrown if no such component exists.
    /// </summary>
    public static void GetComponentInChildrenOrError<T>(
        this MonoBehaviour obj, 
        out T result
    ) {
        result = obj.GetComponentInChildren<T>();
        if (result == null) {
            throw new Exception($"`{obj.name}`'s children are missing component of type `{typeof(T).Name}`!");
        }
    }

    /// <summary>
    /// Similar to GetComponentInChildren, except an error will be thrown if no such component exists.
    /// </summary>
    public static void GetComponentInChildrenOrError<T>(
        this GameObject obj, 
        out T result
    ) {
        result = obj.GetComponentInChildren<T>();
        if (result == null) {
            throw new Exception($"`{obj.name}`'s children are missing component of type `{typeof(T).Name}`!");
        }
    }

    /// <summary>
    /// Returns an iterator of actions the player input has.
    /// </summary>
    /// <param name="input">The input in question</param>
    /// <returns>IEnumerable for the PlayerInput's actions</returns>
    public static IEnumerable<InputAction> ActionIter(this PlayerInput input) {
        return input.actions.actionMaps.Aggregate(
            new List<InputAction>().AsEnumerable(),
            (aux, next) => aux.Concat(next).Concat(next)
        );
    }

    /// <summary>
    /// Gets the action the player input has for a given action id.
    /// </summary>
    /// <param name="input">The player input in question</param>
    /// <param name="actionId">The ID of the action</param>
    /// <returns>The player input's copy of the action</returns>
    public static InputAction PlayerInputActionOfActionId(this PlayerInput input, Guid actionId) {
        return input.ActionIter()
            .Where(action => action.id == actionId)
            .First();
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

    /// <summary>
    /// Picks a random element from a list.
    /// Returns a random element or null if the list is empty.
    /// </summary>
    public static T RandomElement<T>(this IEnumerable<T> list)
    {
        return list.ElementAtOrDefault(UnityEngine.Random.Range(0, list.Count()));
    }

    /// <summary>
    /// Returns true if the set of aura flags is not normal.
    /// </summary>
    public static bool IsSpecial(this AuraType aura)
    {
        return aura != AuraType.Normal;
    }

    /// For a given layer (0 to 31), returns the mask of all layers which it collides with, according to the physics collision matrix. <br/>
    /// Note that this function loops through 32 layers each call, so you might want to avoid calling it in Update.
    /// </summary>
    /// <param name="layerNumber"></param>
    /// <returns></returns>
    public static LayerMask GetCollidingLayerMask(int layerNumber)
    {
        LayerMask result = 0;
        for (int i = 0; i < 32; i++) {
            if(!Physics.GetIgnoreLayerCollision(layerNumber, i))  {
                result |= 1 << i;
            }
        }
        return result;
    }

    public static Vector3 Abs(this Vector3 v) {
        return new(
            Mathf.Abs(v.x),
            Mathf.Abs(v.y),
            Mathf.Abs(v.z)
        );
    }
}
