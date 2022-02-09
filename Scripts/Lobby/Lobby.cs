using Godot;
using System;

public class Lobby : Node2D
{
	Control menuroot;
	MarginContainer mainmenu;
	MarginContainer localmenu;
	MarginContainer netplaymenu;
	VBoxContainer mainmenubuttons;
	VBoxContainer localmenubuttons;
	VBoxContainer entries;
	HBoxContainer netplaybuttons;
	
	Control inputmenu;
	VBoxContainer column;
	
	public override void _Ready()
	{
		menuroot = GetNode<Control>("MenuRoot");
		mainmenu = menuroot.GetNode<MarginContainer>("MainMenu");
		mainmenubuttons = mainmenu.GetNode<VBoxContainer>("CenterContainer/MainMenuButtons");
		localmenu = menuroot.GetNode<MarginContainer>("LocalMenu");
		netplaymenu = menuroot.GetNode<MarginContainer>("NetPlayMenu");
		localmenubuttons = localmenu.GetNode<VBoxContainer>("LocalButtons");
		entries = netplaymenu.GetNode<VBoxContainer>("Entries");
		netplaybuttons = entries.GetNode<HBoxContainer>("NetPlayButtons");
		
		//button check menus
		inputmenu = GetNode<Control>("InputMenu/InputMenu");
		column = inputmenu.GetNode<VBoxContainer>("ConfigOverlay/Column");
		
		
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
	
	public void _on_ButtonConfig_pressed()
	{
		HideButtons();
		inputmenu.GetNode<ColorRect>("ConfigOverlay").Visible = true;
		column.GetNode<Button>("ReturnMainMenu").Visible = true;
		column.GetNode<Button>("ReturnToInGameMenu").Visible = false;
		GetNode("/root/Events").Connect("MainMenuPressed", this, nameof(OnLobbyReset));
	}
	
	public void OnButtonCheckDownInGame()
	{
		HideButtons();
		inputmenu.GetNode<ColorRect>("ConfigOverlay").Visible = true;
		column.GetNode<Button>("ReturnToInGameMenu").Visible = true;
		column.GetNode<Button>("ReturnMainMenu").Visible = false;
	}
	
	public void LocalLobbyReturn()
	{
		OnLobbyReset();
		menuroot.GetNode<MarginContainer>("MainMenu").Visible = false;
		menuroot.GetNode<MarginContainer>("LocalMenu").Visible = true;
		menuroot.GetNode<MarginContainer>("NetPlayMenu").Visible = false;
		localmenubuttons.GetNode<Button>("Local").GrabFocus();
	}
	
	public void NetPlayLobbyReturn()
	{
		OnLobbyReset();
		menuroot.GetNode<MarginContainer>("MainMenu").Visible = false;
		menuroot.GetNode<MarginContainer>("LocalMenu").Visible = false;
		menuroot.GetNode<MarginContainer>("NetPlayMenu").Visible = true;
		netplaybuttons.GetNode<Button>("Host").GrabFocus();
	}
	
	public void OnLobbyReset()
	{
		GetNode<Control>("MenuRoot").Visible = true;
		inputmenu.GetNode<ColorRect>("ConfigOverlay").Visible = false;
		
		if (menuroot.GetNode<MarginContainer>("MainMenu").Visible = true)
		{
			mainmenubuttons.GetNode<ToolButton>("Local").GrabFocus();
		}
	}

	private void HideButtons()
	{
		GetNode<Control>("MenuRoot").Visible = false;
		inputmenu.GetNode<ColorRect>("ConfigOverlay").Visible = false;
	}

	public void Begin(bool host)
	{
		HideButtons();
		GetNode("/root/Events").Connect("ButtonConfigPressed", this, nameof(OnButtonCheckDownInGame));
		GetNode("/root/Globals").Connect("LocalLobbyReturn", this, nameof(LocalLobbyReturn));
		GetNode("/root/Globals").Connect("NetPlayLobbyReturn", this, nameof(NetPlayLobbyReturn));
		
		string ip = "127.0.0.1";
		int localPort = 0;
		int otherPort = 0;

		if (Globals.mode == Globals.Mode.GGPO)
		{
			ip = entries.GetNode<LineEdit>("OpponentIp").Text;

			otherPort = int.Parse(entries.GetNode<LineEdit>("OpponentPort").Text);
			localPort = int.Parse(entries.GetNode<LineEdit>("LocalPort").Text);
		}
		
		var mainScene = (PackedScene) ResourceLoader.Load("res://Scenes/MainScene.tscn");
		var mainInstance = mainScene.Instance() as MainScene;
		AddChild(mainInstance);
		mainInstance.Connect("LobbyReturn", this, nameof(OnLobbyReset));
		mainInstance.Begin(ip, localPort, otherPort, host);
	}
}

