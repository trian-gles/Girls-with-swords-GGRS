public class AirBackdash : AirDash 
{
	public override void Enter()
	{
		base.Enter();
		owner.ScheduleEvent(EventScheduler.EventType.AUDIO, "Backdash", "AirBackdash");
		owner.velocity.y = 0;
	}
}