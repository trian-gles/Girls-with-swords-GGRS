using Godot;
using System;

public class Tech : Fall
{
	[Export]
	public Vector2 techVector = new Vector2(0, 0);

	[Export]
	public int length = 12;
	public override void Enter()
	{
		base.Enter();
		owner.ResetComboAndProration();
		owner.canDoubleJump = true;
		owner.CheckTurnAround();
		if (owner.CheckHeldKey('6'))
			owner.velocity = techVector;
		else if (owner.CheckHeldKey('4'))
			owner.velocity = new Vector2(-techVector.x, techVector.y);
		else
			owner.velocity = new Vector2(0, techVector.y);
	}
}
