using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public class GroundTech : Tech
{
    public override string animationName => "Fall";
    public override void HandleInput(InputContainer.CharPair inputArr) // no inputs during ground tech
    {
    }
}