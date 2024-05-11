using System;
using System.Collections.Generic;
using Godot;

public class WinScene : BaseGame
{
	private int winScreenFrame;
	private int[] lastFrameInputs = {0, 0};

	private int[] cursorPositions = {0, 0};

	private bool[] selected = {false, false};

	const int TOTALFRAMES = 300;

	private Sprite slWin;
	private Control ui;
	private Control cursors;

	private Node events;

	[Serializable]
	private struct GameState
	{
		public int winScreenFrame { get; set; }
		public int p1Pos {get; set; }
		public int p2Pos {get; set; }
		public int[] lastFrameInputs { get; set; }
		public bool[] selected { get; set; }
	}

	[Signal]
	public delegate void Rematch(string winner);

	[Signal]
	public delegate void ReselectChar(string winner);

	private void HideSelf()
	{
		ui.Visible = false;
		slWin.Visible = false;
	}

	private void ShowSelf()
	{
		ui.Visible = true;
		slWin.Visible = true;
		cursors.Visible = true;
	}

	public override void _Ready()
	{
		base._Ready();
		slWin = GetNode<Sprite>("CanvasLayer/SnailWin");
		ui = GetNode<Control>("CanvasLayer/UI");
		cursors = GetNode<Control>("CanvasLayer/UI/Cursors");
		events = GetNode<Node>("/root/Events");
		HideSelf();
		
	}

	public void Config(string winner)
	{
		ShowSelf();
		winScreenFrame = 0;
		
		cursorPositions = new int[] { 0, 0};
		selected = new bool[] { false, false };
	}

	private void MoveCursor(int player, int direction)
	{
		cursorPositions[player] += direction + 3;
		cursorPositions[player] %= 3;
	}

	public override byte[] SaveState(int winScreenFrame)
	{
		var state = new WinScene.GameState();
		state.selected = new bool[2];
		state.winScreenFrame = this.winScreenFrame;
		state.p1Pos = this.cursorPositions[0];
		state.p2Pos = this.cursorPositions[1];
		state.lastFrameInputs = lastFrameInputs;
		state.selected[0] = this.selected[0];
		state.selected[1] = this.selected[1];
		return Serialize<WinScene.GameState>(state);
	}

	public override void LoadState(int winScreenFrame, byte[] buffer, int checksum)
	{
		var state = Deserialize<WinScene.GameState>(buffer);
		this.winScreenFrame = state.winScreenFrame;
		this.winScreenFrame = state.winScreenFrame;
		cursorPositions[0] = state.p1Pos;
		cursorPositions[1] = state.p2Pos;
		this.lastFrameInputs = state.lastFrameInputs;
		this.selected[0] = state.selected[0];
		this.selected[1] = state.selected[1];
	}

	public override void AdvanceFrame(int p1Inps, int p2Inps)
	{
		base.AdvanceFrame(p1Inps, p2Inps);
		winScreenFrame++;

		int[] combinedInputs = new int[] { p1Inps, p2Inps };

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

			if ((inputs & 16) != 0 && (playerLastFrameInputs & 16) == 0)
			{
				selected[i] = true;

				if (winScreenFrame == TOTALFRAMES || (selected[0] && selected[1]) || (Globals.mode != Globals.Mode.GGPO && selected[0]))
				{
					FinishWinScreen();
					HideSelf();
				}

			}
		}


		SyncCursorLocation();
		lastFrameInputs = combinedInputs;
	}

	private void FinishWinScreen()
	{
		if (cursorPositions[0] == 2 || cursorPositions[1] == 2)
		{
			events.Call("emit_signal", "MainMenuPressed");
		}
		else if (cursorPositions[0] == 1 || cursorPositions[1] == 1)
		{
			EmitSignal("ReselectChar");
		}
		else
		{
			EmitSignal("Rematch");
		}
	}

	public override bool AcceptingInputs()
	{
		return (!selected[0] || !selected[1]);
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
		cursors.Call("SetCursor", cursorPositions[pos]);
		if (selected[pos]) // sloppy, I know...
			cursors.Visible = false;
		
		
	}
}
