using Godot;
using System;

public class Block : State
{
    public override void FrameAdvance()
    {
        stunRemaining--;
        if (stunRemaining == 0)
        {
            EmitSignal(nameof(StateFinished), "Idle");
        }

    }

    public override void receiveDamage(int dmg)
    {
    }
}

