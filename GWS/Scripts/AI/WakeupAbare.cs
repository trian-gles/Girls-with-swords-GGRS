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
            if (owner.p1Tags.Contains(Globals.Tags.idle))
                return DoButtonPress(Globals.SLASH + Globals.SPECIAL) + GetForwardInput(state);
            else
                return 0;
        return DoButtonPress(Globals.PUNCH);
    }

    public override string GetNextState(GameStateObjectRedesign.GameState state)
    {
        if (owner.CheckGroundHitConfirm(state))
        {
            return "Combo";
        }
        if (owner.CheckMixupConfirm(state))
        {
            return "Mixup";
        }
        else if (!owner.p2Tags.Contains(Globals.Tags.knockdown) && state.P2State.frameCount > 6)
        {
            if (random.Next(2) == 1)
                return "Zone";
            else
                return "Chase";
        }

        return "";
    }

}