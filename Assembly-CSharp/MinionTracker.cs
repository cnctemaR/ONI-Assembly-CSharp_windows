using System;

public abstract class MinionTracker : Tracker
{
	public MinionTracker(MinionIdentity identity)
	{
		this.identity = identity;
	}

	public MinionIdentity identity;
}
