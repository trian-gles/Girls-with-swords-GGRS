using Godot;
using System;
using System.Collections.Generic;

public class Kick : GroundAttack
{
	private const string CrouchBString = "CrouchB";
	private const string CrouchCString = "CrouchC";
	private const string SlashString = "Slash";
	private const string GrabStartString = "GrabStart";
	private const string ShieldString = "Shield";
	private const string SixKString = "6K";
	public override void _Ready()
	{
		base._Ready();
		tags.Add(Globals.Tags.kick);
		AddCommandNormals(owner.commandNormals);
		AddGatling(new char[] { 'k', 'p' }, () => owner.CheckHeldKey('2'), CrouchBString);
		AddGatling(new char[] { 's', 'p' }, () => owner.CheckHeldKey('2'), CrouchCString);
		AddGatling(new char[] { 's', 'p' }, SlashString);
		AddGatling(new char[] { 'b', 'p' }, SlashString);
		AddExSpecials(owner.groundExSpecials);
		AddSpecials(owner.groundSpecials);
		AddEasyGroundSpecials();
		
		AddKara(new char[] { 's', 'p' }, () => owner.CanGrab(), GrabStartString);

		AddKara(new char[] { 'p', 'p' }, () => owner.CanShield(), ShieldString);
    }

    public override void Enter()
    {
        base.Enter();
		if ((owner.CheckHeldKey('6') && owner.facingRight) || (owner.CheckHeldKey('4') && !owner.facingRight))
			owner.ChangeState(SixKString);
	}
}

