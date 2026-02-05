using Godot;

public class ElectricShock : Node2D
{
	[Export] public int PointCount = 30;
	[Export] public float Radius = 16f;
	[Export] public float RandomRadius = 6f;

	private Vector2[] _points;

	public override void _Ready()
	{
		// Allocate ONCE (+1 to close the polyline)
		_points = new Vector2[PointCount + 1];

		GenPoints();
	}

	private void GenPoints()
	{
		float twoPi = 2f * Mathf.Pi;

		for (int i = 0; i < PointCount; i++)
		{
			float angle = twoPi * i / PointCount;
			float randAngle = GD.Randf() * twoPi;
			float randRad = GD.Randf() * RandomRadius;

			float height = Mathf.Sin(angle) * Radius + Mathf.Sin(randAngle) * randRad;
			float width  = Mathf.Cos(angle) * Radius + Mathf.Cos(randAngle) * randRad;

			_points[i] = new Vector2(height, width);
		}

		// Close the polyline
		_points[PointCount] = _points[0];

		Update();
	}

	public override void _Draw()
	{
		DrawPolyline(_points, Colors.White, 1.1f, false);
	}

	private void _on_Timer_timeout()
	{
		GenPoints();
	}
}
