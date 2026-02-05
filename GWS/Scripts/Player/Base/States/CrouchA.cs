using Godot;
using System;
using System.Collections.Generic;

public class CrouchA : GroundAttack
{
	private const string CrouchAString = "CrouchA";
	private const string CrouchBString = "CrouchB";
	private const string CrouchCString = "CrouchC";
	private const string JabString = "Jab";
	private const string KickString = "Kick";
	private const string SlashString = "Slash";
	private const string ShieldString = "Shield";
	private const string GrabStartString = "GrabStart";
	public override void _Ready()
	{
		base._Ready();
		AddSpecials(owner.groundSpecials);
		AddExSpecials(owner.groundExSpecials);
		AddCommandNormals(owner.commandNormals);
		AddEasyGroundSpecials();
		whiffGatlings.Add(new NormalGatling { input = new[] { 'p', 'p' }, state = CrouchAString, reqCall = () => owner.CheckHeldKey('2') });
		AddGatling(new char[] { 'p', 'p' }, () => owner.CheckHeldKey('2'), CrouchAString);
		AddGatling(new char[] { 'k', 'p' }, () => owner.CheckHeldKey('2'), CrouchBString);
		AddGatling(new char[] { 's', 'p' }, () => owner.CheckHeldKey('2'), CrouchCString);
		AddGatling(new char[] { 'p', 'p' }, JabString);
		AddGatling(new char[] { 'k', 'p' }, KickString);
		AddGatling(new char[] { 's', 'p' }, SlashString);
		AddGatling(new char[] { 'b', 'p' }, KickString);

		AddKara(new char[] { 'k', 'p' }, () => owner.CanShield(), ShieldString);
	}
}

