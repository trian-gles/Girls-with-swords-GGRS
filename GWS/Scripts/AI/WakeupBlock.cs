using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class WakeupBlock : BehaviourState
{
    private Random random = new Random();
    public override int Poll(GameStateObjectRedesign.GameState state)
    {
        int input = 0;

        
            
        if (state.P2State.facingRight)
            input += 8;
        else
            input += 4;

        if (state.P1State.grounded == false)
        {
            input += 2;
        }



            

        return input;
    }

    public override string GetNextState(GameStateObjectRedesign.GameState state)
    {
        if (!(state.P2State.currentState == "Knockdown") && state.P2State.frameCount > 2)
        {
            if (random.Next(2) == 1)
                return "Zone";
            else
                return "Chase";
        }

        return base.GetNextState(state);
    }

}
