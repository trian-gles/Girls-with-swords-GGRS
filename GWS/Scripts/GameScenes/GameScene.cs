using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;


/// <summary>
/// Collection of constants and static functions
/// </summary>
/// 
public class GameScene : BaseGame
{

	[Export]
	public PackedScene[] charScenes = new PackedScene[0];

	public class Recording
	{
		public int p1char { get; set; }
		public int p2char { get; set; }
		public int p1col { get; set; }
		public int p2col { get; set; }
		public int[,] allInputs { get; set; }
	}

	/// <summary>
	/// Used to prevent physics process
	/// </summary>
	private bool configured = false;

	public Player P1;
	public Player P2;
	private Label P1Combo;
	private Label P2Combo;
	private TextureProgress P1Health;
	private TextureProgress P2Health;
	private Camera2D camera;
	private GameStateObjectRedesign gsObj;
	private Label timer;
	private Label centerText;
	private Label statsText;
	private Node mainMenuReturn;
	private MainGFX mainGFX;
	private CanvasLayer HUD;
	private Label P1Counter;
	private Label P2Counter;
	private Control P1SnailRadar;
	private Control P2SnailRadar;
	public Label P1Rhythm;
	public Label P2Rhythm;
	private Label superText;
	private Control P1Meter;
	private Control P2Meter;
	private AudioStreamPlayer music;

	private Label recordingText;
	private ColorRect recordingBack;


	// TIME HANDLING
	public bool ignoreTime = false;

	/// the frame when we land on this gamescene
	private int readyFrame;

	/// the frame the countdown finishes
	private int startFrame;

	private int timeOutFrame;

	// Stored in gamestate
	public int possibleEndingFrame;
	public TimeStatus currTime;

	private int trueEndingFrame;
	private int exitFrame;
	

	// RECORDING
	public bool recordMatch = true;
	/// will contain alternating inputs [p1, p2, p1, p2, ...] for easy saving
	private int[,] allInputs = new int[7000, 2];
	private bool savedFile = false;
	private int p1Ind;
	private int p2Ind;

	[Signal]
	public delegate void RoundFinished(string winner);

	/// <summary>
	/// Used for training mode, where after a combo health will reset
	/// </summary>
	/// <param name="player"></param>
	[Signal]
	public delegate void ComboFinished(string player);

	public enum TimeStatus
	{
		PREROUND,
		GAME,
		FAKEEND,
		TRUEEND
	}

	private Dictionary<string, PackedScene> characterMap = new Dictionary<string, PackedScene>();

	public override void _Ready()
	{
		HUDText = GetNode<Label>("HUD/DebugText");
		inputText = GetNode<Label>("HUD/InputText");
		inputTextP2 = GetNode<Label>("HUD/InputTextP2");
		P1Counter = GetNode<Label>("HUD/P1Counter");
		P2Counter = GetNode<Label>("HUD/P2Counter");
		P1Rhythm = GetNode<Label>("HUD/P1Rhythm");
		P2Rhythm = GetNode<Label>("HUD/P2Rhythm");
		P1Meter = GetNode<Control>("HUD/P1Meter");
		P2Meter = GetNode<Control>("HUD/P2Meter");
		recordingBack = GetNode<ColorRect>("HUD/RecordingBack");
		recordingText = GetNode<Label>("HUD/RecordingText");
		music = GetNode<AudioStreamPlayer>("BkgMusic");
		superText = GetNode<Label>("HUD/OhShit");
		P1SnailRadar = GetNode<Control>("HUD/P1SnailRadar");
		P2SnailRadar = GetNode<Control>("HUD/P2SnailRadar");
		base._Ready();

		// hide the recording text
		SetRecordingText("");
		
		// used to hide behind the char select screen
		HUD = GetNode<CanvasLayer>("HUD");
		HUD.Transform = new Transform2D(Vector2.Right, Vector2.Zero, Vector2.Zero);

		// the default, which will be changed for certain modes
		SetDebugVisibility(false);

		SetRhythmVisibility(Globals.rhythmGame);
	}

	public void config(int playerOneIndex, int playerTwoIndex, int colorOne, int colorTwo, bool hosting, int frame, int bkg)
	{
		Globals.Log($"Starting game config on frame {frame}");
		((MainGFX)GetNode("MainGFX")).Init(bkg);
		HUD.Transform = new Transform2D(Vector2.Right, Vector2.Down, Vector2.Zero);

		//p1
		var playerOne = charScenes[playerOneIndex];
		P1 = playerOne.Instance() as Player;
		P1.Name = "P1";
		P1.Position = new Vector2(133, 240);
		P1.colorScheme = colorOne;
		AddChild(P1);
		MoveChild(P1, 4);
		p1Ind = playerOneIndex;

		//p2
		var playerTwo = charScenes[playerTwoIndex];
		P2 = playerTwo.Instance() as Player;
		P2.Name = "P2";
		P2.Position = new Vector2(330, 240);
		P2.colorScheme = colorTwo;
		AddChild(P2);
		MoveChild(P2, 5);
		p2Ind = playerTwoIndex;

		P1.Connect("ComboChanged", this, nameof(OnPlayerComboChange));
		P2.Connect("ComboChanged", this, nameof(OnPlayerComboChange));
		P1.Connect("ComboSet", this, nameof(OnPlayerComboSet));
		P2.Connect("ComboSet", this, nameof(OnPlayerComboSet));
		P1.Connect("HealthChanged", this, nameof(OnPlayerHealthChange));
		P2.Connect("HealthChanged", this, nameof(OnPlayerHealthChange));
		P1.Connect("HealthSet", this, nameof(OnPlayerHealthSet));
		P2.Connect("HealthSet", this, nameof(OnPlayerHealthSet));
		P1.Connect("MeterChanged", this, nameof(OnPlayerMeterChange));
		P2.Connect("MeterChanged", this, nameof(OnPlayerMeterChange));
		P1.Connect("HadoukenEmitted", this, nameof(OnHadoukenEmitted));
		P2.Connect("HadoukenEmitted", this, nameof(OnHadoukenEmitted));
		P1.Connect("HadoukenRemoved", this, nameof(OnHadoukenRemoved));
		P2.Connect("HadoukenRemoved", this, nameof(OnHadoukenRemoved));
		P1.Connect("CounterHit", this, nameof(OnPlayerCounterHit));
		P2.Connect("CounterHit", this, nameof(OnPlayerCounterHit));
		P1.Connect("CounterHit", this, nameof(OnPlayerCounterHit));
		P1.Connect("SuperFlash", this, nameof(OnSuperActivate));
		P2.Connect("SuperFlash", this, nameof(OnSuperActivate));

		


		P1Combo = GetNode<Label>("HUD/P1Combo");
		P2Combo = GetNode<Label>("HUD/P2Combo");
		P1Health = GetNode<TextureProgress>("HUD/P1Health");
		P2Health = GetNode<TextureProgress>("HUD/P2Health");
		timer = GetNode<Label>("HUD/Timer");
		centerText = GetNode<Label>("HUD/CenterText");
		statsText = GetNode<Label>("HUD/NetStats");
		mainGFX = GetNode<MainGFX>("MainGFX");
		camera = GetNode<Camera2D>("Camera2D");
		centerText.Visible = true;
		inputText.Call("clear");
		inputTextP2.Call("clear");

		P1Combo.Text = "";
		P2Combo.Text = "";

		gsObj = new GameStateObjectRedesign();
		gsObj.config(P1, P2, this, hosting);
		P1.Connect("LevelUp", this, nameof(OnLevelUp));
		P2.Connect("LevelUp", this, nameof(OnLevelUp));

		music.Play();
		ConfigTime();
		configured = true;
		
	}

	public void SetDebugVisibility(bool visible)
	{
		foreach (var path in new string[] { "HUD/DebugBack", "HUD/DebugText", "HUD/InputBack", "HUD/InputBackP2", "HUD/DebugText" })
			((Control)GetNode(path)).Visible = visible;
	}

	public void SetRhythmVisibility(bool visible)
	{
		((Control)GetNode("HUD/RhythmTrack")).Visible = visible;
	}

	public void SetRecordingText(string msg)
	{
		if (msg == "")
		{
			recordingBack.Visible = false;
			recordingText.Visible = false;
		}
		else
		{
			recordingBack.Visible = true;
			recordingText.Visible = true;
			recordingText.Text = msg;
		}
	}

	public void SetTrainingControlledPlayer(bool p1Control, bool p2Control)
	{
		if (P1 != null) // this may be called before players are instantiated
		{
			P1.trainingControlledPlayer = p1Control;
			P2.trainingControlledPlayer = p2Control;
		}
	}
	
	/// <summary>
	/// Update the gamestate only if we're in regular time.  Note that in a potentially ending we do not update.
	/// </summary>
	/// <param name="p1Inps"></param>
	/// <param name="p2Inps"></param>
	public override void AdvanceFrame(int p1Inps, int p2Inps)
	{
		

		if (currTime == TimeStatus.GAME)
		{
			gsObj.Update(p1Inps, p2Inps);

			if (recordMatch)
				SaveFrameInputs(p1Inps, p2Inps);
		}
		else
		{
			gsObj.Update(0, 0);
		}
			
		HandleTime();
	}

	public override void TimeAdvance()
	{
		P1.TimeAdvance();
		P2.TimeAdvance();
		camera.Call("adjust", P1.Position, P2.Position); // Camera is written in GDscript due to my own laziness
	}

	/// <summary>
	/// We only accept inputs for actual gameplay and for the couple 
	/// </summary>
	/// <returns></returns>
	public override bool AcceptingInputs()
	{
		return (currTime == TimeStatus.GAME || currTime == TimeStatus.FAKEEND);
	}

	public void DisplayInputs(int p1Inps, int p2Inps)
	{
		if (configured)
		{
			inputText.Call("inputs", p1Inps);
			inputTextP2.Call("inputs", p2Inps);
		}
			
	}

	// ----------------
	// Built in Godot Handling
	// ----------------
	public override void _PhysicsProcess(float delta)
	{
		if (!configured)
			return;
		camera.Call("adjust", P1.Position, P2.Position);
	}


	// ----------------
	// GGRS Handling
	// ----------------

	public override byte[] SaveState(int frame)
	{
		return gsObj.SaveGameState();
	}

	public override void LoadState(int frame, byte[] buffer, int checksum)
	{
		


		// This will occur if the game finishes locally but a remote input changes the result
		if (currTime == TimeStatus.FAKEEND && frame < possibleEndingFrame)
			currTime = TimeStatus.GAME;
		
		gsObj.LoadGameState(buffer);
		mainGFX.Rollback(frame);
		P1Counter.Call("rollback", frame);
		P2Counter.Call("rollback", frame);
		P1Rhythm.Call("rollback", frame);
		P2Rhythm.Call("rollback", frame);
	}

	public override void GGRSAdvanceFrame(int p1Inps, int p2Inps)
	{
		AdvanceFrame(p1Inps, p2Inps);
	}

	public override void CompareStates(byte[] serializedOldState)
	{
		gsObj.RedesignCompareStates(serializedOldState);
	}

	// ----------------
	// Signal Receptors
	// ----------------
	public void OnPlayerComboChange(string name, int combo)
	{
		//GD.Print($"Combo change for {name} to combo {combo}");
		if (name == "P2")
		{
			if (combo > 1)
			{
				P1Combo.Call("combo", combo);
			}
			else
			{
				P1Combo.Call("off");
				EmitSignal("ComboFinished", "P1");
			}
		}

		else
		{
			if (combo > 1)
			{
				P2Combo.Call("combo", combo);
			}
			else
			{
				P2Combo.Call("off");
				EmitSignal("ComboFinished", "P2");
			}
		}
	}

	public void OnPlayerComboSet(string name, int combo)
	{
		if (name == "P2")
		{
			P1Combo.Call("combo_set", combo);
		}

		else
		{
			P2Combo.Call("combo_set", combo);
		}
	}

	public void OnPlayerCounterHit(string name)
	{
		if (name == "P1")
			P1Counter.Call("display", Globals.frame);
		else
			P2Counter.Call("display", Globals.frame);
	}

	/// <summary>
	/// Called during rollbacks
	/// </summary>
	/// <param name="name"></param>
	/// <param name="health"></param>
	public void OnPlayerHealthSet(string name, int health)
	{
		if (name == "P1")
		{
			P1Health.Value = health;
		}

		else
		{
			P2Health.Value = health;
		}
	}

	public void OnPlayerHealthChange(string name, int health)
	{

		int prevHealth;
		if (name == "P1")
		{
			prevHealth = (int)P1Health.Value;
			P1Health.Value = health;
		}

		else
		{
			prevHealth = (int)P2Health.Value;
			P2Health.Value = health;
		}
			


		if (prevHealth > 1 && health < 1)
		{
			centerText.Visible = true;
			if (name == "P2")
			{
				P2Health.Value = 0;
				centerText.Text = "P1 WINS";
			}

			else
			{
				P1Health.Value = 0;
				centerText.Text = "P2 WINS";
			}

			TryEndRound();
		}
		
	}
	public void OnPlayerMeterChange(string name, int meter){
		if (name == "P1")
			P1Meter.Call("set_meter", (int)Math.Floor((double)meter/100));
		else
			P2Meter.Call("set_meter", (int)Math.Floor((double)meter /100));
	}
	
	public void OnHadoukenEmitted(HadoukenPart h)
	{
		AddChild(h); // Add the hadouken as a child
		gsObj.NewHadouken(h); // let the gamestate object control it. this still needs to be cleaned up on deletion

	}

	public void OnHadoukenRemoved(HadoukenPart h)
	{

		gsObj.RemoveHadouken(h);
	}

	public void OnLevelUp()
	{
		mainGFX.LevelUp(Globals.frame);
	}

	public void OnGhostEmitted()
	{

	}

	public void OnSuperActivate()
	{
		superText.Call("display", Globals.frame);
		gsObj.SuperFreeze();
	}

	public void ConnectSnail(Snail s)
	{
		s.Connect("SnailUpdate", this, nameof(OnSnailUpdate));
	}

	public void OnSnailUpdate(string name, int pos, Color color)
	{
		if (name == "P1")
		{
			P1SnailRadar.Call("draw_snail", pos, color);
		}
		else
		{
			P2SnailRadar.Call("draw_snail", pos, color);
		}
	}

	// ----------------
	// Time Handling
	// ----------------
	private void ConfigTime()
	{
		readyFrame = Globals.frame;
		startFrame = Globals.frame + 60 * 3;
		timeOutFrame = startFrame + 60 * 99;
		currTime = TimeStatus.PREROUND;
		timer.Text = "99";
	}

	private void HandleTime()
	{
		switch (currTime)
		{
			case TimeStatus.PREROUND:
				HandlePreroundTime();
				break;
			case TimeStatus.GAME:
				HandleGameTime();
				break;
			case TimeStatus.FAKEEND:
				HandleFakeEndTime();
				break;
			case TimeStatus.TRUEEND:
				HandleTrueEndTime();
				break;
		}


	}

	private void HandlePreroundTime()
	{
		if (Globals.frame == startFrame)
		{
			currTime = TimeStatus.GAME;
			centerText.Text = "FIGHT!";
			return;
		}
			
		

		int trueFrame = Globals.frame - readyFrame;
		centerText.Text = (3 - Math.Floor((float)trueFrame / 60)).ToString();
	}

	private void HandleGameTime()
	{
		if (ignoreTime)
		{
			centerText.Visible = false;
			return;
		}
		if (Globals.frame == timeOutFrame)
		{
			TryEndRound();
			centerText.Text = "TIME UP";
			timer.Text = "0";
			return;
		}
		else if (Globals.frame > startFrame + 60)
			centerText.Visible = false;

		int timerFrame = Globals.frame - startFrame;

		timer.Text = (99 - Math.Floor((float)timerFrame / 60)).ToString();
	}

	private void HandleFakeEndTime()
	{
		Globals.Log($"IN FAKEEND TIME.  True ending frame = {trueEndingFrame} Frame = {Globals.frame}");
		if (Globals.frame == trueEndingFrame)
		{
			EndRound();
		}
	}

	private void HandleTrueEndTime()
	{
		centerText.Visible = true;
		if (Globals.frame == exitFrame) // Later we'll manage keeping score
			EmitSignal("RoundFinished", "P1");
	}

	private void TryEndRound()
	{

		GD.Print($"Potentially ending game on frame {Globals.frame}");
		currTime = TimeStatus.FAKEEND;
		possibleEndingFrame = Globals.frame;
		trueEndingFrame = Globals.frame + 8;
	}

	private void EndRound()
	{
		if (recordMatch && !savedFile)
			WriteInputsToFile();
		currTime = TimeStatus.TRUEEND;
		exitFrame = Globals.frame + 180;
	}

	// ----------------
	// Special Tools
	// ----------------

	public void ResetHealth(string player)
	{
		OnPlayerHealthChange(player, 1600);

		if (player == "P1")
		{
			P1.ResetHealth();
		}
		else
		{
			P2.ResetHealth();
		}
			
	}

	public void ResetAll()
	{
		ResetHealth("P1");
		ResetHealth("P2");
		P1.Reset();
		P2.Reset();
		gsObj.ResetHadoukens();
		P1.internalPos = new Vector2(13300, 24000);
		P2.internalPos = new Vector2(33000, 24000);
		ConfigTime();
		if (Globals.mode == Globals.Mode.TRAINING)
		{
			P1Meter.Call("set_meter", 100);
			P2Meter.Call("set_meter", 100);
		}
		else
		{
			P1Meter.Call("set_meter", 0);
			P2Meter.Call("set_meter", 0);
		}
		if (recordMatch)
			savedFile = false;
	}

	/// <summary>
	/// Doesn't reset time for easy resets
	/// </summary>
	public void ResetTraining()
	{
		ResetHealth("P1");
		ResetHealth("P2");
		P1.Reset();
		P2.Reset();

		P1.internalPos = new Vector2(13300, 24000);
		P2.internalPos = new Vector2(33000, 24000);
	}

	public void ConnectTrainingSignals(TrainingManager manager)
	{
		P1.Connect("Recovery", manager, nameof(manager.OnCharacterRecovery));
		P2.Connect("Recovery", manager, nameof(manager.OnCharacterRecovery));
	}


	////
	// Specifically for AI
	////
	
	public HashSet<string> GetP2Tags()
	{
		return P2.currentState.tags;
	}

	public HashSet<string> GetP1Tags()
	{
		return P1.currentState.tags;
	}
	public GameStateObjectRedesign.GameState GetGameState()
	{
		return gsObj.GetGameState();
	}

	////
	// Recording
	////
	
	public int GetFramesSinceStart()
	{
		return (Globals.frame - startFrame);
	}
	private void SaveFrameInputs(int p1Inputs, int p2Inputs)
	{
		int inp_frame = GetFramesSinceStart();

		allInputs[inp_frame, 0] = p1Inputs;
		allInputs[inp_frame, 1] = p2Inputs;
	}

	private string MakeFilename()
	{
		var dict = OS.GetDatetime();
		string filename = "";

		foreach (var key in new[] {"year", "month", "day", "hour", "minute" })
		{
			filename += dict[key].ToString();
		}
		return filename;
	}

	private void WriteInputsToFile()
	{
		var recording = new Godot.Collections.Dictionary();
		recording["p1col"] = P1.colorScheme;
		recording["p2col"] = P2.colorScheme;
		recording["p1char"] = p1Ind;
		recording["p2char"] = p2Ind;
		recording["allInputs"] = allInputs;

		Globals.Log("Saving file");
		var dir = new Godot.Directory();
		dir.Open("user://");

		dir.MakeDir("recordings");
		string content = JSON.Print(recording);
		DateTime now = DateTime.Now;
		string filename = MakeFilename();

		var file = new Godot.File();
		file.Open($"user://recordings/{filename}.json", Godot.File.ModeFlags.Write);
		file.StoreString(content);
		file.Close();
		savedFile = true;
	}

	public override void _Draw()
	{
		GD.Print("Gamescene draw");
	}
}
