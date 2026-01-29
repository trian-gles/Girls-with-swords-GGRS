using Godot;
using System;
using System.Collections.Generic;


public abstract class AirNormal : AirAttack
{
    [Export]
	public bool j2CGatling = false;
    private string fallString = "Fall";
    public override void _Ready()
    {
        base._Ready();
        if (j2CGatling)
		    AddAirCommandNormals(owner.airCommandNormals);
        AddSpecials(owner.airSpecials);
        AddExSpecials(owner.airExSpecials);
        AddEasyAirSpecials();

        if (jumpCancelable)
            AddJumpCancel();
    }

    public override void AnimationFinished()
    {
        owner.ChangeState(fallString);
    }

    public override void Enter()
    {
        base.Enter();
        
    }
}
