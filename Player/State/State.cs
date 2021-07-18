using Godot;
using System;

public class State : Node
{
    public Player owner;

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

    public virtual void HandleInput(InputEvent @event)
    {
        GD.Print(@event);
    }

    public virtual void FrameAdvance()
    {

    }

    public virtual void PushMovement(float xVel) 
    { 
    
    }

    public virtual void PushAttack() 
    { 
    
    }
}
