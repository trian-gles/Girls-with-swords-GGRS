using Godot;
using System;

public class Lobby : Node2D
{

	public override void _Ready()
	{
		Globals.Tests();
	}
	public void OnHostButtonDown()
	{
		
		Globals.mode = Globals.Mode.GGPO;
		Begin(true);
	}

	public void OnJoinButtonDown()
	{
		
		Globals.mode = Globals.Mode.GGPO;
		Begin(false);
	}

	public void OnLocalButtonDown()
	{
		
		GD.Print($"Starting local match");
		Begin(true);
		Globals.mode = Globals.Mode.LOCAL;
	}

	public void OnTrainingButtonDown()
	{
		Begin(true);
		Globals.mode = Globals.Mode.TRAINING;
	}

	public void Begin(bool host)
	{
		GetNode<Button>("Host").Visible = false;
		GetNode<Button>("Join").Visible = false;
		GetNode<Button>("Local").Visible = false;
		GetNode<Button>("Training").Visible = false;

		string ip = "127.0.0.1";
		int localPort = 0;
		int otherPort = 0;

		if (Globals.mode == Globals.Mode.GGPO)
		{
			ip = GetNode<LineEdit>("OpponentIp").Text;

			otherPort = int.Parse(GetNode<LineEdit>("OpponentPort").Text);
			localPort = int.Parse(GetNode<LineEdit>("LocalPort").Text);
		}
		GetNode<LineEdit>("OpponentPort").Visible = false;
		GetNode<LineEdit>("OpponentIp").Visible = false;
		GetNode<LineEdit>("LocalPort").Visible = false;

		var mainScene = (PackedScene) ResourceLoader.Load("res://MainScene.tscn");
		var mainInstance = mainScene.Instance() as MainScene;
		AddChild(mainInstance);
		mainInstance.Begin(ip, localPort, otherPort, host);
	}
}
