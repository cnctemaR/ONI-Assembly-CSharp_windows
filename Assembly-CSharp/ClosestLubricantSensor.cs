using System;

public class ClosestLubricantSensor : ClosestPickupableSensor<Pickupable>
{
	public ClosestLubricantSensor(Sensors sensors, bool shouldStartActive)
		: base(sensors, GameTags.SolidLubricant, shouldStartActive)
	{
	}
}
