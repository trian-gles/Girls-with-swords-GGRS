using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

/// <summary>
/// Simply mashes jab at 30 HZ
/// </summary>
public class Combo : BehaviourState
{
    

    public override int Poll(GameStateObjectRedesign.GameState state)
    {
        if ((256 & owner.lastInp) == 0)
        {
            return 256;
        }
        else
        {
            return 0;
        }
    }

    public override string GetNextState(GameStateObjectRedesign.GameState state)
    {
        if (!AIBehaviour.groundHitConfirmStates.Contains(state.P1State.currentState))
        {
            return "Oki";
        }
        else
        {
            return "";
        }
    }

}