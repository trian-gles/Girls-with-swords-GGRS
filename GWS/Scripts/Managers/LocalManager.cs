using Godot;
using System;
using System.Collections.Generic;

class LocalManager : BaseManager
{

	public override void _Ready()
	{
		base._Ready();
		Globals.mode = Globals.Mode.LOCAL;
		Globals.autoTech = false;
	}

	public override void _PhysicsProcess(float delta)
	{
		int p1Inputs = GetInputs(0);
		int p2Inputs = GetInputs(1);
		Globals.frame++;
		currGame.AdvanceFrame(p1Inputs, p2Inputs);
	}

	public override void OnCharactersSelected(int playerOne, int playerTwo, int colorOne, int colorTwo, int bkgIndex)
	{
		base.OnCharactersSelected(playerOne, playerTwo, colorOne, colorTwo, bkgIndex);
		OnRematch();
	}


}
