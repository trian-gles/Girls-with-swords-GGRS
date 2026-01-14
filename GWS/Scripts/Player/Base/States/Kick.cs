using Godot;
using System;
using System.Collections.Generic;

public class Kick : GroundAttack
{
	public override void _Ready()
	{
		base._Ready();
		AddCommandNormals(owner.commandNormals);
		
		AddGatling(new char[] { 'k', 'p' }, () => owner.CheckHeldKey('2'), "CrouchB");
		AddGatling(new char[] { 's', 'p' }, () => owner.CheckHeldKey('2'), "CrouchC");
		AddGatling(new char[] { 's', 'p' }, "Slash");
		AddGatling(new char[] { 'b', 'p' }, "Slash");
		AddExSpecials(owner.groundExSpecials);
		AddSpecials(owner.groundSpecials);
		AddEasyGroundSpecials();
		
		AddKara(new char[] { 's', 'p' }, () => owner.CanGrab(), "GrabStart");

        AddKara(new char[] { 'p', 'p' }, () => owner.CanShield(), "Shield");

        AddBurstKara('p', 'a');
    }

    public override void Enter()
    {
        base.Enter();
		if ((owner.CheckHeldKey('6') && owner.facingRight) || (owner.CheckHeldKey('4') && !owner.facingRight))
			owner.ChangeState("6K");
	}
}

