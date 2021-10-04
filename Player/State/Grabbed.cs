using Godot;
using System;

public class Grabbed : State
{
    public override void FrameAdvance()
    {
        base.FrameAdvance();
        owner.velocity = new Vector2(0, 0);
        GD.Print($"Grabbed global position = {owner.GlobalPosition}");
    }
    public void Release()
    {
        EmitSignal(nameof(StateFinished), "Float");
    }
}
