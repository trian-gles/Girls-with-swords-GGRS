using Godot;
using System;
using System.Collections.Generic;

class AIManager : LocalManager
{

	private AIBehaviour ai;

	
	private Random random = new Random();

	public override void _Ready()
	{
		base._Ready();
		Globals.mode = Globals.Mode.CPU;
	}

	public override void OnCharactersSelected(int playerOne, int playerTwo, int colorOne, int colorTwo, int bkgIndex)
	{
		ai = new AIBehaviour(playerTwo);
		base.OnCharactersSelected(playerOne, playerTwo, colorOne, colorTwo, bkgIndex);
		gameScene.SetP2AI();
	}

	public override void _PhysicsProcess(float delta)
	{
		int p1Inputs = 0; 
		int p2Inputs = 0;
		Globals.frame++;

		if (currGame.Name == "GameScene" && currGame.AcceptingInputs())
		{
			p1Inputs = GetInputs("");
			p2Inputs = ai.Poll(gameScene.GetGameState());
		}
		else if (currGame.Name == "CharSelectScreen")
		{
			(p1Inputs, p2Inputs) = GetCharSelectSceneP1Inputs();
			
		}
		else { p1Inputs = GetInputs(""); }

		currGame.AdvanceFrame(p1Inputs, p2Inputs);

	}

	public override void OnGameWon(string winner, int character)
	{
		base.OnGameWon(winner, character);
		p1KeyReleased = false;
		ai = new AIBehaviour(playerTwo);
	}


}
