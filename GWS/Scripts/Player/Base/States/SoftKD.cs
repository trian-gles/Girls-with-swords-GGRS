using Godot;
using System;

public class SoftKD : HitState
{
    public override void _Ready()
    {
        base._Ready();
        loop = false;
    }
    public override void Enter()
    {
        stunRemaining = 0;
        frameCount = 0;
        owner.velocity.x = 0;
        owner.velocity.y = 0;
        //owner.GFXEvent("Blood");
        ResetTerminalVelocity();
    }
    public override void AnimationFinished()
    {

        owner.ResetComboAndProration();
        owner.invulnFrames = 2;
        owner.EmitSignal("MissedTech", owner.Name);
        EmitSignal(nameof(StateFinished), "Idle");
    }

    public override void ReceiveHit(Globals.AttackDetails details)
    {
        ReceiveHitNoBlock(details);
    }

    public override void FrameAdvance()
    {
        base.FrameAdvance();
        if (frameCount < 16)
        {
            GD.Print("Trying to tech from soft KD");
            TryTech();
        }
    }
    
    public override bool DelayInputs()
	{
		return false;
	}
}
