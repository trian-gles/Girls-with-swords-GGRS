using Godot;
using System;
using System.Collections.Generic;


/// <summary>
/// Collection of constants and static functions
/// </summary>
/// 
public class GameScene : BaseGame
{

	[Signal]
	public delegate void GameWon(string winner, int chosenCharacter);

	/// <summary>
	/// Used to prevent physics process
	/// </summary>
	private bool configured = false;

	public Player P1;
	public Player P2;
	private HUDCombo P1Combo;
	private HUDCombo P2Combo;
	private TextureProgress P1Health;
	private TextureProgress P2Health;
	private TextureProgress P1PreComboHealth;
	private TextureProgress P2PreComboHealth;
	private Camera camera;
	private GameStateObjectRedesign gsObj;
	private Label timer;
	private Label centerText;
	private Label statsText;
	private Node mainMenuReturn;
	private MainGFX mainGFX;
	private Control[] debugControls;
	private CanvasLayer HUD;
	private SplashText P1Mixup;
	private SplashText P2Mixup;
	private SplashText P1Escape;
	private SplashText P2Escape;
	private SplashText P1Missed;
	private SplashText P2Missed;
	private SnailRadar P1SnailRadar;
	private SnailRadar P2SnailRadar;
	private OhShit superText;
	private ProgressBar P1Meter;
	private ProgressBar P2Meter;
	private TextureProgress P1Salt;
	private TextureProgress P2Salt;
	private AudioStreamPlayer music;
	private HBoxContainer p1RoundCounters;
	private HBoxContainer p2RoundCounters;
	private Node2D p1Logos;
	private Node2D p2Logos;
	private Control splashText;
	private Godot.Collections.Array<SplashText> splashTexts;
	private Label recordingText;
	private ColorRect recordingBack;
	private Control tutorialContainer;
	private List<string> timerStrings = new List<string>();

	// WIN STATE
	private int p1Wins = 0;
	private int p2Wins = 0;

	private const string CounterString = "COUNTER";
	private const string CounterLowerString = "Counter";
	private const string FightString = "FIGHT";
	private const string ThreeString = "THREE";
	private const string TwoString = "TWO";
	private const string OneString = "ONE";
	private const string DownString = "DOWN";

	// Display and runtime call strings
	private const string PlayerOneString = "P1";
	private const string PlayerTwoString = "P2";
	private const string DownDisplayString = "DOWN!";
	private const string FightDisplayString = "FIGHT!";
	private const string TimeUpString = "TIME UP";
	private const string SelectedCharLogoString = "selected_char_logo";
	private const string WinCounterUpString = "win_counter_up";
	private const string ClearCallString = "clear";
	private const string InputsCallString = "inputs";
	private const string ComboCallString = "combo";
	private const string ComboOffString = "off";
	private const string ComboSetCallString = "combo_set";
	private const string DisplayCallString = "display";
	private const string SetMeterCallString = "set_meter";
	private const string SetLevelCallString = "set_level";
	private const string PlayIdxCallString = "play_idx";
	private const string DrawSnailCallString = "draw_snail";
	private const string SnailUpdateString = "SnailUpdate";
	private const string TutorialContainerPath = "HUD/TutorialContainer";


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

	public enum ResetPos
	{
		ROUNDSTART,
		ROUNDSTARTREVERSED,
		P1CORNEREDLEFT,
		P1CORNEREDRIGHT,
		P2CORNEREDLEFT,
		P2CORNEREDRIGHT
	}

	public Godot.Collections.Dictionary<string, SplashText> p1SplashTexts = new Godot.Collections.Dictionary<string, SplashText>();
	public Godot.Collections.Dictionary<string, SplashText> p2SplashTexts = new Godot.Collections.Dictionary<string, SplashText>();
	public override void _Ready()
	{
		splashText = GetNode<Control>("HUD/SplashText");
		splashTexts = new Godot.Collections.Array<SplashText>();
		foreach (var c in splashText.GetChildren())
		{
			var sText = (SplashText) c;
			splashTexts.Add(sText);
			var nameEnd = sText.Name.Substr(2, sText.Name.Length - 2);
			if (sText.Name.BeginsWith(PlayerOneString))
				p1SplashTexts.Add(nameEnd, sText);
			else
				p2SplashTexts.Add(nameEnd, sText);
		}
		HUDText = GetNode<Control>("HUD/DebugText");
		inputText = GetNode<Label>("HUD/InputText");
		inputTextP2 = GetNode<Label>("HUD/InputTextP2");
		P1Mixup = splashText.GetNode<SplashText>("P1Mixup");
		P2Mixup = splashText.GetNode<SplashText>("P2Mixup");
		P1Missed = splashText.GetNode<SplashText>("P1Missed");
		P2Missed = splashText.GetNode<SplashText>("P2Missed");
		P1Escape = splashText.GetNode<SplashText>("P1Escape");
		P2Escape = splashText.GetNode<SplashText>("P2Escape");
		P1Meter = GetNode<ProgressBar>("HUD/P1Meter/ProgressBar");
		P2Meter = GetNode<ProgressBar>("HUD/P2Meter/ProgressBar");
		P1Salt = GetNode<TextureProgress>("HUD/Salt/TextureProgress");
		P2Salt = GetNode<TextureProgress>("HUD/Salt2/TextureProgress");
		recordingBack = GetNode<ColorRect>("HUD/RecordingBack");
		recordingText = GetNode<Label>("HUD/RecordingText");
		music = GetNode<AudioStreamPlayer>("BkgMusic");
		superText = GetNode<OhShit>("HUD/OhShit");
		P1SnailRadar = GetNode<SnailRadar>("HUD/P1SnailRadar");
		P2SnailRadar = GetNode<SnailRadar>("HUD/P2SnailRadar");
		p1RoundCounters = GetNode<HBoxContainer>("HUD/P1RoundCounters");
		p2RoundCounters = GetNode<HBoxContainer>("HUD/P2RoundCounters");
		p1Logos = GetNode<Node2D>("HUD/P1Logo");
		p2Logos = GetNode<Node2D>("HUD/P2Logo");
		P1Combo = GetNode<HUDCombo>("HUD/P1Combo");
		P2Combo = GetNode<HUDCombo>("HUD/P2Combo");
		P1Health = GetNode<TextureProgress>("HUD/P1Health");
		P2Health = GetNode<TextureProgress>("HUD/P2Health");
		P1PreComboHealth = GetNode<TextureProgress>("HUD/P1PreComboHealth");
		P2PreComboHealth = GetNode<TextureProgress>("HUD/P2PreComboHealth");
		timer = GetNode<Label>("HUD/Timer");
		centerText = GetNode<Label>("HUD/CenterText");
		statsText = GetNode<Label>("HUD/NetStats");
		mainGFX = GetNode<MainGFX>("MainGFX");
		camera = GetNode<Camera>("Camera2D");
		// cache frequently used HUD controls to avoid runtime GetNode calls
		mainGFX = GetNode<MainGFX>("MainGFX");
		debugControls = new Control[] {
			//GetNode<Control>("HUD/InputBack"),
			//GetNode<Control>("HUD/InputBackP2"),
			GetNode<Control>("HUD/DebugText"),
			GetNode<Control>("HUD/DebugText/DebugTextLabel")
		};
		tutorialContainer = GetNode<Control>(TutorialContainerPath);
		splashText = GetNode<Control>("HUD/SplashText");


		base._Ready();
		gsObj = new GameStateObjectRedesign();

		// hide the recording text
		SetRecordingText("");

		// used to hide behind the char select screen
		HUD = GetNode<CanvasLayer>("HUD");
		HUD.Transform = new Transform2D(Vector2.Right, Vector2.Zero, Vector2.Zero);

		// the default, which will be changed for certain modes
		SetDebugVisibility(false);

		for (int i = 0; i <= 99; i++)
		{
			timerStrings.Add((99 - i).ToString());
		}

		Globals.ConnectPlayerSingleArgSignalListener(Globals.PlayerSignal.ComboChanged, OnPlayerComboChange);
		Globals.ConnectPlayerSingleArgSignalListener(Globals.PlayerSignal.ComboSet, OnPlayerComboSet);
		Globals.ConnectPlayerSingleArgSignalListener(Globals.PlayerSignal.HealthChanged, OnPlayerHealthChange);
		Globals.ConnectPlayerSingleArgSignalListener(Globals.PlayerSignal.PreComboHealthChanged, OnPlayerPreComboHealthChange);
		Globals.ConnectPlayerSingleArgSignalListener(Globals.PlayerSignal.HealthSet, OnPlayerHealthSet);
		Globals.ConnectPlayerSingleArgSignalListener(Globals.PlayerSignal.MeterChanged, OnPlayerMeterChange);
		Globals.ConnectPlayerSingleArgSignalListener(Globals.PlayerSignal.BurstSet, OnPlayerBurstSet);
		Globals.ConnectPlayerSingleArgSignalListener(Globals.PlayerSignal.HitStop, OnHitStop);

		Globals.ConnectPlayerNoArgSignalListener(Globals.PlayerSignal.Counter, OnPlayerCounterHit);
		Globals.ConnectPlayerNoArgSignalListener(Globals.PlayerSignal.Mixup, OnPlayerMixup);
		Globals.ConnectPlayerNoArgSignalListener(Globals.PlayerSignal.CanTech, OnPlayerCanEscape);
		Globals.ConnectPlayerNoArgSignalListener(Globals.PlayerSignal.MissedTech, OnPlayerMissedEscape);
		Globals.ConnectPlayerNoArgSignalListener(Globals.PlayerSignal.SuperFlash, OnSuperActivate);

		Globals.ConnectPlayerGenericGfxEmitted(OnGenericGFXEmitted);
		Globals.logBuffer.Clear();
	}

	public void config(int playerOneIndex, int playerTwoIndex, int colorOne, int colorTwo, bool hosting, int frame, int bkg)
	{
		mainGFX.Init(bkg);
		HUD.Transform = new Transform2D(Vector2.Right, Vector2.Down, Vector2.Zero);
		HUD.Layer = 1;

		//p1
		P1 = Globals.P1Characters[playerOneIndex];
		P1.Name = PlayerOneString;
		P1.Position = new Vector2(133, 240);
		P1.colorScheme = colorOne;
		AddChild(P1);
		P1.Init();
		MoveChild(P1, 4);
		p1Ind = playerOneIndex;
		p1Logos.Call(SelectedCharLogoString, playerOneIndex);
		P1.aiControlled = false;
		Globals.P1CharacterMoves = P1.characterMoves;

		//p2
		P2 = Globals.P2Characters[playerTwoIndex];
		P2.Name = PlayerTwoString;
		P2.Position = new Vector2(330, 240);
		P2.colorScheme = colorTwo;
		P2.aiControlled = false;
		AddChild(P2);
		P2.Init();
		MoveChild(P2, 5);
		p2Ind = playerTwoIndex;
		p2Logos.Call(SelectedCharLogoString, playerTwoIndex);
		Globals.P2CharacterMoves = P2.characterMoves;

		if (Globals.mode == Globals.Mode.TRAINING || Globals.mode == Globals.Mode.TUTORIAL)
		{
			P1Meter.Value = 100;
			P2Meter.Value = 100;
			p1RoundCounters.Visible = false;
			p2RoundCounters.Visible = false;
		}
		else
		{
			p1RoundCounters.Visible = true;
			p2RoundCounters.Visible = true;
			P1Meter.Value = 0;
			P2Meter.Value = 0;
		}
		SetPos(ResetPos.ROUNDSTART);

		centerText.Visible = true;
		inputText.Call(ClearCallString);
		inputTextP2.Call(ClearCallString);

		P1Combo.Text = "";
		P2Combo.Text = "";

		
		gsObj.config(P1, P2, this, hosting);
		SetPos(ResetPos.ROUNDSTART);
		music.Call(PlayIdxCallString, bkg);  // PASSABLE
		ConfigTime();
		configured = true;

	}

	public override void _ExitTree()
	{
		base._ExitTree();
	}

	public void SetP2AI()
	{
		P2.aiControlled = true;
	}
	public void SetDebugVisibility(bool visible)
	{
		foreach (var c in debugControls)
			c.Visible = visible;
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
		camera.Adjust(P1.Position, P2.Position); // Camera is written in GDscript due to my own laziness
	}

	public void ScreenShake(float amount) {
		if (Globals.DISABLESHAKE)
			return;
		camera.SetTrauma(amount);
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
			inputText.Call(InputsCallString, p1Inps);
			inputTextP2.Call(InputsCallString, p2Inps);
		}

	}

	// ----------------
	// Built in Godot Handling
	// ----------------
	public override void _PhysicsProcess(float delta)
	{
		if (!configured)
			return;
		camera.Adjust(P1.Position, P2.Position);
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

		if (currTime == TimeStatus.TRUEEND && frame < trueEndingFrame)
			currTime = TimeStatus.FAKEEND;

		gsObj.LoadGameState(buffer);
		mainGFX.Rollback(frame);
		for (int i = 0; i < splashTexts.Count; i++)
		{
			var txt = splashTexts[i];
			txt.Rollback(frame);
		}
	}

	public override void GGRSAdvanceFrame(int p1Inps, int p2Inps)
	{
		AdvanceFrame(p1Inps, p2Inps);
	}

	public override bool CompareStates(byte[] serializedOldState)
	{
		return gsObj.RedesignCompareStates(serializedOldState);
	}

	// ----------------
	// Signal Receptors
	// ----------------
	public void OnGenericGFXEmitted(string fxName, string playerName)
	{
		Godot.Collections.Dictionary<string, SplashText> dict;

		if (playerName == PlayerOneString)
			dict = p1SplashTexts;
		else
			dict = p2SplashTexts;

		dict[fxName].Display(Globals.frame);
	}


	public void OnPlayerComboChange(string name, int combo)
	{
		if (name == PlayerTwoString)
		{
			if (combo > 1)
			{
				P1Combo.Combo(combo);
			}
			else
			{
				P1Combo.Off();
				if (Globals.mode == Globals.Mode.TRAINING || Globals.mode == Globals.Mode.TUTORIAL)
					EmitSignal(nameof(ComboFinished), PlayerOneString);
			}
		}

		else
		{
			if (combo > 1)
			{
				P2Combo.Combo(combo);
			}
			else
			{
				P2Combo.Off();
				if (Globals.mode == Globals.Mode.TRAINING || Globals.mode == Globals.Mode.TUTORIAL)
					EmitSignal(nameof(ComboFinished), PlayerTwoString); // PASSABLE
			}
		}
	}

	public void OnPlayerComboSet(string name, int combo)
	{
		if (name == PlayerTwoString)
		{
			P1Combo.ComboSet(combo);
		}

		else
		{
			P2Combo.ComboSet(combo);
		}
	}

	public void OnPlayerCounterHit(string name)
	{
		OnGenericGFXEmitted(CounterLowerString, name);
		if (name == PlayerOneString)
		{
			P1.ForceEvent(EventScheduler.EventType.AUDIO, CounterString);
		}
		else
		{
			P2.ForceEvent(EventScheduler.EventType.AUDIO, CounterString);
		}
		ScreenShake(0.6f);
	}

	public void OnPlayerMixup(string name)
	{
		if (name == PlayerOneString)
			P1Mixup.Display(Globals.frame);
		else
			P2Mixup.Display(Globals.frame);
	}
	public void OnPlayerCanEscape(string name)
	{
		if (name == PlayerOneString)
		{
			P1Escape.Display(Globals.frame);
			P1Missed.Visible = false;
		}
		else
		{
			P2Escape.Display(Globals.frame);
			P2Missed.Visible = false;
		}
			
	}
	public void OnPlayerMissedEscape(string name)
	{
		if (name == PlayerOneString)
		{
			P1Missed.Display(Globals.frame);
			P1Escape.Display(Globals.frame);
		}

		else
		{
			P2Missed.Display(Globals.frame);
			P2Escape.Display(Globals.frame);
		}
			
	}

	/// <summary>
	/// Called during rollbacks
	/// </summary>
	/// <param name="name"></param>
	/// <param name="health"></param>
	public void OnPlayerHealthSet(string name, int health)
	{
		if (name == PlayerOneString)
		{
			P1Health.Value = health;
		}

		else
		{
			P2Health.Value = health;
		}
	}

	public void OnPlayerPreComboHealthChange(string name, int preComboHealth)
	{
		if (name == PlayerOneString)
		{
			P1PreComboHealth.Value = preComboHealth;
		}
		else
		{
			P2PreComboHealth.Value = preComboHealth;
		}
	}

	public void OnPlayerHealthChange(string name, int health)
	{

		int prevHealth;
		if (name == PlayerOneString)
		{
			prevHealth = (int)P1Health.Value;
			P1Health.Value = health;
		}

		else
		{
			prevHealth = (int)P2Health.Value;
			P2Health.Value = health;
		}



		if (prevHealth >= 1 && health < 1)
		{
			centerText.Visible = true;
			centerText.Text = DownDisplayString;

			TryEndRound();
		}

	}
	public void OnPlayerMeterChange(string name, int meter) {
		if (name == PlayerOneString)
			P1Meter.Value = (int)Math.Floor((double)meter / 100);
		else
			P2Meter.Value = (int)Math.Floor((double)meter / 100);
	}

	public void OnPlayerBurstSet(string name, int burstMeter)
	{
		if (name == PlayerOneString)
			P1Salt.Value = burstMeter;
		else
			P2Salt.Value = burstMeter;
	}

	public void OnHadoukenEmitted(HadoukenPart h)
	{
		gsObj.NewHadouken(h); // let the gamestate object control it. this still needs to be cleaned up on deletion
		AddChild(h);
		
		//CallDeferred("add_child", h); // Add the hadouken as a child
	}

	public void OnHadoukenRemoved(HadoukenPart h)
	{
		gsObj.RemoveHadouken(h);
	}

	public void OnHadoukenCommand(string playerName, string projectileName, HadoukenPart.ProjectileCommand command)
	{
		gsObj.HadoukenCommand(playerName, projectileName, command);
	}

	public void OnGhostEmitted()
	{

	}

	public void OnSuperActivate(string name)
	{
		superText.Display(Globals.frame);
		gsObj.SuperFreeze(name);
	}

	public void ConnectSnail(Snail s)
	{
		if (!s.IsConnected(SnailUpdateString, this, nameof(OnSnailUpdate)))
			s.Connect(SnailUpdateString, this, nameof(OnSnailUpdate));
	}

	public void OnSnailUpdate(string name, int pos, Color color)
	{
		if (name == PlayerOneString)
		{
			P1SnailRadar.DrawSnail(pos, color);
		}
		else
		{
			P2SnailRadar.DrawSnail(pos, color);
		}
	}

	public void OnHitStop(string playerName, int stun)
	{
		gsObj.HandleHitStop(stun);
	}

	// ----------------
	// Time Handling
	// ----------------
	private void ConfigTime()
	{
		timer.Text = "99";
		readyFrame = Globals.frame;
		if (Globals.mode == Globals.Mode.TRAINING || Globals.mode == Globals.Mode.TUTORIAL)
		{
			startFrame = Globals.frame;
			currTime = TimeStatus.GAME;
		}
		else
		{
			startFrame = Globals.frame + 60 * 3;
			timeOutFrame = startFrame + 40 * 99;
			currTime = TimeStatus.PREROUND;
		}


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
			centerText.Text = FightDisplayString;
			P1.ForceEvent(EventScheduler.EventType.AUDIO, FightString);
			return;
		}



		int trueFrame = Globals.frame - readyFrame;
		int displayNum =(int) (3 - Math.Floor((float)trueFrame / 60));

		centerText.Text = displayNum.ToString();
		if (trueFrame % 60 == 1)
		{
			if (displayNum == 3)
				P1.ForceEvent(EventScheduler.EventType.AUDIO, ThreeString);
			if (displayNum == 2)
				P1.ForceEvent(EventScheduler.EventType.AUDIO, TwoString);
			if (displayNum == 1)
				P1.ForceEvent(EventScheduler.EventType.AUDIO, OneString);
		}
			

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
			centerText.Text = TimeUpString;
			timer.Text = "0";
			return;
		}
		else if (Globals.frame > startFrame + 60)
			centerText.Visible = false;

		int timerFrame = Globals.frame - startFrame;

		timer.Text = timerStrings[(int)Math.Floor((float)timerFrame / 40)];
	}

	private void HandleFakeEndTime()
	{
		if (Globals.frame == trueEndingFrame)
		{
			EndRound();
		}
	}

	private void HandleTrueEndTime()
	{
		centerText.Visible = true;

		if (Globals.frame == trueEndingFrame + 30)
		{
			if (P1Health.Value > P2Health.Value)
			{
				p1Wins++;
				p1RoundCounters.Call(WinCounterUpString, p1Wins); // PASSABLE
			}
			else
			{
				p2Wins++;
				p2RoundCounters.Call(WinCounterUpString, p2Wins);  // PASSABLE
			}
		}
			
		if (Globals.frame == exitFrame)
		{
			
			if (p1Wins == 2)
			{
				ResetWin(); 
				
				EmitSignal(nameof(GameWon), PlayerOneString, p1Ind);

			}
			else if (p2Wins == 2)
			{
				ResetWin();
				EmitSignal(nameof(GameWon), PlayerTwoString, p2Ind);

			}
			else
				ResetRound();
		}
	}

	private void TryEndRound()
	{
		currTime = TimeStatus.FAKEEND;
		possibleEndingFrame = Globals.frame;
		trueEndingFrame = Globals.frame + 8;
	}

	private void EndRound()
	{

		P1.ForceEvent(EventScheduler.EventType.AUDIO, DownString);
		currTime = TimeStatus.TRUEEND;
		exitFrame = Globals.frame + 180;
		

	}

	// ----------------
	// Special Tools
	// ----------------

	public void ResetHealth(string player)
	{
		OnPlayerHealthChange(player, 1800);


		if (player == PlayerOneString)
		{
			P1.ResetHealth();
		}
		else
		{
			P2.ResetHealth();
		}

	}

	public void SetPos(ResetPos resetPos)
	{
		switch (resetPos) {
			case ResetPos.ROUNDSTART:
				P1.internalPos = new Vector2(18300, 24000);
				P2.internalPos = new Vector2(27000, 24000);
				return;
			case ResetPos.ROUNDSTARTREVERSED:
				P1.internalPos = new Vector2(27000, 24000);
				P2.internalPos = new Vector2(18300, 24000);
				return;
			case ResetPos.P1CORNEREDLEFT:
				P1.internalPos = new Vector2(0, 24000);
				P2.internalPos = new Vector2(13300, 24000);
				return;
			case ResetPos.P1CORNEREDRIGHT:
				P1.internalPos = new Vector2(50000, 24000);
				P2.internalPos = new Vector2(33000, 24000);
				return;
			case ResetPos.P2CORNEREDLEFT:
				P2.internalPos = new Vector2(0, 24000);
				P1.internalPos = new Vector2(13300, 24000);
				return;
			case ResetPos.P2CORNEREDRIGHT:
				P2.internalPos = new Vector2(50000, 24000);
				P1.internalPos = new Vector2(33000, 24000);
				return;
		}
	}

	public void Quit()
	{
		music.Stop();
		if (configured)
		{
			tutorialContainer.Call("reset");
			ResetRound();
			mainGFX.Quit();
			OnPlayerBurstSet(PlayerOneString, 100);
			OnPlayerBurstSet(PlayerTwoString, 100);
			p1Wins = 0;
			p2Wins = 0;
			centerText.Text = "";
			p1RoundCounters.Call("_ready");
			p2RoundCounters.Call("_ready");
			RemoveChild(P1);
			RemoveChild(P2);
			configured = false;
			HUD.Layer = -1;
			Globals.autoTech = false;
			Globals.alwaysBlock = false;
		}
	}

/// <summary>
/// 
/// </summary>
	public override void ResetRound()
	{
		superText.last_display = 0;
		ResetHealth(PlayerOneString);
		ResetHealth(PlayerTwoString);
		P1.Reset();
		P2.Reset();
		gsObj.ResetHadoukens();
		SetPos(ResetPos.ROUNDSTART);
		
		ConfigTime();
		if (Globals.mode == Globals.Mode.TRAINING || Globals.mode == Globals.Mode.TUTORIAL)
		{
			P1Meter.Value = 100;
			P2Meter.Value = 100;
		}
		else
		{
			P1Meter.Value = 0;
			P2Meter.Value = 0;
		}
		if (recordMatch)
			savedFile = false;
	}

	/// <summary>
	/// Doesn't reset time for easy resets
	/// </summary>
	public void ResetTraining()
	{
		ResetHealth(PlayerOneString);
		ResetHealth(PlayerTwoString);

		gsObj.ResetHadoukens();
		SetPos(ResetPos.ROUNDSTART);

		P1.Reset();
		P2.Reset();
		P1Meter.Value = 100;
		P2Meter.Value = 100;
	}

	private void ResetWin() 
	{
		ResetRound();
		centerText.Text = "";
		p1RoundCounters.Call("_ready");
		p2RoundCounters.Call("_ready");
		p1Wins = 0;
		p2Wins = 0;
		RemoveChild(P1);
		RemoveChild(P2);
		P1.burstMeter = 100;
		P2.burstMeter = 100;
		OnPlayerBurstSet(PlayerOneString, 100);
		OnPlayerBurstSet(PlayerTwoString, 100);
		configured = false;
		HUD.Layer = -1;
	}

	public void ConnectTrainingSignals(TrainingManager manager)
	{
		Globals.ConnectPlayerNoArgSignalListener(Globals.PlayerSignal.Recovery, manager.OnCharacterRecovery);
	}


	////
	// Specifically for AI
	////
	
	public HashSet<Globals.Tags> GetP2Tags()
	{
		return P2.currentState.tags;
	}

	public HashSet<Globals.Tags> GetP1Tags()
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

	protected string MakeFilename()
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

	public void WriteLogs()
	{
		Globals.Log("Saving file");

		WriteInputsToFile();

		var dir = new Godot.Directory();
		dir.Open("user://");

		dir.MakeDir("logs");
		//Globals.logBuffer.Slice
		string content = String.Join("\n", Globals.logBuffer.ToArray());
		DateTime now = DateTime.Now;
		string filename = MakeFilename();

		var file = new Godot.File();
		file.Open($"user://logs/{filename}.txt", Godot.File.ModeFlags.Write);
		file.StoreString(content);
		file.Close();
		Globals.logBuffer.Clear();
		
	}

	public override void _Draw()
	{
	}
}
