using Godot;
using System;

public class Crouch : State
{
    public override void _Ready()
    {
        base._Ready();
        loop = true;
    }
    public override void Enter()
    {
        base.Enter();
        owner.gravityPos = 0;
        owner.velocity.x = 0;
        owner.velocity.y = 0;
        
        if (owner.CheckHeldKey('8'))
        {
            EmitSignal(nameof(StateFinished), "Jump");
        }
    } 
    public override void HandleInput(char[] inputArr)
    {
        if (Globals.CheckKeyRelease(inputArr, '2'))
        {
            EmitSignal(nameof(StateFinished), "Idle");
        }

        else if (Globals.CheckKeyPress(inputArr, '8'))
        {
            EmitSignal(nameof(StateFinished), "Jump");
        }
    }

    public override void FrameAdvance()
    {
        base.FrameAdvance();
        owner.velocity.x = 0;
        owner.CheckTurnAround();
    }
}

