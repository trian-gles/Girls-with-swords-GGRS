using Godot;
using System;

public class Float : HitStun
{
	public override void _Ready()
	{
		base._Ready();
		stop = false;
		tags.Add("aerial");
	}

	public override bool DelayInputs()
	{
		return false; // this messes up teching
	}

	public override void Enter()
	{
		base.Enter();
		owner.grounded = false;
		owner.CheckTurnAround();
		stunRemaining += 4;
		
	}

	public override void ReceiveStunDamage(Globals.AttackDetails details)
	{
		details.hitStun += 2;
		base.ReceiveStunDamage(details);
	}

	/// <summary>
	/// I have to override this because float always goes into float!
	/// </summary>
	/// <param name="knockdown"></param>
	/// <param name="launch"></param>
	protected override void EnterHitState(bool knockdown, Vector2 launch, Vector2 collisionPnt, BaseAttack.EXTRAEFFECT effect, BaseAttack.GRAPHICEFFECT gfx)
	{
		GetNode<Node>("/root/Globals").EmitSignal(nameof(PlayerFXEmitted), collisionPnt, "hit", owner.OtherPlayerOnLeft());
		//GD.Print(launch.y);

		if (!(launch == Vector2.Zero))
		{
			owner.velocity = launch;
		}

		if (launch.y == 0)
		{
			owner.velocity.y = -438;
		}
		HandleHitGFX(gfx);

		owner.ComboUp();
		if (effect == BaseAttack.EXTRAEFFECT.GROUNDBOUNCE)
		{
			EmitSignal(nameof(StateFinished), "GroundBounce");
		}
		else if (knockdown)
		{
			EmitSignal(nameof(StateFinished), "AirKnockdown");
		}
		else
		{
			EmitSignal(nameof(StateFinished), "Float");
		}
		
	}

	public override void FrameAdvance()
	{
		frameCount++;
		if (owner.grounded)
		{
			//GD.Print("On ground, knocking down");
			if (stunRemaining > 20)
				EmitSignal(nameof(StateFinished), "Knockdown");
			else
			{
				owner.grounded = false;
				TryGroundTech();
			}
				
			owner.ResetComboAndProration();
		}

		stunRemaining--;

		TryTech();

		//if (frameCount == 9 && owner.internalPos.y < 14000 && owner.velocity.y < -300) 
		//{
		//	owner.EmitSignal(nameof(Player.LevelUp));
		//	EmitSignal(nameof(StateFinished), "AirKnockdown");
		//}
		

		ApplyGravity();
	}

	protected void TryGroundTech()
	{
		if (owner.CheckHeldKey('p') || owner.CheckHeldKey('k') || owner.CheckHeldKey('s') || Globals.autoTech)
			EmitSignal(nameof(StateFinished), "Tech");
		else
			EmitSignal(nameof(StateFinished), "Knockdown");
	}

	protected void TryTech()
	{
		if (stunRemaining == 0)
		{
			if (owner.CheckHeldKey('p') || owner.CheckHeldKey('k') || owner.CheckHeldKey('s') || Globals.autoTech)
				EmitSignal(nameof(StateFinished), "Tech");
		}
	}

}
