using Godot;
using System;

public class SoftKD : HitState
{
    private const string MissedTechString = "MissedTech";
    private const string IdleString = "Idle";
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
        tags.Add(Globals.Tags.techable);
    }
    public override void AnimationFinished()
    {

        owner.ResetComboAndProration();
        owner.invulnFrames = 2;
        Globals.EmitSignal(Globals.PlayerSignal.MissedTech, owner.Name);
        owner.ChangeState(IdleString);
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
            TryTech();
        }
    }
    
    public override bool DelayInputs()
	{
		return false;
	}
}
