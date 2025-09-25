using Godot;
using System;

public class JumpC : AirNormal
{

	public override void _Ready()
	{
		base._Ready();
		AddKara(new char[] { 'k', 'p' }, "AirGrabStart");
	}
}
