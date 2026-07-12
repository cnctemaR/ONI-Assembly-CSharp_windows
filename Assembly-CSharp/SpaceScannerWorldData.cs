using System;
using System.Collections.Generic;

public class SpaceScannerWorldData
{
	public SpaceScannerWorldData(WorldContainer world)
	{
		this.world = world;
	}

	public WorldContainer world;

	public float networkQuality01;

	public Dictionary<string, float> targetIdToRandomValue01Map = new Dictionary<string, float>();

	public HashSet<string> targetIdsDetected = new HashSet<string>();

	public SpaceScannerWorldData.Scratchpad scratchpad = new SpaceScannerWorldData.Scratchpad();

	public class Scratchpad
	{
		public void Reset()
		{
			this.ballisticObjects.Clear();
		}

		public List<ClusterTraveler> ballisticObjects = new List<ClusterTraveler>();
	}
}
