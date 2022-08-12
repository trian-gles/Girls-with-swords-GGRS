using Godot;
using System;
using System.Collections.Generic;

class BaseManager : Node2D
{

	protected bool currGameFinished;

	/// <summary>
	/// Only modified for GGRS, but kept here anyways.
	/// </summary>
	protected bool hosting;


	/// <summary>
	/// All used for playback of inputs
	/// </summary>
	protected List<int> recordedInputs = new List<int>();
	protected bool recordingInputs = false;
	protected bool playbackInputs = false;
	protected int inputHead = 0;

	protected bool usesHUDNode;
	protected Control HUDNode;

	[Export]
	protected PackedScene packedGameScene;
	[Export]
	protected PackedScene packedCharSelectScene;

	protected GameScene gameScene;
	protected CharSelectScene charSelectScene;

	[Signal]
	public delegate void Finished(string nextGame);

	protected BaseGame currGame;

	

	PackedScene playerOne, playerTwo;
	int colorOne, colorTwo;
	protected int frame = 0;

	public override void _Ready()
	{
		charSelectScene = packedCharSelectScene.Instance() as CharSelectScene;
		AddChild(charSelectScene);
		charSelectScene.Connect("CharacterSelected", this, nameof(OnCharactersSelected));
		currGame = charSelectScene;
		
		gameScene = packedGameScene.Instance() as GameScene;
		gameScene.Connect("RoundFinished", this, nameof(OnRoundFinished));
		gameScene.Connect("ComboFinished", this, nameof(OnComboFinished));
		AddChild(gameScene);



		charSelectScene.ChangeHUDText("");
		gameScene.ChangeHUDText("");

	//gameScene.Visible = false;



}

	protected virtual void ChangeGame()
	{
		
	}


	// ----------------
	// Signal Receptors
	// ----------------
	public virtual void OnGameFinished(string nextGameName)
	{
		GD.Print($"Scene finished, moving to {nextGameName}");
		if (currGame.Name == "CharSelectScreen") // I HATE THE WAY THIS LOOKS
		{
			
			currGame = gameScene;
			MoveChild(charSelectScene, 0);

			gameScene.config(playerOne, playerTwo, colorOne, colorTwo, hosting, frame);
			charSelectScene.HideAll();
		}
		else
		{
			gameScene.ResetAll();
		}
			
	}

	/// <summary>
	/// Eventually this should handle keeping score
	/// </summary>
	/// <param name="winner"></param>
	public virtual void OnRoundFinished(string winner)
	{
		
	}

	public virtual void OnCharactersSelected(PackedScene playerOne, PackedScene playerTwo, int colorOne, int colorTwo)
	{
		this.playerOne = playerOne;
		this.playerTwo = playerTwo;
		this.colorOne = colorOne;
		this.colorTwo = colorTwo;
	}

	public virtual void OnQuit()
	{
		QueueFree();
	}

	public virtual void OnComboFinished(string player)
	{
	}

	protected int GetInputs(string end)
	{
		int inputs = 0;
		if (Input.IsActionPressed("8" + end))
		{
			inputs |= 1;
		}

		if (Input.IsActionPressed("2" + end))
		{
			inputs |= 2;
		}

		if (Input.IsActionPressed("6" + end))
		{
			inputs |= 4;
		}

		if (Input.IsActionPressed("4" + end))
		{
			inputs |= 8;
		}

		if (Input.IsActionPressed("p" + end))
		{
			inputs |= 16;
		}

		if (Input.IsActionPressed("k" + end))
		{
			inputs |= 32;
		}

		if (Input.IsActionPressed("s" + end))
		{
			inputs |= 64;
		}

		return inputs;
	}
}
