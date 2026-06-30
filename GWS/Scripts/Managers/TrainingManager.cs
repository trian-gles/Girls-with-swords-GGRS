using Godot;
using System;
using System.Collections.Generic;

public class TrainingManager : BaseManager
{
	
	private bool inputsOnRecovery = false;

	public override void Start()
	{
		base.Start();
		Globals.autoTech = true;
		charSelectScene.ChangeHUDText("P1");
		gameScene.ChangeHUDText("P1");
		gameScene.recordMatch = false;
		Globals.mode = Globals.Mode.TRAINING;
		
	}

	public override void Quit()
	{
		gameScene.ignoreTime = false;
		base.Quit();
	}

	public override void _PhysicsProcess(float delta)
	{
		int p1Inputs;
		int p2Inputs;
		if (currGame.Name == "CharSelectScreen")
		{
			(p1Inputs, p2Inputs) = GetCharSelectSceneP1Inputs();

		}
		else
		{
			int playerInputs = GetInputs(0);
			int otherInputs = 0;

			if (recordingInputs)
				recordedInputs.Add(playerInputs);
			
			if (recordingInputs2)
				recordedInputs2.Add(playerInputs);


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

			if (playbackInputs2)
			{
				if (inputHead2 < recordedInputs2.Count)
				{
					playerInputs = recordedInputs2[inputHead2];
				}
				else
				{
					StopInputPlayback(2);
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
		}



		Globals.frame++;

		currGame.AdvanceFrame(p1Inputs, p2Inputs);
		if (recordingInputs || playbackInputs)
			inputHead++;

		if (playbackInputs2)
			inputHead2++;
	}

	

	public override void _Input(InputEvent @event)
	{
		if (currGame.Name != "CharSelectScreen")
			HandleSpecialInputs(@event);

	}

	public void OnCharacterRecovery(string name)
	{
		if (inputsOnRecovery && (name == "P1") == (flippedPlayers))
		{
			StartInputPlayback();
		}
			
	}

	public override void OnCharactersSelected(int playerOne, int playerTwo, int colorOne, int colorTwo, int bkgIndex)
	{
		base.OnCharactersSelected(playerOne, playerTwo, colorOne, colorTwo, bkgIndex);
		OnNewGame();
		gameScene.ignoreTime = true;
		gameScene.SetDebugVisibility(true);
		gameScene.ConnectTrainingSignals(this);
		gameScene.SetTrainingControlledPlayer(!flippedPlayers, flippedPlayers);
		gameScene.ResetRound();
	}

	public override void OnGameWon(string winner, int character)
	{
		OnNewGame();
	}

	public override void OnComboFinished(string player)
	{
		string targetPlayer = (player == "P2") ? "P1" : "P2";
		gameScene.ResetHealth(targetPlayer);
	}
}
