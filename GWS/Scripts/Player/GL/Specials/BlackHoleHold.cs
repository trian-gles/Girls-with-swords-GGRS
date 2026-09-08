using Godot;
using System;

public class BlackHoleHold : BlackHolePlace
{
    public override string animationName => "BlackHolePlace";

    public override void Enter()
    {
        base.Enter();
        owner.CommandHadouken("BlackHole", HadoukenPart.ProjectileCommand.BlackHoleDeactivate);
    }
    protected override bool CanBlackHole()
	{
		return true;
	}
}