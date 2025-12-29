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
        if (state.P2State.meter > 50)
            if (state.P2State.currentState == "Idle")
                return DoButtonPress(Globals.SLASH + Globals.SPECIAL) + GetForwardInput(state);
            else
                return 0;
        return DoButtonPress(Globals.PUNCH);
    }

    public override string GetNextState(GameStateObjectRedesign.GameState state)
    {
        if (AIBehaviour.groundHitConfirmStates.Contains(state.P1State.currentState))
        {
            return "Combo";
        }
        if (AIBehaviour.mixupConfirmStates.Contains(state.P1State.currentState))
        {
            return "Mixup";
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