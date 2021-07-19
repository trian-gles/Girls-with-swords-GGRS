using Godot;
using System;

public class Idle : State
{
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
    public override void HandleInput(InputEvent @event)
    {
        if (@event.IsActionPressed("right"))
        {
            owner.velocity.x = owner.speed;
            EmitSignal(nameof(StateFinished), "Walk");
        }
        else if (@event.IsActionPressed("left"))
        {
            owner.velocity.x = -owner.speed;
            EmitSignal(nameof(StateFinished), "Walk");
        }

        else if (@event.IsActionPressed("up")) 
        {
            EmitSignal(nameof(StateFinished), "Jump");
        }
    }

    public override void FrameAdvance()
    {
        owner.velocity.x = 0;
    }
}

