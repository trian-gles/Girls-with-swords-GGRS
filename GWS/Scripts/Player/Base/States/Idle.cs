using Godot;
using System;
using System.Collections.Generic;

public class Idle : State
{
	public override HashSet<string> tags { get; set; } = new HashSet<string>() { "idle" };

	public override void _Ready()
	{
		
		base._Ready();
		loop = true;
		AddSpecials(owner.groundSpecials);
		AddCommandNormals(owner.commandNormals);
		AddEasyGroundSpecials();

		AddGatling(new[] { '2', 'p' }, "Crouch");
		AddGatling(new[] { '6', 'p' }, "Walk", () => owner.velocity.x = owner.speed);
		AddGatling(new[] { '4', 'p' }, "Walk", () => owner.velocity.x = -owner.speed);
		AddGatling(new[] { '8', 'p' }, "PreJump");
		AddNormals();
		

		AddGatling(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '6', 'r' }, new char[] { '6', 'p' } }, "PreRun", () => { owner.velocity.x = owner.speed; if (!owner.facingRight) { owner.velocity.x *= -1; } }, false);
		AddGatling(new List<char[]>() { new char[] { '4', 'p' }, new char[] { '4', 'r' }, new char[] { '4', 'p' } }, "Backdash", () => { owner.velocity.x = owner.speed * -2; if (!owner.facingRight) { owner.velocity.x *= -1; } }, false);
		
	}
	public override void Enter()
	{
		base.Enter();
		// THIS NEEDS TO BE FIXED ASAP
		owner.velocity.y = 0;
		owner.ResetComboAndProration();
		owner.canDoubleJump = true;
		owner.canAirDash = true;
		if (owner.CheckHeldKey('2'))
		{
			EmitSignal(nameof(StateFinished), "Crouch");
			return;
		}

		if (owner.CheckHeldKey('6'))
		{
			owner.velocity.x = owner.speed;
			
			EmitSignal(nameof(StateFinished), "Walk");
			return;
		}

		else if (owner.CheckHeldKey('4'))
		{
			owner.velocity.x = -owner.speed;
			EmitSignal(nameof(StateFinished), "Walk");
			return;
		}

		else if (owner.CheckHeldKey('8'))
		{
			EmitSignal(nameof(StateFinished), "PreJump");
			return;
		}
	}

	public override void HandleInput(char[] inputArr)
	{
		base.HandleInput(inputArr);
	}

	public override void FrameAdvance()
	{
		base.FrameAdvance();
		owner.velocity.x = 0;
		owner.CheckTurnAround();
	}
}

