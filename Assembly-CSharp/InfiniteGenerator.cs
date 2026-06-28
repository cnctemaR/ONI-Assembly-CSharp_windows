using System;

public class InfiniteGenerator : Generator
{
	public override bool IsEmpty
	{
		get
		{
			return false;
		}
	}

	public override void EnergySim200ms(float dt)
	{
		base.EnergySim200ms(dt);
		this.ApplyDeltaJoules(base.WattageRating * dt, false);
	}
}
