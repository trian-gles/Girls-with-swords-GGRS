using System;
using System.Collections.Generic;
using Godot;

public class WinScene : BaseGame
{
	private int winScreenFrame;
	private int p1Pos;
	private int p2Pos;
	private bool p1Selected;
	private bool p2Selected;
	private int[] lastFrameInputs;

	const int TOTALFRAMES = 300;

	private Sprite slWin;

	[Serializable]
	private struct GameState
	{
		public int winScreenFrame { get; set; }
		public int p1Pos {get; set; }
		public int p2Pos {get; set; }
		public bool p1Selected {get; set; }
		public bool p2Selected {get; set; }
		public int[] lastFrameInputs { get; set; }
	}

	[Signal]
	public delegate void Rematch(string winner);

	public override void _Ready()
	{
		base._Ready();
		slWin = GetNode<Sprite>("CanvasLayer/SnailWin");
	}

	public void Config(string winner)
	{
		slWin.Visible = true;
		winScreenFrame = 0;
		GD.Print("Snail wins!");
	}

	private void MoveCursor(int player, int direction)
	{

	}

	public override byte[] SaveState(int winScreenFrame)
	{
		var state = new WinScene.GameState();
		state.winScreenFrame = this.winScreenFrame;
		state.p1Pos = this.p1Pos;
		state.p2Pos = this.p2Pos;
		state.p1Selected = this.p1Selected;
		state.p2Selected = this.p2Selected;
		state.lastFrameInputs = lastFrameInputs;
		return Serialize<WinScene.GameState>(state);
	}

	public override void LoadState(int winScreenFrame, byte[] buffer, int checksum)
	{
		var state = Deserialize<WinScene.GameState>(buffer);
		this.winScreenFrame = state.winScreenFrame;
		this.winScreenFrame = state.winScreenFrame;
		this.p1Pos = state.p1Pos;
		this.p2Pos = state.p2Pos;
		this.p1Selected = state.p1Selected;
		this.p2Selected = state.p2Selected;
		this.lastFrameInputs = state.lastFrameInputs;
	}

	public override void AdvanceFrame(int p1Inps, int p2Inps)
	{
		base.AdvanceFrame(p1Inps, p2Inps);
		winScreenFrame++;
		
		if (winScreenFrame == TOTALFRAMES)
		{
			EmitSignal("Rematch");
			slWin.Visible = false;
		}

		int[] combinedInputs = new int[] { p1Inps, p2Inps };

		for (int i = 0; i <= 1; i++)
		{
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
		}



		lastFrameInputs = combinedInputs;
	}

	public override bool AcceptingInputs()
	{
		return false;
	}
}
