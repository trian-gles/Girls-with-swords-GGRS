using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Godot;

public class RandomMash : BehaviourState
{

    private Random random = new Random();

    int distance = 10000;

    public override int Poll(GameStateObjectRedesign.GameState state)
    {
        distance = Math.Abs(state.P1State.position[0] - state.P2State.position[0]);
        if (state.P2State.grounded)
        {
            if (distance < 2000 && state.P1State.grounded)
            {
                if (state.P1State.currentState == "Knockdown")
                    return 1;
                if (owner.lastInp != 32 + 64)
                    {
                        return 32 + 64;
                    }
                    else
                        return 0;
            }
            else if (distance < 4000)
            {

                if (state.P1State.grounded)
                {
                    if (owner.lastInp != 18)
                        return 2 + 16;
                    else
                        return 0;
                }
                else
                {

                    if ((owner.lastInp & 16) == 0)
                    {
                        return 16 + GetForwardInput(state);
                    }
                    else
                        return 0;
                }

            }
            else if (distance < 8000)
                if (state.P1State.grounded)
                {
                    if ((owner.lastInp & 32) == 0)
                    {
                        if (state.P1State.position[0] % 3 == 0)
                            return 32;
                        else if (state.P1State.position[0] % 3 == 1)
                            return 2 + 32;
                        else
                            return 32 + GetForwardInput(state);
                    }
                    else
                    {
                        return 0;
                    }
                        
                }
                else
                {
                    if ((owner.lastInp & 64) == 0)
                    {

                        return 64 + GetForwardInput(state);
                    }
                    else
                        return 0;
                }
            else
            {
                return 0;
            }
        }
        else
        {
            GD.Print("RANDUM");
            return random.Next(511);
        }
            
    }

    public override string GetNextState(GameStateObjectRedesign.GameState state)
    {
        if (AIBehaviour.groundHitConfirmStates.Contains(state.P1State.currentState))
            return "Combo";
        else if (AIBehaviour.mixupConfirmStates.Contains(state.P1State.currentState))
            return "Mixup";

        if (Math.Abs(state.P1State.position[0] - state.P2State.position[0]) > 2000)
        {
            if (random.Next(20) == 1)
                return "Chase";
        }

        if (Math.Abs(state.P1State.position[0] - state.P2State.position[0]) > 8000)
        {
            var rand = random.Next(4);
            if (rand == 1)
                return "Zone";
            else if (rand == 2)
                return "Chase";
        }

        return base.GetNextState(state);
    }
}