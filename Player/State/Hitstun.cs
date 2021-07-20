using Godot;
using System;

public class HitStun : State
{
    public override void Enter()
    {
        owner.combo++;
        GD.Print($"Combo {owner.combo}");
    }


    public override void FrameAdvance()
    {
        stunRemaining--;
        if (stunRemaining == 0) 
        {
            owner.combo = 0;
            EmitSignal(nameof(StateFinished), "Idle");
        }

    }

    public override void ReceiveHit(bool rightAttack, string height, Vector2 push)
    {
        owner.velocity = push;
        EmitSignal(nameof(StateFinished), "HitStun"); ;
    }
}

