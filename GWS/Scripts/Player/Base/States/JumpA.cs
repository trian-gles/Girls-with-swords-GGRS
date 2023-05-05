using Godot;
using System;

public class JumpA : AirNormal
{
	public override void _Ready()
	{
		base._Ready();
		AddAirCommandNormals(owner.airCommandNormals);
		AddGatling(new char[] { 'p', 'p' }, "JumpA");
		AddGatling(new char[] { 'k', 'p' }, "JumpB");
		AddGatling(new char[] { 's', 'p' }, "JumpC");

		
	}
}
