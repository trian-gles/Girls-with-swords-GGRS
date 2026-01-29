public class AirBackdash : AirDash 
{
	private string backdashString = "Backdash";
	private string airBackdashString = "AirBackDash";
	public override void Enter()
	{
		base.Enter();
		owner.ScheduleEvent(EventScheduler.EventType.AUDIO, backdashString, airBackdashString);
		owner.velocity.y = 0;
	}
}