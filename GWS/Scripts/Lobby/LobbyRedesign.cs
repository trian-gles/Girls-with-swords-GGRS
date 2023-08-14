using Godot;
using System;

public class LobbyRedesign : Node2D
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

	[Export]
	public bool syncTest = false;

	[Export]
	public bool log = false;

	[Export]
	public bool alwaysBlock = false;

	[Export]
	public bool autoTech = false;

	[Export]
	public PackedScene localManager;

	[Export]
	public PackedScene trainingManager;

	[Export]
	public PackedScene aiManager;

	[Export]
	public PackedScene ggrsManager;

	[Export]
	public PackedScene syncTestManager;

	public bool host = false;
	
	private BaseManager activeManager;
	
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

		// connect in game menu
		GetNode<Node>("/root/Events").Connect("MainMenuPressed", this, nameof(OnLobbyReset));

		// set up debug globals
		Globals.autoTech = autoTech;
		Globals.alwaysBlock = alwaysBlock;
		Globals.logOn = log;

		if (syncTest)
			syncTestBegin();
	}

	private void syncTestBegin()
	{
		var syncTestScene = syncTestManager.Instance<SyncTestManager>();
		AddChild(syncTestScene);
		HideButtons();
	}

	//netplay buttons
	public void OnHostButtonDown()
	{
		string ip = entries.GetNode<LineEdit>("OpponentIp").Text;
		activeManager = ggrsManager.Instance<GGRSManager>();
		AddChild(activeManager);
		HideButtons();
		((GGRSManager)activeManager).ManualConfig(ip, true);
	}

	public void OnJoinButtonDown()
	{
		string ip = entries.GetNode<LineEdit>("OpponentIp").Text;
		var ggrsScene = ggrsManager.Instance<GGRSManager>();
		AddChild(ggrsScene);
		HideButtons();
		ggrsScene.ManualConfig(ip, false);
	}

	public void OnAutoConnectDown()
	{
		var ggrsScene = ggrsManager.Instance<GGRSManager>();
		AddChild(ggrsScene);
		HideButtons();
	}

	public void OnHostTestButtonDown()
	{
		entries.GetNode<LineEdit>("OpponentIp").Text = "127.0.0.1";
	}

	public void OnJoinTestButtonDown()
	{
		entries.GetNode<LineEdit>("OpponentIp").Text = "127.0.0.1";
	}
	
	//local buttons
	public void OnLocalButtonDown()
	{
		BeginManager(localManager);
	}

	public void OnTrainingButtonDown()
	{
		BeginManager(trainingManager);
		
	}

	public void OnCPUButtonDown()
	{
		BeginManager(aiManager);
	}
	
	private void BeginManager(PackedScene managerScene){
		activeManager = managerScene.Instance<BaseManager>();
		AddChild(activeManager);
		HideButtons();
	}
	
	//config
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
	
	public void OnLobbyReset()
	{
		if (activeManager != null)
		{
			activeManager.QueueFree();
			activeManager = null;
		}
			
		var menu = GetNode<Control>("MenuRoot");
		menu.Visible = true;
		inputmenu.GetNode<ColorRect>("ConfigOverlay").Visible = false;
		
		if (menuroot.GetNode<MarginContainer>("MainMenu").Visible == true)
		{
			mainmenubuttons.GetNode<ToolButton>("Local").GrabFocus();
		}

		menu.Call("_on_BackButton_pressed");
	}

	private void HideButtons()
	{
		GetNode<Control>("MenuRoot").Visible = false;
		inputmenu.GetNode<ColorRect>("ConfigOverlay").Visible = false;
	}
}
