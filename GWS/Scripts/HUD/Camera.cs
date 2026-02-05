using Godot;

public class Camera : Camera2D
{
	[Export] public bool DebugMode = true;

	private Rect2 _cameraRect = new Rect2();
	private Rect2 _viewportRect = new Rect2();

	[Export] public float Decay = 0.8f;              // How quickly the shaking stops [0, 1]
	[Export] public Vector2 MaxOffset = new Vector2(100, 75); // Max hor/ver shake in pixels
	[Export] public float MaxRoll = 0.1f;             // Max rotation in radians

	private float _trauma = 0.0f;     // Current shake strength
	private int _traumaPower = 2;     // Trauma exponent [2, 3]

	public override void _Ready()
	{
		_viewportRect = GetViewportRect();
	}

	public void SetTrauma(float amount)
	{
		_trauma = amount;
	}

	public void Adjust(Vector2 p1Pos, Vector2 p2Pos)
	{
		if (_trauma > 0.0f)
		{
			_trauma = Mathf.Max(_trauma - Decay / 60f, 0f);
		}

		float amount = Mathf.Pow(_trauma, _traumaPower);

		Rotation = MaxRoll * amount * (float)GD.RandRange(-1.0, 1.0);

		// Build camera rect from the two positions
		_cameraRect = new Rect2(p1Pos, Vector2.Zero);
		_cameraRect = _cameraRect.Expand(p2Pos);

		Vector2 desiredZoom = CalculateZoom(_cameraRect, _viewportRect.Size);
		Vector2 desiredOffset = CalculateCenter(_cameraRect);

		Offset = Offset.LinearInterpolate(desiredOffset, 0.5f);

		// Apply shake
		Offset += new Vector2(
			MaxOffset.x * amount * (float)GD.RandRange(-1.0, 1.0),
			MaxOffset.y * amount * (float)GD.RandRange(-1.0, 1.0)
		);

		Zoom = Zoom.LinearInterpolate(desiredZoom, 0.5f);

		// --- Vertical bounds ---
		float yHeight = 270f * Zoom.y;
		float yBottom = yHeight / 2f + Offset.y;
		float yBelow = 260f - yBottom;
		float yAbove = Offset.y - yHeight / 2f;

		if (yAbove < 0f)
			Offset = new Vector2(Offset.x, Offset.y - yAbove);

		if (yBelow < 0f)
			Offset = new Vector2(Offset.x, Offset.y + yBelow);

		// --- Horizontal bounds ---
		float xWidth = 480f * Zoom.x;
		float xBelow = Offset.x - xWidth / 2f;

		if (xBelow < 0f)
			Offset = new Vector2(Offset.x - xBelow, Offset.y);

		float xRight = xWidth / 2f + Offset.x;
		float xAbove = 480f - xRight;

		if (xAbove < 0f)
			Offset = new Vector2(Offset.x + xAbove, Offset.y);
	}

	private Vector2 CalculateCenter(Rect2 rect)
	{
		return new Vector2(
			rect.Position.x + rect.Size.x / 2f,
			rect.Position.y + rect.Size.y / 2f
		);
	}

	private Vector2 CalculateZoom(Rect2 rect, Vector2 viewportSize)
	{
		float maxZoom = Mathf.Max(
			Mathf.Max(0.6f, rect.Size.x / viewportSize.x + 0.06f),
			Mathf.Max(0.6f, rect.Size.y / viewportSize.y)
		);

		if (maxZoom > 1f)
			maxZoom = 1f;

		return new Vector2(maxZoom, maxZoom);
	}
}
