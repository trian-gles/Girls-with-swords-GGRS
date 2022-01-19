using Godot;
using System;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading;


public class MainMenu : Button
{
	[Signal]
	public delegate void QuitPressed();
	
	
	private void _on_button_down()
	{	
		EmitSignal(nameof(QuitPressed));
	}
	
	public void _on_PauseOverlay_visibility_changed()
	{
		GrabFocus();
	}
}
