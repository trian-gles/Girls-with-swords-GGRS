using Godot;
using System;

public class CounterHit : HitStun
{
	public override void receiveStun(int hitStun, int blockStun)
	{
		//GD.Print("COUNTER HIT");
		base.receiveStun(hitStun * 2, blockStun);
	}
}
