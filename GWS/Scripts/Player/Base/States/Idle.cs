using Godot;
using System;
using System.Collections.Generic;

public class Idle : State
{
	public override HashSet<string> tags { get; set; } = new HashSet<string>() { "idle", "recovery" };

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
		AddGatling(new List<char[]>() { new char[] { '4', 'p' }, new char[] { '4', 'r' }, new char[] { '4', 'p' } }, () => owner.backdashCooldownRemaining == 0, "Backdash", 
			() => 
			{ 
				owner.velocity.x = owner.speed * -2; 
				if (!owner.facingRight) 
				{ 
					owner.velocity.x *= -1; 
				} 
				owner.backdashCooldownRemaining = 30;
			}, 
			false);
		
	}
	public override void Enter()
	{
		base.Enter();
		// THIS NEEDS TO BE FIXED ASAP
		owner.velocity.y = 0;
		owner.ResetComboAndProration();
		owner.canDoubleJump = true;
		owner.hasDoubleOrSuperJumped = false;
		owner.canAirDash = true;

		if (owner.CheckFlippableHeldKey('4'))
		{
			if (owner.CheckHeldKey('p') && owner.CheckHeldKey('k'))
			{
                owner.ChangeState("Shield");
                return;
            }
                
        }
		if (owner.CheckHeldKey('2'))
		{
			owner.ChangeState("Crouch");
			return;
		}

		if (owner.CheckHeldKey('6'))
		{
			owner.velocity.x = owner.speed;
			
			owner.ChangeState("Walk");
			return;
		}

		else if (owner.CheckHeldKey('4'))
		{
			owner.velocity.x = -owner.speed;
			owner.ChangeState("Walk");
			return;
		}

		else if (owner.CheckHeldKey('8'))
		{
			owner.ChangeState("PreJump");
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

