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


    private List<List<char[]>> savedInputs = new List<List<char[]>>();
    private List<GameState> savedStates = new List<GameState>();
    private List<char[]> thisFrameInputs = new List<char[]>();

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

    private Color colColor = new Color(0, 0, 255, 0.5f);
    public override void _Ready()
    {
        GD.Print("Running");
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
    private void SetGameState(GameState gState)
    {
        Frame = gState.frame;
        hitStopRemaining = gState.hitStopRemaining;
        P1.SetState(gState.P1State);
        P2.SetState(gState.P2State);
        
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
        P1.SetUnhandledInputs(thisFrameInputs);

        AdvanceFrameAndHitstop();

        thisFrameInputs = new List<char[]>(); // reset the inputs
        

        camera.Call("adjust", P1.Position, P2.Position);
        FrameAdvancePlayers();

    }

    private void AdvanceFrameAndHitstop()
    {
        Frame++;
        if (hitStopRemaining > 0)
        {
            hitStopRemaining--;
        }
    }

    private void FrameAdvancePlayers()
    {
        if (hitStopRemaining < 1)
        {
            P1.FrameAdvance();
            P2.FrameAdvance();
            CheckFixCollision();
            P1.MoveSlideDeterministicTwo();
            P2.MoveSlideDeterministicTwo();
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
        FrameAdvance();
       
        
    }
    public override void _Input(InputEvent @event)
    {

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
