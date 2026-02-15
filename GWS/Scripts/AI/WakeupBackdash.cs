using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

/// <summary>
/// Simply mashes jab at 30 HZ
/// </summary>
public class WakeupBackdash : BehaviourState
{

    private Random random = new Random();

    
    public override int Poll(GameStateObjectRedesign.GameState state)
    {
        int bDashInp = state.P2State.facingRight ? 8 : 4;
        return DoButtonPress(bDashInp);
    }

    public override string GetNextState(GameStateObjectRedesign.GameState state)
    {
       if (!owner.p2Tags.Contains(Globals.Tags.knockdown) && state.P2State.frameCount > 6)
        {
            if (random.Next(2) == 1)
                return "Zone";
            else
                return "Chase";
        }

        return "";
    }

}