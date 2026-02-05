using Godot;
using System;

public class ShieldFX : CPUParticles2D
{
	CPUParticles2D shieldHit;
	public int rightPos = 12;
	public int leftPos = -12;
	public int crouchPos = 2;
	public int standPos = -10;
	public bool crouching = false;

	public override void _Ready()
	{
		base._Ready();
		shieldHit = GetNode<CPUParticles2D>("ShieldHit");
	}

	public override void _PhysicsProcess(float delta)
	{
		base._PhysicsProcess(delta);
		if (crouching)
			Position = new Vector2(Position.x, crouchPos);
		else
			Position = new Vector2(Position.x, standPos);

		if (((Player)Owner).facingRight)
		{
			Position = new Vector2(rightPos, Position.y);
			shieldHit.Direction = new Vector2(-1, shieldHit.Direction.y);
		}
		else
		{
			Position = new Vector2(leftPos, Position.y);
			shieldHit.Direction = new Vector2(1, shieldHit.Direction.y);
		}
	}
}
