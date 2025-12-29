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
    private int jabFollowCrouch = 0;
    private int kickFollowCrouch = 0;

    public override void Enter()
    {
        base.Enter();
        kickFollowChoice = (random.Next() % 3) == 0 ? Globals.KICK : Globals.SLASH;
        jabFollowChoice = (random.Next() % 3) == 0 ? Globals.KICK : Globals.SLASH;
        jabFollowCrouch = (random.Next() % 3) == 0 ? 0 : 2;
        kickFollowCrouch = (random.Next() % 3) == 0 ? 0 : 2;
    }
    public override int Poll(GameStateObjectRedesign.GameState state)
    {
        
        if (state.P2State.currentState == "Jab" || state.P2State.currentState == "CrouchA")
        {
            return DoButtonPress(jabFollowChoice) + GetForwardInput(state) + jabFollowCrouch;  
        }

        if (state.P2State.currentState == "Kick" || state.P2State.currentState == "CrouchB")
        {
            
            if (kickFollowChoice == Globals.KICK)
            {
                return DoButtonPress(Globals.KICK)+ GetForwardInput(state);      
            }
            else if (kickFollowChoice == Globals.STRING)
            {
                return DoButtonPress(Globals.SLASH) + kickFollowCrouch;
            }
        }

        return DoButtonPress(Globals.STRING);
    }

    public override string GetNextState(GameStateObjectRedesign.GameState state)
    {
        if (AIBehaviour.mixupConfirmStates.Contains(state.P1State.currentState))
        {
            if (random.Next(90) == 1)
                return "Chase";
            else
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