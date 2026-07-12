using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;

[Serialize]
[SerializationConfig(MemberSerialization.OptIn)]
[Serializable]
public class SpaceScannerWorldData
{
	[Serialize]
	public SpaceScannerWorldData(int worldId)
	{
		this.worldId = worldId;
	}

	public WorldContainer GetWorld()
	{
		if (this.world == null)
		{
			this.world = ClusterManager.Instance.GetWorld(this.worldId);
		}
		return this.world;
	}

	[NonSerialized]
	private WorldContainer world;

	[Serialize]
	public int worldId;

	[Serialize]
	public float networkQuality01;

	[Serialize]
	public Dictionary<string, float> targetIdToRandomValue01Map = new Dictionary<string, float>();

	[Serialize]
	public HashSet<string> targetIdsDetected = new HashSet<string>();

	[NonSerialized]
	public SpaceScannerWorldData.Scratchpad scratchpad = new SpaceScannerWorldData.Scratchpad();

	public class Scratchpad
	{
		public List<ClusterTraveler> ballisticObjects = new List<ClusterTraveler>();

		public HashSet<MeteorShowerEvent.StatesInstance> lastDetectedMeteorShowers = new HashSet<MeteorShowerEvent.StatesInstance>();

		public HashSet<LaunchConditionManager> lastDetectedRocketsBaseGame = new HashSet<LaunchConditionManager>();

		public HashSet<Clustercraft> lastDetectedRocketsDLC1 = new HashSet<Clustercraft>();
	}
}
