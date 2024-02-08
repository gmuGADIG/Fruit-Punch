using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Gets the action the player input has for a given action id.
    /// </summary>
    /// <param name="input">The player input in question</param>
    /// <param name="actionId">The ID of the action</param>
    /// <returns>The player input's copy of the action</returns>
    public static InputAction PlayerInputActionOfActionId(this PlayerInput input, Guid actionId) {
        var actionIter = input.actions.actionMaps.Aggregate(
            new List<InputAction>().AsEnumerable(),
            (aux, next) => aux.Concat(next).Concat(next)
        );
        return actionIter
            .Where(action => action.id == actionId)
            .First();
    }
}
