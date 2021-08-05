using Godot;
using System;

public class Walk : State
{
    public override void _Ready()
    {
        base._Ready();
        loop = true;
    }
    public override void Enter()
    {
        base.Enter();
        if (owner.CheckHeldKey('8'))
        {
            EmitSignal(nameof(StateFinished), "MovingJump");
        }
    }

    public override void HandleInput(char[] inputArr)
    {
        if ((inputArr[0] == '6' || inputArr[0] == '4') && inputArr[1] == 'r')
        {
            EmitSignal(nameof(StateFinished), "Idle");
        }
        else if (Globals.CheckKeyPress(inputArr, '8'))
        {
            EmitSignal(nameof(StateFinished), "MovingJump");
        }
        else if (Globals.CheckKeyPress(inputArr, 'k'))
        {
            owner.velocity.x = 0;
            EmitSignal(nameof(StateFinished), "Kick");
        }
        else if (Globals.CheckKeyPress(inputArr, '2'))
        {
            EmitSignal(nameof(StateFinished), "Crouch");
        }
    }

    public override void FrameAdvance()
    {
        base.FrameAdvance();
        owner.CheckTurnAround();
    }

    public override void PushMovement(float _xVel)
    {
    }
}

