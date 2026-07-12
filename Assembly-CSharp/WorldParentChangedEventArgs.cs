using System;

public class WorldParentChangedEventArgs
{
	public int lastParentId = (int)ClusterManager.INVALID_WORLD_IDX;

	public WorldContainer world;
}
