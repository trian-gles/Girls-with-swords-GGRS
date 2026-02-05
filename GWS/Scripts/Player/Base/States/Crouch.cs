using Godot;
using System;
using System.Collections.Generic;

public class Crouch : State
{
    public override HashSet<Globals.Tags> tags { get; set; } = new HashSet<Globals.Tags> { Globals.Tags.idle, Globals.Tags.crouching };
    private const string CrouchShieldString = "CrouchShield";
    private const string IdleString = "Idle";
    private const string CrouchAString = "CrouchA";
    private const string CrouchBString = "CrouchB";
    private const string CrouchCString = "CrouchC";
    public override void _Ready()
    {
        base._Ready();
        loop = true;
        AddSpecials(owner.groundSpecials);
        AddCommandNormals(owner.commandNormals);
        AddExSpecials(owner.groundExSpecials);
        AddEasyGroundSpecials();
        AddGatling(new[] { '2', 'r' }, IdleString);
        AddGatling(new[] { 'p', 'p' }, CrouchAString);
        AddGatling(new[] { 'k', 'p' }, CrouchBString);
        AddGatling(new[] { 's', 'p' }, CrouchCString);
        
    }
    public override void Enter()
    {
        base.Enter();
        owner.velocity.x = 0;
        owner.velocity.y = 0;

        if (owner.CheckFlippableHeldKey('4'))
        {
            if (owner.CheckHeldKey('p') && owner.CheckHeldKey('k'))
                owner.ChangeState(CrouchShieldString);
            return;
        }
    }

    public override void FrameAdvance()
    {
        base.FrameAdvance();
        owner.velocity.x = 0;
        owner.CheckTurnAround();
    }
}

