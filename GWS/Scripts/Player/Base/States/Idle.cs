using Godot;
using System;
using System.Collections.Generic;

public class Idle : State
{
	private const string CrouchString = "Crouch";
	private const string WalkString = "Walk";
	private const string PreJumpString = "PreJump";
	private const string PreRunString = "PreRun";
	private const string BackdashString = "Backdash";
	private const string ShieldString = "Shield";
	public override HashSet<Globals.Tags> tags { get; set; } = new HashSet<Globals.Tags> { Globals.Tags.idle };

	public override void _Ready()
	{
		
		base._Ready();
		loop = true;
		AddSpecials(owner.groundSpecials);
		AddCommandNormals(owner.commandNormals);
		AddEasyGroundSpecials();

		AddGatling(new[] { '2', 'p' }, CrouchString);
		AddGatling(new[] { '6', 'p' }, WalkString, () => owner.velocity.x = owner.speed);
		AddGatling(new[] { '4', 'p' }, WalkString, () => owner.velocity.x = -owner.speed);
		AddGatling(new[] { '8', 'p' }, PreJumpString);
		AddNormals();
		

		AddGatling(new InputContainer( new[]{ new char[] { '6', 'p' }, new char[] { '6', 'r' }, new char[] { '6', 'p' } }), PreRunString, () => { owner.velocity.x = owner.speed; if (!owner.facingRight) { owner.velocity.x *= -1; } }, false);
		AddGatling(new InputContainer( new[] { new char[] { '4', 'p' }, new char[] { '4', 'r' }, new char[] { '4', 'p' } }), () => owner.backdashCooldownRemaining == 0, BackdashString, 
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
				owner.ChangeState(ShieldString);
                return;
            }
                
        }
		if (owner.CheckHeldKey('2'))
		{
			owner.ChangeState(CrouchString);
			return;
		}

		if (owner.CheckHeldKey('6'))
		{
			owner.velocity.x = owner.speed;
			
			owner.ChangeState(WalkString);
			return;
		}

		else if (owner.CheckHeldKey('4'))
		{
			owner.velocity.x = -owner.speed;
			owner.ChangeState(WalkString);
			return;
		}

		else if (owner.CheckHeldKey('8'))
		{
			owner.ChangeState(PreJumpString);
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

