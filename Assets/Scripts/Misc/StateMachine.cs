using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// General purpose finite state machine. Each state can have a function run when its entered or left, as well as on each frame.
/// <br/><br/>
/// Example usage:
/// <code>
/// stateMachine = new StateMachine&lt;PlayerState&gt;();
/// stateMachine.AddState(PlayerState.Idle, this.IdleEnter, this.IdleUpdate, this.IdleExit)
/// stateMachine.AddState(PlayerState.Attack, null, null, null)
/// stateMachine.FinalizeAndSetState(PlayerState.Idle)
/// </code>
/// followed by a call to <c>stateMachine.Update()</c> each frame. <br/>
/// Note that all update callbacks would have a return type of PlayerState, representing the new state.
/// </summary>
/// <typeparam name="TState">The enum type which holds each state.</typeparam>
public class StateMachine<TState>
where TState : struct, Enum
{
    public TState currentState { get; private set; }
    public float timeInState { get; private set; }
    private bool setupDone;
    Dictionary<TState, StateCallbacks<TState>> callbacks = new();

    /// <summary>
    /// Internal StateMachine class to hold each state's 3 callbacks: enter, update, and exit. <br/>
    /// Each one may be null. To handle this, they should be called like: `<c>callback.enter?.Invoke()</c>`.
    /// </summary>
    private class StateCallbacks<T>
    {
        [CanBeNull] public Action enter;
        [CanBeNull] public Func<T> update;
        [CanBeNull] public Action exit;

        public StateCallbacks(Action enter, Func<T> update, Action exit)
        {
            this.enter = enter;
            this.update = update;
            this.exit = exit;
        }
    }

    /// <summary>
    /// Adds a new state to the state machine, and sets up it's enter, update, and exit callbacks. <br/>
    /// The update callback should return the resulting state.
    /// </summary>
    public void AddState(TState state, [CanBeNull] Action enterCallback, [CanBeNull] Func<TState> updateCallback, [CanBeNull] Action exitCallback)
    {
        callbacks.Add(state, new StateCallbacks<TState>(enterCallback, updateCallback, exitCallback));
    }

    /// <summary>
    /// Called after implementing all state callbacks. Finalizes the state machine and sets its default state (calling its enter function). <br/>
    /// The state machine will not work properly if this function isn't called. This is to make sure the initial state has been properly established.
    /// </summary>
    public void FinalizeAndSetState(TState defaultState)
    {
        currentState = defaultState;
        callbacks[currentState].enter?.Invoke();
        setupDone = true;
    }

    /// <summary>
    /// Changes the current state, calling the appropriate callbacks unless no change occurs (<c>currentState == newState</c>). <br/>
    /// This is an alternative to update callbacks returning the new state, often useful when an external event results in a state change.
    /// </summary>
    public void SetState(TState newState)
    {
        if (callbacks.ContainsKey(newState) == false) Debug.LogError($"Attempting to set state `{newState}`, but no callbacks exist! Make sure it's set up with `stateManager.AddState`");
        if (newState.Equals(currentState)) return;

        Debug.Log($"Entering state {newState}.");

        callbacks[currentState].exit?.Invoke();
        timeInState = 0;
        currentState = newState;
        callbacks[currentState].enter?.Invoke();
    }

    /// <summary>
    /// Calls the current state's update callback, applying any state change if applicable. 
    /// </summary>
    public void Update()
    {
        if (!setupDone) Debug.LogError("StateMachine updated before being finalized! Finalize it with `stateMachine.FinalizeAndSetState`");
        var updateCallback = callbacks[currentState].update;
        if (updateCallback != null)
        {
            var newState = updateCallback.Invoke();
            this.SetState(newState);
        }

        timeInState += Time.deltaTime;
    }
}
