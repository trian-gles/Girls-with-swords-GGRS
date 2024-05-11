using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

/// <summary>
/// Simply mashes jab at 30 HZ
/// </summary>
public class WakeupAbare : BehaviourState
{

    private Random random = new Random();

    private HashSet<string> groundHitConfirmStates = new HashSet<string>
    {
        "HitStun",
        "Stagger",
        "Block",
    };
    public override int Poll(GameStateObjectRedesign.GameState state)
    {
        if ((16 & owner.lastInp) == 0)
        {
            return 16;
        }
        else
        {
            return 0;
        }
    }

    public override string GetNextState(GameStateObjectRedesign.GameState state)
    {
        if (groundHitConfirmStates.Contains(state.P1State.currentState))
        {
            return "Combo";
        }
        else if (!(state.P2State.currentState == "Knockdown") && state.P2State.frameCount > 6)
        {
            if (random.Next(2) == 1)
                return "Zone";
            else
                return "Chase";
        }

        return "";
    }

}