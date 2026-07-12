using System;

public class ClosestElectrobankSensor : ClosestPickupableSensor<Electrobank>
{
	public ClosestElectrobankSensor(Sensors sensors, bool shouldStartActive)
		: base(sensors, GameTags.ChargedPortableBattery, shouldStartActive)
	{
	}
}
