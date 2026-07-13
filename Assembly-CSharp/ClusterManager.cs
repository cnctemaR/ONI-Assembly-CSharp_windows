using System;
using System.Collections.Generic;
using FMOD.Studio;
using KSerialization;
using ProcGenGame;
using TUNING;
using UnityEngine;

public class ClusterManager : KMonoBehaviour, ISaveLoadable
{
	public static void DestroyInstance()
	{
		ClusterManager.Instance = null;
	}

	public int worldCount
	{
		get
		{
			return this.m_worldContainers.Count;
		}
	}

	public int activeWorldId
	{
		get
		{
			return this.activeWorldIdx;
		}
	}

	public List<WorldContainer> WorldContainers
	{
		get
		{
			return this.m_worldContainers;
		}
	}

	public ClusterPOIManager GetClusterPOIManager()
	{
		return this.m_clusterPOIsManager;
	}

	public Dictionary<int, List<IAssignableIdentity>> MinionsByWorld
	{
		get
		{
			this.minionsByWorld.Clear();
			for (int i = 0; i < Components.MinionAssignablesProxy.Count; i++)
			{
				if (!Components.MinionAssignablesProxy[i].GetTargetGameObject().HasTag(GameTags.Dead))
				{
					int id = Components.MinionAssignablesProxy[i].GetTargetGameObject().GetComponent<KMonoBehaviour>().GetMyWorld()
						.id;
					if (!this.minionsByWorld.ContainsKey(id))
					{
						this.minionsByWorld.Add(id, new List<IAssignableIdentity>());
					}
					this.minionsByWorld[id].Add(Components.MinionAssignablesProxy[i]);
				}
			}
			return this.minionsByWorld;
		}
	}

	public void RegisterWorldContainer(WorldContainer worldContainer)
	{
		this.m_worldContainers.Add(worldContainer);
	}

	public void UnregisterWorldContainer(WorldContainer worldContainer)
	{
		base.Trigger(-1078710002, worldContainer.id);
		this.m_worldContainers.Remove(worldContainer);
	}

	public List<int> GetWorldIDsSorted()
	{
		ListPool<WorldContainer, ClusterManager>.PooledList pooledList = ListPool<WorldContainer, ClusterManager>.Allocate(this.m_worldContainers);
		pooledList.Sort((WorldContainer a, WorldContainer b) => a.DiscoveryTimestamp.CompareTo(b.DiscoveryTimestamp));
		this._worldIDs.Clear();
		foreach (WorldContainer worldContainer in pooledList)
		{
			this._worldIDs.Add(worldContainer.id);
		}
		pooledList.Recycle();
		return this._worldIDs;
	}

	public List<int> GetDiscoveredAsteroidIDsSorted()
	{
		ListPool<WorldContainer, ClusterManager>.PooledList pooledList = ListPool<WorldContainer, ClusterManager>.Allocate(this.m_worldContainers);
		pooledList.Sort((WorldContainer a, WorldContainer b) => a.DiscoveryTimestamp.CompareTo(b.DiscoveryTimestamp));
		this._discoveredAsteroidIds.Clear();
		for (int i = 0; i < pooledList.Count; i++)
		{
			if (pooledList[i].IsDiscovered && !pooledList[i].IsModuleInterior)
			{
				this._discoveredAsteroidIds.Add(pooledList[i].id);
			}
		}
		pooledList.Recycle();
		return this._discoveredAsteroidIds;
	}

	public WorldContainer GetStartWorld()
	{
		foreach (WorldContainer worldContainer in this.WorldContainers)
		{
			if (worldContainer.IsStartWorld)
			{
				return worldContainer;
			}
		}
		return this.WorldContainers[0];
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		ClusterManager.Instance = this;
		SaveLoader instance = SaveLoader.Instance;
		instance.OnWorldGenComplete = (Action<Cluster>)Delegate.Combine(instance.OnWorldGenComplete, new Action<Cluster>(this.OnWorldGenComplete));
	}

	protected override void OnSpawn()
	{
		if (this.m_grid == null)
		{
			this.m_grid = new ClusterGrid(this.m_numRings);
		}
		this.UpdateWorldReverbSnapshot(this.activeWorldId);
		base.OnSpawn();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	public WorldContainer activeWorld
	{
		get
		{
			return this.GetWorld(this.activeWorldId);
		}
	}

	private void OnWorldGenComplete(Cluster clusterLayout)
	{
		this.m_numRings = clusterLayout.numRings;
		this.m_grid = new ClusterGrid(this.m_numRings);
		AxialI axialI = AxialI.ZERO;
		foreach (WorldGen worldGen in clusterLayout.worlds)
		{
			int id = this.CreateAsteroidWorldContainer(worldGen).id;
			Vector2I position = worldGen.GetPosition();
			Vector2I vector2I = position + worldGen.GetSize();
			if (worldGen.isStartingWorld)
			{
				axialI = worldGen.GetClusterLocation();
			}
			for (int i = position.y; i < vector2I.y; i++)
			{
				for (int j = position.x; j < vector2I.x; j++)
				{
					int num = Grid.XYToCell(j, i);
					Grid.WorldIdx[num] = (byte)id;
					Pathfinding.Instance.AddDirtyNavGridCell(num);
				}
			}
			if (worldGen.isStartingWorld)
			{
				this.activeWorldIdx = id;
			}
		}
		this.GetSMI<ClusterFogOfWarManager.Instance>().RevealLocation(axialI, 1);
		this.m_clusterPOIsManager.PopulatePOIsFromWorldGen(clusterLayout);
	}

	private int GetNextWorldId()
	{
		HashSetPool<int, ClusterManager>.PooledHashSet pooledHashSet = HashSetPool<int, ClusterManager>.Allocate();
		foreach (WorldContainer worldContainer in this.m_worldContainers)
		{
			pooledHashSet.Add(worldContainer.id);
		}
		global::Debug.Assert(this.m_worldContainers.Count < 255, "Oh no! We're trying to generate our 255th world in this save, things are going to start going badly...");
		for (int i = 0; i < 255; i++)
		{
			if (!pooledHashSet.Contains(i))
			{
				pooledHashSet.Recycle();
				return i;
			}
		}
		pooledHashSet.Recycle();
		return 255;
	}

	private WorldContainer CreateAsteroidWorldContainer(WorldGen world)
	{
		int nextWorldId = this.GetNextWorldId();
		GameObject gameObject = global::Util.KInstantiate(Assets.GetPrefab("Asteroid"), null, null);
		WorldContainer component = gameObject.GetComponent<WorldContainer>();
		component.SetID(nextWorldId);
		component.SetWorldDetails(world);
		AsteroidGridEntity component2 = gameObject.GetComponent<AsteroidGridEntity>();
		if (world != null)
		{
			AxialI clusterLocation = world.GetClusterLocation();
			component2.Init(component.GetRandomName(), clusterLocation, world.Settings.world.asteroidIcon);
		}
		else
		{
			component2.Init("", AxialI.ZERO, "");
		}
		if (component.IsStartWorld)
		{
			OrbitalMechanics component3 = gameObject.GetComponent<OrbitalMechanics>();
			if (component3 != null)
			{
				component3.CreateOrbitalObject(Db.Get().OrbitalTypeCategories.backgroundEarth.Id);
			}
		}
		gameObject.SetActive(true);
		return component;
	}

	private void CreateDefaultAsteroidWorldContainer()
	{
		if (this.m_worldContainers.Count == 0)
		{
			global::Debug.LogWarning("Cluster manager has no world containers, create a default using Grid settings.");
			WorldContainer worldContainer = this.CreateAsteroidWorldContainer(null);
			int id = worldContainer.id;
			int num = (int)worldContainer.minimumBounds.y;
			while ((float)num <= worldContainer.maximumBounds.y)
			{
				int num2 = (int)worldContainer.minimumBounds.x;
				while ((float)num2 <= worldContainer.maximumBounds.x)
				{
					int num3 = Grid.XYToCell(num2, num);
					Grid.WorldIdx[num3] = (byte)id;
					Pathfinding.Instance.AddDirtyNavGridCell(num3);
					num2++;
				}
				num++;
			}
		}
	}

	public void InitializeWorldGrid()
	{
		if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 20))
		{
			this.CreateDefaultAsteroidWorldContainer();
		}
		bool flag = false;
		foreach (WorldContainer worldContainer in this.m_worldContainers)
		{
			Vector2I worldOffset = worldContainer.WorldOffset;
			Vector2I vector2I = worldOffset + worldContainer.WorldSize;
			for (int i = worldOffset.y; i < vector2I.y; i++)
			{
				for (int j = worldOffset.x; j < vector2I.x; j++)
				{
					int num = Grid.XYToCell(j, i);
					Grid.WorldIdx[num] = (byte)worldContainer.id;
					Pathfinding.Instance.AddDirtyNavGridCell(num);
				}
			}
			flag |= worldContainer.IsDiscovered;
		}
		if (!flag)
		{
			global::Debug.LogWarning("No worlds have been discovered. Setting the active world to discovered");
			this.activeWorld.SetDiscovered(false);
		}
	}

	public void SetActiveWorld(int worldIdx)
	{
		int num = this.activeWorldIdx;
		if (num != worldIdx)
		{
			this.activeWorldIdx = worldIdx;
			Game.Instance.Trigger(1983128072, new global::Tuple<int, int>(this.activeWorldIdx, num));
			this.UpdateRocketInteriorAudio();
		}
	}

	public void TimelapseModeOverrideActiveWorld(int overrideValue)
	{
		this.activeWorldIdx = overrideValue;
	}

	public WorldContainer GetWorld(int id)
	{
		for (int i = 0; i < this.m_worldContainers.Count; i++)
		{
			if (this.m_worldContainers[i].id == id)
			{
				return this.m_worldContainers[i];
			}
		}
		return null;
	}

	public WorldContainer GetWorldFromPosition(Vector3 position)
	{
		foreach (WorldContainer worldContainer in this.m_worldContainers)
		{
			if (worldContainer.ContainsPoint(position))
			{
				return worldContainer;
			}
		}
		return null;
	}

	public float CountAllRations()
	{
		float num = 0f;
		foreach (WorldContainer worldContainer in this.m_worldContainers)
		{
			WorldResourceAmountTracker<RationTracker>.Get().CountAmount(null, worldContainer.worldInventory, true);
		}
		return num;
	}

	public Dictionary<Tag, float> GetAllWorldsAccessibleAmounts()
	{
		Dictionary<Tag, float> dictionary = new Dictionary<Tag, float>();
		foreach (WorldContainer worldContainer in this.m_worldContainers)
		{
			foreach (KeyValuePair<Tag, float> keyValuePair in worldContainer.worldInventory.GetAccessibleAmounts())
			{
				if (dictionary.ContainsKey(keyValuePair.Key))
				{
					Dictionary<Tag, float> dictionary2 = dictionary;
					Tag key = keyValuePair.Key;
					dictionary2[key] += keyValuePair.Value;
				}
				else
				{
					dictionary.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
		}
		return dictionary;
	}

	public void MigrateMinion(MinionIdentity minion, int targetID)
	{
		this.MigrateMinion(minion, targetID, minion.GetMyWorldId());
	}

	public void MigrateCritter(GameObject critter, int targetID)
	{
		this.MigrateCritter(critter, targetID, critter.GetMyWorldId());
	}

	public void MigrateCritter(GameObject critter, int targetID, int prevID)
	{
		this.critterMigrationEvArg.entity = critter;
		this.critterMigrationEvArg.prevWorldId = prevID;
		this.critterMigrationEvArg.targetWorldId = targetID;
		Game.Instance.Trigger(1142724171, this.critterMigrationEvArg);
	}

	public void MigrateMinion(MinionIdentity minion, int targetID, int prevID)
	{
		if (!ClusterManager.Instance.GetWorld(targetID).IsDiscovered)
		{
			ClusterManager.Instance.GetWorld(targetID).SetDiscovered(false);
		}
		if (!ClusterManager.Instance.GetWorld(targetID).IsDupeVisited)
		{
			ClusterManager.Instance.GetWorld(targetID).SetDupeVisited();
		}
		this.migrationEvArg.minionId = minion;
		this.migrationEvArg.prevWorldId = prevID;
		this.migrationEvArg.targetWorldId = targetID;
		Game.Instance.assignmentManager.RemoveFromWorld(minion, this.migrationEvArg.prevWorldId);
		Game.Instance.Trigger(586301400, this.migrationEvArg);
	}

	public int GetLandingBeaconLocation(int worldId)
	{
		foreach (object obj in Components.LandingBeacons)
		{
			LandingBeacon.Instance instance = (LandingBeacon.Instance)obj;
			if (instance.GetMyWorldId() == worldId && instance.CanBeTargeted())
			{
				return Grid.PosToCell(instance);
			}
		}
		return Grid.InvalidCell;
	}

	public int GetRandomClearCell(int worldId)
	{
		bool flag = false;
		int num = 0;
		while (!flag && num < 1000)
		{
			num++;
			int num2 = global::UnityEngine.Random.Range(0, Grid.CellCount);
			if (!Grid.Solid[num2] && !Grid.IsLiquid(num2) && (int)Grid.WorldIdx[num2] == worldId)
			{
				return num2;
			}
		}
		num = 0;
		while (!flag && num < 1000)
		{
			num++;
			int num3 = global::UnityEngine.Random.Range(0, Grid.CellCount);
			if (!Grid.Solid[num3] && (int)Grid.WorldIdx[num3] == worldId)
			{
				return num3;
			}
		}
		return Grid.InvalidCell;
	}

	private bool NotObstructedCell(int x, int y)
	{
		int num = Grid.XYToCell(x, y);
		return Grid.IsValidCell(num) && Grid.Objects[num, 1] == null;
	}

	private int LowestYThatSeesSky(int topCellYPos, int x)
	{
		int num = topCellYPos;
		while (!this.ValidSurfaceCell(x, num))
		{
			num--;
		}
		return num;
	}

	private bool ValidSurfaceCell(int x, int y)
	{
		int num = Grid.XYToCell(x, y - 1);
		return Grid.Solid[num] || Grid.Foundation[num];
	}

	public int GetRandomSurfaceCell(int worldID, int width = 1, bool excludeTopBorderHeight = true)
	{
		WorldContainer worldContainer = this.m_worldContainers.Find((WorldContainer match) => match.id == worldID);
		int num = Mathf.RoundToInt(global::UnityEngine.Random.Range(worldContainer.minimumBounds.x + (float)(worldContainer.Width / 10), worldContainer.maximumBounds.x - (float)(worldContainer.Width / 10)));
		int num2 = Mathf.RoundToInt(worldContainer.maximumBounds.y);
		if (excludeTopBorderHeight)
		{
			num2 -= Grid.TopBorderHeight;
		}
		int num3 = num;
		int num4 = this.LowestYThatSeesSky(num2, num3);
		int num5;
		if (this.NotObstructedCell(num3, num4))
		{
			num5 = 1;
		}
		else
		{
			num5 = 0;
		}
		while (num3 + 1 != num && num5 < width)
		{
			num3++;
			if ((float)num3 > worldContainer.maximumBounds.x)
			{
				num5 = 0;
				num3 = (int)worldContainer.minimumBounds.x;
			}
			int num6 = this.LowestYThatSeesSky(num2, num3);
			bool flag = this.NotObstructedCell(num3, num6);
			if (num6 == num4 && flag)
			{
				num5++;
			}
			else if (flag)
			{
				num5 = 1;
			}
			else
			{
				num5 = 0;
			}
			num4 = num6;
		}
		if (num5 < width)
		{
			return -1;
		}
		return Grid.XYToCell(num3, num4);
	}

	public bool IsPositionInActiveWorld(Vector3 pos)
	{
		if (this.activeWorld != null && !CameraController.Instance.ignoreClusterFX)
		{
			Vector2 vector = this.activeWorld.maximumBounds * Grid.CellSizeInMeters + new Vector2(1f, 1f);
			Vector2 vector2 = this.activeWorld.minimumBounds * Grid.CellSizeInMeters;
			if (pos.x < vector2.x || pos.x > vector.x || pos.y < vector2.y || pos.y > vector.y)
			{
				return false;
			}
		}
		return true;
	}

	public WorldContainer CreateRocketInteriorWorld(GameObject craft_go, string interiorTemplateName, global::System.Action callback)
	{
		Vector2I rocket_INTERIOR_SIZE = ROCKETRY.ROCKET_INTERIOR_SIZE;
		Vector2I vector2I;
		if (Grid.GetFreeGridSpace(rocket_INTERIOR_SIZE, out vector2I))
		{
			int nextWorldId = this.GetNextWorldId();
			craft_go.AddComponent<WorldInventory>();
			WorldContainer worldContainer = craft_go.AddComponent<WorldContainer>();
			worldContainer.SetRocketInteriorWorldDetails(nextWorldId, rocket_INTERIOR_SIZE, vector2I);
			Vector2I vector2I2 = vector2I + rocket_INTERIOR_SIZE;
			for (int i = vector2I.y; i < vector2I2.y; i++)
			{
				for (int j = vector2I.x; j < vector2I2.x; j++)
				{
					int num = Grid.XYToCell(j, i);
					Grid.WorldIdx[num] = (byte)nextWorldId;
					Pathfinding.Instance.AddDirtyNavGridCell(num);
				}
			}
			global::Debug.Log(string.Format("Created new rocket interior id: {0}, at {1} with size {2}", nextWorldId, vector2I, rocket_INTERIOR_SIZE));
			worldContainer.PlaceInteriorTemplate(interiorTemplateName, delegate
			{
				if (callback != null)
				{
					callback();
				}
				craft_go.GetComponent<CraftModuleInterface>().TriggerEventOnCraftAndRocket(GameHashes.RocketInteriorComplete, null);
			});
			craft_go.AddOrGet<OrbitalMechanics>().CreateOrbitalObject(Db.Get().OrbitalTypeCategories.landed.Id);
			base.Trigger(-1280433810, worldContainer.id);
			return worldContainer;
		}
		global::Debug.LogError("Failed to create rocket interior.");
		return null;
	}

	public void DestoryRocketInteriorWorld(int world_id, ClustercraftExteriorDoor door)
	{
		WorldContainer world = this.GetWorld(world_id);
		if (world == null || !world.IsModuleInterior)
		{
			global::Debug.LogError(string.Format("Attempting to destroy world id {0}. The world is not a valid rocket interior", world_id));
			return;
		}
		GameObject gameObject = door.GetComponent<RocketModuleCluster>().CraftInterface.gameObject;
		if (this.activeWorldId == world_id)
		{
			if (gameObject.GetComponent<WorldContainer>().ParentWorldId == world_id)
			{
				this.SetActiveWorld(ClusterManager.Instance.GetStartWorld().id);
			}
			else
			{
				this.SetActiveWorld(gameObject.GetComponent<WorldContainer>().ParentWorldId);
			}
		}
		OrbitalMechanics component = gameObject.GetComponent<OrbitalMechanics>();
		if (!component.IsNullOrDestroyed())
		{
			global::UnityEngine.Object.Destroy(component);
		}
		bool flag = gameObject.GetComponent<Clustercraft>().Status == Clustercraft.CraftStatus.InFlight;
		PrimaryElement moduleElemet = door.GetComponent<PrimaryElement>();
		AxialI clusterLocation = world.GetComponent<ClusterGridEntity>().Location;
		Vector3 rocketModuleWorldPos = door.transform.position;
		if (!flag)
		{
			world.EjectAllDupes(rocketModuleWorldPos);
		}
		else
		{
			world.SpacePodAllDupes(clusterLocation, moduleElemet.ElementID);
		}
		world.CancelChores();
		HashSet<int> noRefundTiles;
		world.DestroyWorldBuildings(out noRefundTiles);
		this.UnregisterWorldContainer(world);
		if (!flag)
		{
			GameScheduler.Instance.ScheduleNextFrame("ClusterManager.world.TransferResourcesToParentWorld", delegate(object obj)
			{
				world.TransferResourcesToParentWorld(rocketModuleWorldPos + new Vector3(0f, 0.5f, 0f), noRefundTiles);
			}, null, null);
			GameScheduler.Instance.ScheduleNextFrame("ClusterManager.DeleteWorldObjects", delegate(object obj)
			{
				this.DeleteWorldObjects(world);
			}, null, null);
			return;
		}
		GameScheduler.Instance.ScheduleNextFrame("ClusterManager.world.TransferResourcesToDebris", delegate(object obj)
		{
			world.TransferResourcesToDebris(clusterLocation, noRefundTiles, moduleElemet.ElementID);
		}, null, null);
		GameScheduler.Instance.ScheduleNextFrame("ClusterManager.DeleteWorldObjects", delegate(object obj)
		{
			this.DeleteWorldObjects(world);
		}, null, null);
	}

	public void UpdateWorldReverbSnapshot(int worldId)
	{
		if (DlcManager.FeatureClusterSpaceEnabled())
		{
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().SmallRocketInteriorReverbSnapshot, STOP_MODE.ALLOWFADEOUT);
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MediumRocketInteriorReverbSnapshot, STOP_MODE.ALLOWFADEOUT);
		}
		AudioMixer.instance.PauseSpaceVisibleSnapshot(false);
		WorldContainer world = this.GetWorld(worldId);
		if (world.IsModuleInterior)
		{
			PassengerRocketModule passengerModule = world.GetComponent<Clustercraft>().ModuleInterface.GetPassengerModule();
			AudioMixer.instance.Start(passengerModule.interiorReverbSnapshot);
			AudioMixer.instance.PauseSpaceVisibleSnapshot(true);
			this.UpdateRocketInteriorAudio();
		}
	}

	public void UpdateRocketInteriorAudio()
	{
		WorldContainer activeWorld = this.activeWorld;
		if (activeWorld != null && activeWorld.IsModuleInterior)
		{
			activeWorld.minimumBounds + new Vector2((float)activeWorld.Width * Grid.CellSizeInMeters, (float)activeWorld.Height * Grid.CellSizeInMeters) / 2f;
			Clustercraft component = activeWorld.GetComponent<Clustercraft>();
			ClusterManager.RocketStatesForAudio rocketStatesForAudio = ClusterManager.RocketStatesForAudio.Grounded;
			switch (component.Status)
			{
			case Clustercraft.CraftStatus.Grounded:
				rocketStatesForAudio = (component.LaunchRequested ? ClusterManager.RocketStatesForAudio.ReadyForLaunch : ClusterManager.RocketStatesForAudio.Grounded);
				break;
			case Clustercraft.CraftStatus.Launching:
				rocketStatesForAudio = ClusterManager.RocketStatesForAudio.Launching;
				break;
			case Clustercraft.CraftStatus.InFlight:
				rocketStatesForAudio = ClusterManager.RocketStatesForAudio.InSpace;
				break;
			case Clustercraft.CraftStatus.Landing:
				rocketStatesForAudio = ClusterManager.RocketStatesForAudio.Landing;
				break;
			}
			ClusterManager.RocketInteriorState = rocketStatesForAudio;
		}
	}

	private void DeleteWorldObjects(WorldContainer world)
	{
		Grid.FreeGridSpace(world.WorldSize, world.WorldOffset);
		WorldInventory worldInventory = null;
		if (world != null)
		{
			worldInventory = world.GetComponent<WorldInventory>();
		}
		if (worldInventory != null)
		{
			global::UnityEngine.Object.Destroy(worldInventory);
		}
		if (world != null)
		{
			global::UnityEngine.Object.Destroy(world);
		}
	}

	public static int MAX_ROCKET_INTERIOR_COUNT = 16;

	public static ClusterManager.RocketStatesForAudio RocketInteriorState = ClusterManager.RocketStatesForAudio.Grounded;

	public static ClusterManager Instance;

	private ClusterGrid m_grid;

	[Serialize]
	private int m_numRings = 9;

	[Serialize]
	private int activeWorldIdx;

	public const byte INVALID_WORLD_IDX = 255;

	public static Color[] worldColors = new Color[]
	{
		Color.HSVToRGB(0.15f, 0.3f, 0.5f),
		Color.HSVToRGB(0.3f, 0.3f, 0.5f),
		Color.HSVToRGB(0.45f, 0.3f, 0.5f),
		Color.HSVToRGB(0.6f, 0.3f, 0.5f),
		Color.HSVToRGB(0.75f, 0.3f, 0.5f),
		Color.HSVToRGB(0.9f, 0.3f, 0.5f)
	};

	private List<WorldContainer> m_worldContainers = new List<WorldContainer>();

	[MyCmpGet]
	private ClusterPOIManager m_clusterPOIsManager;

	private Dictionary<int, List<IAssignableIdentity>> minionsByWorld = new Dictionary<int, List<IAssignableIdentity>>();

	private MinionMigrationEventArgs migrationEvArg = new MinionMigrationEventArgs();

	private MigrationEventArgs critterMigrationEvArg = new MigrationEventArgs();

	private List<int> _worldIDs = new List<int>();

	private List<int> _discoveredAsteroidIds = new List<int>();

	public enum RocketStatesForAudio
	{
		Grounded,
		ReadyForLaunch,
		Launching,
		InSpace,
		Landing
	}
}
