using Godot;
using System;

public class JumpB : AirNormal
{
	public override void _Ready()
	{
		base._Ready();
		AddAirCommandNormals(owner.airCommandNormals);
		AddGatling(new char[] { 's', 'p' }, "JumpC");
		AddKara(new char[] { 's', 'p' }, "AirGrabStart");
	}
}

