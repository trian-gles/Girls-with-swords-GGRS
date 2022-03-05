using Godot;
using System;
using System.Collections.Generic;

public class GroundBounce : Float
{
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
                EmitSignal(nameof(StateFinished), "Knockdown");
                owner.ResetComboAndProration();
            }
            else
            {
                GD.Print("BOUNCE");
                bounced = true;
                owner.grounded = false;
                owner.velocity.y = owner.velocity.y * -1;
            }
            
        }

        stunRemaining--;

        if (stunRemaining == 0)
        {
            EmitSignal(nameof(StateFinished), "Tech");
        }

        if (frameCount == 9 && owner.internalPos.y < 14000 && owner.velocity.y < -300)
        {
            owner.EmitSignal(nameof(Player.LevelUp));
            EmitSignal(nameof(StateFinished), "AirKnockdown");
        }


        ApplyGravity();
    }
}

