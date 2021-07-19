using Godot;
using System;

public class State : Node
{
    public Player owner;
    protected int frameCount = 0;
    [Signal]
    public delegate void StateFinished(string nextStateName);

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

    public virtual void PushAttack() 
    { 
    
    }
}
