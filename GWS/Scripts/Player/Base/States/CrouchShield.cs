using Godot;
using System;
using System.Collections.Generic;

public class CrouchShield : Shield
{
    private const string CrouchBlockString = "CrouchBlock";
    private const string CrouchString = "Crouch";
    private const string IdleString = "Idle";
    private const string ShieldString = "Shield";
    public override HashSet<Globals.Tags> tags { get; set; } = new HashSet<Globals.Tags>() { Globals.Tags.block, Globals.Tags.crouching };
    public override string animationName { get { return CrouchBlockString;} }

    protected override void ExitShield()
    {
        if (owner.CheckHeldKey('2'))
            owner.ChangeState(CrouchString);
        else
            owner.ChangeState(IdleString);
    }

    protected override void CheckShieldSwitch()
    {
        if (!owner.CheckHeldKey('2'))
            owner.ChangeState(ShieldString);
    }
}
