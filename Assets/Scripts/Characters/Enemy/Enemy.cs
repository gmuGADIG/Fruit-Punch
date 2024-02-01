using System;
using System.Linq;
using UnityEngine;

public enum EnemyState
{
    Approaching, Attacking, Hurt
}

public class Enemy : MonoBehaviour
{
    [Tooltip("How fast the enemy approaches the player, in \"meters\" per second")]
    [SerializeField] private float walkingSpeed;
    
    [Tooltip("When the enemy is this close to the player, it will start attacking.")]
    [SerializeField] private float attackingDistance;
    
    private BeltCharacter beltChar;
    StateMachine<EnemyState> stateMachine = new();

    #region Approaching-State Values
    /// <summary>
    /// If the enemy is in the approaching state, this value will be the object it's going towards.
    /// Otherwise, it's value is meaningless.
    /// </summary>
    private BeltCharacter approachingCurrentTarget = null;
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
        stateMachine.AddState(EnemyState.Approaching, EnterApproaching, UpdateApproaching, null);
        stateMachine.AddState(EnemyState.Attacking, EnterAttacking, UpdateAttacking, null);
        stateMachine.AddState(EnemyState.Hurt, EnterHurt, UpdateHurt, null);
        stateMachine.FinalizeAndSetState(EnemyState.Approaching);
    }

    private void Update()
    {
        stateMachine.Update();
    }

    void EnterApproaching()
    {
        // TODO: get players by the Player component instead of tag "Player"
        var playerBeltChars =
            GameObject.FindGameObjectsWithTag("Player")
            .Select(x => x.GetComponent<BeltCharacter>())
            .Where(x => x != null)
            .ToList();
        // find closest player (because list.MinBy only exists in future version >_<)
        BeltCharacter nearest = null;
        float nearestDist = float.MaxValue;
        foreach (var b in playerBeltChars)
        {
            var dist = Vector3.Distance(b.internalPosition, this.beltChar.internalPosition);
            if (dist < nearestDist)
            {
                nearest = b;
                nearestDist = dist;
            }
        }

        approachingCurrentTarget = nearest;
    }
    
    EnemyState UpdateApproaching()
    {
        var walkTo = approachingCurrentTarget.internalPosition;
        walkTo.y = this.beltChar.internalPosition.y;
        beltChar.internalPosition = Vector3.MoveTowards(
            beltChar.internalPosition, 
            walkTo,
            walkingSpeed *  Time.deltaTime
        );

        if (Vector3.Distance(beltChar.internalPosition, walkTo) < attackingDistance)
        {
            return EnemyState.Attacking;
        }

        return stateMachine.currentState;
    }

    void EnterAttacking()
    {
        attackingTimeLeft = 2;
        print("Enemy: WHAM!!");
    }
    
    EnemyState UpdateAttacking()
    {
        attackingTimeLeft -= Time.deltaTime;
        if (attackingTimeLeft <= 0) return EnemyState.Approaching;
        else return stateMachine.currentState;
    }

    public void Hurt(float damage)
    {
        print($"health down to {100 - damage}");
        stateMachine.SetState(EnemyState.Hurt);
    }
    
    void EnterHurt()
    {
        print("Enemy: Ouch!!");
    }
    
    EnemyState UpdateHurt()
    {
        hurtTimeLeft -= Time.deltaTime;
        if (hurtTimeLeft <= 0) return EnemyState.Approaching;
        else return stateMachine.currentState;
    }
}
