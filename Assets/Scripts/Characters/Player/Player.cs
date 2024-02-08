using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Normal,
    Jumping
}

/// <summary>
/// Playable character script. <br/>
/// Handled through a state machine. For specific state code, see functions like NormalUpdate, JumpingEnter, etc.
/// </summary>
[RequireComponent(typeof(BeltCharacter))]
public class Player : MonoBehaviour
{
    private StateMachine<PlayerState> stateMachine;
    private BeltCharacter beltChar;
    
    [ReadOnlyInInspector, SerializeField] private Vector3 velocity = Vector3.zero;

    [Tooltip("Maximum speed the player can move (m/s).")]
    [SerializeField] float maxSpeed;
    
    [Tooltip("How quickly the player accelerates and decelerates (m/s^2).")]
    [SerializeField] float runAccel;
    
    [Tooltip("From 0 to 1, how much the player can influence their motion while midair.")]
    [SerializeField] float jumpControlMult = 1;

    [Tooltip("Initial vertical speed when the player jumps (m/s).")]
    [SerializeField] float jumpSpeed;

    [Tooltip("How quickly the player accelerates down when in mid-air (m/s^2). Should be positive.")]
    [SerializeField] float gravity;
    
    void Start()
    {
        this.GetComponentOrError(out beltChar);

        stateMachine = new StateMachine<PlayerState>();
        stateMachine.AddState(PlayerState.Normal, null, NormalUpdate, null);
        stateMachine.AddState(PlayerState.Jumping, JumpEnter, JumpUpdate, null);
        stateMachine.FinalizeAndSetState(PlayerState.Normal);
    }

    void Update()
    {
        stateMachine.Update();
        
        beltChar.internalPosition += velocity * Time.deltaTime;
    }

    void ApplyDirectionalMovement(float controlMult = 1)
    {
        var targetVel = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * maxSpeed;
        targetVel.y = velocity.y;
        velocity = Vector3.MoveTowards(
            velocity,
            targetVel,
            runAccel * Time.deltaTime * controlMult
        );
    }

    PlayerState NormalUpdate()
    {
        ApplyDirectionalMovement();

        if (Input.GetKeyDown(KeyCode.Space)) // todo: swap with new input system
        {
            return PlayerState.Jumping;
        }

        return stateMachine.currentState;
    }

    void JumpEnter()
    {
        velocity += Vector3.up * jumpSpeed;
    }

    PlayerState JumpUpdate()
    {
        ApplyDirectionalMovement(jumpControlMult);

        velocity += Vector3.down * (gravity * Time.deltaTime);
        if (velocity.y < 0 && beltChar.internalPosition.y  <= 0)
        {
            beltChar.internalPosition.y = 0;
            velocity.y = 0;
            return PlayerState.Normal;
        }

        return stateMachine.currentState;
    }
}
