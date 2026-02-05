using Godot;
using System;

public class DrawFX : Node2D
{
    Vector2 start;
    Vector2 end;

    private int drawFrames = 0;

    RandomNumberGenerator rng = new RandomNumberGenerator();

    public void Slash(Vector2 pos)
    {
        rng.Seed = (ulong)pos.x;
        var rotation = (rng.RandfRange(0, 1) * Mathf.Pi);
        var mod = new Vector2(1000, 0).Rotated(rotation);
        end = pos + mod;
        start = pos - mod;
        drawFrames = 20;
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        Update();
        if (drawFrames > 0)
            drawFrames--;
    }

    public override void _Draw()
    {
        base._Draw();
        if (drawFrames > 0)
		    DrawLine(start, end, new Color(0, 0, 0), drawFrames / 3);
    }
}