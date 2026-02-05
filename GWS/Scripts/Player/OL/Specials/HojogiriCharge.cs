using Godot;
using System;

public class HojogiriCharge : GroundAttack // used only to inherit counter hit
{
    private const string CommandRunChargedString = "CommandRunCharged";
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
        owner.ChangeState(CommandRunChargedString);
    }
}
