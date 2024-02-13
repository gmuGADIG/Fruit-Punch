using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class StateMachineComponent<T, E> : MonoBehaviour 
    where T : IState<E> 
    where E : Enum
{
    protected Dictionary<E, T> states;
    public E CurrentState { get; private set; }

    /// <summary>
    /// Use to initialize the states dictionary and set the initial state
    /// </summary>
    protected abstract void InitializeStates();
    private void Start()
    {
        InitializeStates();
    }
    private void Update()
    {
        if (CurrentState != null)
        {
            E newState = states[CurrentState].UpdateState();
            if (newState != null && !newState.Equals(CurrentState))
            {
                ChangeState(newState);
            }

        }
    }
    public void ChangeState(E newState)
    {
        if (CurrentState != null)
        {
            states[CurrentState].ExitState();
        }
        CurrentState = newState;
        states[CurrentState].EnterState();
    }
}
