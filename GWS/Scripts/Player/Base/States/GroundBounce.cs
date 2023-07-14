using Godot;
using System;
using System.Collections.Generic;

public class GroundBounce : Float
{
	public override string animationName { get { return "Float"; } }
	private bool bounced = false;


	public override void Enter()
	{
		base.Enter();
		bounced = false;
	}
	public override void Load(Dictionary<string, int> loadData)
	{
		bounced = Convert.ToBoolean(loadData["bounced"]);
	}

	public override Dictionary<string, int> Save()
	{
		var dict = new Dictionary<string, int>();
		dict["bounced"] = Convert.ToInt32(bounced);
		return dict;
	}

	public override void FrameAdvance()
	{
		frameCount++;
		if (owner.grounded)
		{
			if (bounced)
			{
				if (stunRemaining > 20)
					EmitSignal(nameof(StateFinished), "Knockdown");
				else
				{
					owner.grounded = false;
					TryGroundTech();
				}

				owner.ResetComboAndProration();
			}
			else if (owner.canGroundbounce)
			{
				bounced = true;
				owner.grounded = false;
				owner.velocity.y = (int)Math.Floor(owner.velocity.y * -3 / 4);
				owner.canGroundbounce = false;
			}
			else
			{
				owner.grounded = false;
				TryGroundTech();
			}
			
		}

		stunRemaining--;

		TryTech();

		if (frameCount == 9 && owner.internalPos.y < 14000 && owner.velocity.y < -300)
		{
			owner.EmitSignal(nameof(Player.LevelUp));
			EmitSignal(nameof(StateFinished), "AirKnockdown");
		}


		ApplyGravity();
	}
}

