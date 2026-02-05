public class AirBackdash : AirDash 
{
	private const string BackdashString = "Backdash";
	private const string AirBackdashString = "AirBackDash";
	public override void Enter()
	{
		base.Enter();
		owner.ScheduleEvent(EventScheduler.EventType.AUDIO, BackdashString, AirBackdashString);
		owner.velocity.y = 0;
	}
}