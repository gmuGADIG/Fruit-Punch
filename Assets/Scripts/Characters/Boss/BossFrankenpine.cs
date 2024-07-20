using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum BossFrankenpineState {
    Wander,

    Grabbed, Thrown,

    JunkThrow, SummonMinions, AcidSpit,

    OpeningCutscene, Dead
}

[System.Serializable]
struct JunkProjectile {
     public GameObject prefab;
     public float speed;
     public float damage;
}

[RequireComponent(typeof(Animator))]
public class BossFrankenpine : Boss
{
    static class WanderVars {
        public const string AnimationName = "Wander";
        public static float timeLeft;
    }
    static class JunkThrowVars {
        public const string AnimationName = "Throw";

        public static Stack<JunkProjectile> thrownJunk = new();
        public static float throwTimer;
    }

    static class SummonMinionsVars {
        public class EnemySpawn {
            public GameObject Prefab;
            public AuraType Aura;
        }

        public const string AnimationName = "Wander";

        public static List<EnemySpawn> enemySpawns = new();
        public static float spawnTimer;
        public static float stillTimer;
    }

    static class AcidSpitVars {
        public const string AnimationName = "Wander";

        public static float spitTimer;
        public static bool firstSpitFired;
        public static bool secondSpitFired;
    }

    Player target;
    StateMachine<BossFrankenpineState> stateMachine = new();
    bool isPhaseTwo { get {
        var health = GetComponent<Health>();
        return health.CurrentHealth <= health.MaxHealth / 2;
    }}

    Animator animator;
    event System.Action junkThrowOver;

    [SerializeField] float walkSpeed = 1f;
    [SerializeField] Transform projectileStartPoint;
    [SerializeField] float wanderInterval = 1f;

    [SerializeField] JunkProjectile[] junkPrefabs;
    [SerializeField] float postJunkThrowWaitTime = .5f;

    [SerializeField] float spawnInterval = .5f;
    // [SerializeField] float postSpawnTimer = .5f;
    [SerializeField] GameObject projectileEnemyPrefab;
    [SerializeField] GameObject basicEnemyPrefab;

    [SerializeField] GameObject spitProjectilePrefab;
    [SerializeField] float spitInterval = .5f;

    [SerializeField] float spitDamage = 10f;

#if UNITY_EDITOR
    [SerializeField] [ReadOnlyInInspector] BossFrankenpineState state;
#endif

    new void Start() {
        base.Start();
        animator = GetComponent<Animator>();
        
        stateMachine.AddState(BossFrankenpineState.Wander, WanderEnter, null, WanderExit);
        stateMachine.AddState(BossFrankenpineState.Grabbed, GrabbedEnter, null, GrabbedExit);
        stateMachine.AddState(BossFrankenpineState.Thrown, ThrownEnter, null, ThrownExit);
        stateMachine.AddState(BossFrankenpineState.JunkThrow, JunkThrowEnter, JunkThrowUpdate, JunkThrowExit);
        stateMachine.AddState(BossFrankenpineState.SummonMinions, SummonMinionsEnter, SummonMinionsUpdate, null);
        stateMachine.AddState(BossFrankenpineState.AcidSpit, AcidSpitEnter, AcidSpitUpdate, null);
        stateMachine.AddState(BossFrankenpineState.OpeningCutscene, OpeningCutsceneEnter, null, null);
        stateMachine.AddState(BossFrankenpineState.Dead, DeadEnter, null, null);

        stateMachine.FinalizeAndSetState(BossFrankenpineState.OpeningCutscene);

#if UNITY_EDITOR
        stateMachine.OnStateChange += s => this.state = s;
#endif

        groundCheck.GroundHit.AddListener(() => {
            if (stateMachine.currentState == BossFrankenpineState.Thrown) {
                stateMachine.SetState(GetRandomAttackState());
            }
        });
        grabbable.onGrab.AddListener(() => stateMachine.SetState(BossFrankenpineState.Grabbed));
        grabbable.onThrow.AddListener(() => stateMachine.SetState(BossFrankenpineState.Thrown));

        Boss.IntroCutsceneOver += OnIntroCutsceneOver;
        Boss.OutroCutsceneOver += OnOutroCutsceneOver;
        health.onDeath += () => stateMachine.SetState(BossFrankenpineState.Dead);
    }

    void OnIntroCutsceneOver() {
        stateMachine.SetState(BossFrankenpineState.Wander);
    }

    void OnDestroy() {
        Boss.OutroCutsceneOver -= OnOutroCutsceneOver;
        Boss.IntroCutsceneOver -= OnIntroCutsceneOver;
    }

    BossFrankenpineState GetRandomAttackState() {
        BossFrankenpineState[] states = {
            BossFrankenpineState.AcidSpit,
            BossFrankenpineState.SummonMinions,
            BossFrankenpineState.JunkThrow,
        };

        return states.OrderBy(_ => Random.Range(0f, 1f)).First();
    }

    void Update() {
        stateMachine.Update();
    }


    void WanderEnter() {
        target = FindObjectsOfType<Player>().OrderBy(player => Random.Range(0f, 1f)).First();
        animator.Play(WanderVars.AnimationName);
        WanderVars.timeLeft = wanderInterval;

        rb.isKinematic = false;
        grabbable.enabled = true;

        StartCoroutine(WanderCoroutine());
    }

    IEnumerator WanderCoroutine() {
        var targetPos = GetNearestPlayer().transform.position;
        navMesh.enabled = true; // justin probably turned off the navmesh and i dont care enough to fix it
        navMesh.SetDestination(targetPos);

        var timeStart = Time.time;

        while (Vector3.Distance(transform.position, targetPos) > 0.5f && Time.time - timeStart < 3f) { 
            rb.velocity = (navMesh.steeringTarget - transform.position).normalized * walkSpeed;
            
            yield return null;
            if (stateMachine.currentState != BossFrankenpineState.Wander) yield break;
        }

        rb.velocity = Vector3.zero;
        rb.isKinematic = true;

        // yield return new WaitForSeconds(3f);
        if (stateMachine.currentState != BossFrankenpineState.Wander) yield break;

        stateMachine.SetState(GetRandomAttackState());
    }

    void WanderExit(BossFrankenpineState _newState) {
        if (_newState != BossFrankenpineState.Grabbed) {
            grabbable.enabled = false;
        }
    }
    

    void GrabbedEnter() {
        rb.isKinematic = false;
        animator.speed = 0;
    }

    void GrabbedExit(BossFrankenpineState _newState) {
        rb.isKinematic = true;
        grabbable.enabled = false;
        animator.speed = 1;
    }
    

    void ThrownEnter() {
        rb.isKinematic = false;
        grabbable.enabled = false;
    }

    void ThrownExit(BossFrankenpineState _newState) {
        rb.isKinematic = true;
    }


    void JunkThrowEnter() {
        JunkThrowVars.throwTimer = postJunkThrowWaitTime;
        animator.Play(JunkThrowVars.AnimationName);

        JunkThrowVars.thrownJunk = 
            new(junkPrefabs
            .OrderBy(_ => Random.Range(0, 1f))
            .Take(isPhaseTwo ? 3 : 2)
            .ToList());
    }

    void ThrowJunk() {
        var junk = JunkThrowVars.thrownJunk.Pop();
        var projectile = Instantiate(junk.prefab, projectileStartPoint.position, Quaternion.identity)
                                .GetComponent<EnemyProjectile>();

        var direction = Mathf.Sign(target.transform.position.x - transform.position.x);
        projectile.Setup(junk.damage, Vector3.right * junk.speed * direction); 

        if (JunkThrowVars.thrownJunk.Count == 0) {
            IEnumerator WaitThenWander() {
                var waitingForAnimationEnd = true;
                junkThrowOver += () => waitingForAnimationEnd = false;
                while (waitingForAnimationEnd) { 
                    /* no-op */
                    yield return null;
                }
                if (stateMachine.currentState != BossFrankenpineState.JunkThrow) yield break;

                animator.Play(WanderVars.AnimationName);

                yield return new WaitForSeconds(postJunkThrowWaitTime);
                if (stateMachine.currentState != BossFrankenpineState.JunkThrow) yield break;
                stateMachine.SetState(BossFrankenpineState.Wander);
            }

            StartCoroutine(WaitThenWander());
        }
    }

    void JunkThrowOver() {
        junkThrowOver?.Invoke();
    }

    BossFrankenpineState JunkThrowUpdate() {
        var direction = Mathf.Sign(target.transform.position.x - transform.position.x);

        transform.rotation = Quaternion.Euler(Vector3.up * (direction == -1 ? 0f : 180f));

        return stateMachine.currentState;
    }

    void JunkThrowExit(BossFrankenpineState _newState) {
        animator.Play(WanderVars.AnimationName);
    }


    void SummonMinionsEnter() {
        SummonMinionsVars.stillTimer = 0;
        SummonMinionsVars.spawnTimer = spawnInterval;

        SummonMinionsVars.enemySpawns = new();
        SummonMinionsVars.enemySpawns.Add(new SummonMinionsVars.EnemySpawn {
            Prefab = basicEnemyPrefab,
            Aura = AuraType.Normal
        });
        SummonMinionsVars.enemySpawns.Add(new SummonMinionsVars.EnemySpawn {
            Prefab = basicEnemyPrefab,
            Aura = AuraType.Normal
        });
        SummonMinionsVars.enemySpawns.Add(new SummonMinionsVars.EnemySpawn {
            Prefab = projectileEnemyPrefab,
            Aura = AuraType.Normal
        });

        if (isPhaseTwo) {
            AuraType[] auras = {
                AuraType.Throw,
                AuraType.Strike,
                AuraType.JumpAtk
            };

            var aura = auras[Random.Range(0, auras.Length)];
            var enemySpawn = SummonMinionsVars.enemySpawns[Random.Range(0, auras.Length)];
            enemySpawn.Aura = aura;
        }
    }

    BossFrankenpineState SummonMinionsUpdate() {
        // we must stay still for 5 seconds because there are no
        // enemies to spawn
        if (SummonMinionsVars.enemySpawns.Count == 0) {
            SummonMinionsVars.stillTimer -= Time.deltaTime;
            if (SummonMinionsVars.stillTimer <= 0) {
                return BossFrankenpineState.Wander;
            }

            return stateMachine.currentState;
        }
        
        if (SummonMinionsVars.spawnTimer <= 0) {
            var idx = SummonMinionsVars.enemySpawns.Count - 1;
            var enemySpawn = SummonMinionsVars.enemySpawns[idx];
            SummonMinionsVars.enemySpawns.RemoveAt(idx);

            var enemyHealth = Instantiate(
                enemySpawn.Prefab,
                transform.position,
                Quaternion.identity
            ).GetComponent<Health>();
            enemyHealth.vulnerableTypes = enemySpawn.Aura;

            SummonMinionsVars.spawnTimer = spawnInterval;
        } else {
            SummonMinionsVars.spawnTimer -= Time.deltaTime;
        }

        return stateMachine.currentState;
    }

    void AcidSpitEnter() {
        AcidSpitVars.firstSpitFired = false;
        AcidSpitVars.secondSpitFired = !isPhaseTwo;
        AcidSpitVars.spitTimer = spitInterval;
    }

    BossFrankenpineState AcidSpitUpdate() {
        if (AcidSpitVars.firstSpitFired && AcidSpitVars.secondSpitFired) {
            return BossFrankenpineState.Wander;
        }

        AcidSpitVars.spitTimer -= Time.deltaTime;
        if (AcidSpitVars.spitTimer <= 0) {
            const float projectileGravity = 16f;
            Vector3 toTarget = target.transform.position - transform.position;

            if (!AcidSpitVars.firstSpitFired) {
                AcidSpit acidSpit = Instantiate(spitProjectilePrefab, projectileStartPoint.position, Quaternion.identity)
                    .GetComponent<AcidSpit>();

                var lookVector = transform.localEulerAngles.y == 0 ? Vector3.right : Vector3.left;

                acidSpit.Setup(
                        spitDamage,
                        new Vector3(Mathf.Sign(toTarget.x) * 2f, 3f, 0f),
                        Vector3.down * projectileGravity
                );

                AcidSpitVars.firstSpitFired = true;
            } else {
                const float halfProjectileDuration = 0.5f;
                const float verticalVelocity = projectileGravity * halfProjectileDuration;

                AcidSpit acidSpit = Instantiate(spitProjectilePrefab, projectileStartPoint.position, Quaternion.identity)
                    .GetComponent<AcidSpit>();

                float horizontalVelocity = toTarget.x / (halfProjectileDuration * 2);
                float depthVelocity = toTarget.z / (halfProjectileDuration * 2);

                acidSpit.Setup(
                        spitDamage,
                        new Vector3(horizontalVelocity, verticalVelocity, depthVelocity),
                        Vector3.down * projectileGravity
                );

                AcidSpitVars.secondSpitFired = true;
            }
            
            AcidSpitVars.spitTimer = spitInterval;
        }

        return stateMachine.currentState;
    }

    void OpeningCutsceneEnter() {
        StartCoroutine(IntroCutscene());
    }

    void OnOutroCutsceneOver() {
        animator.Play("Dead");
    }

    void DeadEnter() {
        animator.Play(WanderVars.AnimationName);
        foreach (var health in FindObjectsOfType<Enemy>().Select(e => e.GetComponent<Health>())) {
            health.Die();
        }

        StartCoroutine(OutroCutscene());
    }
}
