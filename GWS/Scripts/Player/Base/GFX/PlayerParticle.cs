using Godot;
using System;

public class PlayerParticle : CPUParticles2D
{
	public int startFrame = 0;

	public void Trigger(int frame, Vector2 pos, bool facingRight)
	{
		startFrame = frame;
		if (Direction.x != 0){
			if (facingRight)
			{
				Direction = new Vector2(-1, 0);
			}
			else 
			{
				Direction = new Vector2(1, 0);
			}
		}
		
		Emitting = true;
		

		Position = pos;
	}

	public void Rollback(int frame)
	{
		if (startFrame > frame)
		{
			Emitting = false;
		}
	}
}
