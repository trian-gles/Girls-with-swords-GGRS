using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Globals : Node
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public static bool ArrayInList(List<string[]> arr, string[] element)
    {
        var elementTest = (from e in arr select Enumerable.SequenceEqual(e, element));
        return elementTest.Contains(true);
    }

    public static bool CheckKeyPress(string[] input, string desiredPress)
    {
        return (input[1] == "press" && input[0] == desiredPress);
    }

    public static bool CheckKeyRelease(string[] input, string desiredRelease)
    {
        return (input[1] == "press" && input[0] == desiredRelease);
    }
}
