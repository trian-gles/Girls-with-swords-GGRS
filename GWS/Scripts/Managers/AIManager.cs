using Godot;
using System;
using System.Collections.Generic;

class AIManager : LocalManager
{

	private AIBehaviour ai;

	private bool p1KeyReleased = false;
	private int lastP1Key = 0; // this funny logic relates to allowing the P1 key to be released before choosing p2
	private Random random = new Random();

	public override void _Ready()
	{
		base._Ready();
		Globals.mode = Globals.Mode.CPU;
	}

	public override void OnCharactersSelected(int playerOne, int playerTwo, int colorOne, int colorTwo, int bkgIndex)
	{
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
		else
		{
			if (charSelectScene.p1Selected)
			{
				if (p1KeyReleased)
				{
					p2Inputs = GetInputs("");
				}
				else
				{
					p1KeyReleased = (GetInputs("") != lastP1Key);
				}
			}
				

			else
			{
				p1Inputs = GetInputs("");
				lastP1Key = p1Inputs;
			}
				
		}

		currGame.AdvanceFrame(p1Inputs, p2Inputs);

	}

	public override void OnGameFinished(string nextGameName)
	{
		base.OnGameFinished(nextGameName);
		ai = new AIBehaviour();
	}

	public HashSet<string> GetP1Tags()
	{
		return gameScene.GetP1Tags();
	}

	public HashSet<string> GetP2Tags()
	{
		return gameScene.GetP2Tags();
	}
}
