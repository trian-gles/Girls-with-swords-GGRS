using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

/// <summary>
/// Simply mashes jab at 30 HZ
/// </summary>
public class WakeupReversal : BehaviourState
{
    Random random = new Random();
    public override int Poll(GameStateObjectRedesign.GameState state)
    {
        if (state.P2State.currentState == "Knockdown")
        {
            GD.Print("Trying to reversal but knocked down");
            return 0;
        }
        else
        {
            if (state.P2State.frameCount == 0)
                return Globals.SPECIAL + GetForwardInput(state);
            if ((Globals.SPECIAL & owner.lastInp) == 0)
            {
                return Globals.SPECIAL + GetForwardInput(state);
            }
            else
            {
                return GetForwardInput(state);
            }
        }
        
    }

    public override string GetNextState(GameStateObjectRedesign.GameState state)
    {
        if (state.P2State.grounded == false || state.P2State.specialBreakFramesRemaining > 0)
        {
            if (random.Next(2) == 1)
                return "Zone";
            else
                return "Chase";
        }

        return "";
    }

}