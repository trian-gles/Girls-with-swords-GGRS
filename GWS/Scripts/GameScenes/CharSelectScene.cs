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

	[Export]
	public PackedScene SLScene;

	private Sprite P1Cursor;
	private Sprite P2Cursor;
	
	private Godot.AnimationPlayer animationPlayer;
	
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
	private int charSelectFrame = 0;
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
		public int charSelectFrame { get; set; }

	}

	private int CENTERX;//where the most left character is
	private int p1TopPos;  // upper point for p1 cursor
	private int p2TopPos; // upper point for p2 cursor

	

	[Signal]
	public delegate void CharacterSelected(int playerOne, int playerTwo, int colorOne, int colorTwo);

	private List<PackedScene> characterScenes;

	public override void _Ready()
	{
		animationPlayer = GetNode<Godot.AnimationPlayer>("CanvasLayer/P1ColorSelect/Animation");
		
		HUDText = GetNode<Label>("CanvasLayer/DebugText");
		base._Ready();
		characterScenes = new List<PackedScene>() { OLScene, GLScene };
		lastInputs = new int[2] { 0, 0 };
		P1Cursor = GetNode<Sprite>("CanvasLayer/P1Cursor");
		P2Cursor = GetNode<Sprite>("CanvasLayer/P2Cursor");

		audio = GetNode<CharSelectAudio>("CharSelectAudio");

		var p1CharImages = new List<Sprite>() {
			GetNode<Sprite>("CanvasLayer/P1Selected/OLSprite"),
			GetNode<Sprite>("CanvasLayer/P1Selected/GLSprite"),
			GetNode<Sprite>("CanvasLayer/P1Selected/SLSprite"),
			GetNode<Sprite>("CanvasLayer/P1Selected/HLSprite")
		};

		var p2CharImages = new List<Sprite>() {
			GetNode<Sprite>("CanvasLayer/P2Selected/OLSprite"),
			GetNode<Sprite>("CanvasLayer/P2Selected/GLSprite"),
			GetNode<Sprite>("CanvasLayer/P2Selected/SLSprite"),
			GetNode<Sprite>("CanvasLayer/P2Selected/HLSprite")
		};

		charImages = new List<List<Sprite>>() { p1CharImages, p2CharImages };

		bkgImages = GetNode("CanvasLayer/Bkgs").GetChildren();

		p1TopPos = (int)P1Cursor.Position.y;
		p2TopPos = (int)P2Cursor.Position.y;

		CENTERX = (int)P1Cursor.Position.x;


		//		CheckOverlap();

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
		CompareValues(charSelectFrame, oldState.charSelectFrame, "Char select frame");
	
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
		state.charSelectFrame = charSelectFrame;
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
		charSelectFrame = state.charSelectFrame;

		// Cleanup background selection
		((Sprite)bkgImages[selectedStage]).Visible = false;
		selectedStage = state.selectedStage;
		((Sprite)bkgImages[selectedStage]).Visible = true;

		// Cleanup selection images
		HighlightChar(0, p1Pos);

		HighlightChar(1, p2Pos);



//		CheckOverlap();
	}

	public override void AdvanceFrame(int p1Inps, int p2Inps)
	{
		charSelectFrame++;
		if (charSelectFrame < 120)
			return;

		int[] combinedInputs = new int[] { p1Inps, p2Inps };
		for (int i = 0; i <= 1; i++)
		{
			int inputs = combinedInputs[i];
			int lastFrameInputs = lastInputs[i];

			if ((inputs & 1) != 0 && (lastFrameInputs & 1) == 0)
			{
				//MoveStageSelection(-1);
				//up
				MoveCursor(i, -2);

			}

			if ((inputs & 2) != 0 && (lastFrameInputs & 2) == 0)
			{
				//MoveStageSelection(1);
				//down
				MoveCursor(i, 2);
			}

			if ((inputs & 4) != 0 && (lastFrameInputs & 4) == 0)
			{
				//right
				MoveCursor(i, 1);
				if (i==0){
					animationPlayer.Play("Right");
				}
			}

			if ((inputs & 8) != 0 && (lastFrameInputs & 8) == 0)
			{
				//left
				MoveCursor(i, -1);
				if (i==0){
					animationPlayer.Play("Left");
				}
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

//	private void CheckOverlap()
//	{
//		if (p1Pos == p2Pos && !p1Selected && !p2Selected)
//		{
//			P1Cursor.Texture = BothTexture;
//			P2Cursor.Visible = false;
//		}
//		else
//		{
//			P1Cursor.Texture = P1Texture;
//			if (!p2Selected)
//				P2Cursor.Visible = true;
//		}
//	}

	int Mod(int x, int m)
	{
		int r = x % m;
		return r < 0 ? r + m : r;
	}

	private void HighlightChar(int playerNum, int sprite)
	{
		foreach (var charImg in charImages[playerNum])
			charImg.Visible = false;

		charImages[playerNum][sprite].Visible = true;
	}

	private Vector2 CalcCursor(int pos, int top)
	{
		int y = pos < 2 ? 0 : 1;
		int x = pos % 2;

		return new Vector2(CENTERX + x * 80, top + y * 80);
	}

	private void MoveCursor(int playerNum, int movement)
	{
		
		if (playerNum == 0 && !p1Selected)
		{
			
			p1Pos = Math.Min(Math.Max(0, p1Pos + movement), 3);

			P1Cursor.Position = CalcCursor(p1Pos, p1TopPos);
			HighlightChar(playerNum, p1Pos);
		}
			
		else if (playerNum == 1 && !p2Selected)
		{
			p2Pos = Math.Min(Math.Max(0, p2Pos + movement), 3);
			P2Cursor.Position = CalcCursor(p2Pos, p2TopPos);

			HighlightChar(playerNum, p2Pos);
		}
			
	
//		CheckOverlap();
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
			if (p2Color == p1Color && p1Pos == p2Pos)
			{
				//GD.Print("colors match");
				if (p2Color == 0)
					p2Color = 1;
				else
					p2Color = 0;
			}
			// This may be called multiple times during rollbacks but it isn't a huge issue
			EmitSignal("CharacterSelected", p1Pos, p2Pos,
				p1Color, p2Color, selectedStage);
		}
			
	}

	public override void Reset()
	{
		base.Reset();
		p1Selected = false;
		p2Selected = false;
	}

	public void Reload()
	{
		ShowAll();
		lastInputs = new[] { 16 + 32 + 64, 16 + 32 + 64 }; // prevent held down keys from immediately selecting
		p1Selected = false;
		p2Selected = false;
		HighlightChar(0, p1Pos);

		HighlightChar(1, p2Pos);

		foreach (Sprite bkg in bkgImages)
		{
			bkg.Visible = false;
		}

		((Sprite)bkgImages[selectedStage]).Visible = true;
	}
}
