using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

public class GameStateObject : Node
{
    public Player P1;
    public Player P2;
    private bool hosting;

    private StreamPeerBuffer testSave;

    public int Frame = 0;

    private int hitStopRemaining = 0;

    private int maxHitStop = 14;

    /// <summary>
    /// Stores all vital data about positions in the game in a single struct
    /// </summary>
    [Serializable]
    private struct GameState
    {
        public int frame { get; set; }
        public Player.PlayerState P1State { get; set; }
        public Player.PlayerState P2State { get; set; }
        public int hitStopRemaining { get; set; }
    }

    public void config(Player P1, Player P2, bool hosting)
    {
        this.P1 = P1;
        this.P2 = P2;
        this.hosting = hosting;
        P1.Connect("HitConfirm", this, nameof(HitStop));
        P2.Connect("HitConfirm", this, nameof(HitStop));

        P1.otherPlayer = P2;
        P2.otherPlayer = P1;
        P1.CheckTurnAround();
        P2.CheckTurnAround();

        P2.SetUnhandledInputs(new List<char[]>() { new char[] {'6', 'p' }, new char[] {'2', 'p' } });
    }
    private byte[] Serialize<T>(T data)
    where T : struct
    {
        var formatter = new BinaryFormatter();
        var stream = new MemoryStream();
        formatter.Serialize(stream, data);
        return stream.ToArray();
    }
    private T Deserialize<T>(byte[] array)
        where T : struct
    {
        var stream = new MemoryStream(array);
        var formatter = new BinaryFormatter();
        return (T)formatter.Deserialize(stream);
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
    private StreamPeerBuffer SerializeGamestate(GameState gameState)
    {
        
        byte[] arr = Serialize(gameState);

        

        var stream = new StreamPeerBuffer();
        stream.Clear();
        stream.Put32(0); // for the checksum
        stream.Put32(arr.Length); // to know how much data to pull out

        foreach (byte b in arr)
        {
            stream.PutU8(b);
        }

        int checkSum = CalcFletcher32(stream);
        stream.Seek(0);
        stream.Put32(checkSum);

        return stream;

    }

    /// <summary>
    /// Return the serialized game state for GGPO to hold on to
    /// </summary>
    /// <returns></returns>
    public StreamPeerBuffer SaveGameState()
    {
        return SerializeGamestate(GetGameState());
    }
    private int CalcFletcher32(StreamPeerBuffer stream)
    {
        int sum1 = 0;
        int sum2 = 0;
        var index = stream.DataArray;
        for (int i = 0; i < index.Length; i++)
        {
            sum1 = (sum1 + index[i] % 0xffff);
            sum2 = (sum1 + sum2) % 0xffff;

        }
        return ((sum2 << 16) | sum1);
    }
    private void SetGameState(GameState gState)
    {
        Frame = gState.frame;
        hitStopRemaining = gState.hitStopRemaining;
        P1.SetState(gState.P1State);
        P2.SetState(gState.P2State);
    }
    private GameState DeserializeGamestate(StreamPeerBuffer stream)
    {
        stream.Seek(0);
        stream.Get32();
        int length = stream.Get32();
        byte[] arr = new byte[length];
        for (int i = 0; i < length; i++)
        {
            arr[i] = stream.GetU8();
        }

        var retrievedGamestate = Deserialize<GameState>(arr);

        return retrievedGamestate;
    }

    /// <summary>
    /// Load the game state provided by GGPO
    /// </summary>
    /// <param name="stream"></param>
    public void LoadGameState(StreamPeerBuffer stream)
    {
        SetGameState(DeserializeGamestate(stream));
    }

    /// <summary>
    /// Updates the gamestate by one frame with the given inputs
    /// </summary>
    /// <param name="thisFrameInputs"></param>
    public void Update(Godot.Collections.Array thisFrameInputs)
    {

        if (hosting)
        {
            P1.SetUnhandledInputs(ConvertInputs((int)thisFrameInputs[0]));
            P2.SetUnhandledInputs(ConvertInputs((int)thisFrameInputs[1]));
        }
        else
        {
            P1.SetUnhandledInputs(ConvertInputs((int)thisFrameInputs[1]));
            P2.SetUnhandledInputs(ConvertInputs((int)thisFrameInputs[0]));
        }
        

        AdvanceFrameAndHitstop();
        FrameAdvancePlayers();

    }

    /// <summary>
    /// Takes the inputs list and turns it back into List<char[]>
    /// </summary>
    /// <param name="inputs"></param>
    /// <returns></returns>
    private List<char[]> ConvertInputs(int inputs)
    {
        var convertedInputs = new List<char[]>();
        if (inputs == 0)
        {
            return convertedInputs; //no inputs, don't do anything
        }

        string stringInputs = inputs.ToString();
        int count = stringInputs.Length / 2;
        for (int i = 0; i < count; i++)
        {
            int key = int.Parse(stringInputs[i * 2].ToString());
            int press = int.Parse(stringInputs[i * 2 + 1].ToString());
            char[] convertedInput = ConvertInput(key, press);
            if (Enumerable.SequenceEqual(convertedInput, new char[] { '4', 'r' }))
            {
                GD.Print("Releasing 4 in GameState");
            }
            else if (Enumerable.SequenceEqual(convertedInput, new char[] { '6', 'r' }))
            {
                GD.Print("Releasing 6 in GameState");
            }
            convertedInputs.Add(convertedInput);
        }
        return convertedInputs;

    }

    private char[] ConvertInput(int key, int press)
    {
        var input = new char[2];
        if (key == (int)Globals.Inputs.UP)
        {
            input[0] = '8';
        }
        else if (key == (int)Globals.Inputs.DOWN)
        {
            input[0] = '2';
        }
        else if (key == (int)Globals.Inputs.LEFT)
        {
            input[0] = '4';
        }
        else if (key == (int)Globals.Inputs.RIGHT)
        {
            input[0] = '6';
        }
        else if (key == (int)Globals.Inputs.PUNCH)
        {
            input[0] = 'p';
        }
        else if (key == (int)Globals.Inputs.KICK)
        {
            input[0] = 'k';
        }
        else if (key == (int)Globals.Inputs.SLASH)
        {
            input[0] = 's';
        }

        if (press == (int)Globals.Press.PRESS)
        {
            input[1] = 'p';
        }
        else
        {
            input[1] = 'r';
        }

        return input;
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
        P1.FrameAdvanceInputs(hitStopRemaining);
        P2.FrameAdvanceInputs(hitStopRemaining);
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

    /// <summary>
    /// Check if player collision boxes are colliding and adjust accordingly
    /// </summary>
    private void CheckFixCollision()
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
    private bool CheckRects()
    {
        Rect2 P1rect = P1.GetCollisionRect();
        P1rect.Position = P1rect.Position + P1.Position;
        Rect2 P2rect = P2.GetCollisionRect();
        P2rect.Position = P2rect.Position + P2.Position;
        return P1rect.Intersects(P2rect);
    }

    /// <summary>
    /// Reset the hitstop counter, called by player signals on hit
    /// </summary>
    public void HitStop()
    {
        hitStopRemaining = maxHitStop;
        GD.Print("HitStop");
    }

    
}
