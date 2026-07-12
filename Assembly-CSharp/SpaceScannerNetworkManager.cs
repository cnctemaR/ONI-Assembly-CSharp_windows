using System;
using System.Collections.Generic;
using Klei.AI;
using UnityEngine;

public class SpaceScannerNetworkManager : ISim1000ms
{
	public Dictionary<int, SpaceScannerWorldData> DEBUG_GetWorldIdToDataMap()
	{
		return this.worldIdToDataMap;
	}

	public bool IsTargetDetectedOnWorld(int worldId, SpaceScannerTarget target)
	{
		SpaceScannerWorldData spaceScannerWorldData;
		return this.worldIdToDataMap.TryGetValue(worldId, out spaceScannerWorldData) && spaceScannerWorldData.targetIdsDetected.Contains(target.id);
	}

	public MathUtil.MinMax GetDetectTimeRangeForWorld(int worldId)
	{
		return SpaceScannerNetworkManager.GetDetectTimeRange(this.GetQualityForWorld(worldId));
	}

	public float GetQualityForWorld(int worldId)
	{
		SpaceScannerWorldData spaceScannerWorldData;
		if (this.worldIdToDataMap.TryGetValue(worldId, out spaceScannerWorldData))
		{
			return spaceScannerWorldData.networkQuality01;
		}
		return 0f;
	}

	private SpaceScannerWorldData GetOrCreateWorldData(int worldId)
	{
		SpaceScannerWorldData spaceScannerWorldData;
		if (!this.worldIdToDataMap.TryGetValue(worldId, out spaceScannerWorldData))
		{
			spaceScannerWorldData = new SpaceScannerWorldData(ClusterManager.Instance.GetWorld(worldId));
			this.worldIdToDataMap[worldId] = spaceScannerWorldData;
		}
		return spaceScannerWorldData;
	}

	public void Sim1000ms(float dt)
	{
		SpaceScannerNetworkManager.UpdateWorldDataScratchpads(this.worldIdToDataMap);
		foreach (int num in Components.DetectorNetworks.GetWorldsIds())
		{
			WorldContainer world = ClusterManager.Instance.GetWorld(num);
			if (!world.IsModuleInterior && world.IsDiscovered)
			{
				SpaceScannerWorldData orCreateWorldData = this.GetOrCreateWorldData(world.id);
				SpaceScannerNetworkManager.UpdateNetworkQualityFor(orCreateWorldData);
				SpaceScannerNetworkManager.UpdateDetectionOfTargetsFor(orCreateWorldData);
			}
		}
	}

	private static void UpdateNetworkQualityFor(SpaceScannerWorldData worldData)
	{
		float num = SpaceScannerNetworkManager.CalcWorldNetworkQuality(worldData.world);
		foreach (object obj in Components.DetectorNetworks.CreateOrGetCmps(worldData.world.id))
		{
			((DetectorNetwork.Instance)obj).Internal_SetNetworkQuality(num);
		}
		worldData.networkQuality01 = num;
	}

	private static void UpdateDetectionOfTargetsFor(SpaceScannerWorldData worldData)
	{
		using (HashSetPool<string, SpaceScannerNetworkManager>.PooledHashSet pooledHashSet = PoolsFor<SpaceScannerNetworkManager>.AllocateHashSet<string>())
		{
			using (HashSetPool<string, SpaceScannerNetworkManager>.PooledHashSet pooledHashSet2 = PoolsFor<SpaceScannerNetworkManager>.AllocateHashSet<string>())
			{
				foreach (string text in worldData.targetIdsDetected)
				{
					pooledHashSet.Add(text);
					pooledHashSet2.Add(text);
				}
				worldData.targetIdsDetected.Clear();
				if (SpaceScannerNetworkManager.IsDetectingAnyMeteorShower(worldData))
				{
					worldData.targetIdsDetected.Add(SpaceScannerTarget.MeteorShower().id);
				}
				if (SpaceScannerNetworkManager.IsDetectingAnyBallisticObject(worldData))
				{
					worldData.targetIdsDetected.Add(SpaceScannerTarget.BallisticObject().id);
				}
				foreach (Spacecraft spacecraft in SpacecraftManager.instance.GetSpacecraft())
				{
					if (SpaceScannerNetworkManager.IsDetectingRocketBaseGame(worldData, spacecraft.launchConditions))
					{
						worldData.targetIdsDetected.Add(SpaceScannerTarget.RocketBaseGame(spacecraft.launchConditions).id);
					}
				}
				foreach (object obj in Components.Clustercrafts)
				{
					Clustercraft clustercraft = (Clustercraft)obj;
					if (SpaceScannerNetworkManager.IsDetectingRocketDlc1(worldData, clustercraft))
					{
						worldData.targetIdsDetected.Add(SpaceScannerTarget.RocketDlc1(clustercraft).id);
					}
				}
				foreach (string text2 in worldData.targetIdsDetected)
				{
					pooledHashSet2.Add(text2);
				}
				foreach (string text3 in pooledHashSet2)
				{
					bool flag = pooledHashSet.Contains(text3);
					if (!worldData.targetIdsDetected.Contains(text3) && flag)
					{
						worldData.targetIdToRandomValue01Map[text3] = global::UnityEngine.Random.value;
					}
				}
			}
		}
	}

	private static bool IsDetectingAnyMeteorShower(SpaceScannerWorldData worldData)
	{
		SpaceScannerNetworkManager.meteorShowerInstances.Clear();
		SaveGame.Instance.GetComponent<GameplayEventManager>().GetActiveEventsOfType<MeteorShowerEvent>(worldData.world.id, ref SpaceScannerNetworkManager.meteorShowerInstances);
		float num = float.MaxValue;
		foreach (GameplayEventInstance gameplayEventInstance in SpaceScannerNetworkManager.meteorShowerInstances)
		{
			MeteorShowerEvent.StatesInstance statesInstance = gameplayEventInstance.smi as MeteorShowerEvent.StatesInstance;
			if (statesInstance != null)
			{
				num = Mathf.Min(num, statesInstance.TimeUntilNextShower());
			}
		}
		return num <= SpaceScannerNetworkManager.GetDetectTime(worldData, SpaceScannerTarget.MeteorShower());
	}

	private static bool IsDetectingAnyBallisticObject(SpaceScannerWorldData worldData)
	{
		float num = float.MaxValue;
		foreach (ClusterTraveler clusterTraveler in worldData.scratchpad.ballisticObjects)
		{
			num = Mathf.Min(num, clusterTraveler.TravelETA());
		}
		return num < SpaceScannerNetworkManager.GetDetectTime(worldData, SpaceScannerTarget.BallisticObject());
	}

	private static bool IsDetectingRocketBaseGame(SpaceScannerWorldData worldData, LaunchConditionManager rocket)
	{
		Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(rocket);
		if (spacecraftFromLaunchConditionManager.IsNullOrDestroyed())
		{
			return false;
		}
		if (spacecraftFromLaunchConditionManager.state == Spacecraft.MissionState.Destroyed)
		{
			return false;
		}
		Spacecraft.MissionState state = spacecraftFromLaunchConditionManager.state;
		return state - Spacecraft.MissionState.Launching <= 3 && spacecraftFromLaunchConditionManager.GetTimeLeft() <= SpaceScannerNetworkManager.GetDetectTime(worldData, SpaceScannerTarget.RocketBaseGame(rocket));
	}

	private static bool IsDetectingRocketDlc1(SpaceScannerWorldData worldData, Clustercraft clustercraft)
	{
		if (clustercraft.IsNullOrDestroyed())
		{
			return false;
		}
		ClusterTraveler component = clustercraft.GetComponent<ClusterTraveler>();
		bool flag = false;
		if (clustercraft.Status != Clustercraft.CraftStatus.Grounded)
		{
			bool flag2 = component.GetDestinationWorldID() == worldData.world.id;
			bool flag3 = component.IsTraveling();
			bool flag4 = clustercraft.HasResourcesToMove(1, Clustercraft.CombustionResource.All);
			float num = component.TravelETA();
			flag = (flag2 && flag3 && flag4 && num < SpaceScannerNetworkManager.GetDetectTime(worldData, SpaceScannerTarget.RocketDlc1(clustercraft))) || (!flag3 && flag2 && clustercraft.Status == Clustercraft.CraftStatus.Landing);
			if (!flag)
			{
				ClusterGridEntity adjacentAsteroid = clustercraft.GetAdjacentAsteroid();
				flag = ((adjacentAsteroid != null) ? ClusterUtil.GetAsteroidWorldIdAtLocation(adjacentAsteroid.Location) : 255) == worldData.world.id && clustercraft.Status == Clustercraft.CraftStatus.Launching;
			}
		}
		return flag;
	}

	private static float GetDetectTime(SpaceScannerWorldData worldData, SpaceScannerTarget target)
	{
		float value;
		if (!worldData.targetIdToRandomValue01Map.TryGetValue(target.id, out value))
		{
			value = global::UnityEngine.Random.value;
			worldData.targetIdToRandomValue01Map[target.id] = value;
		}
		return SpaceScannerNetworkManager.GetDetectTimeRange(worldData.networkQuality01).Lerp(value);
	}

	private static MathUtil.MinMax GetDetectTimeRange(float networkQuality01)
	{
		return new MathUtil.MinMax(Mathf.Lerp(1f, 200f, networkQuality01), 200f);
	}

	private static float CalcWorldNetworkQuality(WorldContainer world)
	{
		int width = world.Width;
		global::Debug.Assert(width <= 1024, "More world columns than expected");
		bool[] array = new bool[width];
		for (int i = 0; i < width; i++)
		{
			array[i] = false;
		}
		using (HashSetPool<int, SpaceScannerNetworkManager>.PooledHashSet pooledHashSet = PoolsFor<SpaceScannerNetworkManager>.AllocateHashSet<int>())
		{
			foreach (object obj in Components.DetectorNetworks.CreateOrGetCmps(world.id))
			{
				DetectorNetwork.Instance instance = (DetectorNetwork.Instance)obj;
				if (instance.GetComponent<Operational>().IsOperational)
				{
					CometDetectorConfig.SKY_VISIBILITY_INFO.CollectVisibleCellsTo(pooledHashSet, Grid.PosToCell(instance.gameObject.transform.position));
				}
			}
			foreach (int num in pooledHashSet)
			{
				int num2 = Grid.CellToXY(num).x - world.WorldOffset.x;
				array[num2] = true;
			}
		}
		int num3 = 0;
		for (int j = 0; j < width; j++)
		{
			if (array[j])
			{
				num3++;
			}
		}
		return Mathf.Clamp01(((float)num3 / (float)width).Remap(new ValueTuple<float, float>(0f, 0.5f), new ValueTuple<float, float>(0f, 1f)));
	}

	private static void UpdateWorldDataScratchpads(Dictionary<int, SpaceScannerWorldData> worldIdToDataMap)
	{
		foreach (KeyValuePair<int, SpaceScannerWorldData> keyValuePair in worldIdToDataMap)
		{
			int num;
			SpaceScannerWorldData spaceScannerWorldData;
			keyValuePair.Deconstruct<int, SpaceScannerWorldData>(out num, out spaceScannerWorldData);
			spaceScannerWorldData.scratchpad.Reset();
		}
		if (Components.DetectorNetworks.GetWorldsIds().Count == 0)
		{
			return;
		}
		foreach (object obj in Components.ClusterTravelers)
		{
			ClusterTraveler clusterTraveler = (ClusterTraveler)obj;
			SpaceScannerWorldData spaceScannerWorldData2;
			if (clusterTraveler.IsTraveling() && clusterTraveler.GetComponent<Clustercraft>().IsNullOrDestroyed() && worldIdToDataMap.TryGetValue(clusterTraveler.GetDestinationWorldID(), out spaceScannerWorldData2))
			{
				spaceScannerWorldData2.scratchpad.ballisticObjects.Add(clusterTraveler);
			}
		}
	}

	private Dictionary<int, SpaceScannerWorldData> worldIdToDataMap = new Dictionary<int, SpaceScannerWorldData>();

	private static List<GameplayEventInstance> meteorShowerInstances = new List<GameplayEventInstance>();
}
