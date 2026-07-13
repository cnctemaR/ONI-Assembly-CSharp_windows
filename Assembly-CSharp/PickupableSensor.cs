using System;

public class PickupableSensor : Sensor
{
	public PickupableSensor(Sensors sensors)
		: base(sensors)
	{
		this.worker = base.GetComponent<WorkerBase>();
		this.navigator = base.GetComponent<Navigator>();
	}

	public override void Update()
	{
		GlobalChoreProvider.Instance.UpdateFetches(this.navigator);
		Game.Instance.fetchManager.UpdatePickups(this.navigator, this.worker);
	}

	private Navigator navigator;

	private WorkerBase worker;
}
