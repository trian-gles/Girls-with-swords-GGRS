using Godot;
using System;
using System.Collections.Generic;

public class CommandRun : GroundAttack
{
	[Export]
	public int minLen = 10;

	[Export]
	public int maxLen = 80;

	[Export]
	public int speed = 450;

	/// <summary>
	/// Used because this move has two instances
	/// </summary>
	protected string exitState;

	public override void _Ready()
	{
		base._Ready();
		loop = true;
		exitState = "Hojogiri";

		AddRhythmSpecials(owner.rhythmSpecials);

	}
	public override void Enter()
	{
		base.Enter();
		if (owner.facingRight)
		{
			owner.velocity.x = speed;
		}
		else
		{
			owner.velocity.x = -speed;
		}
	}


	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (frameCount > maxLen)
		{
			EmitSignal(nameof(StateFinished), exitState);
		}
		
	}
}
