using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// Collection of constants and static functions
/// </summary>
/// 
public class CharSelectScene : BaseGame
{
	[Export]
	public Texture P1Texture;

	[Export]
	public Texture BothTexture;

	[Export]
	public PackedScene OLScene;

	[Export]
	public PackedScene GLScene;

	private Sprite P1Cursor;
	private Sprite P2Cursor;

	private CharSelectAudio audio;

	private List<List<Sprite>> charImages;
	
	private Godot.Collections.Array bkgImages;

	private int p1Pos = 0;
	private int p2Pos = 0;
	public bool p1Selected = false; // public as this is required for AI selection
	private bool p2Selected = false;
	private int p1Color;
	private int p2Color;
	private int[] lastInputs;
	private int extraFrames = 12;
	private int selectedStage = 0;

	[Serializable]
	private struct GameState
	{
		public int p1Pos { get; set; }
		public int p2Pos { get; set; }
		public bool p1Selected { get; set; }
		public bool p2Selected { get; set; }
		public int p1Color { get; set; }
		public int p2Color	{ get; set; }
		public int[] lastFrameInputs { get; set; }
		public int extraFrames { get; set; }
		public int selectedStage { get; set; }

	}

	const int CENTER = 185;

	

	[Signal]
	public delegate void CharacterSelected(PackedScene playerOne, PackedScene playerTwo, int colorOne, int colorTwo);

	private List<PackedScene> characterScenes;

	public override void _Ready()
	{
		HUDText = GetNode<Label>("CanvasLayer/DebugText");
		base._Ready();
		characterScenes = new List<PackedScene>() { OLScene, GLScene };
		lastInputs = new int[2] { 0, 0 };
		P1Cursor = GetNode<Sprite>("CanvasLayer/P1Cursor");
		P2Cursor = GetNode<Sprite>("CanvasLayer/P2Cursor");

		audio = GetNode<CharSelectAudio>("CharSelectAudio");

		var p1CharImages = new List<Sprite>() {
			GetNode<Sprite>("CanvasLayer/P1Selected/OLSprite"),
			GetNode<Sprite>("CanvasLayer/P1Selected/GLSprite")
		};

		var p2CharImages = new List<Sprite>() {
			GetNode<Sprite>("CanvasLayer/P2Selected/OLSprite"),
			GetNode<Sprite>("CanvasLayer/P2Selected/GLSprite")
		};

		charImages = new List<List<Sprite>>() { p1CharImages, p2CharImages };

		bkgImages = GetNode("CanvasLayer/Bkgs").GetChildren();

		CheckOverlap();

	}

	public override void CompareStates(byte[] serializedOldState)
	{
		base.CompareStates(serializedOldState);
		var oldState = Deserialize<GameState>(serializedOldState);
		CompareValues(p1Pos, oldState.p1Pos, "p1Pos");
		CompareValues(p2Pos, oldState.p2Pos, "p2Pos");
		CompareValues(p1Color, oldState.p1Color, "p1Color");
		CompareValues(p2Color, oldState.p2Color, "p2Color");
		CompareValues(p1Selected, oldState.p1Selected, "p1Selected");
		CompareValues(p2Selected, oldState.p2Selected, "p2Selected");
	}

	public override byte[] SaveState(int frame)
	{
		var state = new GameState();
		state.p1Color = p1Color;
		state.p2Color = p2Color;
		state.p1Pos = p1Pos;
		state.p2Pos = p2Pos;
		state.p1Selected = p1Selected;
		state.p2Selected = p2Selected;
		state.lastFrameInputs = lastInputs;
		state.selectedStage = selectedStage;
		return Serialize<GameState>(state);
	}

	public override void LoadState(int frame, byte[] buffer, int checksum)
	{
		var state = Deserialize<GameState>(buffer);
		p1Selected = state.p1Selected;
		p2Selected = state.p2Selected;
		p1Color = state.p1Color;
		p2Color = state.p2Color;
		p1Pos = state.p1Pos;
		p2Pos = state.p2Pos;
		lastInputs = state.lastFrameInputs;

		// Cleanup background selection
		((Sprite)bkgImages[selectedStage]).Visible = false;
		selectedStage = state.selectedStage;
		((Sprite)bkgImages[selectedStage]).Visible = true;

		// Cleanup selection images
		charImages[0][p1Pos].Visible = true;
		charImages[0][(p1Pos + 1) % 2].Visible = false;
		charImages[1][p2Pos].Visible = true;
		charImages[1][(p2Pos + 1) % 2].Visible = false;

		

		CheckOverlap();
	}

	public override void AdvanceFrame(int p1Inps, int p2Inps)
	{
		
		int[] combinedInputs = new int[] { p1Inps, p2Inps };
		for (int i = 0; i <= 1; i++)
		{
			int inputs = combinedInputs[i];
			int lastFrameInputs = lastInputs[i];

			if ((inputs & 1) != 0 && (lastFrameInputs & 1) == 0)
			{
				MoveStageSelection(-1);
			}

			if ((inputs & 2) != 0 && (lastFrameInputs & 2) == 0)
			{
				MoveStageSelection(1);
			}

			if ((inputs & 4) != 0 && (lastFrameInputs & 4) == 0)
			{
				MoveCursor(i, 1);
			}

			if ((inputs & 8) != 0 && (lastFrameInputs & 8) == 0)
			{
				MoveCursor(i, -1);
			}

			if ((inputs & 16) != 0 && (lastFrameInputs & 16) == 0)
			{
				SelectPlayer(i, 0);
			}

			if ((inputs & 32) != 0 && (lastFrameInputs & 32) == 0)
			{
				SelectPlayer(i, 1);
			}

			if ((inputs & 64) != 0 && (lastFrameInputs & 64) == 0)
			{
				SelectPlayer(i, 2);
			}
		}
		lastInputs = combinedInputs;
	}

	/// <summary>
	/// After both players are selected we decline inputs to prevent rollbacks
	/// </summary>
	/// <returns></returns>
	public override bool AcceptingInputs()
	{
		return (!p1Selected || !p2Selected);
	}

	private void CheckOverlap()
	{
		if (p1Pos == p2Pos && !p1Selected && !p2Selected)
		{
			P1Cursor.Texture = BothTexture;
			P2Cursor.Visible = false;
		}
		else
		{
			P1Cursor.Texture = P1Texture;
			if (!p2Selected)
				P2Cursor.Visible = true;
		}
	}

	int Mod(int x, int m)
	{
		int r = x % m;
		return r < 0 ? r + m : r;
	}

	private void MoveCursor(int playerNum, int direction)
	{
		
		if (playerNum == 0 && !p1Selected)
		{
			
			p1Pos = Math.Min(Math.Max(0, p1Pos + direction), 1);
			
			P1Cursor.Position = new Vector2(CENTER + p1Pos * 100, P1Cursor.Position.y);
			charImages[0][p1Pos].Visible = true;
			charImages[0][(p1Pos + 1) % 2].Visible = false;
		}
			
		else if (playerNum == 1 && !p2Selected)
		{
			p2Pos = Math.Min(Math.Max(0, p2Pos + direction), 1);
			P2Cursor.Position = new Vector2(CENTER + p2Pos * 100, P2Cursor.Position.y);

			charImages[1][p2Pos].Visible = true;
			charImages[1][(p2Pos + 1) % 2].Visible = false;
		}
			

		CheckOverlap();
	}

	private void MoveStageSelection(int direction)
	{

		((Sprite) bkgImages[selectedStage]).Visible = false;
		selectedStage = Mod(selectedStage + direction, bkgImages.Count);
		((Sprite)bkgImages[selectedStage]).Visible = true;
	}

	private void SelectPlayer(int playerNum, int color)
	{
		audio.PlaySound("CharSelect");

		if (playerNum == 0 && !p1Selected)
		{
			P1Cursor.Visible = false;
			p1Selected = true;
			p1Color = color;
			if (!p2Selected)
			{
				P2Cursor.Visible = true;
			}
		}
		else if (playerNum == 1 && !p2Selected)
		{
			P2Cursor.Visible = false;
			p2Selected = true;
			p2Color = color;
		}
		if (p1Selected && p2Selected)
		{
			//GD.Print("both selected");
			if (p2Color == p1Color)
			{
				//GD.Print("colors match");
				if (p2Color == 0)
					p2Color = 1;
				else
					p2Color = 0;
			}
			// This may be called multiple times during rollbacks but it isn't a huge issue
			EmitSignal("CharacterSelected", characterScenes[p1Pos], characterScenes[p2Pos],
				p1Color, p2Color, selectedStage);
		}
			
	}
}
