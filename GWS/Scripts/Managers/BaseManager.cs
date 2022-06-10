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

	[Export]
	public Dictionary<string, PackedScene> gameMapping = new Dictionary<string, PackedScene>();

	[Signal]
	public delegate void Finished(string nextGame);

	protected BaseGame currGame;

	PackedScene playerOne, playerTwo;
	int colorOne, colorTwo;
	protected int frame = 0;

	public override void _Ready()
	{
		GD.Print(gameMapping["CharacterSelect"]);
		currGame = gameMapping["CharacterSelect"].Instance() as BaseGame;
		AddChild(currGame);
		currGame.Connect("Finished", this, nameof(OnGameFinished));
		currGame.Connect("CharacterSelected", this, nameof(OnCharactersSelected));
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
		currGame.QueueFree();
		var nextGame = gameMapping[nextGameName];
		currGame = nextGame.Instance() as BaseGame;
		AddChild(currGame);
		currGame.Connect("Finished", this, nameof(OnGameFinished));
		if (currGame.Name == "GameScene") // I HATE THE WAY THIS LOOKS
			((GameScene)currGame).config(playerOne, playerTwo, colorOne, colorTwo, hosting, frame);
	}

	public virtual void OnCharactersSelected(PackedScene playerOne, PackedScene playerTwo, int colorOne, int colorTwo)
	{
		this.playerOne = playerOne;
		this.playerTwo = playerTwo;
		this.colorOne = colorOne;
		this.colorTwo = colorTwo;
		GD.Print("Characters selected");
	}

	public virtual void OnQuit()
	{
		QueueFree();
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
