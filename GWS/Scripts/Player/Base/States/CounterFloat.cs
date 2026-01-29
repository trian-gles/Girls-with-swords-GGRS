using Godot;
using System;

public class CounterFloat : Float
{
private string floatString = "Float";
public override string animationName { get { return floatString; } }

	public override void receiveStun(int hitStun, int blockStun)
	{
		base.receiveStun(hitStun * 2, blockStun);
	}
}
