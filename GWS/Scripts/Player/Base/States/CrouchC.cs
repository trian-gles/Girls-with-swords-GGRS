using Godot;
using System;

public class CrouchC : GroundAttack
{
	private const string GrabStartString = "GrabStart";
	public override void _Ready()
	{
		base._Ready();
		tags.Add(Globals.Tags.slash);
		AddSpecials(owner.groundSpecials);
		AddExSpecials(owner.groundExSpecials);
		AddEasyGroundSpecials();
		AddKara(new char[] { 'k', 'p' }, () => owner.CanGrab(), GrabStartString);
	}
}
