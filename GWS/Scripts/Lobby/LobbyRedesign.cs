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
	Label connectionLabel;
	Popup mustUpdatePopup;
	Popup serverUnavailablePopup;
	Popup desyncPopup;
	Popup disconnectPopup;
	Node holePuncher;
	Node localProxy;
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
		connectionLabel = netplaymenu.GetNode<Label>("ConnectionLabel");

		existingMatchId = entries.GetNode<LineEdit>("ExistingMatchContainer/ExistingMatchID");
		sendToFriendLabel.Visible = false;
		connectionLabel.Visible = false;

		//button check menus
		inputmenu = GetNode<Control>("InputMenu/InputMenu");
		column = inputmenu.GetNode<VBoxContainer>("ConfigOverlay/Column");

		// connect in game menu
		events = GetNode<Node>("/root/Events");
		events.Connect(MainMenuPressedString, this, nameof(OnLobbyReset));
		// cache lobby music player
		lobbyMusic = GetNode<AudioStreamPlayer>("LobbyMusic");

		mustUpdatePopup = GetNode<Popup>("MenuRoot/UpdateRequired");
		serverUnavailablePopup = GetNode<Popup>("MenuRoot/ServerUnavailable");
		desyncPopup = GetNode<Popup>("MenuRoot/DesyncDetected");
		disconnectPopup = GetNode<Popup>("MenuRoot/Disconnected");

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
		hosting = true;
		newMatchId.Text = RandomString(8);
		Globals.netplaySessionName = newMatchId.Text;
		sendToFriendLabel.Visible = true;
		connectionLabel.Visible = true;
		BeginNetplayManager();
	}

	public void OnJoinNetplayMatch()
	{
		hosting = false;
		GD.Print(existingMatchId.Text);
		Globals.netplaySessionName = existingMatchId.Text;
		connectionLabel.Visible = true;
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
	
	private void OnHostProxyTestButtonDown()
	{
		BeginTestNetplayProxySession(true);
	}
	
	private void OnJoinProxyTestButtonDown()
	{
		BeginTestNetplayProxySession(false);
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
		Globals.mode = Globals.Mode.GGPO;
		var result = await ConnectToServerAndPeer();
		if (result)
		{
			HideButtons();
			activeManager.AttachGamescenes(charSelectScene, gameScene, winScene);
			AddChild(activeManager);
			ggrsManager.ManualConfig(opponentIp, hosting, localPort, opponentPort);
			ggrsManager.Connect("DesyncDetected", this, nameof(OnDesyncDetected));
			ggrsManager.Connect("Disconnected", this, nameof(OnDisconnected));
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
		ggrsManager.Connect("Disconnected", this, nameof(OnDisconnected));
		ggrsManager.Visible = true;
	}

	/// <summary>
	/// Connect to the locally hosted proxy network
	/// </summary>
	/// <param name="hosting"></param>
	private void BeginTestNetplayProxySession(bool hosting)
	{

		Globals.netplaySessionName = "TestSession";
		lobbyMusic.Stop();
		HideButtons();
		
		localPort = hosting ? 7001 : 7000;
		opponentPort = hosting ? 8001 : 8000; // replaced with the proxy
		InitProxyConnection(opponentPort, "172.104.215.127"); // production : "172.104.215.127"
		
		activeManager = ggrsManager;
		ggrsManager.AttachGamescenes(charSelectScene, gameScene, winScene);
		AddChild(ggrsManager);
		Globals.mode = Globals.Mode.GGPO;
		bool aiTest = hosting;
		ggrsManager.Start();
		ggrsManager.ManualConfig("127.0.0.1", hosting, localPort, opponentPort, aiTest);
		ggrsManager.Connect("DesyncDetected", this, nameof(OnDesyncDetected));
		ggrsManager.Connect("Disconnected", this, nameof(OnDisconnected));
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

	public void OnServerUnreachable()
	{
		RemoveChild(holePuncher);
		holePuncher.QueueFree();
		OnLobbyReset();
		serverUnavailablePopup.Visible = true;
		serverUnavailablePopup.PopupCentered();
	}

	public void OnUpdateRequired()
	{
		RemoveChild(holePuncher);
		holePuncher.QueueFree();
		OnLobbyReset();
		mustUpdatePopup.Visible = true;
		mustUpdatePopup.PopupCentered();
	}
	
	public void OnDesyncDetected()
	{
		RemoveChild(holePuncher);
		holePuncher.QueueFree();
		OnLobbyReset();
		desyncPopup.Visible = true;
		desyncPopup.PopupCentered();
	}

	public void OnDisconnected()
	{
		RemoveChild(holePuncher);
		holePuncher.QueueFree();

		OnLobbyReset();
		disconnectPopup.Visible = true;
		disconnectPopup.PopupCentered();
	}

	public void OnServerContacted()
	{
		connectionLabel.Text = "Server contacted, waiting for other player...";
	}

	public void OnPeerFound()
	{
		connectionLabel.Text = "Opponent found, routing connection...";
	}

	public void OnHolepunchFailed(object result)
	{
		BeginTestNetplayProxySession((bool)result);

	}

	public void OnLobbyReset()
	{
		if (activeManager != null)
		{
			activeManager.Visible = false;
			activeManager.Quit();
			RemoveChild(activeManager);
		}

		if (localProxy != null)
		{
			RemoveChild(localProxy);
			localProxy.QueueFree();
		}
		
		menuroot.Visible = true;
		inputmenu.GetNode<ColorRect>("ConfigOverlay").Visible = false;
		
		if (menuroot.GetNode<MarginContainer>("MainMenu").Visible == true)
		{
			mainmenubuttons.GetNode<ToolButton>("Local").GrabFocus();
		}
		lobbyMusic.Play();
		connectionLabel.Visible = false;
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

	/// <summary>
	/// Checks version, determines who is P1, and connects to peer
	/// </summary>
	/// <returns></returns>
	private async Task<bool> ConnectToServerAndPeer()
	{
		var version = Globals.GetVersion();
		var holePuncherScript = (Script)(GD.Load("res://addons/Holepunch/holepunch_node.gd"));
		

		holePuncher = (Node)holePuncherScript.Call("new");
		holePuncher.Connect("wrong_version", this, nameof(OnUpdateRequired));
		holePuncher.Connect("server_unreachable", this, nameof(OnServerUnreachable));
		holePuncher.Connect("contacted_server", this, nameof(OnServerContacted));
		holePuncher.Connect("found_peer", this, nameof(OnPeerFound));

		holePuncher.Set("rendevouz_address", "172.104.215.127"); // production : "172.104.215.127"
		holePuncher.Set("rendevouz_port", 4000);
		AddChild(holePuncher);
		char id_addition = hosting ? '0' : '1';
		string player_id = OS.GetUniqueId() + id_addition;
		holePuncher.Call("start_traversal", Globals.netplaySessionName, player_id, version);
		var result = (await ToSignal(holePuncher, "hole_punched"));
		localPort = (int)result[0];
		opponentPort = (int)result[1];
		opponentIp = (string)result[2];
		hosting = ((int)result[3]) == 1;
		bool success = (bool) result[4];
		if (success)
		{
			Globals.hosting = hosting;
			GD.Print("WE HAVE PUNCHED ZE HOLE");
			connectionLabel.Text = "P2P holepunch confirmed, running cleanup...";
			await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
			RemoveChild(holePuncher);
			holePuncher.QueueFree();
			await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
		}
		else
		{
			// connecting via proxy
			opponentIp = "127.0.0.1"; 
			opponentPort = hosting ? 8000 : 8001; // proxy port
			connectionLabel.Text = "Unable to form P2P connection, connecting via proxy...";
			InitProxyConnection(opponentPort, "172.104.215.127"); // production : "172.104.215.127"
			connectionLabel.Text = "Connected via proxy";
			await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
			RemoveChild(holePuncher);
			holePuncher.QueueFree();
			await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
		}

		return true;
	}

	// ----------------
	// PROXY CONNECTION
	// ----------------

/// <summary>
/// Runs a local proxy, which sends the room code to the remote proxy and then routes GGRS traffic to that remote proxy.  Note: the proxy will figure out what port GGRS is running on
/// </summary>
/// <param name="localProxyPort">Port which GGRS must connect to</param>
/// <param name="serverIP"></param>
/// <param name="serverPort"></param>
	private void InitProxyConnection(int localProxyPort, string serverIP)
	{
		var proxyScript = (Script)(GD.Load("res://addons/LocalProxy/local_proxy.gd"));
		localProxy = (Node)proxyScript.Call("new");
		AddChild(localProxy);
		localProxy.Call("start_proxy", Globals.netplaySessionName, serverIP, 9999, localProxyPort);
	}
	
}
