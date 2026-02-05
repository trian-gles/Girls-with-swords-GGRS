using System;
using System.Collections.Generic;
using Godot;

public class WinScene : BaseGame
{
	private int winScreenFrame;
	private int[] lastFrameInputs = {0, 0};

	private int[] cursorPositions = {0, 0};

	private bool[] selected = {false, false};

	private MemoryPool memoryPool;

	const int TOTALFRAMES = 300;

	private List<Control> winPortraits = new List<Control>();
	private Control ui;
	private Control cursors;
	private RichTextLabel winnerText;

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


	private Node events;
	private const string MainMenuPressedString = "MainMenuPressed";
	private const string SetCursorCallString = "SetCursor";

	[Serializable]
	private unsafe struct GameState
	{
		public int winScreenFrame { get; set; }
		public int p1Pos {get; set; }
		public int p2Pos {get; set; }
		public BaseManager.CombinedInputs lastFrameInputs { get; set; }
		public bool p1Selected { get; set; }
		public bool p2Selected { get; set; }
	}

	[Signal]
	public delegate void Rematch(string winner);

	[Signal]
	public delegate void ReselectChar(string winner);

	private unsafe static void SerializeState(ref GameState value, byte* buffer)
	{
		*(GameState*)buffer = value;
	}

	private unsafe static GameState DeserializeState(byte* buffer)
	{
		return *(GameState*)buffer;
	}

	private void HideSelf()
	{
		ui.Visible = false;
		foreach (var p in winPortraits)
		{
			p.Visible = false;
		}
	}

	private void ShowSelf()
	{
		ui.Visible = true;
		cursors.Visible = true;
	}

	public override void _Ready()
	{
		base._Ready();
		foreach (var c in GetNode("CanvasLayer/Portraits").GetChildren())
		{
			winPortraits.Add((Control)c);
		}

		if (Globals.mode == Globals.Mode.SYNCTEST || Globals.mode == Globals.Mode.GGPO)
		{
			unsafe
			{
				memoryPool = new MemoryPool(sizeof(GameState), Globals.ROLLBACKDEPTH + 2);
			}
		}
		


		ui = GetNode<Control>("CanvasLayer/UI");
		winnerText = ui.GetNode<RichTextLabel>("RichTextLabel");
		cursors = GetNode<Control>("CanvasLayer/UI/Cursors");
		events = GetNode<Node>("/root/Events");
		HideSelf();
		
	}

	public void Config(string winner, int character)
	{
		ShowSelf();
		winPortraits[character].Visible = true;
		winnerText.Text = winner + " wins!";
		winScreenFrame = 0;

		timeStatus = TimeStatus.SELECT;

		cursorPositions = new int[] { 0, 0};
		selected = new bool[] { false, false };
	}

	private void MoveCursor(int player, int direction)
	{
		cursorPositions[player] += direction + 3;
		cursorPositions[player] %= 3;
	}

	public unsafe override byte[] SaveState(int winScreenFrame)
	{
		var state = new WinScene.GameState();
		state.winScreenFrame = this.winScreenFrame;
		state.p1Pos = this.cursorPositions[0];
		state.p2Pos = this.cursorPositions[1];
		state.lastFrameInputs.SetInputs(lastFrameInputs[0], lastFrameInputs[1]);
		state.p1Selected = this.selected[0];
		state.p2Selected = this.selected[1];
		var arr = memoryPool.Get();
		fixed (byte* p = arr)
		{
			SerializeState(ref state, p);
		}
		
		return arr;
	}

	public unsafe override void LoadState(int winScreenFrame, byte[] buffer, int checksum)
	{
		GameState state;
		fixed (byte* p = buffer)
		{
			state = DeserializeState(p);
		}
		this.winScreenFrame = state.winScreenFrame;
		cursorPositions[0] = state.p1Pos;
		cursorPositions[1] = state.p2Pos;
		this.lastFrameInputs[0] = state.lastFrameInputs.p1Inps;
		this.lastFrameInputs[1] = state.lastFrameInputs.p2Inps;
		this.selected[0] = state.p1Selected;
		this.selected[1] = state.p2Selected;
		if (timeStatus == TimeStatus.FAKEEND && winScreenFrame < falseEndFrame) timeStatus = TimeStatus.SELECT;
		if (timeStatus == TimeStatus.TRUEEND && winScreenFrame < trueEndFrame) timeStatus = TimeStatus.FAKEEND;
	}

	public override void AdvanceFrame(int p1Inps, int p2Inps)
	{
		base.AdvanceFrame(p1Inps, p2Inps);
		winScreenFrame++;

		int[] combinedInputs = new int[] { p1Inps, p2Inps };

		
		if (timeStatus == TimeStatus.SELECT) SelectUpdate(combinedInputs);
		else if (timeStatus == TimeStatus.FAKEEND) FakeEndUpdate();
		else if (timeStatus == TimeStatus.TRUEEND) TrueEndUpdate();
		SyncCursorLocation();
		lastFrameInputs = combinedInputs;
	}

	private void SelectUpdate(int[] combinedInputs)
	{
		for (int i = 0; i <= 1; i++)
		{
			if (selected[i])
				continue;
			int inputs = combinedInputs[i];
			int playerLastFrameInputs = lastFrameInputs[i];

			if ((inputs & 1) != 0 && (playerLastFrameInputs & 1) == 0)
			{
				MoveCursor(i, -1);

			}
			else if ((inputs & 2) != 0 && (playerLastFrameInputs & 2) == 0)
			{
				MoveCursor(i, 1);
			}

			if (AnyButtonPressed(inputs, playerLastFrameInputs))
			{
				selected[i] = true;

				if (winScreenFrame == TOTALFRAMES || (selected[0] && selected[1]) || (Globals.mode != Globals.Mode.GGPO && selected[0]))
				{
					BeginFakeEnd();

				}

			}
		}
	}


	private void FakeEndUpdate()
	{
		if (winScreenFrame == trueEndFrame)
		{
			BeginTrueEnd();
		}
	}

	private void TrueEndUpdate()
	{
		if (winScreenFrame == finishFrame)
		{
			FinishWinScreen();
			HideSelf();
		}
	}


	private void BeginFakeEnd()
	{
		timeStatus = TimeStatus.FAKEEND;
		falseEndFrame = winScreenFrame;
		trueEndFrame = winScreenFrame + 8;
	}

	private void BeginTrueEnd()
	{
		timeStatus = TimeStatus.TRUEEND;
		finishFrame = winScreenFrame + 30;
	}

	private void FinishWinScreen()
	{
		if (cursorPositions[0] == 2 || cursorPositions[1] == 2)
		{
			events.Call("emit_signal", MainMenuPressedString);
		}
		else if (cursorPositions[0] == 1 || cursorPositions[1] == 1)
		{
			EmitSignal(nameof(ReselectChar));
		}
		else
		{
			EmitSignal(nameof(Rematch));
		}
	}

	public override bool AcceptingInputs()
	{
		return (timeStatus != TimeStatus.TRUEEND);
	}

	/// <summary>
	/// Displays the local player's cursor in GGPO mode, otherwise just player 1
	/// </summary>
	private void SyncCursorLocation()
	{
		int pos = 0;
		if (Globals.mode == Globals.Mode.GGPO)
		{
			bool h = Globals.hosting; 
			pos = h ? 0 : 1;
		}
		cursors.Call(SetCursorCallString, cursorPositions[pos]);
		if (selected[pos]) // sloppy, I know...
			cursors.Visible = false;
		
		
	}
}
