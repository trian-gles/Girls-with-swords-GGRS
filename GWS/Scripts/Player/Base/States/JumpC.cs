using Godot;
using System;

public class JumpC : AirNormal
{

	public override void _Ready()
	{
		base._Ready();
		AddAirCommandNormals(owner.airCommandNormals);
		AddKara(new char[] { 'k', 'p' }, "AirGrabStart");
	}
}
