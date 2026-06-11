using Godot;
using System;

public class GLDP : LaunchAttack
{
    private const string Fire1String = "Fire1";
    [Export]
    public int knockdownFrame = 30;

    Globals.AttackDetails finalAttack = Globals.attackLevels[3].hit;

    public override void _Ready()
    {
        base._Ready();
        tags.Add(Globals.Tags.aerial);
        finalAttack.hitStun = 70;
        finalAttack.knockdown = true;
        finalAttack.graphicFX = BaseAttack.GRAPHICEFFECT.EXPLOSION;
        AddKara(new char[] { 'c', 'p' }, () => owner.grounded, owner.easySuper);

    }
    public override void Enter()
    {
        base.Enter();
        owner.velocity = Vector2.Zero;
        owner.ScheduleEvent(EventScheduler.EventType.AUDIO, Fire1String, Name);
    }

    public override void FrameAdvance()
    {
        base.FrameAdvance();
        if (frameCount == knockdownFrame)
            owner.velocity.y = 0;
        
        if (frameCount == knockdownFrame && hitConnect && owner.otherPlayer.currentState.tags.Contains(Globals.Tags.hitstate))
        {
            owner.ForceEvent(EventScheduler.EventType.AUDIO, hitSound);
            
            owner.otherPlayer.ReceiveHit(finalAttack, finalAttack);
            
        }
    }
}
