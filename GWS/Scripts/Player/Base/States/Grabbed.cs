using Godot;
using System;
using System.Collections.Generic;

public class Grabbed : State
{
	private const string IdleString = "Idle";
	private const string CrouchString = "Crouch";
	private const string WalkString = "Walk";
	private const string JumpString = "Jump";
	private const string FallString = "Fall";
	private const string ThrowBreakString = "ThrowBreak";
	private const string GrabbedGfxString = "Grabbed";
	private const string AirKnockdownString = "AirKnockdown";
	private HashSet<string> techableStates = new HashSet<string>() { IdleString, CrouchString, WalkString, JumpString, FallString };
	public override void _Ready()
	{
		base._Ready();
		AddGatling(new char[] { 'k', 'p' }, CanThrowBreak, ThrowBreakString, () => owner.otherPlayer.ChangeState(ThrowBreakString));
		AddGatling(new char[] { 's', 'p' }, CanThrowBreak, ThrowBreakString, () => owner.otherPlayer.ChangeState(ThrowBreakString));
	}

	public override void Enter()
	{
		base.Enter();
		Globals.EmitPlayerGenericGfx(GrabbedGfxString, owner.Name);
	}

	public bool CanThrowBreak()
	{
		bool heldKeys = owner.CheckHeldKey('s') && owner.CheckHeldKey('k');
		bool earlyEnough = frameCount < 4;
		bool lastState = techableStates.Contains(owner.lastStateName);
		return heldKeys && earlyEnough && lastState;   
	}
	public override void FrameAdvance()
	{
		base.FrameAdvance();
		owner.velocity = Vector2.Zero;
		
		if (!owner.otherPlayer.currentState.tags.Contains(Globals.Tags.grab))
			owner.ChangeState(FallString);
	}

	/// <summary>
	/// This is a little bit weird that I'm using ReceiveHit here!  This essentially damages the defender and triggers the release
	/// </summary>
	public override void ReceiveHit(Globals.AttackDetails details)
	{
		switch (details.dir)
		{
			case BaseAttack.ATTACKDIR.RIGHT:
				break;
			case BaseAttack.ATTACKDIR.LEFT:
				details.opponentLaunch.x *= -1;
				details.hitPush *= -1;
				break;
			case BaseAttack.ATTACKDIR.EQUAL:
				details.opponentLaunch.x = 0;
				details.hitPush = 0;
				break;
		}
		owner.velocity = details.opponentLaunch;
		owner.ComboUp();
		owner.grounded = false;

		if (details.effect == BaseAttack.EXTRAEFFECT.NONE)
			owner.ChangeState(AirKnockdownString);
		else if (details.effect == BaseAttack.EXTRAEFFECT.GROUNDBOUNCE)
		{
			owner.ChangeState("GroundBounce");
			owner.currentState.stunRemaining = 100;
		}
			
	}
}
