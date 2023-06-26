using Godot;
using System;

public class ComNorm : GroundAttack
{
	public override void _Ready()
	{
		base._Ready();
		AddSpecials(owner.groundSpecials);
		AddExSpecials(owner.groundExSpecials);
		AddCommandNormals(owner.commandNormals); // FIX THIS
		AddKara(new char[] { 's', 'p' }, () => owner.CanGrab(), "GrabStart");
		AddKara(new char[] { 'k', 'p' }, () => owner.CanGrab(), "GrabStart");
	}
}
