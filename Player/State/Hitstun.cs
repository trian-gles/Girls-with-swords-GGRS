using Godot;
using System;

public class HitStun : State
{
    public override void _Ready()
    {
        base._Ready();
    }
    public override void Enter()
    {
        base.Enter();
        owner.ComboUp();
    }


    public override void FrameAdvance()
    {
        base.FrameAdvance();
        stunRemaining--;

        if (stunRemaining == 0)
        {
            owner.ResetCombo();
            if (owner.grounded)
            {
                EmitSignal(nameof(StateFinished), "Idle");
            }
            else
            {
                EmitSignal(nameof(StateFinished), "Fall");
            }
        }

        if (!owner.grounded)
        {
            ApplyGravity();
        }

    }

    public override void PushMovement(float _xVel)
    {
        if (owner.grounded)
        {
            base.PushMovement(_xVel);
        }
    }

    public override void ReceiveHit(bool rightAttack, HEIGHT height, Vector2 push, bool knockdown)
    {
        if (!rightAttack)
        {
            push.x *= -1;
        }
<<<<<<< Updated upstream
        owner.velocity = push;
=======
        if (!(launch == Vector2.Zero))
        {
            GD.Print("Launch is not zero!");
            owner.velocity = launch;
        }

        if (owner.velocity.y < 0) // make sure the player is registered as in the air if launched 
        {
            owner.grounded = false;
        }

        owner.hitPushRemaining = hitPush;
>>>>>>> Stashed changes
        EnterHitState(knockdown);

    }
}

