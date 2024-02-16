using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

public class Zone : BehaviourState
{

    private Random random = new Random();
    int distance = 10000;

    public override void Enter()
    {
        base.Enter();
        GD.Print("Zoning");
    }

    public override int Poll(GameStateObjectRedesign.GameState state)
    {
        distance = state.P1State.position[0] - state.P2State.position[0];
        int action = 0;
        if (random.Next(2) == 1) {
            if (random.Next(2) == 1)
                action |= 4;
            else
                action |= 8;

            if (random.Next(2) == 1)
                action |= 128;
        }
            

        


        return action;
    }

    public override string GetNextState(GameStateObjectRedesign.GameState state)
    {
        if (Math.Abs(distance) < 4000)
        {
            GD.Print("Ending Zoning");
            return "RandomMash";
        }

        return "";
    }
}