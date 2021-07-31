using Godot;
using System;

public class Lobby : Node2D
{
    public void OnHostButtonDown()
    {
        Begin(7000, 7001, true);
    }

    public void OnJoinButtonDown()
    {
        Begin(7001, 7000, false);
    }

    public void OnSyncTestButtonDown()
    {
        
        GD.Print($"Starting synctest");
    }

    public void Begin(int localPort, int otherPort, bool hosting)
    {
        GetNode<Button>("Host").Visible = false;
        GetNode<Button>("Join").Visible = false;
        GetNode<Button>("SyncTest").Visible = false;
        string ip = GetNode<LineEdit>("IpEdit").Text;
        var mainScene = (PackedScene) ResourceLoader.Load("res://MainScene.tscn");
        var mainInstance = (MainScene)mainScene.Instance();
        AddChild(mainInstance);
        mainInstance.Begin(ip, localPort, otherPort, hosting);
    }
}
