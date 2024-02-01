using System;
using UnityEngine;

public enum EnemyState
{
    Approaching, Attacking, Hurt
}

public class Enemy : MonoBehaviour
{
    [Tooltip("How fast the enemy approaches the player, in \"meters\" per second")]
    [SerializeField] private float walkingSpeed;
    
    [Tooltip("Distance at which the enemy starts attacking.")]
    [SerializeField] private float attackingDistance;
    
    private BeltCharacter beltChar;
    private EnemyState currentState = EnemyState.Approaching;
    
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
        SetState(this.currentState); // initialize state
    }

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.Approaching:
                UpdateApproaching();
                break;
            case EnemyState.Attacking:
                UpdateAttacking();
                break;
            case EnemyState.Hurt:
                UpdateHurt();
                break;
        }
    }

    void EnterApproaching()
    {
        // TODO: get players by the Player component, and sort them for the nearest.
        approachingCurrentTarget = GameObject.FindWithTag("Player").GetComponent<BeltCharacter>();
    }
    
    void UpdateApproaching()
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
            SetState(EnemyState.Attacking);
        }
    }

    void EnterAttacking()
    {
        attackingTimeLeft = 2;
        print("Enemy: WHAM!!");
    }
    
    void UpdateAttacking()
    {
        attackingTimeLeft -= Time.deltaTime;
        if (attackingTimeLeft <= 0) SetState(EnemyState.Approaching);
    }

    public void Hurt(float damage)
    {
        print($"health down to {100 - damage}");
        // health -= damage
        SetState(EnemyState.Hurt);
    }
    
    void EnterHurt()
    {
        print("Enemy: Ouch!!");
    }
    
    void UpdateHurt()
    {
        hurtTimeLeft -= Time.deltaTime;
        if (hurtTimeLeft <= 0) SetState(EnemyState.Approaching);
    }

    void SetState(EnemyState newState)
    {
        currentState = newState;
        switch (newState)
        {
            case EnemyState.Approaching:
                EnterApproaching();
                break;
            case EnemyState.Attacking:
                EnterAttacking();
                break;
            case EnemyState.Hurt:
                EnterHurt();
                break;
            default:
                throw new Exception($"state ({newState}) doesn't exist :<");
        }
    }
}
