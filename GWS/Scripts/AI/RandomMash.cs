using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Godot;

public class RandomMash : BehaviourState
{
// TODO FIX INCESSANT GRABBING
    private Random random = new Random();

    int distance = 10000;

    public override int Poll(GameStateObjectRedesign.GameState state)
    {
        distance = Math.Abs(state.P1State.positionx - state.P2State.positionx);
        if (state.P2State.grounded)
        {
            if (distance < 1500 && state.P1State.grounded)
            {
                if (owner.p1Tags.Contains(Globals.Tags.knockdown))
                    return 1;
                else if (!owner.p2Tags.Contains(Globals.Tags.run))
                    return DoGrab();
                else
                {
                    return 0;
                }
                    
            }
            else if (distance < 3000)
            {

                if (state.P1State.grounded)
                {
                    return 2 + DoButtonPress(Globals.PUNCH);
                }
                else
                {
                    return DoButtonPress(Globals.PUNCH) + GetForwardInput(state);
                }

            }
            else if (distance < 5000)
            {
                if (state.P1State.grounded)
                {
                    int kickInp = DoButtonPress(Globals.KICK);
                    if (state.P1State.positionx % 3 == 0)
                        return kickInp;
                    else if (state.P1State.positionx % 3 == 1)
                        return 2 + kickInp;
                    else
                        return kickInp + GetForwardInput(state);
                        
                }
                else
                {
                    return DoButtonPress(Globals.SLASH) + GetForwardInput(state);
                }
               
            }
            else if (distance < 7000)
            {
                return 2 + DoButtonPress(Globals.SLASH);
            }
            else if (distance < 8000)
            {
                return DoButtonPress(Globals.SLASH);
            }
            else
            {
                return random.Next(511);
            }
        }
            
        
        else
        {
            return random.Next(511);
        }
            
    }

    public override string GetNextState(GameStateObjectRedesign.GameState state)
    {
        if (owner.CheckGroundHitConfirm(state))
            return "Combo";
        else if (owner.CheckMixupConfirm(state))
            return "Mixup";

        if (Math.Abs(state.P1State.positionx - state.P2State.positionx) > 2000)
        {
            if (random.Next(90) == 1)
                return "Chase";
        }

        if (Math.Abs(state.P1State.positionx - state.P2State.positionx) > 8000)
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