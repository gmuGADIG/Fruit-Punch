using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Normal,
    Jumping,
    Strike1, Strike2, Strike3
}

/// <summary>
/// Playable character script. <br/>
/// Handled through a state machine. For specific state code, see functions like NormalUpdate, JumpingEnter, etc.
/// </summary>
[RequireComponent(typeof(BeltCharacter)), RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    private StateMachine<PlayerState> stateMachine;
    private BeltCharacter beltChar;
    private Animator anim;
    
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

    [Header("Animation Details")]
    [SerializeField] AnimationClip strike1Clip;
    [SerializeField] AnimationClip strike2Clip;
    [SerializeField] AnimationClip strike3Clip;
    
    void Start()
    {
        this.GetComponentOrError(out beltChar);
        this.GetComponentOrError(out anim);

        if (strike1Clip == null || strike2Clip == null || strike3Clip == null)
            throw new Exception("Null strike animation clips!");

        stateMachine = new StateMachine<PlayerState>();
        stateMachine.AddState(PlayerState.Normal, NormalEnter, NormalUpdate, null);
        stateMachine.AddState(PlayerState.Jumping, JumpEnter, JumpUpdate, null);
        stateMachine.AddState(PlayerState.Strike1, () => StrikeEnter(1), () => StrikeUpdate(1), null);
        stateMachine.AddState(PlayerState.Strike2, () => StrikeEnter(2), () => StrikeUpdate(2), null);
        stateMachine.AddState(PlayerState.Strike3, () => StrikeEnter(3), () => StrikeUpdate(3), null);
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

    void NormalEnter()
    {
        anim.SetTrigger("Idle");
    }

    PlayerState NormalUpdate()
    {
        ApplyDirectionalMovement();

        if (Input.GetKeyDown(KeyCode.Space)) // todo: swap with new input system
            return PlayerState.Jumping;

        if (Input.GetKeyDown(KeyCode.Z))
            return PlayerState.Strike1;

        return stateMachine.currentState;
    }

    void JumpEnter()
    {
        anim.SetTrigger("Jump");
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

    void StrikeEnter(int strikeState)
    {
        if (strikeState is <= 0 or > 3) throw new Exception($"Invalid strike state {strikeState}!");
        anim.SetTrigger($"Strike{strikeState}"); // e.g. "Strike1", "Strike2", "Strike3"
    }

    PlayerState StrikeUpdate(int strikeState)
    {
        velocity = Vector3.MoveTowards(velocity, Vector3.zero, runAccel * Time.deltaTime);

        if (strikeState is <= 0 or > 3) throw new Exception($"Invalid strike state {strikeState}!");
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (strikeState == 1) return PlayerState.Strike2;
            if (strikeState == 2) return PlayerState.Strike3;
        }

        var animLengths = new[] { strike1Clip.length, strike2Clip.length, strike3Clip.length };
        var thisAnimLength = animLengths[strikeState - 1];
        if (stateMachine.timeInState >= thisAnimLength)
        {
            return PlayerState.Normal;
        }

        return stateMachine.currentState;
    }
}
