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
        if (owner.CheckHeldKey("up"))
        {
            EmitSignal(nameof(StateFinished), "MovingJump");
        }
    }

    public override void HandleInput(string[] inputArr)
    {
        if ((inputArr[0] == "right" || inputArr[0] == "left") && inputArr[1] == "release")
        {
            EmitSignal(nameof(StateFinished), "Idle");
        }
        else if (Globals.CheckKeyPress(inputArr, "up"))
        {
            EmitSignal(nameof(StateFinished), "MovingJump");
        }
        else if (Globals.CheckKeyPress(inputArr, "k"))
        {
            EmitSignal(nameof(StateFinished), "Kick");
        }
    }

    public override void PushMovement(float _xVel)
    {
    }
}

