using Godot;
using System;
using System.Collections.Generic;

public class OLCrouch : Crouch
{
	public override void _Ready()
	{
		base._Ready();
		AddGatling(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { 'p', 'p' } }, "DP");
		AddGatling(new List<char[]>() { new char[] { '2', 'p' }, new char[] { '2', 'p' }, new char[] { 's', 'p' } }, "AntiAir");
		AddGatling(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'p' }, new char[] { '6', 'r' }, new char[] { '4', 'p' }, new char[] { '2', 'r' }, new[] { 'k', 'p' } }, "CommandRun");
	}
}
