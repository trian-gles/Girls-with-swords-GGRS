using Godot;

public class HUDCombo : Label
{
	[Export] public float DissapearTime = 0.5f;
	[Export] public int MaxCombo = 999;

	private Timer _timer;
	private string[] _comboText;

	public override void _Ready()
	{
		_timer = GetNode<Timer>("Timer");

		// Allocate ONCE
		_comboText = new string[MaxCombo + 1];

		_comboText[0] = "";
		_comboText[1] = "";

		for (int i = 2; i <= MaxCombo; i++)
			_comboText[i] = "x" + i;
	}

	public void Combo(int comboNum)
	{
		if (comboNum <= MaxCombo)
			Text = _comboText[comboNum];

		_timer.Stop();
	}

	public void ComboSet(int comboNum)
	{
		if (comboNum <= MaxCombo)
			Text = _comboText[comboNum];
	}

	public void Off()
	{
		_timer.WaitTime = DissapearTime;
		_timer.Start();
	}

	private void _on_Timer_timeout()
	{
		Text = "";
	}
}
