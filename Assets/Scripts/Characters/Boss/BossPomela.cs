using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

enum PomelaState
{
    Aggressive,
    Punching,
    Spitting,
    BigJump,
}

public class BossPomela : Boss
{
    StateMachine<PomelaState> stateMachine;
    GroundCheck groundCheck;
    Animator anim;

    [Tooltip("When doing a ground pound jump, Pomela will first jump to one of these points before launching much higher.")]
    [SerializeField] private Transform[] ropePoints;
    [SerializeField] private float timeBetweenAttacks = 2;
    [SerializeField] float punchDuration = 2;
    [SerializeField] float spittingDuration = 2;
    [SerializeField] float bigJumpDuration = 2;

    IEnumerator activeCoroutine;
    
    void Start()
    {
        this.GetComponentOrError(out anim);
        this.GetComponentOrError(out groundCheck);
        
        stateMachine = new StateMachine<PomelaState>();
        stateMachine.AddState(PomelaState.Aggressive, null, AggressiveUpdate, null);
        stateMachine.AddState(PomelaState.Punching, PunchingEnter, null, AttackStateExit);
        stateMachine.AddState(PomelaState.Spitting, SpittingEnter, null, AttackStateExit);
        stateMachine.AddState(PomelaState.BigJump, BigJumpEnter, null, AttackStateExit);
    }

    PomelaState AggressiveUpdate()
    {
        if (stateMachine.timeInState >= timeBetweenAttacks)
        {
            PomelaState[] stateOptions = { PomelaState.Punching, PomelaState.Spitting, PomelaState.BigJump };
            var newState = stateOptions[Random.Range(0, stateOptions.Length)];
            return newState;
        }
        else return stateMachine.currentState;
    }

    void PunchingEnter()
    {
        activeCoroutine = Coroutine();
        StartCoroutine(activeCoroutine);
        IEnumerator Coroutine()
        {
            yield return WalkToPlayer();
            anim.Play("Punch"); // TODO: this doesn't exist
            yield return new WaitForSeconds(punchDuration);
            stateMachine.SetState(PomelaState.Aggressive);
        }
    }

    void SpittingEnter()
    {
        activeCoroutine = Coroutine();
        StartCoroutine(activeCoroutine);
        IEnumerator Coroutine()
        {
            anim.Play("Spit"); // TODO: make animation exist. add markers in animation for when to shoot.
            yield return new WaitForSeconds(spittingDuration);
            stateMachine.SetState(PomelaState.Aggressive);
        }
    }

    void BigJumpEnter()
    {
        activeCoroutine = Coroutine();
        StartCoroutine(activeCoroutine);
        IEnumerator Coroutine()
        {
            anim.Play("Jump"); // TODO: jump to serializable jump positions, then jump near player
            yield return new WaitForSeconds(bigJumpDuration);
            stateMachine.SetState(PomelaState.Aggressive);
        }
    }

    /// <summary>  Exit callback for all attack states. Ends the active coroutine. </summary>
    void AttackStateExit(PomelaState prevState) => StopCoroutine(activeCoroutine);

    IEnumerator JumpTo(Vector3 targetPosition, float height, float gravity)
    {
        float jumpTime = 2 * Mathf.Sqrt(2 * height / gravity);
        float startVelocity = Mathf.Sqrt(2 * height / gravity) * gravity;

        Vector3 startPosition = rb.position;
        rb.velocity += Vector3.up * startVelocity;
        
        float elapsedTime = 0;
        while (elapsedTime < jumpTime / 2 || !groundCheck.IsGrounded())
        {
            float t = elapsedTime / jumpTime;
            rb.position = new Vector3(
                Mathf.Lerp(startPosition.x, targetPosition.x, t),
                rb.position.y,
                Mathf.Lerp(startPosition.z, targetPosition.z, t)
            );
            rb.velocity += Vector3.down * gravity * Time.deltaTime;
            
            yield return new WaitForEndOfFrame();

            elapsedTime += Time.deltaTime;
            if (elapsedTime > 10f) { // safety exit
                Debug.LogWarning("Safety Exit: big jump didn't land after 10 seconds");
                break;
            }
        }
    }
}
