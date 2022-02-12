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
    protected override void EnterHitState(bool knockdown, Vector2 launch, Vector2 collisionPnt)
    {
        GetNode<Node>("/root/Globals").EmitSignal(nameof(PlayerFXEmitted), collisionPnt, "hit", false);
        

        if (!(launch == Vector2.Zero))
        {
            owner.velocity = launch;
        }

        if (launch.y == 0)
        {
            owner.velocity.y = -400;
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
            EmitSignal(nameof(StateFinished), "Tech");
        }

        if (frameCount == 9 && owner.internalPos.y < 14000 && owner.velocity.y < -300) 
        {
            owner.EmitSignal(nameof(Player.LevelUp));
            EmitSignal(nameof(StateFinished), "AirKnockdown");
        }
        

        ApplyGravity();
    }

}
