using Godot;
using System;

public class Grab : State
{
    [Export]
    public int releaseFrame = 10;

    [Export]
    public Vector2 launch = new Vector2();

    [Export]
    public int dmg = 0;

    public bool released = false;


    public override void Enter()
    {
        base.Enter();
        owner.velocity = new Vector2(0, 0);
        released = false;
        owner.otherPlayer.ChangeState("Grabbed");
        GD.Print("Entering grab");

    }

    public override void FrameAdvance()
    {
        base.FrameAdvance();
        if ((frameCount == releaseFrame) && !released)
        {
            owner.otherPlayer.Release(launch);
        }
    }
    public override void AnimationFinished()
    {
        EmitSignal(nameof(StateFinished), "Idle");
    }
}
