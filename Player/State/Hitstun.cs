using Godot;
using System;

public class HitStun : State
{
    public override void _Ready()
    {
        base._Ready();
        loop = true;
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

    public override void ReceiveHit(bool rightAttack, HEIGHT height, Vector2 push)
    {
        if (!rightAttack)
        {
            push.x *= -1;
        }
        owner.velocity = push;
        EmitSignal(nameof(StateFinished), "HitStun");

    }
}

