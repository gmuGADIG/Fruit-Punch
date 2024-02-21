using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState
{
    Normal,
    Jump,
    Strike1, Strike2, Strike3
}

/// <summary>
/// Playable character script. <br/>
/// To get a good idea of how everything works, see how the state machine is set up in Start.
/// </summary>
[RequireComponent(typeof(BeltCharacter))] 
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(InputBuffer))]
public class Player : MonoBehaviour
{
    private StateMachine<PlayerState> stateMachine;
    private BeltCharacter beltChar;
    private Animator anim;
    private PlayerInput playerInput;
    private InputBuffer inputBuffer;
    private HurtBox hurtBox;
    
    [ReadOnlyInInspector, SerializeField] private Vector3 velocity = Vector3.zero;

    [Tooltip("Maximum speed the player can move (m/s).")]
    [SerializeField] float maxSpeed = 2f;
    
    [Tooltip("How quickly the player accelerates and decelerates (m/s^2).")]
    [SerializeField] float runAccel = 40f;
    
    [Tooltip("From 0 to 1, how much the player can influence their motion while midair.")]
    [SerializeField] float jumpControlMult = 0.3f;

    [Tooltip("Initial vertical speed when the player jumps (m/s).")]
    [SerializeField] float jumpSpeed = 3.2f;

    [Tooltip("How quickly the player accelerates down when in mid-air (m/s^2). Should be positive.")]
    [SerializeField] float gravity = 9;

    float strike1Length = -1;
    float strike2Length = -1;
    float strike3Length = -1;
    
    bool strikeAnimationOver = false;
    
    void Start()
    {
        // gett components
        this.GetComponentOrError(out beltChar);
        this.GetComponentOrError(out anim);
        this.GetComponentOrError(out playerInput);
        this.GetComponentOrError(out inputBuffer);

        // get animation lengths
        foreach (var clip in anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name == "PlayerStrike1") strike1Length = clip.length;
            else if (clip.name == "PlayerStrike2") strike2Length = clip.length;
            else if (clip.name == "PlayerStrike3") strike3Length = clip.length;
        }
        if (strike1Length < 0 || strike2Length < 0 || strike3Length < 0)
            throw new Exception("Animation clips weren't found!");

        // set up state machine
        stateMachine = new StateMachine<PlayerState>();
        stateMachine.AddState(PlayerState.Normal, NormalEnter, NormalUpdate, null);
        stateMachine.AddState(PlayerState.Jump, JumpEnter, JumpUpdate, null);
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

        var targetVel = new Vector3(
            playerInput.actions["gameplay/Left/Right"].ReadValue<float>(),
            0,
            playerInput.actions["gameplay/Up/Down"].ReadValue<float>()
        ).normalized * maxSpeed;

        targetVel.y = velocity.y;
        velocity = Vector3.MoveTowards(
            velocity,
            targetVel,
            runAccel * Time.deltaTime * controlMult
        );

        // flip according to direction
        if (targetVel.x < 0) transform.localScale = new Vector3(-1, 1, 1);
        else if (targetVel.x > 0) transform.localScale = new Vector3(1, 1, 1);
    }

    void NormalEnter()
    {
        print("returning to normal");
        anim.Play("Idle");
    }

    PlayerState NormalUpdate()
    {
        ApplyDirectionalMovement();

        if (playerInput.actions["gameplay/Jump"].triggered)
        {
            return PlayerState.Jump;
        }
        
        if (playerInput.actions["gameplay/Strike"].triggered)
        {
            return PlayerState.Strike1;
        }

        return stateMachine.currentState;
    }

    void JumpEnter()
    {
        // anim.Play("Jump");
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
        // avoid reading inputs before we entered this state
        inputBuffer.ClearAction("gameplay/Strike");
        strikeAnimationOver = false;

        print($"Strike{strikeState}");
        if (strikeState is <= 0 or > 3) throw new Exception($"Invalid strike state {strikeState}!");
        anim.Play($"Strike{strikeState}"); // e.g. "Strike1", "Strike2", "Strike3"
    }

    PlayerState StrikeUpdate(int strikeState)
    {
        velocity = Vector3.MoveTowards(velocity, Vector3.zero, runAccel * Time.deltaTime);

        if (strikeState is <= 0 or > 3) throw new Exception($"Invalid strike state {strikeState}!");
        if (strikeAnimationOver && inputBuffer.CheckAction("gameplay/Strike"))
        {
            if (strikeState == 1) return PlayerState.Strike2;
            if (strikeState == 2) return PlayerState.Strike3;
        }

        var animLengths = new[] { strike1Length, strike2Length, strike3Length };
        var thisAnimLength = animLengths[strikeState - 1];
        if (stateMachine.timeInState >= thisAnimLength)
        {
            print($"{stateMachine.timeInState} >= {thisAnimLength}");
            return PlayerState.Normal;
        }

        return stateMachine.currentState;
    }

    public void OnStikeAnimationOver() {
        strikeAnimationOver = true;
    }
}
