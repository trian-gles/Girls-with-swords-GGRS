using Godot;
using System;

public class CounterFloat : Float
{
private const string FloatString = "Float";
public override string animationName { get { return FloatString; } }

	public override void receiveStun(int hitStun, int blockStun)
	{
		base.receiveStun(hitStun * 2, blockStun);
	}
}
