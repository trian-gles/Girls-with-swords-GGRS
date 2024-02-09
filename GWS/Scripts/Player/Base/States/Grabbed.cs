using Godot;
using System;
using System.Collections.Generic;

public class Grabbed : State
{
    private HashSet<string> techableStates = new HashSet<string>() { "Idle", "Crouch", "Walk", "Jump", "Fall"};
    public override void _Ready()
    {
        base._Ready();
        AddGatling(new char[] { 'k', 'p' }, CanThrowBreak, "ThrowBreak", () => owner.otherPlayer.ChangeState("ThrowBreak"));
        AddGatling(new char[] { 's', 'p' }, CanThrowBreak, "ThrowBreak", () => owner.otherPlayer.ChangeState("ThrowBreak"));
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
		owner.velocity = new Vector2(0, 0);
	}

    /// <summary>
    /// This is a little bit weird that I'm using ReceiveHit here!  This essentially damages the defender and triggers the release
    /// </summary>
    /// <param name="rightAttack"></param>
    /// <param name="height"></param>
    /// <param name="hitPush"></param>
    /// <param name="launch"></param>
    /// <param name="knockdown"></param>
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


        EmitSignal(nameof(StateFinished), "AirKnockdown");
	}
}
