using Godot;
using System;

public class Walk : State
{
    public override void HandleInput(InputEvent @event)
    {
        if (@event.IsActionReleased("right") || @event.IsActionReleased("left"))
        {
            owner.velocity.x = 0;
            EmitSignal(nameof(StateFinished), "Idle");
        }
    }
}

