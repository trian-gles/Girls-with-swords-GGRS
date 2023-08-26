using Godot;
using System;

public class SixK : MovingAttack
{
    public override void _Ready()
    {
        base._Ready();
        AddSpecials(owner.groundSpecials);
        AddExSpecials(owner.groundExSpecials);
        AddKara(new char[] { 's', 'p' }, () => owner.CanGrab(), "GrabStart");
    }
}