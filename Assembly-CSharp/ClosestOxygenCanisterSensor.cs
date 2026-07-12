using System;

public class ClosestOxygenCanisterSensor : ClosestPickupableSensor<Pickupable>
{
	public ClosestOxygenCanisterSensor(Sensors sensors, bool shouldStartActive)
		: base(sensors, GameTags.Gas, shouldStartActive)
	{
		this.requiredTags = new Tag[] { GameTags.Breathable };
	}
}
