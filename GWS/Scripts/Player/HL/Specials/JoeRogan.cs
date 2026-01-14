using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

public class JoeRogan : LaunchAttack
{

    public override void Enter()
    {
        base.Enter();
        owner.ScheduleEvent(EventScheduler.EventType.AUDIO, "JoeRogan", Name);
    }
    public override void AnimationFinished()
    {
        if (owner.CheckHeldKey('a') && !((HL)owner).hatted)
            owner.ChangeState("Teleport");
        else
            base.AnimationFinished();
    }

}
