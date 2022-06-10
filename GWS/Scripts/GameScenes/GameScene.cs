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

	private Dictionary<string, PackedScene> characterMap = new Dictionary<string, PackedScene>();

	public void config(PackedScene playerOne, PackedScene playerTwo, int colorOne, int colorTwo, bool hosting)
	{

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
		centerText.Visible = true;
		
		P1Combo.Text = "";
		P2Combo.Text = "";

		gsObj = new GameStateObjectRedesign();
		gsObj.config(P1, P2, this, hosting);
		P1.Connect("LevelUp", this, nameof(OnLevelUp));
		P2.Connect("LevelUp", this, nameof(OnLevelUp));

	}

	
	public override void AdvanceFrame(int p1Inps, int p2Inps)
	{
		gsObj.Update(p1Inps, p2Inps);
	}

	public override void TimeAdvance()
	{
		P1.TimeAdvance();
		P2.TimeAdvance();
		camera.Call("adjust", P1.Position, P2.Position); // Camera is written in GDscript due to my own laziness
	}


	// ----------------
	// GGRS Handling
	// ----------------

	public override byte[] SaveState(int frame)
	{
		return gsObj.SaveGameState().DataArray;
	}

	public override void LoadState(int frame, byte[] buffer, int checksum)
	{
		// GD.Print($"rollback from frame {gsObj.Frame} to frame {frame}");
		var buf = new StreamPeerBuffer();
		buf.DataArray = buffer;
		gsObj.LoadGameState(buf);
		mainGFX.Rollback(frame);
	}

	public override void GGRSAdvanceFrame(int p1Inps, int p2Inps)
	{
		gsObj.Update(p1Inps, p2Inps);
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
		if (name == "P2")
		{
			if (combo > 1)
			{
				P1Combo.Call("combo", combo);
			}
			else
			{
				P1Combo.Call("off");
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
		if (health < 1)
		{
			if (Globals.mode == Globals.Mode.TRAINING || Globals.mode == Globals.Mode.SYNCTEST) // eventually this should reset player health
			{
				return;
			}
			//EndRound();
			centerText.Visible = true;
		}
		if (name == "P2")
		{
			P2Health.Value = health;
			centerText.Text = "P1 WINS";
		}

		else
		{
			P1Health.Value = health;
			centerText.Text = "P2 WINS";
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
}
