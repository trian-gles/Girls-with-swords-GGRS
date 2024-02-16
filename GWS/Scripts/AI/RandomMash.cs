using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RandomMash : BehaviourState
{

    private Random random = new Random();
    public override int Poll(GameStateObjectRedesign.GameState state)
    {
        return random.Next(511);
    }

    public override string GetNextState(GameStateObjectRedesign.GameState state)
    {
        if (Math.Abs(state.P1State.position[0] - state.P2State.position[0]) > 4000)
        {
            if (random.Next(2) == 1)
                return "Zone";
            else
                return "Chase";
        }

        return base.GetNextState(state);
    }
}