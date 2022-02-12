using Godot;
using System;
using System.Collections.Generic;

public class Idle : State
{
	public override void _Ready()
	{
		base._Ready();
		loop = true;
		
		AddGatling(new[] { '2', 'p' }, "Crouch");
		AddGatling(new[] { '6', 'p' }, "Walk", () => owner.velocity.x = owner.speed);
		AddGatling(new[] { '4', 'p' }, "Walk", () => owner.velocity.x = -owner.speed);
		AddGatling(new[] { '8', 'p' }, "Jump");
		AddGatling(new[] { 'p', 'p' }, "Jab");
		AddGatling(new[] { 'k', 'p' }, "Kick");
		AddGatling(new[] { 's', 'p' }, "Slash");
		
		AddGatling(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '6', 'p' } }, "Run", () => { owner.velocity.x = owner.speed; if (!owner.facingRight) { owner.velocity.x *= -1; } }, false);
		AddGatling(new List<char[]>() { new char[] { '4', 'p' }, new char[] { '4', 'p' } }, "Backdash", () => { owner.velocity.x = owner.speed * -2; if (!owner.facingRight) { owner.velocity.x *= -1; } }, false);
		
	}
	public override void Enter()
	{
		base.Enter();
		owner.ResetComboAndProration();
		owner.canDoubleJump = true;
		owner.velocity.x = 0;
		owner.velocity.y = 0;
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
			EmitSignal(nameof(StateFinished), "Jump");
			return;
		}
	} 

	public override void FrameAdvance()
	{
		base.FrameAdvance();
		owner.velocity.x = 0;
		owner.CheckTurnAround();
	}
}

