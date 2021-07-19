using Godot;
using System;

public class Walk : State
{
    public override void Enter()
    {
        base.Enter();
    }

    public override void HandleInput(InputEvent @event)
    {
        if (@event.IsActionReleased("right") || @event.IsActionReleased("left"))
        {
            EmitSignal(nameof(StateFinished), "Idle");
        }
    }
}

