using Godot;
using System;

public class Lobby : Node2D
{
	HBoxContainer hbox;
	VBoxContainer buttons;
	VBoxContainer entries;
	public override void _Ready()
	{
		hbox = GetNode<HBoxContainer>("HBoxContainer");
		buttons = hbox.GetNode<VBoxContainer>("Buttons");
		entries = hbox.GetNode<VBoxContainer>("Entries");
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

	public void OnHostTestButtonDown()
	{
		entries.GetNode<LineEdit>("OpponentIp").Text = "127.0.0.1";
		entries.GetNode<LineEdit>("OpponentPort").Text = "7000";
		entries.GetNode<LineEdit>("LocalPort").Text = "7001";
	}

	public void OnJoinTestButtonDown()
	{
		entries.GetNode<LineEdit>("OpponentIp").Text = "127.0.0.1";
		entries.GetNode<LineEdit>("OpponentPort").Text = "7001";
		entries.GetNode<LineEdit>("LocalPort").Text = "7000";
	}

	public void OnLocalButtonDown()
	{
		Globals.mode = Globals.Mode.LOCAL;
		GD.Print("Local mode selected");
		Begin(true);
	}

	public void OnTrainingButtonDown()
	{
		Globals.mode = Globals.Mode.TRAINING;
		GD.Print("Training mode selected");
		Begin(true);
		
	}

	public void OnSyncTestButtonDown()
    {
		Globals.mode = Globals.Mode.SYNCTEST;
		GD.Print("Training mode selected");
		Begin(true);
	}

	public void Begin(bool host)
	{

		buttons.GetNode<Button>("Host").Visible = false;
		buttons.GetNode<Button>("Join").Visible = false;
		buttons.GetNode<Button>("Local").Visible = false;
		buttons.GetNode<Button>("Training").Visible = false;

		string ip = "127.0.0.1";
		int localPort = 0;
		int otherPort = 0;

		if (Globals.mode == Globals.Mode.GGPO)
		{
			ip = entries.GetNode<LineEdit>("OpponentIp").Text;

			otherPort = int.Parse(entries.GetNode<LineEdit>("OpponentPort").Text);
			localPort = int.Parse(entries.GetNode<LineEdit>("LocalPort").Text);
		}
		entries.GetNode<LineEdit>("OpponentPort").Visible = false;
		entries.GetNode<LineEdit>("OpponentIp").Visible = false;
		entries.GetNode<LineEdit>("LocalPort").Visible = false;

		var mainScene = (PackedScene) ResourceLoader.Load("res://MainScene.tscn");
		var mainInstance = mainScene.Instance() as MainScene;
		AddChild(mainInstance);
		mainInstance.Begin(ip, localPort, otherPort, host);
	}
}
