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
        owner.velocity.x = 0;
        owner.velocity.y = 0;

        if (owner.CheckHeldKey("right"))
        {
            owner.velocity.x = owner.speed;
            EmitSignal(nameof(StateFinished), "Walk");
        }

        else if (owner.CheckHeldKey("left"))
        {
            owner.velocity.x = -owner.speed;
            EmitSignal(nameof(StateFinished), "Walk");
        }

        else if (owner.CheckHeldKey("up"))
        {
            EmitSignal(nameof(StateFinished), "Jump");
        }
    } 
    public override void HandleInput(string[] inputArr)
    {
        if (inputArr[0] == "right" && inputArr[1] == "press") // Can I make this a bit less confusing to read?
        {
            owner.velocity.x = owner.speed;
            EmitSignal(nameof(StateFinished), "Walk");
        }
        else if (inputArr[0] == "left" && inputArr[1] == "press")
        {
            owner.velocity.x = -owner.speed;
            EmitSignal(nameof(StateFinished), "Walk");
        }

        else if (inputArr[0] == "up" && inputArr[1] == "press")
        {
            EmitSignal(nameof(StateFinished), "Jump");
        }

        else if (inputArr[0] == "k" && inputArr[1] == "press")
        {
            EmitSignal(nameof(StateFinished), "Kick");
        }
    }

    public override void FrameAdvance()
    {
        owner.velocity.x = 0;
        owner.CheckTurnAround();
        if (owner.CheckHeldKey("up"))
        {
            EmitSignal(nameof(StateFinished), "Jump");
        }
    }
}

