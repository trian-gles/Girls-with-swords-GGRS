using Godot;
using System;
using System.Collections.Generic;

public class CrouchShield : Shield
{
    public override HashSet<string> tags { get; set; } = new HashSet<string>() { "block", "crouching" };
    public override string animationName { get { return "CrouchBlock"; } }

    protected override void ExitShield()
    {
        if (owner.CheckHeldKey('2'))
            owner.ChangeState("Crouch");
        else
            owner.ChangeState("Idle");
    }

    protected override void CheckShieldSwitch()
    {
        if (!owner.CheckHeldKey('2'))
            owner.ChangeState("Shield");
    }
}
