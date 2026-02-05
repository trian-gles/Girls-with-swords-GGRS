using Godot;

public class SnailRadar: Control
{
    private const int MaxSnails = 128;

    private Vector2[] _positions = new Vector2[MaxSnails];
    private Color[] _colors = new Color[MaxSnails];
    private int _count = 0;

    public override void _PhysicsProcess(float delta)
    {
        Update();
    }

    public void DrawSnail(int globX, Color color)
    {
        if (globX < 0 || globX > 480)
            return;

        if (_count >= MaxSnails)
            return;

        float x = (float)globX / 480f * RectSize.x;

        _positions[_count] = new Vector2(x, 0f);
        _colors[_count] = color;
        _count++;
    }

    public override void _Draw()
    {
        for (int i = 0; i < _count; i++)
        {
            DrawCircle(_positions[i], 5f, _colors[i]);
        }

        // Reset without deallocating
        _count = 0;
    }
}
