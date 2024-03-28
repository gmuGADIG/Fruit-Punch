using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PomelaState
{
    Aggressive,
    Punching,
    Spitting,
    BigJump,
    Grabbed,
    
}

public class BossPomela : Boss
{
    StateMachine<PomelaState> stateMachine;
    Animator anim;

    [SerializeField] float punchDuration = 2;
    [SerializeField] float spittingDuration = 2;
    [SerializeField] float bigJumpDuration = 2;
    
    void Start()
    {
        stateMachine = new StateMachine<PomelaState>();
        stateMachine.AddState(PomelaState.Aggressive, null, AggressiveUpdate, null);
        
        this.GetComponentOrError(out anim);
    }

    PomelaState AggressiveUpdate()
    {
        PomelaState[] stateOptions = new[] { PomelaState.Punching, PomelaState.Spitting, PomelaState.BigJump};
        var newState = stateOptions[Random.Range(0, stateOptions.Length)];
        return newState;
    }

    void PunchingEnter()
    {
        StartCoroutine(Coroutine());
        IEnumerator Coroutine()
        {
            yield return WalkToPlayer();
            anim.Play("Punch"); // TODO: this doesn't exist
            yield return new WaitForSeconds(punchDuration);
            stateMachine.SetState(PomelaState.Aggressive)
        }
    }

    void SpittingEnter()
    {
        StartCoroutine(Coroutine());
        IEnumerator Coroutine()
        {
            anim.Play("Spit"); // TODO: make animation exist. add markers in animation for when to shoot.
            yield return new WaitForSeconds(spittingDuration);
            stateMachine.SetState(PomelaState.Aggressive)
        }
    }

    void BigJumpEnter()
    {
        StartCoroutine(Coroutine());
        IEnumerator Coroutine()
        {
            anim.Play("Jump"); // TODO: jump to serializable jump positions, then jump near player
            yield return new WaitForSeconds(bigJumpDuration);
            stateMachine.SetState(PomelaState.Aggressive);
        }
    }
}
