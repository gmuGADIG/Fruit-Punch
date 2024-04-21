using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

enum LeadingLadyState
{
    Aggressive,
    Punching,
    Spitting,
    BigJump,
}

public class BossLeadingLady : Boss
{
    StateMachine<LeadingLadyState> stateMachine;
    GroundCheck groundCheck;
    Animator anim;

    [SerializeField] float walkSpeed = 2f;
    [SerializeField] float timeBetweenAttacks = 2;
    
    [Header("Rope Jumps")]
    [Tooltip("When doing a ground pound jump, Pomela will first jump to one of these points before launching much higher.")]
    [SerializeField] Transform[] ropePoints;
    [SerializeField] float jumpEndLag = 2;
    
    [Header("Punches")]
    [SerializeField] float punchDuration = 2;
    
    [Header("Spitting")]
    [SerializeField] float spittingDuration = 2;
    [SerializeField] float projectileSpeed = 2f;
    [SerializeField] private float projectileDamage = 10f;
    [SerializeField] Transform spitEmitPoint;
    [SerializeField] GameObject spitProjectilePrefab;

    IEnumerator activeCoroutine;
    
    new void Start()
    {
        base.Start();
        
        this.GetComponentOrError(out anim);
        this.GetComponentOrError(out groundCheck);
        
        stateMachine = new StateMachine<LeadingLadyState>();
        stateMachine.AddState(LeadingLadyState.Aggressive, null, AggressiveUpdate, null);
        stateMachine.AddState(LeadingLadyState.Punching, PunchingEnter, null, AttackStateExit);
        stateMachine.AddState(LeadingLadyState.Spitting, SpittingEnter, null, AttackStateExit);
        stateMachine.AddState(LeadingLadyState.BigJump, BigJumpEnter, null, AttackStateExit);
        stateMachine.FinalizeAndSetState(LeadingLadyState.Aggressive);
    }

    void Update()
    {
        stateMachine.Update();
        FlipWithVelocity(rb.velocity);
    }

    LeadingLadyState AggressiveUpdate()
    {
        rb.velocity += Vector3.down * 9.8f * Time.deltaTime; // gravity
        
        if (stateMachine.timeInState >= timeBetweenAttacks)
        {
            LeadingLadyState[] stateOptions = { LeadingLadyState.Punching, LeadingLadyState.Spitting, LeadingLadyState.BigJump };
            // LeadingLadyState[] stateOptions = { LeadingLadyState.Spitting };
            var newState = stateOptions[Random.Range(0, stateOptions.Length)];
            print($"Boss entering state {newState}");
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
            yield return WalkToPlayer(walkSpeed);
            anim.Play("Punch");
            yield return new WaitForSeconds(punchDuration);
            stateMachine.SetState(LeadingLadyState.Aggressive);
        }
    }

    void SpittingEnter()
    {
        activeCoroutine = Coroutine();
        StartCoroutine(activeCoroutine);
        IEnumerator Coroutine()
        {
            // face player
            FlipWithVelocity(GetNearestPlayer().transform.position - this.transform.position);
            
            anim.Play("Spit");
            yield return new WaitForSeconds(spittingDuration);
            anim.Play("Idle");
            stateMachine.SetState(LeadingLadyState.Aggressive);
        }
    }

    void BigJumpEnter()
    {
        activeCoroutine = Coroutine();
        StartCoroutine(activeCoroutine);
        IEnumerator Coroutine()
        {
            anim.Play("Jump"); // TODO: this animation clip doesn't exist
            
            // jump to rope
            Utils.Assert(ropePoints.Length != 0);
            var ropePosition = ropePoints[Random.Range(0, ropePoints.Length)].position;
            yield return JumpTo(ropePosition, 2, 10);
            
            // jump to player
            yield return JumpTo(GetNearestPlayer().transform.position, 2, 10);
            anim.Play("Landing");
            yield return new WaitForSeconds(jumpEndLag);
            
            stateMachine.SetState(LeadingLadyState.Aggressive);
        }
    }

    /// <summary>  Exit callback for all attack states. Ends the active coroutine. </summary>
    void AttackStateExit(LeadingLadyState prevState) => StopCoroutine(activeCoroutine);

    IEnumerator JumpTo(Vector3 targetPosition, float height, float gravity)
    {
        navMesh.enabled = false; // NavMeshAgents suck and don't work well with rigidbody motion
        float jumpTime = 2 * Mathf.Sqrt(2 * height / gravity);
        float startVelocity = Mathf.Sqrt(2 * height / gravity) * gravity;
        
        Vector3 startPosition = rb.position;
        rb.velocity = Vector3.up * startVelocity;
        
        float elapsedTime = 0;
        while (elapsedTime < jumpTime)
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
        navMesh.enabled = true;
    }

    /// <summary>
    /// Called by animation events. Spits a single projectile forward.
    /// </summary>
    public void SpitProjectile()
    {
        var proj =
            Instantiate(spitProjectilePrefab, spitEmitPoint.position, Quaternion.identity)
            .GetComponent<EnemyProjectile>();
        proj.GetComponent<HurtBox>().parentTransform = this.transform;
        proj.Setup(projectileDamage, transform.right * projectileSpeed);
    }
}
