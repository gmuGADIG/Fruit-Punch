using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossPhantomatoState {
    Wander,

    Grabbed, Thrown,

    VineErupt,
    VineWhip,
    VineCurtain,

    IntroCutscene,
    Dead,
}

public struct Rect2 {
    public Vector2 SmallCorner;
    public Vector2 BigCorner;

    /// <summary>
    /// Gets a random point contained or on the edge of this Rect2.
    /// </summary>
    public Vector2 GetRandomPoint() {
        return new Vector2(
            Random.Range(SmallCorner.x, BigCorner.x),
            Random.Range(SmallCorner.y, BigCorner.y)
        );
    }
}

public class BossPhantomato : Boss {
    static class WanderVars {
        public const string AnimationName = "Wander";

        public static float timeLeft;
    }

    static class VineEruptVars {
        public const string AnimationName = "VineErupt";
    }

    static class VineWhipVars {
        public const string AnimationName = "VineWhip";
    }

    [SerializeField] float walkSpeed = 1f;
    [SerializeField] float wanderInterval = 1f;
    [SerializeField] GameObject vineEruptionPrefab;

#if UNITY_EDITOR
    [ReadOnlyInInspector] [SerializeField] BossPhantomatoState state;
#endif

    StateMachine<BossPhantomatoState> stateMachine = new();
    Animator animator;
    // VineCurtain vineCurtain;

    Rect2 arenaBounds = new Rect2 {
        SmallCorner = new Vector2(-2, -0.7f),
        BigCorner = new Vector2(2, 0.1f)
    };

    new void Start() {
        base.Start();

        animator = GetComponent<Animator>();
        // vineCurtain = FindObjectOfType<VineCurtain>();

#if UNITY_EDITOR
        stateMachine.OnStateChange += s => state = s;
#endif

        stateMachine.AddState(BossPhantomatoState.Wander, WanderEnter, null, WanderExit);

        stateMachine.AddState(BossPhantomatoState.Grabbed, GrabbedEnter, null, GrabbedExit);
        stateMachine.AddState(BossPhantomatoState.Thrown, ThrownEnter, null, ThrownExit);

        stateMachine.AddState(BossPhantomatoState.VineErupt, VineEruptEnter, null, null);
        stateMachine.AddState(BossPhantomatoState.VineWhip, VineWhipEnter, null, null);
        // stateMachine.AddState(BossPhantomatoState.VineCurtain, vineCurtain.Attack, null, null);
        
        stateMachine.AddState(BossPhantomatoState.IntroCutscene, IntroCutsceneEnter, null, null);
        stateMachine.AddState(BossPhantomatoState.Dead, DeadEnter, null, null);

        groundCheck.GroundHit.AddListener(() => {
            if (stateMachine.currentState == BossPhantomatoState.Thrown) {
                stateMachine.SetState(GetRandomAttackState());
            }
        });
        grabbable.onGrab.AddListener(() => stateMachine.SetState(BossPhantomatoState.Grabbed));
        grabbable.onThrow.AddListener(() => stateMachine.SetState(BossPhantomatoState.Thrown));

        IntroCutsceneOver += OnIntroCutsceneOver;
        OutroCutsceneOver += OnOutroCutsceneOver;

        // vineCurtain.AttackFinished += () => stateMachine.SetState(BossPhantomatoState.Wander);
        health.onDeath += () => stateMachine.SetState(BossPhantomatoState.Dead);

        stateMachine.FinalizeAndSetState(BossPhantomatoState.IntroCutscene);
    }

    void OnDestroy() {
        IntroCutsceneOver -= OnIntroCutsceneOver;
        OutroCutsceneOver -= OnOutroCutsceneOver;
    }

    void OnOutroCutsceneOver() {
        animator.Play("Dead");
    }

    void OnIntroCutsceneOver() {
        stateMachine.SetState(BossPhantomatoState.Wander);
    }

    BossPhantomatoState GetRandomAttackState() {
        BossPhantomatoState[] states = {
            // BossPhantomatoState.VineCurtain,
            BossPhantomatoState.VineErupt,
            BossPhantomatoState.VineWhip,
        };

        return states.OrderBy(_ => Random.Range(0f, 1f)).First();
    }

    void Update() {
        stateMachine.Update();
    }


    void WanderEnter() {
        animator.Play(WanderVars.AnimationName);
        WanderVars.timeLeft = wanderInterval;

        rb.isKinematic = false;
        grabbable.enabled = true;

        StartCoroutine(WanderCoroutine());
    }

    IEnumerator WanderCoroutine() {
        // var targetPos = GetNearestPlayer().transform.position;
        // navMesh.enabled = true; // justin probably turned off the navmesh and i dont care enough to fix it
        // navMesh.SetDestination(targetPos);
        //
        // var timeStart = Time.time;
        //
        // while (Vector3.Distance(transform.position, targetPos) > 0.5f && Time.time - timeStart < 3f) { 
        //     rb.velocity = (navMesh.steeringTarget - transform.position).normalized * walkSpeed;
        //
        //     if (rb.velocity.x > 0) {
        //         transform.localEulerAngles = new Vector3(0f, 180f, 0f);
        //     } else if (rb.velocity.x < 0) {
        //         transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        //     }
        //     
        //     yield return null;
        //     if (stateMachine.currentState != BossPhantomatoState.Wander) yield break;
        // }

        yield return WalkToPlayer(walkSpeed);

        rb.velocity = Vector3.zero;
        rb.isKinematic = true;

        // yield return new WaitForSeconds(1f);
        if (stateMachine.currentState != BossPhantomatoState.Wander) yield break;

        stateMachine.SetState(GetRandomAttackState());
    }

    void WanderExit(BossPhantomatoState _newState) {
        if (_newState != BossPhantomatoState.Grabbed) {
            grabbable.enabled = false;
        }
    }
    

    void GrabbedEnter() {
        rb.isKinematic = false;
        animator.speed = 0;
    }

    void GrabbedExit(BossPhantomatoState _newState) {
        rb.isKinematic = true;
        grabbable.enabled = false;
        animator.speed = 1;
    }
    

    void ThrownEnter() {
        rb.isKinematic = false;
        grabbable.enabled = false;
    }

    void ThrownExit(BossPhantomatoState _newState) {
        rb.isKinematic = true;
    }


    void VineEruptEnter() {
        animator.Play(VineEruptVars.AnimationName);

        var players = FindObjectsOfType<Player>();
        foreach (var player in players) {
            Instantiate(vineEruptionPrefab, player.transform.position, Quaternion.identity);
        }

        for (int idx = 6 - players.Length; idx > 0; idx--) {
            Instantiate(vineEruptionPrefab, arenaBounds.GetRandomPoint().x0y(), Quaternion.identity);
        }

        IEnumerator Coroutine() {
            yield return new WaitForSeconds(232f/60f);

            if (stateMachine.currentState == BossPhantomatoState.VineErupt) {
                stateMachine.SetState(BossPhantomatoState.Wander);
            }
        }   
        StartCoroutine(Coroutine());
    }

    void VineCurtainEnter() {
        

        IEnumerator Coroutine() {
            yield return new WaitForSeconds(3f);

            if (stateMachine.currentState == BossPhantomatoState.VineCurtain) {
                stateMachine.SetState(BossPhantomatoState.Wander);
            }
        }   
        StartCoroutine(Coroutine());
    }


    void VineWhipEnter() {
        animator.Play(VineWhipVars.AnimationName);

        var player = GetNearestPlayer();
        var toPlayer = player.transform.position - transform.position;

        if (toPlayer.x > 0) {
            transform.localEulerAngles = new Vector3(0f, 180f, 0f);
        } else if (toPlayer.x < 0) {
            transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        }
    }

    void ExitVineWhipState() {
        stateMachine.SetState(BossPhantomatoState.Wander);
    }

    void IntroCutsceneEnter() {
        StartCoroutine(IntroCutscene());
    }

    void DeadEnter() {
        animator.Play(WanderVars.AnimationName);
        StartCoroutine(OutroCutscene());
    }
}
