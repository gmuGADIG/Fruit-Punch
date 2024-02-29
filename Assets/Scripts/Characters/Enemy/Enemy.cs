using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum EnemyState
{
    Wandering, Aggressive, Attacking, Hurt
}

[RequireComponent(typeof(Health))]
public class Enemy : MonoBehaviour
{
    private static int currentAttackingEnemies = 0;
    private const int MaxSimultaneousAttackers = 2;
    
    [Tooltip("How fast the enemy approaches the player, in \"meters\" per second")]
    [SerializeField] private float walkingSpeed;

    [Tooltip("When the enemy is this close to the player, it will start attacking.")]
    [SerializeField] private float attackingDistance;

    [SerializeField] private float wanderingTimeMin = 5000;
    [SerializeField] private float wanderingTimeMax = 10000;
    
    private BeltCharacter beltChar;
    StateMachine<EnemyState> stateMachine = new();

    #region Wandering-State Values
    private float wanderingTimeTillAttack = 0;
    private Vector2 wanderingToPosition;
    private float wanderingTimeTillWander;
    #endregion
    
    #region Aggressive-State Values
    /// <summary>
    /// If the enemy is in the approaching state, this value will be the object it's going towards.
    /// Otherwise, it's value is meaningless.
    /// </summary>
    private BeltCharacter aggressiveCurrentTarget = null;
    #endregion

    #region Attacking-State Values
    private float attackingTimeLeft;
    #endregion

    #region Hurt-State Values
    private float hurtTimeLeft;
    #endregion

    private void Start()
    {
        this.GetComponentOrError(out beltChar);
        stateMachine.AddState(EnemyState.Wandering, WanderingEnter, WanderingUpdate, null);
        stateMachine.AddState(EnemyState.Aggressive, AggressiveEnter, AggressiveUpdate, null);
        stateMachine.AddState(EnemyState.Attacking, AttackingEnter, AttackingUpdate, AttackingExit);
        stateMachine.AddState(EnemyState.Hurt, HurtEnter, HurtUpdate, null);
        stateMachine.FinalizeAndSetState(EnemyState.Wandering);
    }

    private void Update()
    {
        stateMachine.Update();
    }

    void WanderingEnter()
    {
        wanderingTimeTillAttack = Random.Range(wanderingTimeMin, wanderingTimeMax);
        
        var position2d = new Vector2(beltChar.internalPosition.x, beltChar.internalPosition.y);
        wanderingToPosition = position2d + new Vector2(Random.Range(-2, 2), Random.Range(-2, 2));
    }
    
    EnemyState WanderingUpdate()
    {
        // changing to aggressive state after waiting some time
        wanderingTimeTillAttack -= Time.deltaTime;
        if (wanderingTimeTillAttack <= 0)
        {
            if (currentAttackingEnemies >= MaxSimultaneousAttackers) // TODO: multiply by player count
            {
                return EnemyState.Aggressive;
            }
        }
        
        // random movement
        if (wanderingTimeTillWander > 0)
        {
            wanderingTimeTillWander -= Time.deltaTime;
        }
        else
        {
            var position2d = new Vector2(beltChar.internalPosition.x, beltChar.internalPosition.y);
            var movement = (wanderingToPosition - position2d);
            if (movement.magnitude <= 0.02f)
            {
                wanderingToPosition = position2d + new Vector2(Random.Range(-2, 2), Random.Range(-2, 2));
            }
            else wanderingTimeTillWander = 2;
        }
        
        return stateMachine.currentState;
    }

    void AggressiveEnter()
    {
        currentAttackingEnemies += 1;
        
        // find all player belt characters
        var playerBeltChars =
            FindObjectsOfType<Player>()
            .Select(x => x.GetComponent<BeltCharacter>())
            .Where(x => x != null);

        // start approaching the nearest one
        // (note: sometimes it feels like differences in z-position are exaggerated for this? might want to use transform position instead, idk)
        aggressiveCurrentTarget =
            playerBeltChars
            .OrderBy(bc => Vector3.Distance(this.beltChar.internalPosition, bc.internalPosition))
            .First();
    }
    
    EnemyState AggressiveUpdate()
    {
        var walkTo = aggressiveCurrentTarget.internalPosition;
        walkTo.y = this.beltChar.internalPosition.y;
        beltChar.internalPosition = Vector3.MoveTowards(
            beltChar.internalPosition,
            walkTo,
            walkingSpeed * Time.deltaTime
        );

        if (Vector3.Distance(beltChar.internalPosition, walkTo) < attackingDistance)
        {
            Debug.Log("I'm attached to: " + aggressiveCurrentTarget.name);
            Component[] c = aggressiveCurrentTarget.GetComponents<MonoBehaviour>();

            foreach (MonoBehaviour script in c)
            {
                Debug.Log(script);
            }

            Health health = aggressiveCurrentTarget.GetComponent<Health>();

            // HARDCODED DAMAGE - TODO: make this a variable
            health.Damage(new DamageInfo(90, Vector2.zero, AuraType.Strike));
            return EnemyState.Attacking;
        }

        return stateMachine.currentState;
    }

    void AttackingEnter()
    {
        attackingTimeLeft = 2;
        // print("Enemy: WHAM!!");
    }
    
    EnemyState AttackingUpdate()
    {
        attackingTimeLeft -= Time.deltaTime;
        if (attackingTimeLeft <= 0) return EnemyState.Wandering;
        else return stateMachine.currentState;
    }

    void AttackingExit()
    {
        currentAttackingEnemies -= 1;
    }

    public void Hurt(DamageInfo info)
    {
        // print($"health down to {100 - damage}");
        // stateMachine.SetState(EnemyState.Hurt);
    }
    
    void HurtEnter()
    {
        print("Enemy: Ouch!!");
        currentAttackingEnemies -= 1;
    }
    
    EnemyState HurtUpdate()
    {
        hurtTimeLeft -= Time.deltaTime;
        if (hurtTimeLeft <= 0) return EnemyState.Aggressive;
        else return stateMachine.currentState;
    }
}
