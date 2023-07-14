using Godot;
using System;

public class AnimationPlayer : Godot.AnimationPlayer
{
	[Signal]
	public delegate void AnimationFinished();

	private int animationLength;
	public int cursor;

	Player owner;
	Sprite frontSprite;
	Sprite backSprite;
	Sprite mainSprite;
	bool useFrontSprite;
	bool useBackSprite;

	/// <summary>
	/// Acquires all sprites for syncing
	/// </summary>
	public void Setup()
	{
		owner = (Player)Owner;
		frontSprite = owner.frontSprite;
		backSprite = owner.behindSprite;
		mainSprite = owner.mainSprite;

		useFrontSprite = frontSprite.Hframes > 1;

		useBackSprite = backSprite.Hframes > 1;
	}

	public void NewAnimation(string animName) 
	{
		if (animName == AssignedAnimation) 
		{

			SetFrame(0);
			cursor = 0;
		}
		else
		{
			Play(animName);
			cursor = 0;
			animationLength = (int)CurrentAnimationLength; //Bad idea?
			Stop();
			SetFrame(0);
		}
		//GD.Print($"new animation {animName}, length = {animationLength}");
	}

	public void SetAnimationAndFrame(string animName, int frame)
	{
		Play(animName);
		animationLength = (int)CurrentAnimationLength;
		Stop();
		cursor = frame;
		SetFrame(cursor);
	}
	public void FrameAdvance() 
	{
		if (cursor < animationLength - 1)
		{
			cursor++;
			SetFrame(cursor);
		}
		else
		{
			EmitSignal(nameof(AnimationFinished), CurrentAnimation);
		}



		if (IsPlaying())
		{
			GD.Print("This SHOULD NOT BE CALLED");
		}
	}

	public void Restart() 
	{
		SetFrame(0);
		cursor = 0;
	}

	private void SetFrame(int frameNum)
	{
		Seek(frameNum, true);
		int spriteFrame = mainSprite.Frame;

		if (useFrontSprite)
		{
			frontSprite.Frame = spriteFrame;
			frontSprite.Offset = mainSprite.Offset;
			frontSprite.Position = mainSprite.Position;
		}

		if (useBackSprite)
		{
			backSprite.Frame = spriteFrame;
			backSprite.Offset = mainSprite.Offset;
			backSprite.Position = mainSprite.Position;
		}
	}

	public int GetRemainingFrames()
	{
		return animationLength - cursor;
	}
}
