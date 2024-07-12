using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState
{
    Dead,
    Normal,
    Jump,
    Strike1, Strike2, Strike3,
    JumpStrike,
    Pearry,
    Grabbing, Throwing,
    // Apple specific
    AppleStrike3,
    // Banana specific
    BananaJumpStrike
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
    public int PlayerNum { get; set; }

    private StateMachine<PlayerState> stateMachine;
    private Rigidbody rb;
    private Animator anim;
    private PlayerInput playerInput;
    private InputBuffer inputBuffer;
    private Grabber grabber;
    private GroundCheck groundCheck;
    private Health health;

    private PauseManager pauseManager;

    private ColorTweaker colorTweaker;

    [Tooltip("Which of the 4 characters the player is. Necessary for character-specific moves")]
    public Character playerCharacter;

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

    [Tooltip("Duration of the pearry state.")]
    [SerializeField] float pearryLength = 1f;

    [Tooltip("Name of the sound effect to play on combo finish")]
    [SerializeField] string comboSoundEffect;

    LayerMask collidingLayers;

    float strike1Length = -1;
    float strike2Length = -1;
    float strike3Length = -1;
    float bananaJumpStrikeUptime = .1f;
    float bananaJumpStrikeVertSpeed = 8f;
    float bananaJumpStrikeFollowThruTime = .1f;
    float timer = 0f;
    bool timerStarted = false;
    
    /// <summary>
    /// True when the player can move on to the next part of the strike animation.
    /// </summary>
    bool strikeAnimationOver = false;
    /// <summary>
    /// True when the player hit something during their strike animation.
    /// </summary>
    bool hasHitSomething = false;
    /// <summary>
    /// Whether to apply gravity to the player
    /// </summary>
    bool applyGravity = true;

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
        this.GetComponentInChildrenOrError(out colorTweaker);
        this.GetComponentOrError(out health);

        foreach (var hb in GetComponentsInChildren<HurtBox>(true)) {
            hb.onHurt += damageInfo => {
                hasHitSomething = true;
            };
        }
        
        // subscribe events
        grabber.OnForceRelease += ForceReleaseCallback;
        

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
        stateMachine.AddState(PlayerState.Pearry, PearryEnter, PearryUpdate, PearryExit);
        stateMachine.AddState(PlayerState.Grabbing, GrabbingEnter, GrabbingUpdate, null);
        stateMachine.AddState(PlayerState.Throwing, ThrowingEnter, ThrowingUpdate, null);
        stateMachine.AddState(PlayerState.Dead, DeadEnter, null, null);
        stateMachine.AddState(PlayerState.AppleStrike3, AppleStrikeEnter, AppleStrikeUpdate, null); // apple specific
        stateMachine.AddState(PlayerState.BananaJumpStrike, BananaJumpStrikeEnter, BananaJumpStateUpdate, BananaJumpStateExit); // banana specific
        
        stateMachine.FinalizeAndSetState(PlayerState.Normal);
        stateMachine.OnStateChange += (PlayerState state) => OnPlayerStateChange?.Invoke(state);

        // get the PauseManager script to allow player to pause the game
        pauseManager = GameObject.Find("PauseManager").GetComponent<PauseManager>();
        
        // attach death state to health component's events
        health.onDeath += () => stateMachine.SetState(PlayerState.Dead);
    }


    void OnDestroy()
    {
        grabber.OnForceRelease -= ForceReleaseCallback;
    }

    void Update()
    {
        if (playerInput.actions["gameplay/Pause"].triggered)
		{
            pauseManager.Pause();
        }
		if (Time.timeScale == 0)
		{
			return;
		}
        stateMachine.Update();
        if (applyGravity)
        {
            rb.velocity += Vector3.down * (gravity * Time.deltaTime);
        }
        timer += Time.deltaTime;
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

        if (CurrentState == PlayerState.Normal)
        {
            if (targetVel.sqrMagnitude != 0)
            {
                anim.Play("Walk");
            }
            else
            {
                anim.Play("Idle");
            }
        }

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

    void NormalEnter()
    {
        anim.Play("Idle");
        colorTweaker.RemoveAuraColor();
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

        if (playerInput.actions["gameplay/Pearry"].triggered)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
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
        anim.Play("Jump");
        SoundManager.Instance.PlaySoundAtPosition("Jump", transform.position);
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
            if (playerCharacter == Character.Banana)
            {
                return PlayerState.BananaJumpStrike;
            }
            else
            {
                return PlayerState.JumpStrike;
            }
        }
        
        if (playerInput.actions["gameplay/Interact"].triggered)
        {
            if (grabber.GrabItem()) return PlayerState.Grabbing;
        }

        return stateMachine.currentState;
    }

    void StrikeEnter(int strikeState)
    {
        colorTweaker.SetAuraColor(AuraType.Strike);
        
        // avoid reading inputs before we entered this state
        inputBuffer.ClearAction("gameplay/Strike");

        // make sure we only move onto the next strike animation when we're ready
        hasHitSomething = false;
        strikeAnimationOver = false;
        
        if (strikeState is <= 0 or > 3) throw new Exception($"Invalid strike state {strikeState}!");
        anim.Play($"Strike{strikeState}"); // e.g. "Strike1", "Strike2", "Strike3"
        SoundManager.Instance.PlaySoundAtPosition(
            (strikeState == 3)  ? "HitHeavy"  : "Hit",
            transform.position
        );

        if (strikeState == 3) {
            SoundManager.Instance.PlaySoundGlobal(comboSoundEffect);
        }
    }

    PlayerState StrikeUpdate(int strikeState)
    {
        rb.velocity = Vector3.MoveTowards(rb.velocity, Vector3.zero, runAccel * Time.deltaTime);

        if (strikeState is <= 0 or > 3) throw new Exception($"Invalid strike state {strikeState}!");
        if (strikeAnimationOver && hasHitSomething && inputBuffer.CheckAction("gameplay/Strike"))
        {
            if (strikeState == 1) return PlayerState.Strike2;
            if (strikeState == 2)
            {
                if (playerCharacter == Character.Apple)
                {
                    return PlayerState.AppleStrike3;
                }
                else
                {
                    return PlayerState.Strike3;
                }
            }
        }

        var animLengths = new[] { strike1Length, strike2Length, strike3Length };
        var thisAnimLength = animLengths[strikeState - 1];
        if (stateMachine.timeInState >= thisAnimLength || !groundCheck.IsGrounded())
        {
            return PlayerState.Normal;
        }

        return stateMachine.currentState;
    }

    void JumpStrikeEnter() {
        colorTweaker.SetAuraColor(AuraType.JumpAtk);
        anim.Play("JumpStrike");
    }

    void PearryEnter() {
        colorTweaker.SetAuraColor(AuraType.Pearry);
        health.Pearrying = true;
        anim.Play("Pearry");
    }

    PlayerState PearryUpdate() {

        if (stateMachine.timeInState >= pearryLength)
        {
            return PlayerState.Normal;
        }

        return stateMachine.currentState;
    }

    void PearryExit(PlayerState state)
    {
        health.Pearrying = false;
    }

    // subscribed to the grabber's onForceRelease event
    void ForceReleaseCallback()
    {
        stateMachine.SetState(PlayerState.Normal);
    }
    
    void GrabbingEnter()
    {
        colorTweaker.SetAuraColor(AuraType.Throw);
        anim.Play("Grab");
    }

    PlayerState GrabbingUpdate()
    {
        Debug.Assert(grabber.IsGrabbing);
        var newVelocity = Vector3.MoveTowards(rb.velocity, Vector3.zero, runAccel * Time.deltaTime);
        rb.velocity = new Vector3(newVelocity.x, rb.velocity.y, newVelocity.z);
        ApplyDirectionalMovement(0);
        
        if (playerInput.actions["gameplay/Interact"].triggered)
        {
            return PlayerState.Throwing;
        }
        
        return stateMachine.currentState;
    }

    void ThrowingEnter()
    {
        colorTweaker.SetAuraColor(AuraType.Throw);
        grabber.ThrowItem(FacingLeft);
        anim.Play("Throw");
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

    /// <summary>
    /// When called, it signals to the player script that the player can strike again for a combo. 
    /// Should be called through an animation event.
    /// </summary>
    public void OnStrikeAnimationOver() {
        strikeAnimationOver = true;
    }

    void AppleStrikeEnter()
    {
        // avoid reading inputs before we entered this state
        inputBuffer.ClearAction("gameplay/Strike");

        // make sure we only move onto the next strike animation when we're ready
        strikeAnimationOver = false;

        anim.Play("Strike3"); // e.g. "Strike1", "Strike2", "Strike3"
        SoundManager.Instance.PlaySoundAtPosition("HitHeavy", transform.position);
        SoundManager.Instance.PlaySoundGlobal(comboSoundEffect);
    }

    PlayerState AppleStrikeUpdate()
    {
        rb.velocity = Vector3.MoveTowards(rb.velocity, Vector3.zero, runAccel * Time.deltaTime);

        var thisAnimLength = strike3Length;
        if (stateMachine.timeInState >= thisAnimLength || !groundCheck.IsGrounded())
        {
            return PlayerState.Normal;
        }

        return stateMachine.currentState;
    }

    void BananaJumpStrikeEnter()
    {
        anim.Play("JumpStrike");
        rb.velocity = Vector3.zero;
        applyGravity = false;
        timerStarted = false;
    }

    PlayerState BananaJumpStateUpdate()
    {
        // while the player is in air
        if (!groundCheck.IsGrounded())
        {
            // short vertical bump
            if (stateMachine.timeInState <= bananaJumpStrikeUptime)
            {
                transform.Translate(bananaJumpStrikeVertSpeed * Time.deltaTime * Vector3.up);
            }
            else
            {
                transform.Translate(bananaJumpStrikeVertSpeed * Time.deltaTime * Vector3.down);
            }
        }
        else if (!timerStarted)
        {
            // when player hits ground, start follow thru timer
            timer = 0;
            timerStarted = true;
            applyGravity = true;
        }
        if (timerStarted && timer >= bananaJumpStrikeFollowThruTime)
        {
            return PlayerState.Normal;
        }
        return stateMachine.currentState;
    }

    void BananaJumpStateExit(PlayerState nextState)
    {
        applyGravity = true;
    }

    void DeadEnter()
    {
        StartCoroutine(Coroutine());

        IEnumerator Coroutine()
        {
            rb.velocity = Vector3.zero;
            anim.Play("Dead");
            
            yield return new WaitForSeconds(1f);
            // at this point, the player is invisible and can be moved around if needed

            yield return new WaitForSeconds(1f);
            // at this point, the player has reappeared
            health.Heal(health.MaxHealth);
            
            yield return new WaitForSeconds(0.2f);
            // animation did it's little flicker thing at the end. return to normal state
            stateMachine.SetState(PlayerState.Normal);
        }
    }
}
