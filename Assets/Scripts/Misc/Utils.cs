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
}
