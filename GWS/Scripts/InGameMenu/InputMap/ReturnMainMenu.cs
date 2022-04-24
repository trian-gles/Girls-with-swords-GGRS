using Godot;
using System;

public class ReturnMainMenu : Button
{
	public override void _Ready()
	{
		
	}
	
	[Signal]
	public delegate void QuitPressed();
	
	
	private void _on_button_down()
	{	
		EmitSignal(nameof(QuitPressed));
		
	}

}

