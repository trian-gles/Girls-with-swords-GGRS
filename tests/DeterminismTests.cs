using Godot;
using System;

public class DeterminismTests : WAT.Test
{
	public override string Title()
	{
		return "Testing if inputs have a deterministic effect on game state";
	}

	[Test]
	public void DeterministicInputs()
	{
		PackedScene mainScene = ResourceLoader.Load("res://MainScene.tscn") as PackedScene;
		MainScene ms = mainScene.Instance() as MainScene;
		GD.Print("Will mainscene call ready?");
		ms._Ready();
		//Assert.IsFalse(ms.recording.Visible);
	}
}
