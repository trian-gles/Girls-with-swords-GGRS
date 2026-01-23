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
        return DoButtonPress(Globals.STRING);
    }

    public override string GetNextState(GameStateObjectRedesign.GameState state)
    {
        if (!AIBehaviour.groundHitConfirmStates.Contains(AIBehaviour.GetP1CurrentState(state)))
        {
            return "Oki";
        }
        else
        {
            return "";
        }
    }

}