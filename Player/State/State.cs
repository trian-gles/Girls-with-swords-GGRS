using Godot;
using System;

public class State : Node
{
    [Signal]
    delegate void StateFinished(string nextStateName);

    public virtual void Enter() 
    { 
    
    }

    public virtual void Exit()
    {

    }

    public virtual void AnimationFinished() 
    { 
    
    }

    public virtual void HandleInput(InputEvent inEvent)
    {

    }

    public virtual void Update()
    {

    }

    public virtual void PushMovement(float xVel) 
    { 
    
    }

    public virtual void PushAttack() 
    { 
    
    }
}
