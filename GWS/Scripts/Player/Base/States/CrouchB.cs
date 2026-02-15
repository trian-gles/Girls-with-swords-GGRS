using Godot;
using System;
using System.Collections.Generic;

public class CrouchB : GroundAttack
{
	private const string CrouchCString = "CrouchC";
	private const string SlashString = "Slash";
	private const string GrabStartString = "GrabStart";
	private const string CrouchShieldString = "CrouchShield";
	public override void _Ready()
	{
		base._Ready();
		tags.Add(Globals.Tags.kick);
		AddSpecials(owner.groundSpecials);
		AddExSpecials(owner.groundExSpecials);
        AddCommandNormals(owner.commandNormals);
        AddEasyGroundSpecials();
		AddGatling(new char[] { 's', 'p' }, () => owner.CheckHeldKey('2'), CrouchCString);

		AddGatling(new char[] { 's', 'p' }, SlashString);
		AddGatling(new char[] { 'b', 'p' }, SlashString);
		AddKara(new char[] { 's', 'p' }, () => owner.CanGrab(), GrabStartString);

        AddKara(new char[] { 'p', 'p' }, () => owner.CanShield(), CrouchShieldString);
    }
}
