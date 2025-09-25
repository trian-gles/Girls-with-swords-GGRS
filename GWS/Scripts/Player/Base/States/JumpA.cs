using Godot;
using System;

public class JumpA : AirNormal
{

	[Export]
	public bool selfWhiffCancel = true;
	public override void _Ready()
	{
		base._Ready();
		AddGatling(new char[] { 'p', 'p' }, "JumpA");
		AddGatling(new char[] { 'k', 'p' }, "JumpB");
		AddGatling(new char[] { 's', 'p' }, "JumpC");

		if (selfWhiffCancel)
			whiffGatlings.Add(new NormalGatling { input = new[] { 'p', 'p' }, state = "JumpA" });

		AddKara(new char[] { 'k', 'p' }, () => owner.CanShield(), "Shield");
		AddBurstKara('k', 'a');
	}
}
