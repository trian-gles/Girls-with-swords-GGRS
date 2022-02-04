using Godot;
using System;

public class OL : Player
{
	public override void _Ready()
	{
		base._Ready();
		charName = "OL";
		AddAltState("CrouchB");
		AddAltState("Walk");
		AddAltState("Slash");
		AddAltState("Crouch");
		AddAltState("Idle");
	}
}
