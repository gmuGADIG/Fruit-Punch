using UnityEngine;

[RequireComponent(typeof(Animator))]
class BasicEnemy : Enemy {
    bool attackingAnimationOver = false;
    Animator animator;

    protected override void Start() {
        this.GetComponentOrError(out animator);
        base.Start();
    }

    protected override void AttackingEnter()
    {
        attackingAnimationOver = false;
        animator.Play("Swing", 0, 0f);
    }

    protected override EnemyState AttackingUpdate()
    {
        base.AttackingUpdate();
        if (groundCheck.IsGrounded() == false) return EnemyState.InAir;

        return attackingAnimationOver ? EnemyState.Wandering : stateMachine.currentState;
    }

    protected override void AttackingExit(EnemyState _newState)
    {
        animator.Play("Wander", 0, 0f);
    }

    public void AttackingAnimationOver() {
        attackingAnimationOver = true;
    }
}
