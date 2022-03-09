using Godot;
using System;
using System.Collections.Generic;

public class Crouch : State
{
    public override void _Ready()
    {
        base._Ready();
        loop = true;
        AddSpecials(owner.groundSpecials);
        AddGatling(new[] { '2', 'r' }, "Idle");
        AddGatling(new[] { 'p', 'p' }, "CrouchA");
        AddGatling(new[] { 'k', 'p' }, "CrouchB");
        AddGatling(new[] { 's', 'p' }, "CrouchC");
        
    }
    public override void Enter()
    {
        base.Enter();
        owner.velocity.x = 0;
        owner.velocity.y = 0;
    }

    public override void FrameAdvance()
    {
        base.FrameAdvance();
        owner.velocity.x = 0;
        owner.CheckTurnAround();
    }
}

