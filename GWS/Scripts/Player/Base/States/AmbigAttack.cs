using Godot;
using System;

public class GroundAttack : BaseAttack
{
	private const string IdleString = "Idle";
	
	public override void _Ready()
	{
		base._Ready();
		AddCancel(IdleString);
		
		if (jumpCancelable){
			AddJumpCancel();
		}
	}
}

