using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

public class Zone : BehaviourState
{

    private Random random = new Random();

    private bool advancing = false;
    int distance = 10000;

    int forward;
    int backward;

    public override void Enter()
    {
        base.Enter();
        advancing = (random.Next() % 2) == 0;
    }

    public override int Poll(GameStateObjectRedesign.GameState state)
    {
        int specialChance = 2;
        if (Globals.aiDifficulty == Globals.AIDIFFICULTY.LO)
            specialChance = 6;
        distance = state.P1State.positionx - state.P2State.positionx;
        ChooseDirection(distance);
        int action = 0;
        if (random.Next(2) == 1) {
            int option = random.Next(3);
            if (option == 1 && (!advancing)) // only back up if not advancing
                action |= backward;
            else if (option == 2)
                action |= forward;

            if (random.Next(specialChance) == 1)
            {
                action |= DoButtonPress(Globals.SPECIAL);
                if (random.Next(2) == 1 && (Math.Abs(distance) < 12000))
                    action |= 2; // sometimes crouch
            }
                
        }
            

        


        return action;
    }

    public override string GetNextState(GameStateObjectRedesign.GameState state)
    {
        if (Math.Abs(distance) < 7000)
        {
            return "RandomMash";
        }

        return "";
    }

    private void ChooseDirection(int distance)
    {
        if (distance < 0)
        {
            forward = 8;
            backward = 4;
        }

        else
        {
            forward = 4;
            backward = 8;
        }
    }
}