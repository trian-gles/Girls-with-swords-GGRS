using Godot;
using System;
using System.Text.Json;

public class MainScene : Node2D
{
    public int Frame = 0;
    public Player P1;
    public Player P2;
    private Label P1Combo;
    private Label P2Combo;
    private TextureProgress P1Health;
    private TextureProgress P2Health;
    private Camera2D camera;


    private int hitStopRemaining = 0;
    [Export]
    private int maxHitStop = 10;

    private char[] allowableInputs = new char[] { '8', '4', '2', '6', 'p', 'k', 's' };

    private struct GameState
    {
        public int frame { get; set; }
        public Player.PlayerState P1State { get; set; }
        public Player.PlayerState P2State { get; set; }
        public int hitStopRemaining { get; set; }
    }

    public override void _Ready()
    {
        camera = GetNode<Camera2D>("Camera2D");

        P1 = GetNode<Player>("P1");
        P2 = GetNode<Player>("P2");

        P1.Connect("ComboChanged", this, nameof(OnPlayerComboChange));
        P2.Connect("ComboChanged", this, nameof(OnPlayerComboChange));
        P1.Connect("HealthChanged", this, nameof(OnPlayerHealthChange));
        P2.Connect("HealthChanged", this, nameof(OnPlayerHealthChange));

        P1Combo = GetNode<Label>("HUD/P1Combo");
        P2Combo = GetNode<Label>("HUD/P2Combo");
        P1Health = GetNode<TextureProgress>("HUD/P1Health");
        P2Health = GetNode<TextureProgress>("HUD/P2Health");

        P1Combo.Text = "";
        P2Combo.Text = "";
        P1.otherPlayer = P2;
        P2.otherPlayer = P1;
        P1.CheckTurnAround();
        P2.CheckTurnAround();
        P2.inputHandler.heldKeys.Add('8');
        P1.Connect("HitConfirm", this, nameof(HitStop));
        P2.Connect("HitConfirm", this, nameof(HitStop));
    }

    private GameState GetGameState()
    {
        GameState gState = new GameState();
        gState.frame = Frame;
        gState.P1State = P1.GetState();
        gState.P2State = P2.GetState();
        gState.hitStopRemaining = hitStopRemaining;

        return gState;
    }


    private void SaveState()
    {
        GameState gState = GetGameState();
        string jsonString = JsonSerializer.Serialize(gState);
        GD.Print(jsonString);
        var file = new File();
        file.Open("res://SavedState.json", File.ModeFlags.Write);
        file.StoreString(jsonString);
        file.Close();
    }
    public void OnPlayerComboChange(string name, int combo)
    {
        if (name == "P2")
        {
            if (combo > 1)
            {
                P1Combo.Call("combo", combo);
            }
            else
            {
                P1Combo.Call("off");
            }
        }

        else
        {
            if (combo > 1)
            {
                P2Combo.Call("combo", combo);
            }
            else
            {
                P2Combo.Call("off");
            }
        }
    }

    public void OnPlayerHealthChange(string name, int health)
    {
        if (name == "P2")
        {
            P2Health.Value = health;
        }

        else
        {
            P1Health.Value = health;
        }
    }

    public void FrameAdvance() 
    {
        Frame++;
        if (hitStopRemaining > 0) 
        { 
            hitStopRemaining--; 
        }

        camera.Call("adjust", P1.Position, P2.Position);
        // GD.Print($"Advance to frame {Frame}");

    }

    public void HitStop() 
    {
        hitStopRemaining = maxHitStop;
        GD.Print("HitStop");
    }

    

    public override void _PhysicsProcess(float _delta)
    {
        FrameAdvance();
        P1.FrameAdvance((hitStopRemaining > 0));
        P2.FrameAdvance((hitStopRemaining > 0));
    }
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_select"))
        {
            GD.Print("Saving state");
            SaveState();
        }

        if (@event is InputEventKey)
        {
            foreach (char actionName in allowableInputs)
            {
                if (@event.IsActionPressed(actionName.ToString()))
                {
                    P1.inputHandler.NewInput(actionName, 'p');
                }

                else if (@event.IsActionReleased(actionName.ToString()))
                {
                    P1.inputHandler.NewInput(actionName, 'r');
                }
            }
        }
    }


}
