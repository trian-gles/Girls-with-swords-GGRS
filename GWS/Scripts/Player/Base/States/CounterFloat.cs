using Godot;
using System;

public class CounterFloat : Float
{
	public override string animationName { get { return "Float"; } }

	public override void receiveStun(int hitStun, int blockStun)
	{
		base.receiveStun(hitStun * 2, blockStun);
	}
}
