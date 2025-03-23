using Godot;
using System;

public class SnailRide : MovingAttack
{
    public override void FrameAdvance()
    {
        base.FrameAdvance();
        if (frameCount % 5 == 0)
            GetNode<Node>("/root/Globals").EmitSignal(nameof(PlayerFXEmitted),
			new Vector2(owner.internalPos.x, owner.GetCollisionRect().End.y),
			"dust", owner.facingRight);
    }
}