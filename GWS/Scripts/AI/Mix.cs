using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

/// <summary>
/// Needs to be fixed later
/// </summary>
public class Mixup : BehaviourState
{
    
    private Random random = new Random();

    private int kickFollowChoice = 0;
    private int jabFollowChoice = 0;

    public override void Enter()
    {
        base.Enter();
        kickFollowChoice = (random.Next() % 3) == 0 ? Globals.KICK : Globals.SLASH;
        jabFollowChoice = (random.Next() % 3) == 0 ? Globals.KICK : Globals.SLASH;
    }
    public override int Poll(GameStateObjectRedesign.GameState state)
    {
        
        if (state.P2State.currentState == "Jab" || state.P2State.currentState == "CrouchA")
        {
            if (owner.lastInp != 0)
                return 0;
            int choice = random.Next() % 3;
            if (choice == 0)
            {
                return jabFollowChoice + GetForwardInput(state);        
            }
            else
            {
                return Globals.DOWN + jabFollowChoice;
            }
        }

        if (state.P2State.currentState == "Kick" || state.P2State.currentState == "CrouchB")
        {
            if (owner.lastInp != 0)
                return 0;

            if (kickFollowChoice == Globals.KICK)
            {
                return Globals.KICK + GetForwardInput(state);        
            }
            else if (kickFollowChoice == Globals.STRING)
            {
                return Globals.STRING;
            }
        }

        if ((Globals.STRING & owner.lastInp) == 0)
        {
            return Globals.STRING;
        }
        else
        {
            return 0;
        }
    }

    public override string GetNextState(GameStateObjectRedesign.GameState state)
    {
        if (AIBehaviour.mixupConfirmStates.Contains(state.P1State.currentState))
        {
            return "";
        }
        else if (AIBehaviour.groundHitConfirmStates.Contains(state.P1State.currentState))
        {
            return "Combo";
        }
        else
        {
            return "Oki";
        }
    }

}