using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Globals : Node
{

    public enum Inputs
    {
        UP = 1,
        DOWN = 2,
        LEFT = 3,
        RIGHT = 4,
        PUNCH = 5,
        KICK = 6,
        SLASH = 7
    }

    public enum Press
    {
        PRESS = 0,
        RELEASE = 1
    }

    public enum Mode
    {
        LOCAL = 0,
        TRAINING = 1,
        GGPO = 2
    }

    public override void _Ready()
    {
        
    }

    static public Mode mode;
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
