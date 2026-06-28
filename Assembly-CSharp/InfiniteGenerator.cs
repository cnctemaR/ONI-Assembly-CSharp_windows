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

	protected override void SimUpdate(float dt)
	{
		base.SimUpdate(dt);
		this.ApplyDeltaJoules(base.WattageRating * dt, false);
	}
}
