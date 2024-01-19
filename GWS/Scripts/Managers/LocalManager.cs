using Godot;
using System;
using System.Collections.Generic;

class LocalManager : BaseManager
{

	public override void _Ready()
	{
		base._Ready();
		Globals.mode = Globals.Mode.LOCAL;
	}

	public override void _PhysicsProcess(float delta)
	{
		int p1Inputs = GetInputs("");
		int p2Inputs = GetInputs("b");
		Globals.frame++;
		currGame.AdvanceFrame(p1Inputs, p2Inputs);
	}

	public override void OnCharactersSelected(int playerOne, int playerTwo, int colorOne, int colorTwo, int bkgIndex)
	{
		base.OnCharactersSelected(playerOne, playerTwo, colorOne, colorTwo, bkgIndex);
		OnGameFinished("Game");
	}

	public override void OnGameWon(string winner)
	{
		OnGameFinished("Game");
	}
}
