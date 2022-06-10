using Godot;
using System;
using System.Collections.Generic;

class TrainingManager : BaseManager
{
	private bool flippedPlayers = false;
	private Label controlledText;

	public override void _Ready()
	{
		base._Ready();
		controlledText = GetNode<Label>("ControlledText");
	}

	public override void _PhysicsProcess(float delta)
	{
		int p1Inputs;
		int p2Inputs;
		if (flippedPlayers)
		{
			p1Inputs = 0;
			p2Inputs = GetInputs("");
		}
		else
		{
			p1Inputs = GetInputs("");
			p2Inputs = 0;
		}
		
		currGame.AdvanceFrame(p1Inputs, p2Inputs);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("switch_players"))
		{
			flippedPlayers = !flippedPlayers;
			if (flippedPlayers)
				controlledText.Text = "P2";
			else
				controlledText.Text = "P1";
		}
			
	}
}
