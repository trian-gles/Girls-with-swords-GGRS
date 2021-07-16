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
        GD.Print(inEvent);
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
