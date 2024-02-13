using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
[RequireComponent(typeof(Player))]
class PlayerStateMachine : StateMachineComponent<AbstractPlayerState, PlayerState>
{
    protected override void InitializeStates()
    {
        Player player = GetComponent<Player>();
        states = new Dictionary<PlayerState, AbstractPlayerState>
        { 
            {PlayerState.Normal, new PlayerNormalState(player)} 
        };

        ChangeState(PlayerState.Normal);
    }
}

internal class PlayerNormalState : AbstractPlayerState
{
    public PlayerNormalState(Player player) : base(player) 
    { }
    public override void EnterState()
    {

    }

    public override void ExitState()
    {
        
    }

    public override PlayerState UpdateState()
    {
        return PlayerState.Normal;
    }
}
