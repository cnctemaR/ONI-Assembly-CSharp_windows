using System;

public class EnvironmentGenerator : Generator
{
	public override void EnergySim200ms(float dt)
	{
		base.EnergySim200ms(dt);
		if (this.operational.IsOperational)
		{
			this.ApplyDeltaJoules(base.WattageRating * dt, false);
			this.operational.SetActive(this.operational.IsOperational, false);
		}
	}
}
