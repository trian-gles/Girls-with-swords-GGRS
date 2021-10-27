using Godot;
using System;

public class Jab : Kick
{
	
	public override void _Ready()
	{
		base._Ready();
		AddGatling(new char[] { 'p', 'p' }, "Jab");
		AddGatling(new char[] { 'k', 'p' }, "Kick");
	}

	
}
