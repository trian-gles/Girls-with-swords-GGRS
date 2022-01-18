using Godot;
using System;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading;


public class MainMenu : Button
{
	[Signal]
	public delegate void LobbyReturn();

	private void CloseMainscene()
	{
		GD.Print("Emitting lobby return signal");
		EmitSignal(nameof(LobbyReturn));
		GD.Print("Emitted lobby return signal, queueing free");
		QueueFree();QueueFree();
	}
	
	private void _on_button_down()
	{	
		GetTree().Paused = false;
		CloseMainscene();
	}
}


