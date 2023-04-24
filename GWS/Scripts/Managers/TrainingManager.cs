using Godot;
using System;
using System.Collections.Generic;

public class TrainingManager : BaseManager
{
	
	private bool inputsOnRecovery = false;

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
		int playerInputs = GetInputs("");
		int otherInputs = 0;

		if (recordingInputs)
			recordedInputs.Add(playerInputs);

		if (playbackInputs)
		{
			if (inputHead < recordedInputs.Count)
			{
				otherInputs = recordedInputs[inputHead];
			}
			else
			{
				StopInputPlayback();
			}
		}
			

		if (flippedPlayers)
		{
			p1Inputs = otherInputs;
			p2Inputs = playerInputs;
		}
		else
		{
			p1Inputs = playerInputs;
			p2Inputs = otherInputs;
		}

		gameScene.DisplayInputs(p1Inputs, p2Inputs);
		currGame.AdvanceFrame(p1Inputs, p2Inputs);
		if (recordingInputs || playbackInputs)
			inputHead++;
	}

	

	public override void _Input(InputEvent @event)
	{
		HandleSpecialInputs(@event);

	}

	public void OnCharacterRecovery(string name)
	{
		GD.Print($"{name} recovery!");
		if ((name == "P1") == (flippedPlayers))
		{
			GD.Print($"recovery playback");
			StartInputPlayback();
		}
			
	}

	public override void OnCharactersSelected(PackedScene playerOne, PackedScene playerTwo, int colorOne, int colorTwo, int bkgIndex)
	{
		base.OnCharactersSelected(playerOne, playerTwo, colorOne, colorTwo, bkgIndex);
		OnGameFinished("Game");
		gameScene.ignoreTime = true;
		gameScene.SetDebugVisibility(true);
		gameScene.ConnectTrainingSignals(this);
		gameScene.SetTrainingControlledPlayer(!flippedPlayers, flippedPlayers);
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
