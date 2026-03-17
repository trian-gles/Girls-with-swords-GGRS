using Godot;
using System;
using static Godot.SpatialMaterial;

public class Float : HitStun
{
	private const string HitString = "hit";
	private const string LaunchString = "Launch";
	private const string GroundBounceString = "GroundBounce";
	private const string AirKnockdownString = "AirKnockdown";
	private const string FloatStateString = "Float";
	private const string TechString = "Tech";
	private const string SoftKDString = "SoftKD";
	private const string CanTechString = "CanTech";
	private const string MissedTechString = "MissedTech";
	private const string RecoveryString = "Recovery";
	private const string BurstString = "Burst";
	public override void _Ready()
	{
		base._Ready();
		stop = false;
		tags.Add(Globals.Tags.aerial);
		tags.Add(Globals.Tags.techable);
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
		if (details.projectile)
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
		Globals.EmitPlayerFXEmitted(collisionPnt, HitString, owner.OtherPlayerOnLeft());

		if (launch != Vector2.Zero)
		{
			owner.velocity = launch;
			owner.velocity.y += owner.combo * 20;
		}

		if (effect == BaseAttack.EXTRAEFFECT.LAUNCHER)
		{
			owner.EmitSignal(nameof(Player.GenericGFX), LaunchString, owner.otherPlayer.Name);
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
			owner.ChangeState(GroundBounceString);
		}
		else if (knockdown || owner.health <= 0)
		{
			owner.ChangeState(AirKnockdownString);
		}
		else
		{
			owner.ChangeState(FloatStateString);
		}

	}

	public override void FrameAdvance()
	{
		frameCount++;
		if (stunRemaining <= 0)
		{
			Globals.EmitSignal(Globals.PlayerSignal.CanTech, owner.Name);
			if (owner.grounded)
				Globals.EmitSignal(Globals.PlayerSignal.MissedTech, owner.Name);
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
				if (!owner.TrySpendBurst()) return;
				owner.EmitSignal(RecoveryString, owner.Name);
				owner.ChangeState(BurstString);
			}
		}
		
		stunRemaining--;

		TryTech();

		//if (frameCount == 9 && owner.internalPos.y < 14000 && owner.velocity.y < -300) 
		//{
		//	owner.EmitSignal(nameof(Player.LevelUp));
		//	owner.ChangeState("AirKnockdown");
		//}


		ApplyGravity();
	}

	protected void TryGroundTech()
	{
		//owner.ChangeState("Tech");
		if (owner.CheckHeldKey('p') || owner.CheckHeldKey('k') || owner.CheckHeldKey('s') || Globals.autoTech)
			owner.ChangeState(TechString);
		else
			owner.ChangeState(SoftKDString);
	}

    public override void ReceiveHit(Globals.AttackDetails details)
    {
		if (stunRemaining <= 0)
			owner.EmitSignal(MissedTechString, owner.Name);
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
