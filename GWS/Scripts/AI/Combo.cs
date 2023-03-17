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
    protected List<int> inputs;
    protected List<int> timings;
    protected int ptr;
    protected bool finished;

    public override void Enter()
    {
        base.Enter();
        ptr = 0;
        finished = false;
        GD.Print("Starting combo");
    }

    public Combo(List<int> inputs, List<int> timings)
    {
        this.inputs = inputs;
        this.timings = timings;
    }

    public override int Poll(GameStateObjectRedesign.GameState state)
    {
        int inp = 0;
        if (frameCount == timings[ptr])
        {
            inp = inputs[ptr];
            ptr++;
            if (ptr == inputs.Count)
            {
                finished = true;
                GD.Print("Finished with combo");
            }
        }
        GD.Print($"Input : {inp}");
        frameCount++;
        return inp;
    }

    public override string GetNextState(GameStateObjectRedesign.GameState state)
    {
        if (finished)
        {
            return "Abare";
        }
        else
        {
            return "";
        }
    }

}