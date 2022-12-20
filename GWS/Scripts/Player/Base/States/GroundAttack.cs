using Godot;
using System;

/// <summary>
/// Attacks that may be grounded or aerial
/// </summary>
public class AmbigAttack : BaseAttack
{
	
	public override void _Ready()
	{
		base._Ready();
		AddCancel("Fall"); // will need to be fixed
		
		if (jumpCancelable){
			AddJumpCancel();
		}
	}

	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (!owner.grounded)
		{
			ApplyGravity();
		}
	}
}

