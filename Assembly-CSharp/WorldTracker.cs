using System;

public abstract class WorldTracker : Tracker
{
	public int WorldID { get; private set; }

	public WorldTracker(int worldID)
	{
		this.WorldID = worldID;
	}
}
