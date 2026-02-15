using Godot;
using System;
using System.Collections.Generic;

public class LocalManager : BaseManager
{

	public override void Start()
	{
		base.Start();
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
