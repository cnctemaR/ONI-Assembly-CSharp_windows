using System;

public class ClosestOxyliteSensor : ClosestPickupableSensor<Pickupable>
{
	public ClosestOxyliteSensor(Sensors sensors, bool shouldStartActive)
		: base(sensors, GameTags.OxyRock, shouldStartActive)
	{
	}
}
