using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class GGRSManager : StateManager
{

	// Networking Objs
	private Node GGRS;

	[Export]
	private PackedScene ggrsScene;

	int port;

	private Node events;

	private Popup mustUpdatePopup;

	private List<string> pingMessages;
	

	private const int MAXPLAYERS = 2;
	private const int PLAYERNUMBERS = 2;
	private int localPlayerHandle;
	private int localHand = 1;
	private int otherHand = 2;
	private int waitFrames = 0;

	private string opponentIp;
	private int opponentPort;
	private int localPort;
	private bool connected = false;

	// For AI integrated testing
	private bool aiTest = false;
	private Random random = new Random();


	public override void _Ready()
	{
		events = GetNode<Node>("/root/Events");
		for (int i = 0; i < 400; i++)
			pingMessages[i] = $"Ping = {i}";
		//GGRS = GetNode("GodotGGRS");
	}

	public override void Start()
	{
		Globals.frame = 0;
		Globals.autoTech = false;
	}

	public override void Quit()
	{
		base.Quit();
		GGRS.QueueFree();
		RemoveChild(GGRS);
	}

	public void OpponentConfirmed()
	{
		ClearHUDText();
		Visible = true;
	}
	
	public void OnUpdateRequiredConfirmed()
	{
		const string MainMenuPressedString = "MainMenuPressed";
		events.Call("emit_signal", MainMenuPressedString);
	}


	public void ManualConfig(string ip, bool hosting, int localPort, int remotePort, bool aiTest=false)
	{
		GGRS = ggrsScene.Instance();
		AddChild(GGRS);
		charSelectScene.ChangeHUDText("Waiting for connection...\n ");
		this.hosting = hosting;
		Globals.hosting = hosting;
		this.aiTest = aiTest;
		
		
		port = localPort;
		GD.Print("Creating new session");
		GGRS.Call("create_new_session", localPort, PLAYERNUMBERS, 8);
		GD.Print("Created new session");
		localPlayerHandle = (int)GGRS.Call("add_local_player");
		GD.Print($"added local player with handle {localPlayerHandle}");
		var otherPlayerHandle = (int)GGRS.Call("add_remote_player", $"{ip}:{remotePort}");
		GD.Print($"added other player with handle {otherPlayerHandle} at {ip}:{remotePort}");


		GGRS.Call("set_callback_node", this);
		GGRS.Call("set_frame_delay", 2, localPlayerHandle);
		GGRS.Call("start_session");
		GD.Print("Settup finished");
		connected = true;
	}

	public override void OnRematch()
	{
		ReadyForChange(GameType.GAME);
	}

	public override void OnReselectChar()
	{
		ReadyForChange(GameType.CHARSELECT);
	}

	public override void OnCharactersSelected(int playerOne, int playerTwo, int colorOne, int colorTwo, int bkgIndex)
	{
		base.OnCharactersSelected(playerOne, playerTwo, colorOne, colorTwo, bkgIndex);
		ReadyForChange(GameType.GAME);
	}

	public override void OnGameWon(string winner, int character)
	{
		base.OnGameWon(winner, character);
	}

	public override void _Process(float delta)
	{
		if (connected)
			GGRS.Call("poll_remote_clients"); // could be handled by a GDSCRIPT object for better performance
		
	}

	// ----------------
	// Frame handling
	// ----------------
	public override void _PhysicsProcess(float _delta)
	{
		if (!connected)
			return;

		currGame.TimeAdvance();

		if ((bool)GGRS.Call("is_running"))
		{
			if (Globals.frame % 60 == 0)
				GetNetStats();
			int currentGGRSFrame = (int)GGRS.Call("get_current_frame"); // should be combined in one call, also with events
			Globals.lastConfirmedFrame = (int)GGRS.Call("get_confirmed_frame");

			// prediction threshold reached
			if (currentGGRSFrame - Globals.lastConfirmedFrame > 7)
			{
				return;
			}

			var events = (Godot.Collections.Array)GGRS.Call("get_events"); // Could be a much simpler call
			foreach (var item in events)
			{
				var itemArr = (Godot.Collections.Array)item;
				if ((string)itemArr[0] == "WaitRecommendation") // definitely shouldn't be a string
				{
					waitFrames = (int)itemArr[1];
				}

			}

			if (waitFrames > 0)
			{
				waitFrames--;
				return;
			}

			if (currGame.AcceptingInputs())
			{
				int inputs = GetInputs(0);
				if (aiTest)
					inputs = random.Next(255);

				GGRS.Call("advance_frame", localPlayerHandle, inputs);
			}
			else
				GGRS.Call("advance_frame", localPlayerHandle, 0);
		}
		else
		{
			return;
		}

	}

	private void GetNetStats()
	{
		var netStats = (Godot.Collections.Array)GGRS.Call("get_network_stats", 1); // should only return ping

		charSelectScene.ChangeHUDText(pingMessages[(int)netStats[1]]);
		gameScene.ChangeHUDText(pingMessages[(int)netStats[1]]);
	}

	// ----------------
	// GGRS Callbacks
	// ----------------
	public byte[] ggrs_save_game_state(int frame)
	{
		return currGame.SaveState(frame);
	}

	public void ggrs_load_game_state(int @frame, byte[] buffer, int checksum)
	{
		Globals.frame = @frame;
		currGame.LoadState(@frame, buffer, checksum);
	}



	public void ggrs_advance_frame(Godot.Collections.Array<Godot.Collections.Array> combinedInputs)
	{	
		Globals.frame++;

		if (readyForChange && --waitBeforeChangeFrames < 0)
		{
			StartNextGame();
			readyForChange = false;
		}

		int p1Inps = (int)combinedInputs[0][2];
		int p2Inps = (int)combinedInputs[1][2];
		if (hosting)
		{
			currGame.GGRSAdvanceFrame(p1Inps, p2Inps);
		}
		else
		{
			currGame.GGRSAdvanceFrame(p2Inps, p1Inps);
		}
	}
}
