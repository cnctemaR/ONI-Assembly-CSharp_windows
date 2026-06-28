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
		FetchManagerUpdater.UpdatePickups(this.pathProber, FetchManager.Instance.pickupables, this.worker);
	}

	private PathProber pathProber;

	private Worker worker;
}
