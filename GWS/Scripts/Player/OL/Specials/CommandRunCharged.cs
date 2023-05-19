using Godot;
using System;
using System.Collections.Generic;

public class CommandRunCharged : CommandRunBase
{
	public override void _Ready()
	{
		base._Ready();
		exitState = "HojogiriCharged";
		
	}
}
