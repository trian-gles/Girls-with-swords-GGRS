using Godot;
using System;
using System.Collections.Generic;

public class BaseManager : Node2D
{

	protected bool currGameFinished;

	/// <summary>
	/// Only modified for GGRS, but kept here anyways.
	/// </summary>
	public bool hosting;


	/// <summary>
	/// All used for playback of inputs
	/// </summary>
	protected List<int> recordedInputs = new List<int>();
	protected bool recordingInputs = false;
	protected bool playbackInputs = false;
	protected int inputHead = 0;


	/// <summary>
	/// playing back recorded matches
	/// </summary>
	protected bool playbackMatch = false;
	private string matchFilename = "coolgrab2";
	protected Godot.Collections.Array matchInputs;

	/// <summary>
	/// Shared by training and synctest
	/// </summary>
	protected bool flippedPlayers = false;

	protected bool usesHUDNode;
	protected Control HUDNode;

	[Export]
	protected PackedScene packedGameScene;
	[Export]
	protected PackedScene packedCharSelectScene;

	[Export]
	protected PackedScene packedWinScene;

	protected GameScene gameScene;
	protected CharSelectScene charSelectScene;
	protected WinScene winScene;

	[Signal]
	public delegate void Finished(string nextGame);

	protected BaseGame currGame;

	

	protected int playerOne, playerTwo;
	protected int colorOne, colorTwo;
	protected int bkgIndex;

	public override void _Ready()
	{
		charSelectScene = packedCharSelectScene.Instance() as CharSelectScene;
		AddChild(charSelectScene);
		charSelectScene.Connect("CharacterSelected", this, nameof(OnCharactersSelected));
		currGame = charSelectScene;
		
		gameScene = packedGameScene.Instance() as GameScene;
		gameScene.Connect("GameWon", this, nameof(OnGameWon));
		gameScene.Connect("ComboFinished", this, nameof(OnComboFinished));
		AddChild(gameScene);

		winScene = packedWinScene.Instance() as WinScene;
		winScene.Connect("Rematch", this, nameof(OnRematch));
		winScene.Connect("ReselectChar", this, nameof(OnReselectChar));
		AddChild(winScene);


		charSelectScene.ChangeHUDText("");
		gameScene.ChangeHUDText("");
		Globals.frame = 0;

		
		if (playbackMatch)
		{
			bkgIndex = 0;
			LoadMatchFile();
			OnNewGame();
		}
	//gameScene.Visible = false;



}

	protected virtual void ChangeGame()
	{
		
	}


	// ----------------
	// Signal Receptors
	// ----------------
	public virtual void OnNewGame()
	{


		Globals.Log($"Restarting game");
			
		currGame = gameScene;
		MoveChild(charSelectScene, 0);
		gameScene.config(playerOne, playerTwo, colorOne, colorTwo, hosting, Globals.frame, bkgIndex);
		charSelectScene.HideAll();
		charSelectScene.Reset();
			
	}

	/// <summary>
	/// Eventually this should handle keeping score
	/// </summary>
	/// <param name="winner"></param>
	public virtual void OnGameWon(string winner)
	{
		winScene.Config(winner);
		currGame = winScene;
		MoveChild(winScene, 0);
		
	}

	public virtual void OnCharactersSelected(int playerOne, int playerTwo, int colorOne, int colorTwo, int bkgIndex)
	{
		this.playerOne = playerOne;
		this.playerTwo = playerTwo;
		this.colorOne = colorOne;
		this.colorTwo = colorTwo;
		this.bkgIndex = bkgIndex;
	}

	public virtual void OnQuit()
	{
		QueueFree();
	}

	public virtual void OnComboFinished(string player)
	{
	}

	public virtual void OnRematch()
	{
		currGame = gameScene;
		MoveChild(charSelectScene, 0);
		charSelectScene.HideAll();
		gameScene.config(playerOne, playerTwo, colorOne, colorTwo, hosting, Globals.frame, bkgIndex);
	}

	public virtual void OnReselectChar()
	{
		charSelectScene.Reload();
		currGame = charSelectScene;
		MoveChild(gameScene, 0);
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

		if (Input.IsActionPressed("6" + end) && !Input.IsActionPressed("4" + end))
		{
			inputs |= 4;
		}

		if (Input.IsActionPressed("4" + end) && !Input.IsActionPressed("6" + end))
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

		if (Input.IsActionPressed("a" + end))
		{
			inputs |= 128;
		}

		if (Input.IsActionPressed("b" + end))
		{
			inputs |= 256;
		}

		return inputs;
	}


	////////
	// TRAINING AND SYNCTEST
	//////// 
	protected void StartInputRecord()
	{
		inputHead = 0;
		recordedInputs.Clear();
		recordingInputs = true;
		gameScene.SetRecordingText("REC");
	}

	protected void StopInputRecord()
	{
		recordingInputs = false;
		gameScene.SetRecordingText("");
	}

	protected void StartInputPlayback()
	{
		inputHead = 0;
		playbackInputs = true;
		gameScene.SetRecordingText("PLAY");
	}

	protected void StopInputPlayback()
	{
		playbackInputs = false;
		gameScene.SetRecordingText("");
	}

	protected void HandleSpecialInputs(InputEvent @event)
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
			gameScene.SetTrainingControlledPlayer(!flippedPlayers, flippedPlayers);
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

	////
	// MATCH PLAYBACK
	////

	/// <summary>
	/// 
	/// </summary>
	protected void LoadMatchFile()
	{
		var file = new File();
		file.Open($"user://recordings/{matchFilename}.json", File.ModeFlags.Read); // C:\Users\%NAME%\AppData\Roaming\Godot\app_userdata\GWS-GGPO\recordings
		string txt = file.GetAsText();
		var res = JSON.Parse(txt).Result;
		var dict = (Godot.Collections.Dictionary)res;


		matchInputs = (Godot.Collections.Array)dict["allInputs"];
		playerOne = (int)(float)dict["p1char"];
		playerTwo = (int)(float)dict["p2char"];
		colorOne = (int)(float)dict["p1col"];
		colorTwo = (int)(float)dict["p2col"];

		file.Close();

	}

	protected int[] GetMatchInputs()
	{
		// multidimensional arrays become single dimensional in godot JSON, hence this.
		int gameFrame = ((GameScene)currGame).GetFramesSinceStart() * 2;
		if (gameFrame < 0)
		{
			return new[] { 0, 0 };
		}

		var p1Inputs = (int)(float)matchInputs[gameFrame - 2];
		var p2Inputs = (int)(float)matchInputs[gameFrame - 1];
		return new int[] { p1Inputs, p2Inputs};
	}
}
