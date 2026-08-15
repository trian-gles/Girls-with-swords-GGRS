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
	private AnimationTree animationTree;
	
	private CharSelectAudio audio;

	private Control scrollText;
	private AudioStreamPlayer charSelectMusic;
	private List<List<Sprite>> charImages;
	
	private Godot.Collections.Array bkgImages;

	private MemoryPool memoryPool;

	private int p1Pos = 0;
	private int p2Pos = 1;
	public bool p1Selected = false; // public as this is required for AI selection
	public bool p2Selected = false;
	private bool stageSelected = false;
	private int selectStagePlayer = 0;
	private int p1Color;
	private int p2Color;
	private BaseManager.CombinedInputs lastInputs;
	private int charSelectFrame = 0;
	private int selectedStage = 0;

	// runtime string constants
	private const string CharSelectSoundString = "CharSelect";
	private const string AnimRightString = "Right";
	private const string AnimLeftString = "Left";
	private const string ChooseStageCallString = "choose_stage";
	private const string ResetCallString = "reset";

	private enum TimeStatus
	{
		SELECT,
		FAKEEND,
		TRUEEND
	}

	private TimeStatus timeStatus = TimeStatus.SELECT;

	private int trueEndFrame = 0;
	private int falseEndFrame = 0;
	private int finishFrame = 0;


	[Serializable]
	private unsafe struct GameState
	{
		public int p1Pos { get; set; }
		public int p2Pos { get; set; }
		public bool p1Selected { get; set; }
		public bool p2Selected { get; set; }
		public bool stageSelected { get; set; }
		public int selectStagePlayer { get; set; }
		public int p1Color { get; set; }
		public int p2Color	{ get; set; }
		public BaseManager.CombinedInputs lastFrameInputs { get; set; }
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
		animationTree = GetNode<AnimationTree>("CanvasLayer/StartupAnimation");
		HUDText = GetNode<Control>("CanvasLayer/DebugText");
		base._Ready();
		unsafe
		{
			memoryPool = new MemoryPool(sizeof(GameState), Globals.ROLLBACKDEPTH * 2 + 3);
		}
		characterScenes = new List<PackedScene>() { OLScene, GLScene };
		lastInputs.SetInputs(0, 0);
		P1Cursor = GetNode<Sprite>("CanvasLayer/P1Cursor");
		P2Cursor = GetNode<Sprite>("CanvasLayer/P2Cursor");

		audio = GetNode<CharSelectAudio>("CharSelectAudio");
		// cache frequently used nodes
		scrollText = GetNode<Control>("CanvasLayer/ScrollText");
		charSelectMusic = GetNode<AudioStreamPlayer>("CharSelectMusic");

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

		HighlightChar(1, 1);

		//		CheckOverlap();

	}

	public unsafe override bool CompareStates(byte[] serializedOldState)
	{
		base.CompareStates(serializedOldState);
		GameState oldState;
		fixed (byte* p = serializedOldState)
		{
			oldState = DeserializeState(p);
		}
		CompareValues(p1Pos, oldState.p1Pos, "p1Pos");
		CompareValues(p2Pos, oldState.p2Pos, "p2Pos");
		CompareValues(p1Color, oldState.p1Color, "p1Color");
		CompareValues(p2Color, oldState.p2Color, "p2Color");
		CompareValues(p1Selected, oldState.p1Selected, "p1Selected");
		CompareValues(p2Selected, oldState.p2Selected, "p2Selected");
		CompareValues(charSelectFrame, oldState.charSelectFrame, "Char select frame");
		CompareValues(selectedStage, oldState.selectedStage, "selectedStage");
		CompareValues(stageSelected, oldState.stageSelected, "stage selected");
		CompareValues(charSelectFrame, oldState.charSelectFrame, "char select frame");
		CompareValues(selectStagePlayer, oldState.selectStagePlayer, "char select frame");
		CompareValues(lastInputs.p1Inps, oldState.lastFrameInputs.p1Inps, "p1 last frame inputs");
		CompareValues(lastInputs.p2Inps, oldState.lastFrameInputs.p2Inps, "p2 last frame inputs");

		return true;
	
	}

	private unsafe static void SerializeState(ref GameState value, byte* buffer)
	{
		*(GameState*)buffer = value;
	}

	private unsafe static GameState DeserializeState(byte* buffer)
	{
		return *(GameState*)buffer;
	}


	public unsafe override byte[] SaveState(int frame)
	{
		var state = new GameState
		{
			p1Color = p1Color,
			p2Color = p2Color,
			p1Pos = p1Pos,
			p2Pos = p2Pos,
			p1Selected = p1Selected,
			p2Selected = p2Selected,
			stageSelected = stageSelected,
			lastFrameInputs = lastInputs,
			selectedStage = selectedStage,
			charSelectFrame = charSelectFrame,
			selectStagePlayer = selectStagePlayer
		};

		var arr = memoryPool.Get();
		fixed (byte* p = arr)
		{
			SerializeState(ref state, p);
		}
		
		return arr;
	}

	public unsafe override void LoadState(int frame, byte[] buffer, int checksum)
	{
		GameState state;
		fixed (byte* p = buffer)
		{
			state = DeserializeState(p);
		}
		p1Selected = state.p1Selected;
		p2Selected = state.p2Selected;
		p1Color = state.p1Color;
		p2Color = state.p2Color;
		stageSelected = state.stageSelected;

		p1Pos = state.p1Pos;
		p2Pos = state.p2Pos;
		lastInputs = state.lastFrameInputs;
		charSelectFrame = state.charSelectFrame;
		selectStagePlayer = state.selectStagePlayer;

		// Cleanup background selection
		((Sprite)bkgImages[selectedStage]).Visible = false;
		selectedStage = state.selectedStage;
		((Sprite)bkgImages[selectedStage]).Visible = true;

		// Cleanup selection images
		HighlightChar(0, p1Pos);

		HighlightChar(1, p2Pos);

		if (timeStatus == TimeStatus.FAKEEND && charSelectFrame < falseEndFrame) timeStatus = TimeStatus.SELECT;
		if (timeStatus == TimeStatus.TRUEEND && charSelectFrame < trueEndFrame) timeStatus = TimeStatus.FAKEEND;

		//		CheckOverlap();
	}
	
	private BaseManager.CombinedInputs combinedInputs;

	public override void AdvanceFrame(int p1Inps, int p2Inps)
	{
		charSelectFrame++;
		if (charSelectFrame < 120)
			return;

		combinedInputs = new BaseManager.CombinedInputs
		{
			p1Inps = p1Inps,
			p2Inps = p2Inps
		};

		if (timeStatus == TimeStatus.SELECT) SelectUpdate(combinedInputs);
		else if (timeStatus == TimeStatus.FAKEEND) FakeEndUpdate();
		else if (timeStatus == TimeStatus.TRUEEND) TrueEndUpdate();
		lastInputs.SetInputs(p1Inps, p2Inps);
	}

	

	/// <summary>
	/// After both players are selected we decline inputs to prevent rollbacks
	/// </summary>
	/// <returns></returns>
	public override bool AcceptingInputs()
	{
		return (timeStatus != TimeStatus.TRUEEND);
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

	private void HighlightChar(int playerNum, int sprite)
	{
		foreach (var charImg in charImages[playerNum])
			charImg.Visible = false;

		if (playerNum == 0)
			P1Cursor.Position = CalcCursor(p1Pos, p1TopPos);
		else
			P2Cursor.Position = CalcCursor(p2Pos, p2TopPos);

		charImages[playerNum][sprite].Visible = true;
		CheckOverlap();
	}

	private Vector2 CalcCursor(int pos, int top)
	{
		int y = pos < 2 ? 0 : 1;
		int x = pos % 2;

		return new Vector2(CENTERX + x * 80, top + y * 80);
	}

	private void MoveCursor(int playerNum, int movement)
	{
		if (playerNum == 0)
		{
			if (!p1Selected) {
				p1Pos = Math.Min(Math.Max(0, p1Pos + movement), 3);
				
				HighlightChar(playerNum, p1Pos);
			}
			else if (selectStagePlayer == playerNum && Math.Abs(movement) == 2)
			{
				MoveStageSelection(movement / 2);
			}
			
		}
			
		else if (playerNum == 1)
		{
			if (!p2Selected) {
				p2Pos = Math.Min(Math.Max(0, p2Pos + movement), 3);
				HighlightChar(playerNum, p2Pos);
			}
			else if (selectStagePlayer == playerNum && Math.Abs(movement) == 2)
			{
				MoveStageSelection(movement / 2);
			}
		}
			
	
//		CheckOverlap();
	}

	private void MoveStageSelection(int direction)
	{

		((Sprite) bkgImages[selectedStage]).Visible = false;
		selectedStage = Mod(selectedStage + direction, bkgImages.Count);
		((Sprite)bkgImages[selectedStage]).Visible = true;
	}

	public void AutoSelectP2GL()
	{
		P2Cursor.Visible = false;
		p2Selected = true;
		p2Color = 0;
		p2Pos = 1;
		scrollText.Visible = false;

	}

	private void SelectPlayer(int playerNum, int color)
	{
		
		if (playerNum == 0 && !p1Selected)
		{
			audio.PlaySound(CharSelectSoundString);
			P1Cursor.Visible = false;
			p1Selected = true;
			p1Color = color;
			if (!p2Selected)
			{
				P2Cursor.Visible = true;
				selectStagePlayer = playerNum;
			}
		}
		else if (playerNum == 1 && !p2Selected)
		{
			P2Cursor.Visible = false;
			p2Selected = true;
			p2Color = color;
			audio.PlaySound(CharSelectSoundString);
			if (!p1Selected)
			{
				selectStagePlayer = playerNum;
			}
		}
		else if (playerNum == selectStagePlayer)
		{
			stageSelected = true;
		}

		if (p1Selected && p2Selected)
		{
			if (stageSelected)
			{
				audio.PlaySound(CharSelectSoundString);
				if (p2Color == p1Color && p1Pos == p2Pos)
				{
					if (p2Color == 0)
						p2Color = 1;
					else
						p2Color = 0;
				}
				BeginFakeEnd();
				
			}
			else
			{
				scrollText.Call(ChooseStageCallString, selectStagePlayer);
			}
		}

	}

	private void SelectUpdate(BaseManager.CombinedInputs combinedInputs)
	{
		for (int i = 0; i <= 1; i++)
		{
			int inputs = combinedInputs.GetInputs(i);
			int lastFrameInputs = lastInputs.GetInputs(i);

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
				if (i == 0)
				{
					animationPlayer.Play(AnimRightString);
				}
			}

			if ((inputs & 8) != 0 && (lastFrameInputs & 8) == 0)
			{
				//left
				MoveCursor(i, -1);
				if (i == 0)
				{
					animationPlayer.Play(AnimLeftString);
				}
			}

			if ((inputs & 16) != 0 && (lastFrameInputs & 16) == 0)
			{
				SelectPlayer(i, 3);
			}

			if ((inputs & 32) != 0 && (lastFrameInputs & 32) == 0)
			{
				SelectPlayer(i, 1);
			}

			if ((inputs & 64) != 0 && (lastFrameInputs & 64) == 0)
			{
				SelectPlayer(i, 2);
			}

			if ((inputs & 128) != 0 && (lastFrameInputs & 128) == 0)
			{
				SelectPlayer(i, 0);
			}
		}
	}

	private void FakeEndUpdate()
	{
		if (charSelectFrame == trueEndFrame)
		{
			BeginTrueEnd();
		}
	}

	private void TrueEndUpdate()
	{
		if (charSelectFrame == finishFrame)
		{
			StopMusic();
			manager.OnCharactersSelected(p1Pos, p2Pos, p1Color, p2Color, selectedStage);
		}
	}

	public void StopMusic()
	{
		charSelectMusic.Stop();
	}


	private void BeginFakeEnd()
	{
		timeStatus = TimeStatus.FAKEEND;
		falseEndFrame = charSelectFrame;
		trueEndFrame = charSelectFrame + 8;
	}

	private void BeginTrueEnd()
	{
		timeStatus = TimeStatus.TRUEEND;
		finishFrame = charSelectFrame + 30;
	}

	public override void ResetRound()
	{
		base.ResetRound();
		p1Selected = false;
		p2Selected = false;
		stageSelected = false;
		scrollText.Call(ResetCallString);
	}

	public void Reload()
	{
		ShowAll();
		p1Selected = false;
		p2Selected = false;
		p1Pos = 0;
		p2Pos = 1;
		selectedStage = 0;
		charSelectFrame = 0;

		HighlightChar(0, p1Pos);
		HighlightChar(1, p2Pos);
		var stateMachine = (AnimationNodeStateMachinePlayback) animationTree.Get("parameters/playback");
		stateMachine.Start("Init");
		lastInputs.SetInputs(16 + 32 + 64, 16 + 32 + 64); // prevent held down keys from immediately selecting
		ResetRound();
		
		
		for (int i = 0; i < bkgImages.Count; i++)
		{
			Sprite bkgImage = (Sprite)bkgImages[i];
			bkgImage.Visible = false;
		}

		((Sprite)bkgImages[selectedStage]).Visible = true;
		timeStatus = TimeStatus.SELECT;
	}
}
