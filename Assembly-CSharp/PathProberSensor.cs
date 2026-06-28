using System;

public class PathProberSensor : Sensor
{
	public PathProberSensor(Sensors sensors)
		: base(sensors)
	{
		this.navigator = sensors.GetComponent<Navigator>();
	}

	public override void Update()
	{
		this.navigator.UpdateProbe();
	}

	private Navigator navigator;
}
