using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

public class JoeRogan : LaunchAttack
{
    public override void AnimationFinished()
    {
        if (owner.CheckHeldKey('a') && !((HL)owner).hatted)
            EmitSignal(nameof(StateFinished), "Teleport");
        else
            base.AnimationFinished();
    }

}
