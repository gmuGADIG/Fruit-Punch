using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public abstract class AbstractPlayerState : IState<PlayerState>
{
    protected Player player { get; private set; }

    public AbstractPlayerState(Player player)
    {
        this.player = player;
    }
    public abstract void EnterState();
    public abstract PlayerState UpdateState();
    public abstract void ExitState();
}
