using Godot;
using System;
using System.Collections.Generic;

class TrainingManager : BaseManager
{
	private bool flippedPlayers = false;

	public override void _Ready()
	{
		base._Ready();
		charSelectScene.ChangeHUDText("P1");
		gameScene.ChangeHUDText("P1");
	}

	public override void _PhysicsProcess(float delta)
	{
		int p1Inputs;
		int p2Inputs;
		if (flippedPlayers)
		{
			p1Inputs = 0;
			p2Inputs = GetInputs("");
		}
		else
		{
			p1Inputs = GetInputs("");
			p2Inputs = 0;
		}
		
		currGame.AdvanceFrame(p1Inputs, p2Inputs);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("switch_players"))
		{
			flippedPlayers = !flippedPlayers;
			string newText;
			if (flippedPlayers)
				newText = "P2";
			else
				newText = "P1";

			charSelectScene.ChangeHUDText(newText);
			gameScene.ChangeHUDText(newText);
		}
		else if (@event.IsActionPressed("reset_training"))
		{
			gameScene.ResetAll();
		}
			
	}

	public override void OnCharactersSelected(PackedScene playerOne, PackedScene playerTwo, int colorOne, int colorTwo)
	{
		base.OnCharactersSelected(playerOne, playerTwo, colorOne, colorTwo);
		OnGameFinished("Game");
		gameScene.ignoreTime = true;
	}

	public override void OnRoundFinished(string winner)
	{
		OnGameFinished("Game");
	}

	public override void OnComboFinished(string player)
	{
		string targetPlayer = (player == "P2") ? "P1" : "P2";
		gameScene.ResetHealth(targetPlayer);
	}
}
