using Godot;
using System;

public class LaunchAttack : AirAttack
{

    [Export]
    protected Vector2 launch = new Vector2();

    [Export]
    protected int launchFrame = 1;

    /// <summary>
    /// This doesn't call base.FrameAdvance() because that state includes things we don't want
    /// </summary>
    public override void FrameAdvance()
    {
        frameCount++;
        if (slowdownSpeed != 0) SlowDown();
        if (frameCount == launchFrame)
        {
            owner.velocity = launch;
            if (!owner.facingRight)
            {
                //GD.Print("Flipping launch x coor");
                owner.velocity.x *= -1;
            }
            owner.grounded = false;
        }
        else if (frameCount > launchFrame)
        {
            ApplyGravity();
            if (owner.grounded)
            {
                owner.velocity.x = 0;
            }
        }
    }

    public override void AnimationFinished()
    {
        EmitSignal(nameof(StateFinished), "Fall");
    }
}
