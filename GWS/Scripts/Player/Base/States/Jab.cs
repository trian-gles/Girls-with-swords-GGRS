using Godot;
using System;

public class Jab : GroundAttack
{
	
	public override void _Ready()
	{
		base._Ready();
		tags.Add(Globals.Tags.jab);
		AddSpecials(owner.groundSpecials);
		AddEasyGroundSpecials();
		AddExSpecials(owner.groundExSpecials);
		AddCommandNormals(owner.commandNormals);
		AddGatling(new char[] { 'p', 'p' }, () => owner.CheckHeldKey('2'), "CrouchA");
		AddGatling(new char[] { 'k', 'p' }, () => owner.CheckHeldKey('2'), "CrouchB");
		AddGatling(new char[] { 's', 'p' }, () => owner.CheckHeldKey('2'), "CrouchC");
		whiffGatlings.Add(new NormalGatling { input = new[] { 'p', 'p' }, state = "Jab" });
		AddGatling(new char[] { 'p', 'p' }, "Jab");
		AddGatling(new char[] { 'k', 'p' }, "Kick");
		AddGatling(new char[] { 's', 'p' }, "Slash");
		AddGatling(new char[] { 'b', 'p' }, "Kick");
		

        AddKara(new char[] { 'k', 'p' }, () => owner.CanShield(), "Shield");
    }
}
