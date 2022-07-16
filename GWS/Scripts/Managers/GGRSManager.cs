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

	public override void _Ready()
	{
		base._Ready();
		GGRS = GetNode("GodotGGRS");
	}


	public void Config(string ip, bool hosting)
	{
		charSelectScene.ChangeHUDText("Waiting for connection...\n ");
		this.hosting = hosting;
		int localPort, remotePort;
		if (hosting)
		{
			localPort = 7070;
			remotePort = 7071;
			Globals.SetLogging("P1");
		}
		else
		{
			localPort = 7071;
			remotePort = 7070;
			Globals.SetLogging("P2");
		}
		port = localPort;
		OpenPort();
		GGRS.Call("create_new_session", localPort, PLAYERNUMBERS, 8);

		localPlayerHandle = (int)GGRS.Call("add_local_player");
		GD.Print($"added local player with handle {localPlayerHandle}");
		var otherPlayerHandle = (int)GGRS.Call("add_remote_player", $"{ip}:{remotePort}");
		GD.Print($"added other player with handle {otherPlayerHandle}");


		GGRS.Call("set_callback_node", this);
		GGRS.Call("set_frame_delay", 2, localPlayerHandle);
		GGRS.Call("start_session");
		GD.Print("Settup finished");
	}

	public override void OnCharactersSelected(PackedScene playerOne, PackedScene playerTwo, int colorOne, int colorTwo)
	{
		base.OnCharactersSelected(playerOne, playerTwo, colorOne, colorTwo);
		ReadyForChange();
	}

	public override void OnRoundFinished(string winner)
	{
		base.OnRoundFinished(winner);
		ReadyForChange();
	}

	public override void _Process(float delta)
	{
		GGRS.Call("poll_remote_clients");
	}

	// ----------------
	// Frame handling
	// ----------------
	public override void _PhysicsProcess(float _delta)
	{
		currGame.TimeAdvance();

		if (currGame.AcceptingInputs() && (bool)GGRS.Call("is_running"))
		{
			if (waitFrames > 0)
			{
				waitFrames--;
				return;
			}
			GGRS.Call("advance_frame", localPlayerHandle, GetInputs(""));

			var events = (Godot.Collections.Array)GGRS.Call("get_events");
			foreach (var item in events)
			{
				var itemArr = (Godot.Collections.Array)item;
				if ((string)itemArr[0] == "WaitRecommendation")
				{
					waitFrames = (int)itemArr[1];
				}

			}

			GetNetStats();

		}
		else if (!currGame.AcceptingInputs() && (bool)GGRS.Call("is_running"))
		{
			GGRS.Call("advance_frame", localPlayerHandle, 0);
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

	public void ggrs_load_game_state(int frame, byte[] buffer, int checksum)
	{
		currGame.LoadState(frame, buffer, checksum);
	}

	public void ggrs_advance_frame(Godot.Collections.Array<Godot.Collections.Array> combinedInputs)
	{
		frame++;
		if (readyForChange && --waitBeforeChangeFrames < 0)
		{
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
