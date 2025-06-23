using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class AirHatThrow : HatThrow
{
	[Export]
	public int dropFrame = 20;

	public override void Enter()
	{
		base.Enter();
		owner.velocity = Vector2.Zero;
	}

	protected override void ApplyGravity() {
		if (frameCount > dropFrame){
			
			base.ApplyGravity();
		}
	}
}
