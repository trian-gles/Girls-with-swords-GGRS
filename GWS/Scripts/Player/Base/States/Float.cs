using Godot;
using System;
using static Godot.SpatialMaterial;

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
		stunRemaining += 1 + (int)Math.Max(4 - (int)Math.Ceiling((double)owner.combo / 4), 0);

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

		if (!(launch == Vector2.Zero))
		{
			owner.velocity = launch;
			owner.velocity.y += owner.combo * 20;
		}

		if (effect == BaseAttack.EXTRAEFFECT.LAUNCHER)
		{
			owner.EmitSignal(nameof(Player.GenericGFX), "Launch", owner.otherPlayer.Name);
			if (owner.hasBeenLaunched)
			{
				owner.velocity.y = owner.velocity.y + (float)Math.Floor(owner.velocity.y / 2);
			}
			else
			{
				owner.hasBeenLaunched = true;
			}
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
		else if (knockdown || owner.health <= 0)
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
		if (stunRemaining <= 0)
		{
			owner.EmitSignal("CanTech", owner.Name);
			if (owner.grounded)
				owner.EmitSignal("MissedTech", owner.Name);
		}
		if (owner.grounded)
		{
			if (owner.electrocuted)
			{
				ReceiveElectrocution();
				return;
			}
			else
			{
				owner.grounded = false;
				TryGroundTech();
			}

		}

		if (frameCount == 1)
		{

			if (owner.CheckHeldKeys(new[] { 'p', 'k', 'a' }))
			{
				owner.EmitSignal("Recovery", owner.Name);
				EmitSignal(nameof(StateFinished), "Burst");
			}
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
		//EmitSignal(nameof(StateFinished), "Tech");
		if (owner.CheckHeldKey('p') || owner.CheckHeldKey('k') || owner.CheckHeldKey('s') || Globals.autoTech)
			EmitSignal(nameof(StateFinished), "Tech");
		else
			EmitSignal(nameof(StateFinished), "SoftKD");
	}

    public override void ReceiveHit(Globals.AttackDetails details)
    {
		if (stunRemaining <= 0)
			owner.EmitSignal("MissedTech", owner.Name);
        base.ReceiveHit(details);
    }

	public override GFXStates GetExtraGFXState()
	{
		if (stunRemaining > 0)
			return base.GetExtraGFXState();
		else
			return GFXStates.CANTECH;
    }

}
