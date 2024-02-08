using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Normal
}

[RequireComponent(typeof(BeltCharacter))]
public class Player : MonoBehaviour
{
    private StateMachine<PlayerState> stateMachine;
    private BeltCharacter beltChar;
    private Vector3 velocity = Vector3.zero;

    [SerializeField] private float maxSpeed;
    [SerializeField] private float runAccel;
    
    void Start()
    {
        this.GetComponentOrError(out beltChar);

        stateMachine = new StateMachine<PlayerState>();
        stateMachine.AddState(PlayerState.Normal, null, NormalUpdate, null);
        stateMachine.FinalizeAndSetState(PlayerState.Normal);
    }

    void Update()
    {
        stateMachine.Update();
        
        beltChar.internalPosition += velocity * Time.deltaTime;
    }

    PlayerState NormalUpdate()
    {
        var movementDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        velocity = Vector3.MoveTowards(
            velocity,
            movementDir * maxSpeed,
            runAccel * Time.deltaTime
        );
        
        return stateMachine.currentState;
    }
}
