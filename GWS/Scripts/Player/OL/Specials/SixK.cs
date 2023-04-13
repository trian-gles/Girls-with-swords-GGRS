using Godot;
using System;

public class SixK : MovingAttack
{
    public override void _Ready()
    {
        base._Ready();
        AddSpecials(owner.groundSpecials);
        AddExSpecials(owner.groundExSpecials);
        AddGatling(new char[] { 's', 'p' }, "Slash");
        AddKara(new char[] { 's', 'p' }, () => owner.CanGrab(), "GrabStart");
    }
}