using Godot;
using System;
using System.Collections.Generic;

public class CommandRunCharged : CommandRunBase
{

	public override string GetNextState()
	{
		return "HojogiriCharged";
	}
}
