using Godot;
using System;
using System.Collections.Generic;

public class CommandRunBase : GroundAttack
{
	private const string HojogiriString = "Hojogiri";
	private const string CommandRunAnimString = "CommandRun";

	[Export]
	public int len = 10;


	[Export]
	public int speed = 450;

	/// <summary>
	/// Used because this move has two instances
	/// </summary>

	public override string animationName { get { return CommandRunAnimString; } }

	public virtual string GetNextState()
	{
		return HojogiriString;
	}
	public override void _Ready()
	{
		base._Ready();
		loop = true;
		turnAroundOnExit = false;
		slowdownSpeed = 0;

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
		if (frameCount > len)
		{
			owner.ChangeState(GetNextState());
		}

	}

	public override void Exit()
	{
		base.Exit();

		
	}
}
