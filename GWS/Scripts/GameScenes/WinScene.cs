using System;
using System.Collections.Generic;
using Godot;

public class WinScene : BaseGame
{
	private int frame;
	const int TOTALFRAMES = 300;

	private Sprite slWin;

	[Serializable]
	private struct GameState
	{
		public int frame { get; set; }

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
		frame = 0;
		GD.Print("Snail wins!");
	}

	public override byte[] SaveState(int frame)
	{
		var state = new WinScene.GameState();
		state.frame = this.frame;
		return Serialize<WinScene.GameState>(state);
	}

	public override void LoadState(int frame, byte[] buffer, int checksum)
	{
		var state = Deserialize<WinScene.GameState>(buffer);
		this.frame = state.frame;
	}

	public override void AdvanceFrame(int p1Inputs, int p2Inputs)
	{
		base.AdvanceFrame(p1Inputs, p2Inputs);
		frame++;
		
		if (frame == TOTALFRAMES)
		{
			EmitSignal("Rematch");
			slWin.Visible = false;
		}
			
	}

	public override bool AcceptingInputs()
	{
		return false;
	}
}
