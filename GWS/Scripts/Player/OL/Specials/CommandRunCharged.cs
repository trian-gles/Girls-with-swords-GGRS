using Godot;
using System;
using System.Collections.Generic;

public class CommandRunCharged : CommandRun
{
	public override void _Ready()
	{
		base._Ready();
		exitState = "HojogiriCharged";
		animationName = "CommandRun";
		
	}
}
