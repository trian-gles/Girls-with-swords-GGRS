using Godot;
using System;

public class ComNorm : GroundAttack
{
	[Export]
	public bool slashCancel = false;

	[Export]
	public bool sweepCancel = false;

	[Export]
	public bool commandNormalCancel = false;
	public override void _Ready()
	{
		base._Ready();
		AddSpecials(owner.groundSpecials);
		AddEasyGroundSpecials();
		AddExSpecials(owner.groundExSpecials);
		if (commandNormalCancel)
		{

			foreach (var comNorm in owner.commandNormals)
			{
				if (comNorm.state != Name)
				{
					AddCommandNormal(comNorm);
				}
			}
		}

		if (sweepCancel)
		{
			AddGatling(new char[] { 's', 'p' }, () => owner.CheckHeldKey('2'), "CrouchC");
			AddGatling(new char[] { 'b', 'p' }, "CrouchC");
		}
			


		if (slashCancel)
		{
			AddGatling(new char[] { 's', 'p' }, "Slash");
			AddGatling(new char[] { 'b', 'p' }, "Slash");
		}


		AddKara(new char[] { 's', 'p' }, () => owner.CanGrab(), "GrabStart");
		AddKara(new char[] { 'k', 'p' }, () => owner.CanGrab(), "GrabStart");
	}
}
