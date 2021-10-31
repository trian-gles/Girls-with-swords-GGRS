using Godot;
using System;

public class Grabbed : State
{
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
    public override void ReceiveHit(bool rightAttack, HEIGHT height, int hitPush, Vector2 launch, bool knockdown)
    {
        if (!rightAttack)
        {
            launch.x *= -1;
        }
        owner.velocity = launch;

        owner.grounded = false;

        EmitSignal(nameof(StateFinished), "AirKnockdown");
    }
}
