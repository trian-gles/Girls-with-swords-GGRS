using Godot;
using System;

public class HojogiriCharge : GroundAttack // used only to inherit counter hit
{
    public override void _Ready()
    {
        base._Ready();
        AddGatling(new char[] { 's', 'r' }, "CommandRun");
        AddGatling(new char[] { 'a', 'r' }, "CommandRun");
    }

    public override void Enter()
    {
        base.Enter();
        hitConnect = true; // used so we can gatling
    }



    public override void AnimationFinished()
    {
        EmitSignal(nameof(StateFinished), "CommandRunCharged");
    }
}
