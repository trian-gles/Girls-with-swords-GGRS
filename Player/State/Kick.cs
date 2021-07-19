using Godot;
using System;

public class Kick : State
{
    private bool hitConnect = false;

    public override void Enter()
    {
        hitConnect = false;
    }
    public override void AnimationFinished()
    {
        EmitSignal(nameof(StateFinished), "Idle");
    }

    public override void InHurtbox()
    {
        if (!hitConnect) 
        {
            GD.Print("HIT");
            hitConnect = true;
        }
              
    }
}

