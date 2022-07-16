using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// Collection of constants and static functions
/// </summary>
/// 
public class GameScene : BaseGame
{
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
	private Node GGRS;
	private Node mainMenuReturn;
	private MainGFX mainGFX;
	private CanvasLayer HUD;
	private int frame;


	// TIME HANDLING
	private int readyFrame;
	private int startFrame;
	private int timeOutFrame;

	private int possibleEndingFrame;
	private int trueEndingFrame;
	private int exitFrame;
	private TimeStatus currTime;

	[Signal]
	public delegate void RoundFinished(string winner);

	/// <summary>
	/// Used for training mode, where after a combo health will reset
	/// </summary>
	/// <param name="player"></param>
	[Signal]
	public delegate void ComboFinished(string player);

	private enum TimeStatus
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
		base._Ready();

		// used to hide behind the char select screen
		HUD = GetNode<CanvasLayer>("HUD");
		HUD.Transform = new Transform2D(Vector2.Right, Vector2.Zero, Vector2.Zero);
	}

	public void config(PackedScene playerOne, PackedScene playerTwo, int colorOne, int colorTwo, bool hosting, int frame)
	{
		Globals.Log($"Starting game config on frame {frame}");
		this.frame = frame;
		HUD.Transform = new Transform2D(Vector2.Right, Vector2.Down, Vector2.Zero);

		//p1
		P1 = playerOne.Instance() as Player;
		P1.Name = "P1";
		P1.Position = new Vector2(133, 240);
		P1.colorScheme = colorOne;
		AddChild(P1);
		MoveChild(P1, 4);

		//p2
		P2 = playerTwo.Instance() as Player;
		P2.Name = "P2";
		P2.Position = new Vector2(330, 240);
		P2.colorScheme = colorTwo;
		AddChild(P2);
		MoveChild(P2, 5);

		P1.Connect("ComboChanged", this, nameof(OnPlayerComboChange));
		P2.Connect("ComboChanged", this, nameof(OnPlayerComboChange));
		P1.Connect("ComboSet", this, nameof(OnPlayerComboSet));
		P2.Connect("ComboSet", this, nameof(OnPlayerComboSet));
		P1.Connect("HealthChanged", this, nameof(OnPlayerHealthChange));
		P2.Connect("HealthChanged", this, nameof(OnPlayerHealthChange));
		P1.Connect("HadoukenEmitted", this, nameof(OnHadoukenEmitted));
		P2.Connect("HadoukenEmitted", this, nameof(OnHadoukenEmitted));
		P1.Connect("HadoukenRemoved", this, nameof(OnHadoukenRemoved));
		P2.Connect("HadoukenRemoved", this, nameof(OnHadoukenRemoved));


		GGRS = GetNode("GodotGGRS");
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
		
		P1Combo.Text = "";
		P2Combo.Text = "";

		gsObj = new GameStateObjectRedesign();
		gsObj.config(P1, P2, this, hosting);
		P1.Connect("LevelUp", this, nameof(OnLevelUp));
		P2.Connect("LevelUp", this, nameof(OnLevelUp));

		ConfigTime();
		configured = true;
		
	}

	
	public override void AdvanceFrame(int p1Inps, int p2Inps)
	{
		if (currTime == TimeStatus.GAME)
			gsObj.Update(p1Inps, p2Inps);
		else
			gsObj.Update(0, 0);
		HandleTime();
		frame++;
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
		if (Math.Abs(this.frame - frame) > 8)
			Globals.Log($"Suspicious rollback from {this.frame} to {frame}");
		// GD.Print($"rollback from frame {gsObj.Frame} to frame {frame}");
		this.frame = frame;

		// This will occur if the game finishes locally but a remote input changes the result
		if (currTime == TimeStatus.FAKEEND && frame < possibleEndingFrame)
			currTime = TimeStatus.GAME;
		
		gsObj.LoadGameState(buffer);
		mainGFX.Rollback(frame);
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
		GD.Print($"Combo change for {name} to combo {combo}");
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
	public void OnPlayerHealthChange(string name, int health)
	{
		if (name == "P1")
			P1Health.Value = health;
		else
			P2Health.Value = health;


		if (health < 1)
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
		mainGFX.LevelUp(gsObj.Frame);
	}

	public void OnGhostEmitted()
	{

	}

	// ----------------
	// Time Handling
	// ----------------
	private void ConfigTime()
	{
		readyFrame = frame;
		startFrame = frame + 60 * 3;
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
		if (frame == startFrame)
		{
			currTime = TimeStatus.GAME;
			centerText.Text = "FIGHT!";
			return;
		}
			
		

		int trueFrame = frame - readyFrame;
		centerText.Text = (3 - Math.Floor((float)trueFrame / 60)).ToString();
	}

	private void HandleGameTime()
	{
		if (frame == timeOutFrame)
		{
			EndRound();
			centerText.Text = "TIME UP";
			timer.Text = "0";
			return;
		}
		else if (frame > startFrame + 60)
			centerText.Visible = false;

		int trueFrame = frame - startFrame;

		timer.Text = (99 - Math.Floor((float)trueFrame / 60)).ToString();
	}

	private void HandleFakeEndTime()
	{
		if (frame == trueEndingFrame)
		{
			EndRound();
		}
	}

	private void HandleTrueEndTime()
	{
		centerText.Visible = true;
		if (frame == exitFrame) // Later we'll manage keeping score
			EmitSignal("RoundFinished", "P1");
	}

	private void TryEndRound()
	{
		currTime = TimeStatus.FAKEEND;
		possibleEndingFrame = frame;
		trueEndingFrame = frame + 8;
	}

	private void EndRound()
	{
		currTime = TimeStatus.TRUEEND;
		exitFrame = frame + 180;
	}

	// ----------------
	// Special Tools
	// ----------------

	public void ResetHealth(string player)
	{
		OnPlayerHealthChange(player, 800);

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

		P1.internalPos = new Vector2(13300, 24000);
		P2.internalPos = new Vector2(33000, 24000);
		ConfigTime();
	}
}
