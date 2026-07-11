using System;

public class PickupableSensor : Sensor
{
	public PickupableSensor(Sensors sensors)
		: base(sensors)
	{
		this.worker = base.GetComponent<Worker>();
		this.pathProber = base.GetComponent<PathProber>();
	}

	public override void Update()
	{
		GlobalChoreProvider.Instance.UpdateFetches(this.pathProber);
		Game.Instance.fetchManager.UpdatePickups(this.pathProber, this.worker);
	}

	private PathProber pathProber;

	private Worker worker;
}
