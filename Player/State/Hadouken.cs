using Godot;
using System;

public class Hadouken : State
{
    public override void AnimationFinished()
    {
        EmitSignal(nameof(StateFinished), "Idle");
    }
}
