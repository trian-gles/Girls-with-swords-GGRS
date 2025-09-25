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

    public override int Poll(GameStateObjectRedesign.GameState state)
    {
        distance = state.P1State.position[0] - state.P2State.position[0];
        int action = 0;
        if (random.Next(2) == 1) {
            int option = random.Next(3);
            if (option == 1)
                action |= 4;
            else if (option == 2)
                action |= 8;

            if (random.Next(2) == 1)
                action |= 128;
        }
            

        


        return action;
    }

    public override string GetNextState(GameStateObjectRedesign.GameState state)
    {
        if (Math.Abs(distance) < 8000)
        {
            return "RandomMash";
        }

        return "";
    }
}