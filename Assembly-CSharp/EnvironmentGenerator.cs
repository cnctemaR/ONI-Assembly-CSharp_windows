using System;

public class EnvironmentGenerator : Generator
{
	protected override void SimUpdate(float dt)
	{
		base.SimUpdate(dt);
		if (this.operational.IsOperational)
		{
			base.ApplyDeltaJoules(base.WattageRating * dt, false);
			this.operational.SetActive(this.operational.IsOperational, false);
		}
	}
}
