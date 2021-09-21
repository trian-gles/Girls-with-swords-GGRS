using Godot;
using System;

public class Grabbed : State
{
    public override void FrameAdvance()
    {
        base.FrameAdvance();
        owner.Position = owner.otherPlayer.grabPos.Position;
    }
    public void Release()
    {
        EmitSignal(nameof(StateFinished), "HitStun");
    }
}
