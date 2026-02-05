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
	/// Secondary buffer
	/// </summary>
	protected List<int> recordedInputs2 = new List<int>();

	protected bool recordingInputs2 = false;
	protected bool playbackInputs2 = false;
	protected int inputHead2 = 0;

	// Need to debug:
	//
	protected string matchFilename = "20261251157";
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

	[Export]
	protected PackedScene popupText;	

	protected GameScene gameScene;
	protected CharSelectScene charSelectScene;
	protected WinScene winScene;

	[Signal]
	public delegate void Finished(string nextGame);

	protected BaseGame currGame;

	[Export]
	public bool p1ChooseP2Char = false;
	protected bool p1KeyReleased = false;
	protected bool p2KeyReleased = false;
	protected int lastP1Key = 0; // this funny logic relates to allowing the P1 key to be released before choosing p2
	protected int lastP2Key = 0; // P2 key to be released before choosing the stage

	

	protected int playerOne, playerTwo;
	protected int colorOne, colorTwo;
	protected int bkgIndex;

	[Serializable]
	public unsafe struct CombinedInputs
	{
		public int p1Inps;
		public int p2Inps;

		public int GetInputs(int num)
		{
			if (num == 0)
				return p1Inps;
			else
				return p2Inps;
		}

		public void SetInputs(int p1, int p2)
		{
			p1Inps = p1;
			p2Inps = p2;
		}
	}

	public override void _Ready()
	{
		// Careful, GGRSManager replaces this
		Globals.ClearSignals();
		CreateGamescenes();

		ClearHUDText();
		
		Globals.frame = 0;

		
		if (matchFilename != "")
		{
			bkgIndex = 0;
			LoadMatchFile();
			OnNewGame();
		}
	//gameScene.Visible = false;



	}

	protected void ClearHUDText()
	{
		charSelectScene.ChangeHUDText("");
		gameScene.ChangeHUDText("");
	}

	protected void CreateGamescenes()
	{
		charSelectScene = packedCharSelectScene.Instance() as CharSelectScene;
		charSelectScene.manager = this;
		AddChild(charSelectScene);
		currGame = charSelectScene;

		gameScene = packedGameScene.Instance() as GameScene;
		gameScene.Connect("GameWon", this, nameof(OnGameWon));
		gameScene.Connect("ComboFinished", this, nameof(OnComboFinished));
		gameScene.manager = this;
		AddChild(gameScene);

		winScene = packedWinScene.Instance() as WinScene;
		winScene.Connect("Rematch", this, nameof(OnRematch));
		winScene.Connect("ReselectChar", this, nameof(OnReselectChar));
		winScene.manager = this;
		AddChild(winScene);
	}

	protected virtual void ChangeGame()
	{
		
	}


	// ----------------
	// Signal Receptors
	// ----------------
	public virtual void OnNewGame()
	{
			
		currGame = gameScene;
		MoveChild(charSelectScene, 0);
		gameScene.config(playerOne, playerTwo, colorOne, colorTwo, hosting, Globals.frame, bkgIndex);
		charSelectScene.HideAll();
		charSelectScene.Reset();
			
	}

	protected Tuple<int, int> GetCharSelectSceneP1Inputs()
	{
		int p1Inputs = 0;
		int p2Inputs = 0;
		if (charSelectScene.p1Selected)
		{

			if (charSelectScene.p2Selected)
			{
				if (p2KeyReleased)
				{
					p1Inputs = GetInputs(0);
				}
				else
				{
					p2KeyReleased = (GetInputs(0) != lastP2Key);
				}

			}
			else if (p1KeyReleased)
			{
				p2Inputs = GetInputs(0);
				lastP2Key = p2Inputs;

			}
			else
			{
				p1KeyReleased = (GetInputs(0) != lastP1Key);
			}
		}


		else
		{
			p1Inputs = GetInputs(0);
			lastP1Key = p1Inputs;
		}
		return new Tuple<int, int>(p1Inputs, p2Inputs);
	}

	/// <summary>
	/// Eventually this should handle keeping score
	/// </summary>
	/// <param name="winner"></param>
	public virtual void OnGameWon(string winner, int chosenCharacter)
	{
		winScene.Config(winner, chosenCharacter);
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
		p2KeyReleased = false;
		p1KeyReleased = false;
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

	protected int GetInputs(int player)
	{
		int inputs = 0;
		if (player == 0)
		{
			if (Input.IsActionPressed(Globals.P1UPACTION))
			{
				inputs |= 1;
			}

			if (Input.IsActionPressed(Globals.P1DOWNACTION))
			{
				inputs |= 2;
			}

			if (Input.IsActionPressed(Globals.P1RIGHTACTION) && !Input.IsActionPressed(Globals.P1LEFTACTION))
			{
				inputs |= 4;
			}

			if (Input.IsActionPressed(Globals.P1LEFTACTION) && !Input.IsActionPressed(Globals.P1RIGHTACTION))
			{
				inputs |= 8;
			}

			if (Input.IsActionPressed(Globals.P1PUNCHACTION))
			{
				inputs |= 16;
			}

			if (Input.IsActionPressed(Globals.P1KICKACTION))
			{
				inputs |= 32;
			}

			if (Input.IsActionPressed(Globals.P1SLASHACTION))
			{
				inputs |= 64;
			}

			if (Input.IsActionPressed(Globals.P1SPECIALACTION))
			{
				inputs |= 128;
			}

			if (Input.IsActionPressed(Globals.P1STRINGACTION))
			{
				inputs |= 256;
			}

			if (Input.IsActionPressed(Globals.P1DASHACTION))
			{
				inputs |= 512;
			}
		}
		else
		{
			if (Input.IsActionPressed(Globals.P2UPACTION))
			{
				inputs |= 1;
			}

			if (Input.IsActionPressed(Globals.P2DOWNACTION))
			{
				inputs |= 2;
			}

			if (Input.IsActionPressed(Globals.P2RIGHTACTION) && !Input.IsActionPressed(Globals.P2LEFTACTION))
			{
				inputs |= 4;
			}

			if (Input.IsActionPressed(Globals.P2LEFTACTION) && !Input.IsActionPressed(Globals.P2RIGHTACTION))
			{
				inputs |= 8;
			}

			if (Input.IsActionPressed(Globals.P2PUNCHACTION))
			{
				inputs |= 16;
			}

			if (Input.IsActionPressed(Globals.P2KICKACTION))
			{
				inputs |= 32;
			}

			if (Input.IsActionPressed(Globals.P2SLASHACTION))
			{
				inputs |= 64;
			}

			if (Input.IsActionPressed(Globals.P2SPECIALACTION))
			{
				inputs |= 128;
			}

			if (Input.IsActionPressed(Globals.P2STRINGACTION))
			{
				inputs |= 256;
			}

			if (Input.IsActionPressed(Globals.P2DASHACTION))
			{
				inputs |= 512;
			}
		}
		

		return inputs;
	}

	protected void Popup(string text)
	{
		
		var popup = popupText.Instance();
		AddChild(popup);
		popup.Call("set_text", text);
	}


	////////
	// TRAINING AND SYNCTEST
	//////// 
	protected virtual void StartInputRecord()
	{
		inputHead = 0;
		recordedInputs.Clear();
		recordingInputs = true;
		gameScene.SetRecordingText("REC");
	}

	protected virtual void StopInputRecord()
	{
		recordingInputs = false;
		gameScene.SetRecordingText("");
	}

	protected void StartInputPlayback(int num = 1)
	{
		if (num == 1)
		{
			inputHead = 0;
			playbackInputs = true;
			gameScene.SetRecordingText("PLAY");
		}
		else
		{
			inputHead2 = 0;
			playbackInputs2 = true;
		}
		
	}

	protected virtual void StopInputPlayback(int num = 1)
	{
		if (num == 1)
		{
			playbackInputs = false;
			gameScene.SetRecordingText("");
		}
		else
		{
			playbackInputs2 = false;
		}
		
	}

	protected virtual void HandleSpecialInputs(InputEvent @event)
	{
		if (@event.IsActionPressed("switch"))
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
		else if (@event.IsActionPressed("reset"))
		{
			gameScene.ResetTraining();
		}
		else if (@event.IsActionPressed("record"))
		{
			if (playbackInputs)
				StopInputPlayback();
			if (recordingInputs)
				StopInputRecord();
			else
				StartInputRecord();
		}
		else if (@event.IsActionPressed("playback"))
		{
			if (recordingInputs)
				StopInputRecord();
			if (playbackInputs)
				StopInputPlayback();
			else
				StartInputPlayback();
		}
		else if (@event.IsActionPressed("save_recording"))
		{
			if (recordingInputs)
				StopInputRecord();
				SaveRecording();
		}
		
	}

	////
	// SAVING RECORDING
	////
	protected virtual void SaveRecording()
	{
		var dict = OS.GetDatetime();
		string filename = "";

		foreach (var key in new[] {"year", "month", "day", "hour", "minute" })
		{
			filename += dict[key].ToString();
		}
		var file = new File();
		file.Open($"user://recordings/recorded_inputs_{filename}.json", Godot.File.ModeFlags.Write);
		file.StoreString(JSON.Print(recordedInputs));
		file.Close();
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
		if (gameFrame < 0 || gameFrame > matchInputs.Count)
		{
			return new[] { 0, 0 };
		}

		var p1Inputs = (int)(float)matchInputs[gameFrame - 2];
		var p2Inputs = (int)(float)matchInputs[gameFrame - 1];
		return new int[] { p1Inputs, p2Inputs};
	}

	public HashSet<Globals.Tags> GetP1Tags()
	{
		return gameScene.GetP1Tags();
	}

	public HashSet<Globals.Tags> GetP2Tags()
	{
		return gameScene.GetP2Tags();
	}
}
