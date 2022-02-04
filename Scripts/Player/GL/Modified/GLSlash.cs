using Godot;
using System;
using System.Collections.Generic;

public class GLSlash : Slash
{
    public override void _Ready()
    {
        base._Ready();
        AddGatling(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'p' }, new char[] { '6', 'r' }, new char[] { '4', 'p' }, new char[] { '2', 'r' }, new[] { 's', 'p' } }, "GunBlazed");
        AddGatling(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { 'p', 'p' } }, "GLDP");
    }

}
