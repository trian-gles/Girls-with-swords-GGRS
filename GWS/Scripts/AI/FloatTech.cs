using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class FloatTech : BehaviourState
{

    public override int Poll(GameStateObjectRedesign.GameState state)
    {
        return 16;
    }

    public override string GetNextState(GameStateObjectRedesign.GameState state)
    {
        if (!AIBehaviour.floatStates.Contains(AIBehaviour.GetP2CurrentState(state)))
        {
            return "Chase";
        }

        return base.GetNextState(state);
    }

}
