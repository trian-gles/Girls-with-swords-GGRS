using Godot;
using System;

public class GL : Player
{
	public override void _Ready()
	{
		base._Ready();
		charName = "GL";
		AddAltState("Crouch");
		AddAltState("Walk");
		AddAltState("Slash");
	}
}
