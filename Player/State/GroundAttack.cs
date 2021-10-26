using Godot;
using System;

public class GroundAttack : BaseAttack
{
	public override void Enter()
	{
		base.Enter();
		owner.velocity.x = 0;
	}
}

