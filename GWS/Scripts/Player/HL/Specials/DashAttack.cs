using Godot;
using System;

public class DashAttack : MovingAttack
{

	public override bool CollisionActive()
	{
		return false;
	}
}
