using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;

/// <summary>
/// Information about the action performed.
/// </summary>
public struct ActionContext {
    /// <summary>
    /// When the action was performed.
    /// </summary>
    public float Time;
    /// <summary>
    /// Underlying input system callback context
    /// </summary>
    public InputAction.CallbackContext CallbackContext;
}

[RequireComponent(typeof(PlayerInput))]
public class InputBuffer : MonoBehaviour
{
    PlayerInput input;
    /// <summary>
    /// Storage location for action contexts. 
    /// How the input buffer "remembers" inputs.
    /// </summary>
    Dictionary<string, ActionContext?> actions = new();

    [Tooltip("How long in seconds the input buffer should \"remember\" an input.")]
    [SerializeField]
    // using 0.16 here since smash uses a 10 frame input buffer
    float BufferLength = 0.16f;

    void Awake() {
        TryGetComponent(out input);
    }

    void Start() {

        foreach (var action in input.ActionIter()) {
            // apparently null is valid for anonymous actions. 
            // i have no clue what to do in that situation.
            Debug.Assert(action.name != null);

            var name = action.actionMap.name + "/" + action.name;
            action.performed += context => {
                actions[name] = new ActionContext {
                    Time = Time.time,
                    CallbackContext = context
                };
            };
        }
    }

    /// <summary>
    /// Checks whether an action has been performed BufferLength seconds ago.
    /// </summary>
    /// <param name="actionName">The path of the action in question. 
    /// This should follow the `actionMap/actionName` pattern.</param>
    /// <param name="context">Information about the action triggered. 
    /// May be not null if false is returned, guarenteed not null if true is returned.</param>
    /// <returns>True if action was performed BufferLength seconds ago. False if not.</returns>
    public bool PeekAction(string actionName, out ActionContext? context) {
        try {
            var _ignored = input.actions[actionName];
        } catch (KeyNotFoundException e) {
            Debug.LogWarning($"Cound not find action {actionName} in PlayerInput.");
        }

        if (actions.TryGetValue(actionName, out context)) {
            return Time.time - context.Value.Time < BufferLength;
        } else {
            context = null;
            return false;
        }
    }

    /// <summary>
    /// Checks whether an action has been performed BufferLength seconds ago.
    /// </summary>
    /// <param name="actionName">The path of the action in question. 
    /// This should follow the `actionMap/actionName` pattern.</param>
    /// <returns>True if action was performed BufferLength seconds ago. False if not.</returns>
    public bool PeekAction(string actionName) {
        return PeekAction(actionName, out var _ignored);
    }

    /// <summary>
    /// Checks whether an action has been performed BufferLength seconds ago.
    /// After checking, this function removes the action from the input buffer.
    /// If you don't want this to happen, use `PeekAction` instead.
    /// </summary>
    /// <param name="actionName">The path of the action in question. 
    /// This should follow the `actionMap/actionName` pattern.</param>
    /// <param name="context">Information about the action triggered. 
    /// May be not null if false is returned, guarenteed not null if true is returned.</param>
    /// <returns>True if action was performed BufferLength seconds ago. False if not.</returns>
    public bool CheckAction(string actionName, out ActionContext? context) {
        var result = PeekAction(actionName, out context);
        actions.Remove(actionName);

        return result;
    }

    /// <summary>
    /// Checks whether an action has been performed BufferLength seconds ago.
    /// After checking, this function removes the action from the input buffer.
    /// If you don't want this to happen, use `PeekAction` instead.
    /// </summary>
    /// <param name="actionName">The path of the action in question. 
    /// This should follow the `actionMap/actionName` pattern.</param>
    /// <returns>True if action was performed BufferLength seconds ago. False if not.</returns>
    public bool CheckAction(string actionName) {
        return CheckAction(actionName, out var _ignored);
    }

    /// <summary>
    /// Empty out the input buffer for a specific action.
    /// </summary>
    /// <param name="actionName">The path of the action in question. 
    /// This should follow the `actionMap/actionName` pattern.</param>
    public void ClearAction(string actionName) {
        actions.Remove(actionName);
    }

    /// <summary>
    /// Empty out the input buffer.
    /// </summary>
    public void Clear() {
        actions.Clear();
    }
}
