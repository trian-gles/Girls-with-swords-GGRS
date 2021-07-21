using Godot;
using System;

public class Block : State
{
    public override void _Ready()
    {
        base._Ready();
        loop = true;
    }
    public override void FrameAdvance()
    {
        stunRemaining--;
        if (stunRemaining == 0)
        {
            if (owner.grounded)
            {
                EmitSignal(nameof(StateFinished), "Idle");
            }
            else
            {
                EmitSignal(nameof(StateFinished), "Fall");
            }
            
        }
        if (!owner.grounded)
        {
            owner.velocity.y += owner.gravity;
        }
    }

    public override void receiveDamage(int dmg)
    {
    }
}

