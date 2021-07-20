using Godot;
using System;

public class State : Node
{
    public Player owner;
    protected int frameCount = 0;
    [Signal]
    public delegate void StateFinished(string nextStateName);

    protected int stunRemaining;
    public bool loop = false;
    

    public override void _Ready()
    {
        owner = GetOwner<Player>();
    }

    public virtual void Enter() 
    { 
    
    }

    public virtual void Exit()
    {

    }

    public virtual void AnimationFinished() 
    { 
    
    }

    public virtual void HandleInput(string[] inputArr)
    {
        GD.Print(inputArr);
    }

    public virtual void FrameAdvance()
    {

    }

    public virtual void PushMovement(float xVel) 
    {
        owner.velocity.x = xVel / 2;
    }

    public virtual void InHurtbox()
    {


    }

    public virtual void ReceiveHit(bool rightAttack, string height, Vector2 push)
    {
        owner.velocity = push;
        if (height == "high") 
        {
            GD.Print("High hit");
        }
        else if (height == "low") 
        {
            GD.Print("Low hit");
        }
        else
        {
            if ((rightAttack && owner.CheckHeldKey("right")) || (!rightAttack && owner.CheckHeldKey("Block"))) 
            {
                EmitSignal(nameof(StateFinished), "Block");
            }
            else 
            {
                EmitSignal(nameof(StateFinished), "HitStun");
            }
        }
    }

    public void receiveStun(int stun)
    {
        stunRemaining = stun;
    }

    public virtual void receiveDamage(int dmg)
    {
        owner.health -= dmg;
    }
}
