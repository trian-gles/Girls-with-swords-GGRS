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

	void StartInputRecord()
	{
		GD.Print("Recording inputs");
		inputHead = 0;
		recordedInputs.Clear();
		recordingInputs = true;
		gameScene.SetRecordingText("REC");
	}

	void StopInputRecord()
	{
		recordingInputs = false;
		gameScene.SetRecordingText("");
	}

	void StartInputPlayback()
	{
		inputHead = 0;
		playbackInputs = true;
		gameScene.SetRecordingText("PLAY");
	}

	void StopInputPlayback()
	{
		playbackInputs = false;
		gameScene.SetRecordingText("");
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
			gameScene.ResetTraining();
		}
		else if (@event.IsActionPressed("record_inputs"))
		{
			if (playbackInputs)
				StopInputPlayback();
			if (recordingInputs)
				StopInputRecord();
			else
				StartInputRecord();
		}
		else if (@event.IsActionPressed("playback_inputs"))
		{
			if (recordingInputs)
				StopInputRecord();
			if (playbackInputs)
				StopInputPlayback();
			else
				StartInputPlayback();
		}

	}

	public override void OnCharactersSelected(PackedScene playerOne, PackedScene playerTwo, int colorOne, int colorTwo)
	{
		base.OnCharactersSelected(playerOne, playerTwo, colorOne, colorTwo);
		OnGameFinished("Game");
		gameScene.ignoreTime = true;
		gameScene.SetDebugVisibility(true);
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
