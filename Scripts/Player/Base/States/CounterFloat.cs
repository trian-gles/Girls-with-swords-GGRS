using Godot;
using System;

public class CounterFloat : Float
{
    public override void receiveStun(int hitStun, int blockStun)
    {
        //GD.Print("COUNTER FLOAT");
        base.receiveStun(hitStun * 2, blockStun);
    }
}
