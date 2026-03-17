using Godot;
using System;
using System.Linq;
using System.Management.Instrumentation;

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
	Label sendToFriendLabel;
	Label waitingForOtherPlayerLabel;

	LineEdit newMatchId;
	LineEdit existingMatchId;
	
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
	public PackedScene localManagerScene;

	[Export]
	public PackedScene trainingManagerScene;

	[Export]
	public PackedScene aiManagerScene;

	[Export]
	public PackedScene ggrsManagerScene;

	[Export]
	public PackedScene syncTestManagerScene;

	[Export]
	public PackedScene tutorialManagerScene;

	[Export]
	public PackedScene comboTrialManagerScene;

	[Export]
	public PackedScene strategyManagerScene;

	[Export]
	public PackedScene[] charScenes = new PackedScene[0];

	public LocalManager localManager;
	public TrainingManager trainingManager;
	public AIManager aiManager;
	public GGRSManager ggrsManager;
	public SyncTestManager syncTestManager;
	public TutorialManager tutorialManager;
	public ComboTrialManager comboTrialManager;
	public StrategyManager strategyManager;


	public bool host = false;
	
	private BaseManager activeManager;
	private Node events;
	private AudioStreamPlayer lobbyMusic;
	
	private static Random random = new Random();


	[Export]
	public PackedScene packedGameScene;
	[Export]
	public PackedScene packedCharSelectScene;
	[Export]
	public PackedScene packedWinScene;

	private GameScene gameScene;
	private CharSelectScene charSelectScene;
	private WinScene winScene;

	// runtime string constants
	private const string MainMenuPressedString = "MainMenuPressed";
	private const string BackButtonPressedCallString = "_on_BackButton_pressed";

	public static string RandomString(int length)
	{
		const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		return new string(Enumerable.Repeat(chars, length)
			.Select(s => s[random.Next(s.Length)]).ToArray());
	}
	
	public override void _Ready()
	{
		Globals.GenerateCharacters(charScenes);
		menuroot = GetNode<Control>("MenuRoot");
		mainmenu = menuroot.GetNode<MarginContainer>("MainMenu");
		mainmenubuttons = mainmenu.GetNode<VBoxContainer>("CenterContainer/MainMenuButtons");
		localmenu = menuroot.GetNode<MarginContainer>("LocalMenu");
		netplaymenu = menuroot.GetNode<MarginContainer>("NetPlayMenu");
		localmenubuttons = localmenu.GetNode<VBoxContainer>("LocalButtons");
		entries = netplaymenu.GetNode<VBoxContainer>("Entries");
		//netplaybuttons = entries.GetNode<HBoxContainer>("NetPlayButtons");

		newMatchId = entries.GetNode<LineEdit>("NewMatchContainer/NewMatchID");
		sendToFriendLabel = newMatchId.GetNode<Label>("SendToFriend");
		waitingForOtherPlayerLabel = netplaymenu.GetNode<Label>("WaitingForOtherPlayer");

		existingMatchId = entries.GetNode<LineEdit>("ExistingMatchContainer/ExistingMatchID");
		sendToFriendLabel.Visible = false;
		waitingForOtherPlayerLabel.Visible = false;

		//button check menus
		inputmenu = GetNode<Control>("InputMenu/InputMenu");
		column = inputmenu.GetNode<VBoxContainer>("ConfigOverlay/Column");

		// connect in game menu
		events = GetNode<Node>("/root/Events");
		events.Connect(MainMenuPressedString, this, nameof(OnLobbyReset));
		// cache lobby music player
		lobbyMusic = GetNode<AudioStreamPlayer>("LobbyMusic");

		// set up debug globals
		Globals.autoTech = autoTech;
		Globals.alwaysBlock = alwaysBlock;
		Globals.logOn = log;
		CreateGamescenes();

		aiManager = aiManagerScene.Instance<AIManager>();
		ggrsManager = ggrsManagerScene.Instance<GGRSManager>();
		localManager = localManagerScene.Instance<LocalManager>();
		trainingManager = trainingManagerScene.Instance<TrainingManager>();
		syncTestManager = syncTestManagerScene.Instance<SyncTestManager>();
		tutorialManager = tutorialManagerScene.Instance<TutorialManager>();
		comboTrialManager = comboTrialManagerScene.Instance<ComboTrialManager>();
		strategyManager = strategyManagerScene.Instance<StrategyManager>();

		if (syncTest)
			syncTestBegin();
	}

	protected void CreateGamescenes()
	{
		charSelectScene = packedCharSelectScene.Instance() as CharSelectScene;
		gameScene = packedGameScene.Instance() as GameScene;
		winScene = packedWinScene.Instance() as WinScene;
	}

	long prevMem = 0;
	public override void _Process(float delta)
	{
		base._Process(delta);
		long mem = GC.GetTotalMemory(false); // allocated managed memory
		if (mem > prevMem)
			GD.Print(mem / 1024);
		prevMem = mem;
	}

	private void syncTestBegin()
	{
		Globals.mode = Globals.Mode.SYNCTEST;
		BeginManager(syncTestManager);
	}

	//netplay buttons
	public void OnHostButtonDown()
	{
		string ip = entries.GetNode<LineEdit>("OpponentIp").Text;
		activeManager = ggrsManagerScene.Instance<GGRSManager>();
		AddChild(activeManager);
		HideButtons();
		((GGRSManager)activeManager).ManualConfig(ip, true);
	}

	public void OnJoinButtonDown()
	{
		string ip = entries.GetNode<LineEdit>("OpponentIp").Text;
		activeManager = ggrsManagerScene.Instance<GGRSManager>();
		AddChild(activeManager);
		HideButtons();
		((GGRSManager)activeManager).ManualConfig(ip, false);
	}

	public void OnNewNetplayMatch()
	{
		newMatchId.Text = RandomString(8);
		Globals.netplaySessionName = newMatchId.Text;
		sendToFriendLabel.Visible = true;
		waitingForOtherPlayerLabel.Visible = true;
		BeginNetplayManager(ggrsManagerScene);
	}

	public void OnJoinNetplayMatch()
	{
		GD.Print(existingMatchId.Text);
		Globals.netplaySessionName = existingMatchId.Text;
		waitingForOtherPlayerLabel.Visible = true;
		BeginNetplayManager(ggrsManagerScene);
	}

	public void OnAutoConnectDown()
	{
		BeginManager(ggrsManager);
	}

	public void OnHostTestButtonDown()
	{
		entries.GetNode<LineEdit>("OpponentIp").Text = "127.0.0.1";
		OnHostButtonDown();
	}

	public void OnJoinTestButtonDown()
	{
		entries.GetNode<LineEdit>("OpponentIp").Text = "127.0.0.1";
		OnJoinButtonDown();
	}
	
	//local buttons
	public void OnLocalButtonDown()
	{
		BeginManager(localManager);
	}
	
	public void OnComboTrialsButtonDown()
	{
		BeginManager(comboTrialManager);
	}

	public void OnStrategyButtonDown()
	{
		BeginManager(strategyManager);
	}

	public void OnTrainingButtonDown()
	{
		BeginManager(trainingManager);

	}

	public void OnCPUButtonDown()
	{
		BeginManager(aiManager);
	}

	public void OnTutorialButtonDown()
	{
		BeginManager(tutorialManager);
	}
	
	private void BeginManager(BaseManager manager){
		activeManager = manager;
		lobbyMusic.Stop();
		HideButtons();
		activeManager.Visible = true;
		activeManager.AttachGamescenes(charSelectScene, gameScene, winScene);
		AddChild(activeManager);
		GD.Print("Attaching gamescenes and starting manager");
		activeManager.Start();
	}

	private void BeginNetplayManager(PackedScene managerScene)
	{
		activeManager = managerScene.Instance<BaseManager>();
		AddChild(activeManager);
		activeManager.Visible = true;
		activeManager.AttachGamescenes(charSelectScene, gameScene, winScene);
		activeManager.Start();
	}

	private void OnNetPlayConnected()
	{
		lobbyMusic.Stop();
		HideButtons();
	}

	//config
	public void _on_ButtonConfig_pressed()
	{
		HideButtons();
		inputmenu.GetNode<ColorRect>("ConfigOverlay").Visible = true;
		column.GetNode<Button>("ReturnMainMenu").Visible = true;
		column.GetNode<Button>("ReturnToInGameMenu").Visible = false;
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
			activeManager.Visible = false;
			activeManager.Quit();
			RemoveChild(activeManager);
		}
		
		menuroot.Visible = true;
		inputmenu.GetNode<ColorRect>("ConfigOverlay").Visible = false;
		
		if (menuroot.GetNode<MarginContainer>("MainMenu").Visible == true)
		{
			mainmenubuttons.GetNode<ToolButton>("Local").GrabFocus();
		}
		lobbyMusic.Play();
		waitingForOtherPlayerLabel.Visible = false;
		sendToFriendLabel.Visible = false;
		newMatchId.Text = "";
		menuroot.Call(BackButtonPressedCallString);
	}

	private void HideButtons()
	{
		menuroot.Visible = false;
		inputmenu.GetNode<ColorRect>("ConfigOverlay").Visible = false;
	}
	
}
