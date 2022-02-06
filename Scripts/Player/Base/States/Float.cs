using Godot;
using System;

public class Float : HitStun
{
    public override void Enter()
    {
        base.Enter();
        owner.grounded = false;
    }

    /// <summary>
    /// I have to override this because float always goes into float!
    /// </summary>
    /// <param name="knockdown"></param>
    /// <param name="launch"></param>
    protected override void EnterHitState(bool knockdown, Vector2 launch)
    {
        if (!(launch == Vector2.Zero))
        {
            owner.velocity = launch;
        }
        owner.ComboUp();

        EmitSignal(nameof(StateFinished), "Float");
    }

    public override void FrameAdvance()
    {
        frameCount++;
        if (owner.grounded)
        {
            GD.Print("On ground, knocking down");
            EmitSignal(nameof(StateFinished), "Knockdown");
            owner.ResetComboAndProration();
        }

        stunRemaining--;

        if (stunRemaining == 0)
        {
            owner.ResetComboAndProration();
            EmitSignal(nameof(StateFinished), "Fall");
        }

        if (frameCount == 9 && owner.internalPos.y < 16000 && owner.velocity.y < -300) 
        {
            owner.EmitSignal(nameof(Player.LevelUp));
            EmitSignal(nameof(StateFinished), "AirKnockdown");
        }
        

        ApplyGravity();
    }

    public override void ReceiveHit(BaseAttack.ATTACKDIR attackDir, HEIGHT height, int hitPush, Vector2 launch, bool knockdown)
    {
        if (launch.y == 0)
        {
            launch.y = -400;
        }
        base.ReceiveHit(attackDir, height, hitPush, launch, knockdown);
    }

}
