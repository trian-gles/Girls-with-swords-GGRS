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
    private Dictionary<int, string> expectedStates = new Dictionary<int, string>();
    private int playSpeed = 1;

    [Export]
    private bool rollbackTesting = false;
    [Export]
    private int targetFrame = 500;
    [Export]
    private int rollbackFrame = 100;
    private File debugFile = new File();

    private bool recInputs = false;

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

    private Color colColor = new Color(0, 0, 255, 0.5f);
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

        if (rollbackTesting)
        {
            recording.Visible = true;
            recording.Text = "RECORDING";
            recInputs = true;
            debugFile.Open("res://tests/SyncTestResults.txt", File.ModeFlags.Write);
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

    private void CheckStateTesting(string expectedState)
    {
        //debugFile.StoreLine($"Sync testing for frame {Frame}");
        if (Frame == rollbackFrame)
        {
            debugFile.StoreLine("\n\nNEW PASS\n\n");
        }
        string foundState = JsonSerializer.Serialize<GameState>(GetGameState());
        bool error = false;
        for (int i = 0; i < foundState.Length; i++)
        {
            if (i >= expectedState.Length)
            {
                break;
            }
            if (expectedState[i] != foundState[i])
            {
                error = true;
            }
            
        }
        if (error)
        {
            GD.Print("Saved state does not match expected state");
            debugFile.StoreLine("Saved state does not match expected state");
            playSpeed = 0;

        }
        else
        {
            GD.Print("Saved state matches expected.");
            debugFile.StoreLine("Saved state matches expected.");
        }
        debugFile.StoreLine($"This state =      {foundState}");
        debugFile.StoreLine($"Expected state = {expectedState}");
    }

    private void SaveStateForTesting(int frame)
    {
        GameState gState = GetGameState();
        expectedStates[frame] = JsonSerializer.Serialize(gState);
    }

    private string LoadStateForTesting(int frame)
    {
        return expectedStates[frame];
    }

    private void SaveInputsForTesting() //
    {
        string jsonString = JsonSerializer.Serialize(savedInputs);
        File f = new File();
        f.Open("res://tests/SavedInputs.json", File.ModeFlags.Write);
        f.StoreString(jsonString);
        GD.Print("Saved inputs in SavedInputs.json");
    }

    public void LoadInputsForTesting() //used for testing
    {
        GD.Print("loading inputs for testing from SavedInputs.json");
        File f = new File();
        f.Open("res://tests/SavedInputs.json", File.ModeFlags.Read);
        GD.Print("File successfully opened");
        string jsonString = f.GetAsText();
        savedInputs = JsonSerializer.Deserialize<List<List<char[]>>>(jsonString);
        
    }


    private void PrintAllSavedInputs()
    {
        GD.Print("All saved inputs = ");
        foreach (List<char[]> frameInputs in savedInputs)
        {
            foreach (char[] input in frameInputs)
            {
                GD.Print(input[0]);
            }
        }
    }
    private void LoadState()
    {
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
        if (recInputs)
        {
            savedInputs.Add(new List<char[]>(thisFrameInputs));
            foreach (char[] input in savedInputs[Frame])
            {
                //GD.Print($"Saving frame {Frame} input {input[0]}");
            }
            if (rollbackTesting)
            {
                SaveStateForTesting(Frame);
            }
        }

        if (Frame == targetFrame && rollbackTesting)
        {
            if (recInputs)
            {
                recInputs = false;
                recording.Text = "PLAYBACK";
                playSpeed = 1;
            }

            GameState rolldState = JsonSerializer.Deserialize<GameState>(LoadStateForTesting(rollbackFrame));
            SetGameState(rolldState);

        }



        if (rollbackTesting && !recInputs)
        {
            CheckStateTesting(LoadStateForTesting(Frame));
            SaveStateForTesting(Frame); // every new pass saves it's state for future comparisons
            foreach (char[] input in savedInputs[Frame])
            {
                debugFile.StoreLine($"calling input {input[0]}, {input[1]} on frame {Frame}");
            }
            P1.inputHandler.setUnhandledInputs(new List<char[]>(savedInputs[Frame]));
            

        }
        else
        {
            if (rollbackTesting)
            {
                foreach (char[] input in thisFrameInputs)
                {
                    debugFile.StoreLine($"calling input {input[0]}, {input[1]} on frame {Frame}");
                }
            }
            P1.inputHandler.setUnhandledInputs(thisFrameInputs);
        }


        


        thisFrameInputs = new List<char[]>(); // reset the inputs
        

        Frame++;
        if (hitStopRemaining > 0) 
        { 
            hitStopRemaining--; 
        }
        

        camera.Call("adjust", P1.Position, P2.Position);
        if (hitStopRemaining < 1)
        {
            P1.FrameAdvance();
            P2.FrameAdvance();
            CheckFixCollision();
            P1.MoveSlideDeterministic();
            P2.MoveSlideDeterministic();
            CheckFixCollision();
        }

    }

    public void CheckFixCollision()
    {
        while (CheckRects())
        {
            if (P1.GlobalPosition < P2.GlobalPosition)
            {
                P1.Position = new Vector2(P1.Position.x - 1, P1.Position.y);
                P2.Position = new Vector2(P2.Position.x + 1, P2.Position.y);
            }
            else
            {
                P1.Position = new Vector2(P1.Position.x + 1, P1.Position.y);
                P2.Position = new Vector2(P2.Position.x - 1, P2.Position.y);
            }
        }
    }
    public bool CheckRects()
    {
        Rect2 P1rect = P1.GetCollisionRect();
        P1rect.Position = P1rect.Position + P1.Position;
        Rect2 P2rect = P2.GetCollisionRect();
        P2rect.Position = P2rect.Position + P2.Position;
        return P1rect.Intersects(P2rect);
    }

    public void HitStop() 
    {
        hitStopRemaining = maxHitStop;
        GD.Print("HitStop");
    }

    

    public override void _PhysicsProcess(float _delta)
    {
        for (int _ = 0; _ < playSpeed; _++)
        {
            FrameAdvance();
        }
        
    }
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("debug_shift"))
        {
            for (int _ = 0; _ < 100; _++)
            {
                FrameAdvance();
            }
            
        }

        else if (@event.IsActionPressed("ui_select"))
        {
            playSpeed = 1;
        }

        if (@event is InputEventKey)
        {
            foreach (char actionName in allowableInputs)
            {
                if (@event.IsActionPressed(actionName.ToString()))
                {
                    thisFrameInputs.Add(new char[] { actionName, 'p' });
                    
                }

                else if (@event.IsActionReleased(actionName.ToString()))
                {
                    thisFrameInputs.Add(new char[] { actionName, 'r' });
                }
            }
        }
    }


}
