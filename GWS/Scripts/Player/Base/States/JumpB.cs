using Godot;
using System;

public class JumpB : AirNormal
{
	public override void _Ready()
	{
		base._Ready();
		AddAirCommandNormals(owner.airCommandNormals);
		AddGatling(new char[] { 's', 'p' }, "JumpC");
        AddGatling(new char[] { 'p', 'p' }, () =>owner.internalPos.y < Globals.MAXJPDEPTH, "JumpA");
        AddKara(new char[] { 's', 'p' }, "AirGrabStart");

        AddKara(new char[] { 'p', 'p' }, () => owner.CanShield(), "Shield");
    }
}

