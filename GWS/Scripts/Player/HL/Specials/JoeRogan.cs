using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

public class JoeRogan : LaunchAttack
{

    private const string JoeRoganString = "JoeRogan";
    private const string TeleportString = "Teleport";

    public override void Enter()
    {
        base.Enter();
        owner.ScheduleEvent(EventScheduler.EventType.AUDIO, JoeRoganString, Name);
    }
    public override void AnimationFinished()
    {
        if (owner.CheckHeldKey('a') && !((HL)owner).hatted)
            owner.ChangeState(TeleportString);
        else
            base.AnimationFinished();
    }

}
