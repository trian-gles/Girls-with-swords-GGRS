using Godot;
using System;

public class Hadouken : GroundAttack
{
	[Export]
	public int releaseFrame = 18;

	[Export]
	public PackedScene hadoukenScene;
	public override void _Ready()
	{
		base._Ready();
		
		var h = hadoukenScene.Instance() as HadoukenPart;
		h.QueueFree();
		// this looks silly but is necessary so that the hadouken loads at game start

		AddRhythmSpecials(owner.rhythmSpecials);
	}	

	public override void Enter()
	{
		base.Enter();
		owner.velocity.x = 0;
	}
	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (frameCount == releaseFrame)
		{
			owner.ScheduleEvent(EventScheduler.EventType.AUDIO);
			EmitHadouken();
		}
	}

	public override void AnimationFinished()
	{
		EmitSignal(nameof(StateFinished), "Idle");
	}

	private void EmitHadouken()
	{
		var h = hadoukenScene.Instance() as HadoukenPart;

		h.Spawn(owner.facingRight, owner.otherPlayer);
		owner.EmitHadouken(h);
		h.GlobalPosition = new Vector2(owner.Position.x, owner.Position.y + 5);
	}
}
