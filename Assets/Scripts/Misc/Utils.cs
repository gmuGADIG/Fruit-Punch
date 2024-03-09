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

    /// <summary>
    /// Returns a list of all colliders in the given layer which overlap this collider's shape. <br/>
    /// Note that only the shape is considered; not it's default layer collision, whether it's enabled, etc.
    /// </summary>
    public static Collider[] OverlapCollider(this Collider col, LayerMask layerMask)
    {
        // chat-gpt helped with getting the relevant parameters from each collider. might regret that later, idk
        switch (col)
        {
            case BoxCollider box:
            {
                Vector3 center = box.transform.TransformPoint(box.center);
                Vector3 halfExtents = Vector3.Scale(box.size, box.transform.lossyScale) * 0.5f;
                return Physics.OverlapBox(center, halfExtents, box.transform.rotation, layerMask);
            }
            case SphereCollider sphere:
            {
                Vector3 center = sphere.transform.TransformPoint(sphere.center);
                float radius = Mathf.Max(sphere.radius * Mathf.Max(sphere.transform.lossyScale.x, Mathf.Max(sphere.transform.lossyScale.y, sphere.transform.lossyScale.z)));
                return Physics.OverlapSphere(center, radius, layerMask);
            }
            case CapsuleCollider capsule:
            {
                Vector3 point1 = capsule.transform.TransformPoint(capsule.center + Vector3.up * capsule.height / 2 - Vector3.up * capsule.radius);
                Vector3 point2 = capsule.transform.TransformPoint(capsule.center - Vector3.up * capsule.height / 2 + Vector3.up * capsule.radius);
                float radius = capsule.radius * Mathf.Max(capsule.transform.lossyScale.x, Mathf.Max(capsule.transform.lossyScale.y, capsule.transform.lossyScale.z));
                return Physics.OverlapCapsule(point1, point2, radius, layerMask);
            }
            default:
                Debug.LogError($"OverlapCollider does not support collider of type {col.GetType().Name}!");
                return new Collider[0];
        }
    }

    /// <summary>
    /// Provides an easy way to run code on collision enter. Only works for boxes, capsules, and spheres. <br/>
    /// Returns a list of overlapping colliders except any in previousCollisions, and updates previousCollisions to the new overlaps.
    /// Example:
    /// <code>
    /// List&lt;Collider&gt; previousCollisions = new();
    /// void Update() {
    ///     var collisions = collider.NewOverlaps(layerMask, ref previousCollisions);
    ///     foreach (var col in collisions) ...
    /// }
    /// </code>
    /// </summary>
    /// <returns></returns>
    public static List<Collider> NewCollisions(this Collider col, LayerMask layerMask, ref List<Collider> previousCollisions)
    {
        var overlaps = col.OverlapCollider(layerMask).ToList();
        var collisions = overlaps.Except(previousCollisions).ToList();
        previousCollisions = overlaps;
        return collisions;
    }
}
