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

    public static bool ArrayInList(List<char[]> arr, char[] element)
    {
        var elementTest = (from e in arr select Enumerable.SequenceEqual(e, element));
        return elementTest.Contains(true);
    }

    public static bool CheckKeyPress(char[] input, char desiredPress)
    {
        return (input[1] == 'p' && input[0] == desiredPress);
    }

    public static bool CheckKeyRelease(char[] input, char desiredRelease)
    {
        return (input[1] == 'p' && input[0] == desiredRelease);
    }
}
