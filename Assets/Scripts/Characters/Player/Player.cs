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
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(InputBuffer))]
public class Player : MonoBehaviour
{
    private StateMachine<PlayerState> stateMachine;
    private Rigidbody rb;
    private Animator anim;
    private PlayerInput playerInput;
    private InputBuffer inputBuffer;
    private HurtBox hurtBox;
    
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
    
    LayerMask collidingLayers;

    float strike1Length = -1;
    float strike2Length = -1;
    float strike3Length = -1;
    
    bool strikeAnimationOver = false;
    
    void Start()
    {
        // get components
        this.GetComponentOrError(out rb);
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
        
        // set layer
        collidingLayers = Utils.GetCollidingLayerMask(LayerMask.NameToLayer("Player"));

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
    }

    void ApplyDirectionalMovement(float controlMult = 1)
    {
        var targetVel = new Vector3(
            playerInput.actions["gameplay/Left/Right"].ReadValue<float>(),
            0,
            playerInput.actions["gameplay/Up/Down"].ReadValue<float>()
        ).normalized * maxSpeed;

        targetVel.y = rb.velocity.y;
        rb.velocity = Vector3.MoveTowards(
            rb.velocity,
            targetVel,
            runAccel * Time.deltaTime * controlMult
        );

        // flip according to direction
        if (targetVel.x < 0) transform.localScale = new Vector3(-1, 1, 1);
        else if (targetVel.x > 0) transform.localScale = new Vector3(1, 1, 1);
    }

    bool IsGrounded()
    {
        // overlap a box below the player to see if it's grounded
        // uses the hitbox's x and z size, with an arbitrary height
        var hitBox = GetComponent<BoxCollider>();
        const float groundBoxHeight = 0.05f;
        
        var overlaps = Physics.OverlapBox(
            transform.position, 
            new Vector3(hitBox.size.x, groundBoxHeight, hitBox.size.z),
            Quaternion.identity,
            collidingLayers
        );

        return overlaps.Length > 0;
    }

    void NormalEnter()
    {
        print("returning to normal");
        anim.Play("Idle");
    }

    PlayerState NormalUpdate()
    {
        ApplyDirectionalMovement();
        rb.velocity += Vector3.down * (gravity * Time.deltaTime); // gravity, just in case it isn't quite grounded

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
        print("Jumping");
        rb.velocity += Vector3.up * jumpSpeed;
    }
    
    PlayerState JumpUpdate()
    {
        ApplyDirectionalMovement(jumpControlMult);
        rb.velocity += Vector3.down * (gravity * Time.deltaTime);

        var isFalling = rb.velocity.y < 0;
        if (isFalling && IsGrounded())
        {
            // transform.position = new Vector3(transform.position.x, 0, transform.position.y);
            // rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
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
        rb.velocity = Vector3.MoveTowards(rb.velocity, Vector3.zero, runAccel * Time.deltaTime);

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
