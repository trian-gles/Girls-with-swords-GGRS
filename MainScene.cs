using Godot;
using System;
using System.Text.Json;
using System.Collections.Generic;

public class MainScene : Node2D
{
    
    public Player P1;
    public Player P2;
    private Label P1Combo;
    private Label P2Combo;
    private TextureProgress P1Health;
    private TextureProgress P2Health;
    private Camera2D camera;
    private GameStateObject gsObj;

    private const int MAXPLAYERS = 2;
    private const int PLAYERNUMBERS = 2;
    private int localPlayerHandle;
    private int localHand = 1;
    private int otherHand = 2;


    private int inputs = 0; //Store all inputs on this frame as a single int because that's what GGPO accepts.
    
    /// <summary>
    /// Godot doesn't allow constructors so I have to do stuff like this instead
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="localPort"></param>
    /// <param name="remotePort"></param>
    /// <param name="hosting"></param>
    public void Begin(string ip, int localPort, int remotePort, bool hosting)
    {
        
        //Basic config
        camera = GetNode<Camera2D>("Camera2D");

        P1 = GetNode<Player>("P1");
        P2 = GetNode<Player>("P2");
        
        P1.Connect("ComboChanged", this, nameof(OnPlayerComboChange));
        P2.Connect("ComboChanged", this, nameof(OnPlayerComboChange));
        P1.Connect("HealthChanged", this, nameof(OnPlayerHealthChange));
        P2.Connect("HealthChanged", this, nameof(OnPlayerHealthChange));
        P1.Connect("HadoukenEmitted", this, nameof(OnHadoukenEmitted));
        P2.Connect("HadoukenEmitted", this, nameof(OnHadoukenEmitted));
        P1.Connect("HadoukenRemoved", this, nameof(OnHadoukenRemoved));
        P2.Connect("HadoukenRemoved", this, nameof(OnHadoukenRemoved));
        P1Combo = GetNode<Label>("HUD/P1Combo");
        P2Combo = GetNode<Label>("HUD/P2Combo");
        P1Health = GetNode<TextureProgress>("HUD/P1Health");
        P2Health = GetNode<TextureProgress>("HUD/P2Health");
        P1Combo.Text = "";
        P2Combo.Text = "";


        gsObj = new GameStateObject();
        gsObj.config(P1, P2, this, hosting);

        if (Globals.mode == Globals.Mode.GGPO)
        {
            //GGPO Config
            int errorcode = GGPO.StartSession("ark", PLAYERNUMBERS, localPort);
            GD.Print($"Starting GGPO session, errorcode {errorcode}");


            
            ConnectEvents();
            Godot.Collections.Dictionary localHandle = GGPO.AddPlayer(GGPO.PlayertypeLocal, localHand, "127.0.0.1", 7000);
            localPlayerHandle = (int)localHandle["playerHandle"];
            GD.Print($"Local add result: {localHandle["result"]}");

            int frameDelayError = GGPO.SetFrameDelay(localPlayerHandle, 2);
            GD.Print($"Frame delay error code: {frameDelayError}");
            GGPO.CreateInstance(gsObj, nameof(gsObj.SaveGameState));
            Godot.Collections.Dictionary remoteHandle = GGPO.AddPlayer(GGPO.PlayertypeRemote, otherHand, ip, remotePort);
            GD.Print($"Remote add result:{remoteHandle["result"]}");
        }
        
    }



    /// <summary>
    /// Connect GGPO callbacks
    /// </summary>
    private void ConnectEvents()
    {
        GGPO.Singleton.Connect("advance_frame", this, nameof(OnAdvanceFrame));
        GGPO.Singleton.Connect("load_game_state", this, nameof(OnLoadGameState));
        GGPO.Singleton.Connect("event_disconnected_from_peer", this, nameof(OnEventDisconnectedFromPeer));
        GGPO.Singleton.Connect("save_game_state", this, nameof(OnSaveGameState));
        GGPO.Singleton.Connect("event_connected_to_peer", this, nameof(OnEventConnectedToPeer));
    }


    public override void _PhysicsProcess(float _delta)
    {
        camera.Call("adjust", P1.Position, P2.Position); // Camera is written in GDscript due to my own laziness
        if (Globals.mode == Globals.Mode.GGPO)
        {
            GGPOPhysicsProcess();
        }
        else if (Globals.mode == Globals.Mode.TRAINING) 
        {
            TrainingPhysicsProcess();
        }
        inputs = 0; // reset the inputs
    }

    public void GGPOPhysicsProcess()
    {
        GGPO.Idle(30);
        int result;
        if (localPlayerHandle != GGPO.InvalidHandle)
        {
            // GD.Print($"Adding local input {input}"); this works
            result = GGPO.AddLocalInput(localPlayerHandle, inputs);
        }
        else
        {
            result = 99;
        }
        if (result == GGPO.ErrorcodeSuccess)
        {
            Godot.Collections.Dictionary resultDict = GGPO.SynchronizeInput(MAXPLAYERS);
            if ((int)resultDict["result"] == GGPO.ErrorcodeSuccess)
            {

                Advance_Frame((Godot.Collections.Array)resultDict["inputs"]);
            }

        }
    }

    public void TrainingPhysicsProcess()
    {
        var combinedInputs = new int[2] {inputs, 0 }; 
        gsObj.Update(new Godot.Collections.Array(combinedInputs));
    }
    
    /// <summary>
    /// Non callback advance frame that we use with GGPO
    /// </summary>
    /// <param name="combinedInputs"></param>
    public void Advance_Frame(Godot.Collections.Array combinedInputs)
    {
        gsObj.Update(combinedInputs);
        GGPO.AdvanceFrame();
    }


    //GGPO callbacks
    public void OnSaveGameState()
    {

    }

    public void OnLoadGameState(StreamPeerBuffer buffer)
    {
        gsObj.LoadGameState(buffer);
    }

    public void OnEventConnectedToPeer(int handle)
    {
        GD.Print($"Connected to peer with handle {handle}");
    }

    /// <summary>
    /// Callback function for advancing frames given to GGPO to execute rollbacks
    /// </summary>
    /// <param name="combinedInputs"></param>
    public void OnAdvanceFrame(Godot.Collections.Array combinedInputs)
    {
        gsObj.Update(combinedInputs);
        GGPO.AdvanceFrame();
    }

    public void OnEventDisconnectedFromPeer()
    {
        GGPO.CloseSession();
    }


    /// <summary>
    /// Called whenever the user presses a key, which gets added to the inputs int
    /// </summary>
    /// <param name="event"></param>
    public override void _Input(InputEvent @event)
    {

        if (@event.IsActionPressed("8"))
        {
            AddPress((int) Globals.Inputs.UP);
        }
        else if (@event.IsActionPressed("2"))
        {
            AddPress((int)Globals.Inputs.DOWN);
        }
        else if (@event.IsActionPressed("4"))
        {
            AddPress((int)Globals.Inputs.LEFT);
        }
        else if (@event.IsActionPressed("6"))
        {
            AddPress((int)Globals.Inputs.RIGHT);
        }
        else if (@event.IsActionPressed("p"))
        {
            AddPress((int)Globals.Inputs.PUNCH);
        }
        else if (@event.IsActionPressed("k"))
        {
            AddPress((int)Globals.Inputs.KICK);
        }
        else if (@event.IsActionPressed("s"))
        {
            AddPress((int)Globals.Inputs.SLASH);
        }

        else if (@event.IsActionReleased("8"))
        {
            AddRelease((int)Globals.Inputs.UP);
        }
        else if (@event.IsActionReleased("2"))
        {
            AddRelease((int)Globals.Inputs.DOWN);
        }
        else if (@event.IsActionReleased("4"))
        {
            AddRelease((int)Globals.Inputs.LEFT);
        }
        else if (@event.IsActionReleased("6"))
        {
            AddRelease((int)Globals.Inputs.RIGHT);
        }
        else if (@event.IsActionReleased("p"))
        {
            AddRelease((int)Globals.Inputs.PUNCH);
        }
        else if (@event.IsActionReleased("k"))
        {
            AddRelease((int)Globals.Inputs.KICK);
        }
        else if (@event.IsActionReleased("s"))
        {
            AddRelease((int)Globals.Inputs.SLASH);
        }
    }
    private void AddPress(int key)
    {
        int thisInput = key * 10;
        AddInput(thisInput);
    }
    private void AddRelease(int key)
    {
        int thisInput = key * 10 + 1;
        AddInput(thisInput);
    }
    private void AddInput(int input)
    {
        if (inputs == 0) //This is the first input of the frame
        {
            inputs = input;
            return;
        }

        string inputsCurr = inputs.ToString();
        
        if (inputsCurr.Length > 10) //Max 5 inputs per frame to prevent overflow
        {
            GD.Print("Too many inputs");
            return;
        }
        string newInput = input.ToString();
        inputs = int.Parse(inputsCurr + newInput);
    }

    // HUD
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
    public void OnHadoukenEmitted(HadoukenPart h)
    {
        AddChild(h); // Add the hadouken as a child
        gsObj.NewHadouken(h); // let the gamestate object control it. this still needs to be cleaned up on deletion
        
    }

    public void OnHadoukenRemoved(HadoukenPart h)
    {
        
        gsObj.RemoveHadouken(h);
    }
}
