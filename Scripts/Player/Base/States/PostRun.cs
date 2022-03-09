using Godot;
using System;
using System.Collections.Generic;

public class PostRun : MoveState
{
	public override void _Ready()
	{
		base._Ready();
		loop = true;
		slowdownSpeed = 30;
		AddGatling(new[] { 's', 'p' }, () => {
			bool pos = Mathf.Abs(owner.internalPos.x - owner.otherPlayer.internalPos.x) < 3500;
			bool grabbable = owner.otherPlayer.IsGrabbable();
			bool heldKey = owner.CheckHeldKey('4') || owner.CheckHeldKey('6');
			return pos && grabbable && heldKey;

		}, "Grab");
		AddSpecials(owner.groundSpecials);
		AddNormals();
	}
	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (frameCount  == 12)
		{
			EmitSignal(nameof(StateFinished), "Idle");
		}
	}

}

