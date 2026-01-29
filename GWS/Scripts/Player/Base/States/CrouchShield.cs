using Godot;
using System;
using System.Collections.Generic;

public class CrouchShield : Shield
{
    private string crouchBlockString = "CrouchBlock";
    private string crouchString = "Crouch";
    private string idleString = "Idle";
    private string shieldString = "Shield";
    public override HashSet<Globals.Tags> tags { get; set; } = new HashSet<Globals.Tags>() { Globals.Tags.block, Globals.Tags.crouching };
    public override string animationName { get { return crouchBlockString;} }

    protected override void ExitShield()
    {
        if (owner.CheckHeldKey('2'))
            owner.ChangeState(crouchString);
        else
            owner.ChangeState(idleString);
    }

    protected override void CheckShieldSwitch()
    {
        if (!owner.CheckHeldKey('2'))
            owner.ChangeState(shieldString);
    }
}
