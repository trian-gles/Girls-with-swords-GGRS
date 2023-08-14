using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

class GGRSManager : StateManager
{

	// Networking Objs
	private Node GGRS;
	private UPNP upnp;
	int port;

	

	private const int MAXPLAYERS = 2;
	private const int PLAYERNUMBERS = 2;
	private int localPlayerHandle;
	private int localHand = 1;
	private int otherHand = 2;
	private int waitFrames = 0;
	private Queue<int> queueLengths = new Queue<int>();

	private string opponentIp;
	private int opponentPort;
	private int localPort;
	private bool holePunched = false;
	private bool connected = false;

	// For AI integrated testing
	private bool aiTest = false;
	private Random random = new Random();


	public override void _Ready()
	{
		base._Ready();
		GGRS = GetNode("GodotGGRS");
		Globals.mode = Globals.Mode.GGPO;
		NatTraversal();
	}


	public void ManualConfig(string ip, bool hosting)
	{
		charSelectScene.ChangeHUDText("Waiting for connection...\n ");
		this.hosting = hosting;
		int localPort, remotePort;
		if (hosting)
		{
			localPort = 7070;
			remotePort = 7071;
			if (ip == "127.0.0.1")
				Globals.SetLogging("P1");
		}
		else
		{
			localPort = 7071;
			remotePort = 7070;
			
			if (ip == "127.0.0.1")
			{
				GD.Print("RUNNING TEST 'AI'");
				Globals.SetLogging("P2");
				aiTest = true;
			}
				

		}
		port = localPort;
		OpenPort();
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

	private void GGRSConfig()
	{
		GD.Print("Creating new session");
		GGRS.Call("create_new_session", localPort, PLAYERNUMBERS, 8);
		GD.Print("Created new session");
		localPlayerHandle = (int)GGRS.Call("add_local_player");
		GD.Print($"added local player with handle {localPlayerHandle}");
		var otherPlayerHandle = (int)GGRS.Call("add_remote_player", $"{opponentIp}:{opponentPort}");
		GD.Print($"added other player with handle {otherPlayerHandle} at {opponentIp}:{opponentPort}");


		GGRS.Call("set_callback_node", this);
		GGRS.Call("set_frame_delay", 2, localPlayerHandle);
		GGRS.Call("start_session");
		GD.Print("Settup finished");
		connected = true;
	}

	public override void OnCharactersSelected(int playerOne, int playerTwo, int colorOne, int colorTwo, int bkgIndex)
	{
		base.OnCharactersSelected(playerOne, playerTwo, colorOne, colorTwo, bkgIndex);
		ReadyForChange();
	}

	public override void OnRoundFinished(string winner)
	{
		GD.Print($"Game definitevly won on frame {Globals.frame}");
		base.OnRoundFinished(winner);
		ReadyForChange();
	}

	public override void _Process(float delta)
	{
		if (!holePunched)
			return;
		if (!connected)
			GGRSConfig();

		GGRS.Call("poll_remote_clients");
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

			int currentGGRSFrame = (int)GGRS.Call("get_current_frame");
			Globals.lastConfirmedFrame = (int)GGRS.Call("get_confirmed_frame");
			if (hosting && Globals.frame != currentGGRSFrame)
				Globals.Log($"FRAME MISMATCH Last confirmed frame is {Globals.lastConfirmedFrame}, current GGRS frame is {currentGGRSFrame}");

			// prediction threshold reached
			if (currentGGRSFrame - Globals.lastConfirmedFrame > 7)
			{
				GD.Print("TOO FAR AHEAD SKIPPING FRAME");
				return;
			}

			var events = (Godot.Collections.Array)GGRS.Call("get_events");
			foreach (var item in events)
			{
				var itemArr = (Godot.Collections.Array)item;
				if ((string)itemArr[0] == "WaitRecommendation")
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
				int inputs = GetInputs("");
				if (aiTest)
					inputs = random.Next(255);

				GGRS.Call("advance_frame", localPlayerHandle, inputs);
			}
			else
				GGRS.Call("advance_frame", localPlayerHandle, 0);

			

			GetNetStats();

		}
		else
		{
			return;
		}

	}

	private void GetNetStats()
	{
		var netStats = (Godot.Collections.Array)GGRS.Call("get_network_stats", 1);
		queueLengths.Enqueue((int)netStats[0]);
		if (queueLengths.Count > 5)
			queueLengths.Dequeue();

		charSelectScene.ChangeHUDText($"Ping = { netStats[1]}");
		gameScene.ChangeHUDText($"Ping = { netStats[1]}");
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

		if (Math.Abs(Globals.frame - frame) > 8)
			Globals.Log($"Suspicious rollback from {Globals.frame} to {frame}");
		Globals.frame = @frame;
		currGame.LoadState(@frame, buffer, checksum);
	}

	public void ggrs_advance_frame(Godot.Collections.Array<Godot.Collections.Array> combinedInputs)
	{	
		Globals.frame++;

		if (readyForChange && --waitBeforeChangeFrames < 0)
		{
			GD.Print($"Restarting game on frame {Globals.frame}");
			OnGameFinished("Game");
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

	// ----------------
	// NAT
	// ----------------
	private async void NatTraversal()
	{
		var holePuncherScript = (Script)(GD.Load("res://addons/Holepunch/holepunch_node.gd"));
		var holePuncher = (Node)holePuncherScript.Call("new");
		holePuncher.Set("rendevouz_address", "172.104.21.51");
		holePuncher.Set("rendevouz_port", 4000);
		AddChild(holePuncher);
		string player_id = OS.GetUniqueId();
		holePuncher.Call("start_traversal", 1, player_id);
		var result = (await ToSignal(holePuncher, "hole_punched"));
		localPort = (int)result[0];
		opponentPort = (int)result[1];
		opponentIp = (string)result[2];
		hosting = ((int)result[3]) == 1;
		holePunched = true;

		

		//hole_puncher.rendevouz_address = "1.1.1.1"
		//# the port the HolePuncher python application is running on
		//hole_puncher.rendevouz_port = "3000"
		//add_child(hole_puncher)

	}

	// ----------------
	// UPNP
	// ----------------
	private void OpenPort()
	{
		upnp = new UPNP();
		int err = upnp.Discover();

		if (err != 0)
		{
			GD.PushError(err.ToString());
			return;
		}

		if ((upnp.GetGateway() != null) && upnp.GetGateway().IsValidGateway())
		{
			err = upnp.AddPortMapping(port, port, (string)ProjectSettings.GetSetting("application/config/name"), "UDP");
			if (err != 0)
			{
				GD.PushError(err.ToString());
				return;
			}
			else
			{
				GD.Print($"Port {port} opened by UPNP");
			}
		}
		else
		{
			GD.Print("Unable to add UPNP for some reason");
		}

		
	}

	public override void _Notification(int what)
	{
		if (what == MainLoop.NotificationWmQuitRequest)
		{
			if (upnp != null)
			{
				upnp.DeletePortMapping(port);
			}

			GetTree().Quit();
		}
	}
}
