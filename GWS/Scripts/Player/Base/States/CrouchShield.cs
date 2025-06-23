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
            EmitSignal(nameof(StateFinished), "Crouch");
        else
            EmitSignal(nameof(StateFinished), "Idle");
    }

    protected override void CheckShieldSwitch()
    {
        if (!owner.CheckHeldKey('2'))
            EmitSignal(nameof(StateFinished), "Shield");
    }
}
