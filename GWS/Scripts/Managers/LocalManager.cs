using Godot;
using System;
using System.Collections.Generic;

class LocalManager : BaseManager
{

	public override void _PhysicsProcess(float delta)
	{
		int p1Inputs = GetInputs("");
		int p2Inputs = GetInputs("b");
		currGame.AdvanceFrame(p1Inputs, p2Inputs);
	}

	public override void OnCharactersSelected(PackedScene playerOne, PackedScene playerTwo, int colorOne, int colorTwo)
	{
		base.OnCharactersSelected(playerOne, playerTwo, colorOne, colorTwo);
		OnGameFinished("Game");
	}

	public override void OnRoundFinished(string winner)
	{
		OnGameFinished("Game");
	}
}
