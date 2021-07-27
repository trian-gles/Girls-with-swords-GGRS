using Godot;
using System;

public class Idle : State
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

        if (owner.CheckHeldKey('6'))
        {
            owner.velocity.x = owner.speed;
            EmitSignal(nameof(StateFinished), "Walk");
        }

        else if (owner.CheckHeldKey('4'))
        {
            owner.velocity.x = -owner.speed;
            EmitSignal(nameof(StateFinished), "Walk");
        }

        else if (owner.CheckHeldKey('8'))
        {
            EmitSignal(nameof(StateFinished), "Jump");
        }
    } 
    public override void HandleInput(char[] inputArr)
    {
        if (Globals.CheckKeyPress(inputArr, '6'))
        {
            owner.velocity.x = owner.speed;
            EmitSignal(nameof(StateFinished), "Walk");
        }
        else if (Globals.CheckKeyPress(inputArr, '4'))
        {
            owner.velocity.x = -owner.speed;
            EmitSignal(nameof(StateFinished), "Walk");
        }

        else if (Globals.CheckKeyPress(inputArr, '8'))
        {
            EmitSignal(nameof(StateFinished), "Jump");
        }

        else if (Globals.CheckKeyPress(inputArr, 'k'))
        {
            EmitSignal(nameof(StateFinished), "Kick");
        }
    }

    public override void FrameAdvance()
    {
        base.FrameAdvance();
        owner.velocity.x = 0;
        owner.CheckTurnAround();
        if (owner.CheckHeldKey('8'))
        {
            EmitSignal(nameof(StateFinished), "Jump");
        }
    }
}

