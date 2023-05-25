using Godot;
using System;
using System.Collections.Generic;

public class CommandRunBase : GroundAttack
{

	[Export]
	public int len = 10;


	[Export]
	public int speed = 450;

	/// <summary>
	/// Used because this move has two instances
	/// </summary>
	protected string exitState;

    public override string animationName { get { return "CommandRun"; } }


    public override void _Ready()
	{
		base._Ready();
		loop = true;
		exitState = "Hojogiri";
		turnAroundOnExit = false;

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
			EmitSignal(nameof(StateFinished), exitState);
		}

	}
}
