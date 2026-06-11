using Godot;
using System;
using System.Linq;
using System.Management.Instrumentation;
using System.Threading.Tasks;

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
	Popup mustUpdatePopup;
	Popup desyncPopup;
	private string opponentIp;
	private int opponentPort;
	private int localPort;
	private bool hosting;

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
	public bool gcStressTest = false;

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
		for (int i = 0; i < 4; i++)
		{
			AddChild(Globals.P1Characters[i]);
			RemoveChild(Globals.P1Characters[i]);
			AddChild(Globals.P2Characters[i]);
			RemoveChild(Globals.P2Characters[i]);
		}

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

		//mustUpdatePopup = GetNode<Popup>("CanvasLayer/UpdateRequired");
		desyncPopup = GetNode<Popup>("MenuRoot/DesyncDetected");

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
	public override void _Process(float delta)
	{
		base._Process(delta);
		if (gcStressTest)
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
		}
	}

	private void syncTestBegin()
	{
		Globals.mode = Globals.Mode.SYNCTEST;
		BeginManager(syncTestManager);
	}


	public void OnNewNetplayMatch()
	{
		newMatchId.Text = RandomString(8);
		Globals.netplaySessionName = newMatchId.Text;
		sendToFriendLabel.Visible = true;
		waitingForOtherPlayerLabel.Visible = true;
		BeginNetplayManager();
	}

	public void OnJoinNetplayMatch()
	{
		GD.Print(existingMatchId.Text);
		Globals.netplaySessionName = existingMatchId.Text;
		waitingForOtherPlayerLabel.Visible = true;
		BeginNetplayManager();
	}



	public void OnHostTestButtonDown()
	{
		BeginTestNetplaySession(true);
	}

	public void OnJoinTestButtonDown()
	{
		BeginTestNetplaySession(false);
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

	private async void BeginNetplayManager()
	{
		activeManager = ggrsManager;
		var result = await NatTraversal();
		if (result)
		{
			activeManager.AttachGamescenes(charSelectScene, gameScene, winScene);
			AddChild(activeManager);
			ggrsManager.ManualConfig(opponentIp, hosting, localPort, opponentPort);
			ggrsManager.Connect("DesyncDetected", this, nameof(OnDesyncDetected));
			activeManager.Visible = true;
			activeManager.Start();
		}

	}

	private void BeginTestNetplaySession(bool hosting)
	{
		lobbyMusic.Stop();
		HideButtons();
		localPort = hosting ? 7001 : 7000;
		opponentPort = hosting ? 7000 : 7001;
		activeManager = ggrsManager;
		ggrsManager.AttachGamescenes(charSelectScene, gameScene, winScene);
		AddChild(ggrsManager);
		Globals.mode = Globals.Mode.GGPO;
		bool aiTest = hosting;
		ggrsManager.Start();
		ggrsManager.ManualConfig("127.0.0.1", hosting, localPort, opponentPort, aiTest);
		ggrsManager.Connect("DesyncDetected", this, nameof(OnDesyncDetected));
		ggrsManager.Visible = true;
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
	
	public void OnDesyncDetected()
	{
		desyncPopup.Visible = true;
		desyncPopup.PopupCentered();
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


	// ----------------
	// NAT
	// ----------------
	private async Task<bool> NatTraversal()
	{
		var version = Globals.GetVersion();
		var holePuncherScript = (Script)(GD.Load("res://addons/Holepunch/holepunch_node.gd"));
		

		var holePuncher = (Node)holePuncherScript.Call("new");
		holePuncher.Connect("wrong_version", this, nameof(OnWrongVersionReject));

		holePuncher.Set("rendevouz_address", "172.104.215.127"); // production : "172.104.215.127"
		holePuncher.Set("rendevouz_port", 4000);
		AddChild(holePuncher);
		string player_id = OS.GetUniqueId();
		holePuncher.Call("start_traversal", Globals.netplaySessionName, player_id, version);
		var result = (await ToSignal(holePuncher, "hole_punched"));
		localPort = (int)result[0];
		opponentPort = (int)result[1];
		opponentIp = (string)result[2];
		hosting = ((int)result[3]) == 1;
		Globals.hosting = hosting;
		GD.Print("WE HAVE PUNCHED ZE HOLE");
		RemoveChild(holePuncher);
		holePuncher.QueueFree();
		return true;
	}

	public void OnWrongVersionReject()
	{
		GD.Print("Outdated!");
		//mustUpdatePopup.PopupCentered();
	}
	
}
