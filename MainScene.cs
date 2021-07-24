using Godot;
using System;
using System.Text.Json;
using System.Collections.Generic;

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
    public Label recording;
    private List<List<char[]>> savedInputs = new List<List<char[]>>();
    private List<char[]> thisFrameInputs = new List<char[]>();

    [Export]
    private bool testing = false;

    private int hitStopRemaining = 0;
    [Export]
    private int maxHitStop = 10;

    private char[] allowableInputs = new char[] { '8', '4', '2', '6', 'p', 'k', 's' };

    private string savedState;
    private int finalFrame = 0;
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
        recording = GetNode<Label>("HUD/Recording");
        recording.Visible = false;

        

        P1Combo.Text = "";
        P2Combo.Text = "";
        P1.otherPlayer = P2;
        P2.otherPlayer = P1;
        P1.CheckTurnAround();
        P2.CheckTurnAround();
        P2.inputHandler.heldKeys.Add('8');
        P1.Connect("HitConfirm", this, nameof(HitStop));
        P2.Connect("HitConfirm", this, nameof(HitStop));

        if (testing)
        {
            recording.Visible = true;
            recording.Text = "PLAYBACK";
            LoadInputsForTesting();
        }
    }

    private GameState GetGameState()
    {
        GameState gState = new GameState();
        gState.frame = Frame;
        gState.P1State = P1.GetState();
        gState.P2State = P2.GetState();
        gState.hitStopRemaining = hitStopRemaining;
        recording.Visible = true;
        
        return gState;
    }

    private void SetGameState(GameState gState)
    {
        Frame = gState.frame;
        hitStopRemaining = gState.hitStopRemaining;
        P1.SetState(gState.P1State);
        P2.SetState(gState.P2State);
        
    }

    private void SaveState()
    {
        GameState gState = GetGameState();
        savedState = JsonSerializer.Serialize(gState);

    }

    private void CheckStateTesting(string savedState)
    {
        string expectedState = LoadStateForTesting();
        GD.Print($"This state = {savedState}");
        GD.Print($"Expected state = {expectedState}");
        bool error = false;
        for (int i = 0; i < savedState.Length; i++)
        {
            if (expectedState[i] != savedState[i])
            {
                GD.Print($"{savedState[i]} != {expectedState[i]}");
                error = true;
            }
            
        }
        if (error)
        {
            GD.Print("Saved state does not match expected state");
        }
        else
        {
            GD.Print("Saved state matches expected.");
        }
    }

    private void SaveStateForTesting(string savedState)
    {
        File f = new File();
        f.Open("res://tests/FinalState.json", File.ModeFlags.Write);
        f.StoreString(savedState);
    }

    private string LoadStateForTesting()
    {
        File f = new File();
        f.Open("res://tests/FinalState.json", File.ModeFlags.Read);
        return f.GetAsText();
    }

    private void SaveInputsForTesting() //
    {
        string jsonString = JsonSerializer.Serialize(savedInputs);
        File f = new File();
        f.Open("res://tests/SavedInputs.json", File.ModeFlags.Write);
        f.StoreString(jsonString);
    }

    public void LoadInputsForTesting() //used for testing
    {
        
        File f = new File();
        f.Open("res://tests/SavedInputs.json", File.ModeFlags.Read);
        string jsonString = f.GetAsText();
        savedInputs = JsonSerializer.Deserialize<List<List<char[]>>>(jsonString);
    }

    private void LoadState()
    {
        GD.Print($"P1 has finally arrived at {P1.Position}");
        finalFrame = Frame;
        GameState gState = JsonSerializer.Deserialize<GameState>(savedState);
        SetGameState(gState);
        
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
        if (Frame == 249 && testing)
        {
            SaveState();
            CheckStateTesting(savedState);
        }
        


        savedInputs.Add(thisFrameInputs);
        thisFrameInputs = new List<char[]>();
        if (Frame < savedInputs.Count)
        {
            foreach (char[] inp in savedInputs[Frame])
            {
                P1.inputHandler.NewInput(inp[0], inp[1]);
            }

        }

        Frame++;
        if (hitStopRemaining > 0) 
        { 
            hitStopRemaining--; 
        }
        

        camera.Call("adjust", P1.Position, P2.Position);

        P1.FrameAdvance((hitStopRemaining > 0));
        P2.FrameAdvance((hitStopRemaining > 0));
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
        FrameAdvance();
    }
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("debug_shift"))
        {
            GD.Print("Saving final state and inputs");
            SaveState();
        }

        else if (@event.IsActionPressed("ui_select"))
        {
            FrameAdvance();
        }

        if (@event is InputEventKey)
        {
            foreach (char actionName in allowableInputs)
            {
                if (@event.IsActionPressed(actionName.ToString()))
                {
                    P1.inputHandler.NewInput(actionName, 'p');
                    thisFrameInputs.Add(new char[] { actionName, 'p' });
                    
                }

                else if (@event.IsActionReleased(actionName.ToString()))
                {
                    P1.inputHandler.NewInput(actionName, 'r');
                    thisFrameInputs.Add(new char[] { actionName, 'r' });
                }
            }
        }
    }


}
