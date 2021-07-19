using Godot;
using System;

public class Walk : State
{
    public override void Enter()
    {
        if (owner.CheckHeldKey("up"))
        {
            EmitSignal(nameof(StateFinished), "MovingJump");
        }
    }

    public override void HandleInput(InputEvent @event)
    {
        if (@event.IsActionReleased("right") || @event.IsActionReleased("left"))
        {
            EmitSignal(nameof(StateFinished), "Idle");
        }
        else if (@event.IsActionPressed("up")) 
        {
            EmitSignal(nameof(StateFinished), "MovingJump");
        }
    }
}

