using Godot;
using System;

public class Lobby : Node2D
{
    public void OnHostButtonDown()
    {
        Begin(true);
    }

    public void OnJoinButtonDown()
    {
        Begin(false);
    }

    public void OnSyncTestButtonDown()
    {
        
        GD.Print($"Starting synctest");
    }

    public void Begin(bool host)
    {
        GetNode<Button>("Host").Visible = false;
        GetNode<Button>("Join").Visible = false;
        GetNode<Button>("SyncTest").Visible = false;

        
        string ip = GetNode<LineEdit>("OpponentIp").Text;
        
        int otherPort = int.Parse(GetNode<LineEdit>("OpponentPort").Text);
        int localPort = int.Parse(GetNode<LineEdit>("LocalPort").Text);
        GetNode<LineEdit>("OpponentPort").Visible = false;
        GetNode<LineEdit>("LocalPort").Visible = false;

        var mainScene = (PackedScene) ResourceLoader.Load("res://MainScene.tscn");
        var mainInstance = (MainScene)mainScene.Instance();
        AddChild(mainInstance);
        mainInstance.Begin(ip, localPort, otherPort, host);
    }
}
