using Godot;
using System;
using System.Collections.Generic;

public class Kick : Slash
{
	public override void _Ready()
	{
		base._Ready();
		AddGatling(new char[] { 's', 'p' }, "Slash");
		AddGatling(new char[] { 'k', 'p' }, () => owner.CheckHeldKey('2'), "CrouchB");
		AddGatling(new char[] { 's', 'p' }, () => owner.CheckHeldKey('2'), "CrouchC");
		AddGatling(new char[] { 'b', 'p' }, "Slash");
		AddKara(new char[] { 's', 'p' }, () => owner.CanGrab(), "GrabStart");
	}

    public override void Enter()
    {
        base.Enter();
		if ((owner.CheckHeldKey('6') && owner.facingRight) || (owner.CheckHeldKey('4') && !owner.facingRight))
			EmitSignal(nameof(StateFinished), "6K");
	}
}

