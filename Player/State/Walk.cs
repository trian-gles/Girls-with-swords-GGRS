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
        else if (inputArr[0] == "up" && inputArr[1] == "press")
        {
            EmitSignal(nameof(StateFinished), "MovingJump");
        }
    }
}

