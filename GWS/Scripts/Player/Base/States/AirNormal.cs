using Godot;
using System;
using System.Collections.Generic;


public abstract class AirNormal : AirAttack
{
    public override void _Ready()
    {
        base._Ready();
        AddSpecials(owner.airSpecials);
    }

    public override void AnimationFinished()
    {
        EmitSignal(nameof(StateFinished), "Fall");
    }

    public override void Enter()
    {
        base.Enter();
        AddJumpCancel();
    }
}
