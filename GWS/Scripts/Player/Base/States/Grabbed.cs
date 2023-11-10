using Godot;
using System;

public class Grabbed : State
{

    public override void _Ready()
    {
        base._Ready();
        AddGatling(new char[] { 'k', 'p' }, () => owner.CheckHeldKey('s') && frameCount < 4, "ThrowBreak", () => owner.otherPlayer.ChangeState("ThrowBreak"));
        AddGatling(new char[] { 's', 'p' }, () => owner.CheckHeldKey('k') && frameCount < 4, "ThrowBreak", () => owner.otherPlayer.ChangeState("ThrowBreak"));
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
