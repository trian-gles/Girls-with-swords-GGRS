using Godot;
using System;

public class Idle : State
{
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
    }
}

