using Godot;
using System;

public class AnimationPlayer : Godot.AnimationPlayer
{
    [Signal]
    public delegate void AnimationFinished();
    
    public void NewAnimation(string animName) 
    {
        GD.Print($"Starting animation {animName}");
        if (animName == CurrentAnimation) 
        {
            Seek(0, true);
            GD.Print($"Restarting animation {CurrentAnimation}");
        }
        else
        {
            Play(animName);
            Stop();
            Seek(0, true);
            GD.Print($"Switching to new animation {animName}");
        }
    }

    public void FrameAdvance() 
    {
        if (CurrentAnimationPosition < CurrentAnimationLength)
        {
            Advance(1);
        }
        else
        {
            EmitSignal(nameof(AnimationFinished), CurrentAnimation);
        }
    }

    public void Restart() 
    {
        Seek(0, true);
        GD.Print($"Restarting animation {CurrentAnimation}");
    }
}
