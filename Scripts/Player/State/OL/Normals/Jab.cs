using Godot;
using System;

public class OLJab : OLKick
{
	
	public override void _Ready()
	{
		base._Ready();
		AddGatling(new char[] { 'p', 'p' }, "Jab");
		AddGatling(new char[] { 'k', 'p' }, "Kick");
	}

	
}
