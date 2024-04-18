using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public enum PlayerState
{
    Normal,
    Jump,
    Strike1, Strike2, Strike3,
    JumpStrike,
    Pearry,
    Grabbing, Throwing
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
    public PlayerState CurrentState => stateMachine.currentState;
    public event Action<PlayerState> OnPlayerStateChange;

    private StateMachine<PlayerState> stateMachine;
    private Rigidbody rb;
    private Animator anim;
    private PlayerInput playerInput;
    private InputBuffer inputBuffer;
    private Grabber grabber;
    private GroundCheck groundCheck;
    private float halfPlayerSizeX;

    [SerializeField] SpriteRenderer playerSprite;

    [Tooltip("Maximum speed the player can move (m/s).")]
    [SerializeField] float maxSpeed = 2f;
    
    [Tooltip("How quickly the player accelerates and decelerates (m/s^2).")]
    [SerializeField] float runAccel = 40f;
    
    [Tooltip("From 0 to 1, how much the player can influence their motion while midair.")]
    [SerializeField] float jumpControlMult = 0.3f;

    [Tooltip("Initial vertical speed when the player jumps (m/s).")]
    [SerializeField] float jumpSpeed = 3.2f;

    [Tooltip("How quickly the player accelerates down when in mid-air (m/s^2). Should be positive.")]
    [SerializeField] float gravity = 16;

    LayerMask collidingLayers;

    float strike1Length = -1;
    float strike2Length = -1;
    float strike3Length = -1;
    float pearryLength = 3;
    
    bool strikeAnimationOver = false;

    private bool FacingLeft => transform.localEulerAngles.y > 90;

    void Start()
    {
        // get components
        this.GetComponentOrError(out rb);
        this.GetComponentOrError(out anim);
        this.GetComponentOrError(out playerInput);
        this.GetComponentOrError(out inputBuffer);
        this.GetComponentInChildrenOrError(out grabber);
        this.GetComponentInChildrenOrError(out groundCheck);
        
        // subscribe events
        grabber.onForceRelease += ForceReleaseCallback;
        

        // get animation lengths
        foreach (var clip in anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name.EndsWith("Strike1")) strike1Length = clip.length;
            else if (clip.name.EndsWith("Strike2")) strike2Length = clip.length;
            else if (clip.name.EndsWith("Strike3")) strike3Length = clip.length;
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
        stateMachine.AddState(PlayerState.JumpStrike, JumpStrikeEnter, JumpUpdate, null);
        stateMachine.AddState(PlayerState.Pearry, PearryEnter, PearryUpdate, null);
        stateMachine.AddState(PlayerState.Grabbing, null, GrabbingUpdate, null);
        stateMachine.AddState(PlayerState.Throwing, ThrowingEnter, ThrowingUpdate, null);
        stateMachine.FinalizeAndSetState(PlayerState.Normal);

        stateMachine.OnStateChange += (PlayerState state) => OnPlayerStateChange?.Invoke(state);
    }


    void OnDestroy()
    {
        grabber.onForceRelease -= ForceReleaseCallback;
    }

    void Update()
    {
        stateMachine.Update();
        rb.velocity += Vector3.down * (gravity * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Pickup>(out var pickup))
            pickup.PickUp(this);
    }

    /// <summary>
    /// Moves the player according to input.
    /// </summary>
    /// <param name="controlMult">How much the player can influence their movement (may be less than 1 for in-air movement)</param>
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
        if (targetVel.x < 0) transform.localRotation = new Quaternion(0, 180, 0, 1);
        else if (targetVel.x > 0) transform.localRotation = Quaternion.identity;
    }

    // bool IsGrounded()
    // {
    //     // overlap a box below the player to see if it's grounded
    //     // uses the hitbox's x and z size, with an arbitrary height
    //     var hitBox = GetComponent<BoxCollider>();
    //     const float groundBoxHeight = 0.05f;
    //     
    //     var overlaps = Physics.OverlapBox(
    //         transform.position, 
    //         new Vector3(hitBox.size.x, groundBoxHeight, hitBox.size.z),
    //         Quaternion.identity,
    //         collidingLayers
    //     );
    //
    //     return overlaps.Length > 0;
    // }

    void NormalEnter()
    {
        anim.Play("Idle");
    }

    PlayerState NormalUpdate()
    {
        ApplyDirectionalMovement();

        if (playerInput.actions["gameplay/Jump"].triggered)
        {
            //Placeholder test for sound manager
            SoundManager.playSound("ArcadeTest");
            return PlayerState.Jump;
        }
        
        if (playerInput.actions["gameplay/Strike"].triggered)
        {
            return PlayerState.Strike1;
        }

        if (playerInput.actions["gameplay/Pearry"].triggered)
        {
            return PlayerState.Pearry;
        }
        
        if (playerInput.actions["gameplay/Interact"].triggered)
        {
            if (grabber.GrabItem()) return PlayerState.Grabbing;
        }

        return stateMachine.currentState;
    }

    void JumpEnter()
    {
        anim.Play("PlayerJump");
        rb.velocity += Vector3.up * jumpSpeed;
    }
    
    // NOTE: Is also the update function for jump attack!
    PlayerState JumpUpdate()
    {
        ApplyDirectionalMovement(jumpControlMult);

        var isFalling = rb.velocity.y < 0;
        if (isFalling && groundCheck.IsGrounded())
        {
            // transform.position = new Vector3(transform.position.x, 0, transform.position.y);
            // rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            return PlayerState.Normal;
        }


        if (playerInput.actions["gameplay/Strike"].triggered
            && stateMachine.currentState != PlayerState.JumpStrike) 
        {
            return PlayerState.JumpStrike;
        }
        
        if (playerInput.actions["gameplay/Interact"].triggered)
        {
            if (grabber.GrabItem()) return PlayerState.Grabbing;
        }

        return stateMachine.currentState;
    }

    void StrikeEnter(int strikeState)
    {
        // avoid reading inputs before we entered this state
        inputBuffer.ClearAction("gameplay/Strike");
        strikeAnimationOver = false;
        
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
            return PlayerState.Normal;
        }

        return stateMachine.currentState;
    }

    void JumpStrikeEnter() {
        anim.Play("PlayerJumpStrike");
    }

    // new Pearry Script has been moved from 
    // Pearry.cs to its own PearryEnter and PearryUpdate methods
    void PearryEnter() {
        // anim.Play("PlayerPearry");
    }

    PlayerState PearryUpdate() {

        if (stateMachine.timeInState >= pearryLength)
        {
            return PlayerState.Normal;
        }

        return stateMachine.currentState;
    }

    // subscribed to the grabber's onForceRelease event
    void ForceReleaseCallback()
    {
        stateMachine.SetState(PlayerState.Normal);
    }
    
    PlayerState GrabbingUpdate()
    {
        Debug.Assert(grabber.IsGrabbing);
        rb.velocity = Vector3.MoveTowards(rb.velocity, Vector3.zero, runAccel * Time.deltaTime);
        ApplyDirectionalMovement(0);
        
        if (playerInput.actions["gameplay/Interact"].triggered)
        {
            return PlayerState.Throwing;
        }
        
        return stateMachine.currentState;
    }

    void ThrowingEnter()
    {
        grabber.ThrowItem(FacingLeft);
    }
    
    PlayerState ThrowingUpdate()
    {
        // TODO: throwing should be tied better into the animation
        if (stateMachine.timeInState >= .1f)
        {
            return PlayerState.Normal;
        }

        return stateMachine.currentState;
    }

    public void OnStikeAnimationOver() {
        strikeAnimationOver = true;
    }
}
