using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Klei;
using KSerialization;
using ProcGen;
using UnityEngine;

namespace ProcGenGame
{
	[Serializable]
	public class Cluster
	{
		public ClusterLayout clusterLayout { get; private set; }

		public bool IsGenerationComplete { get; private set; }

		public bool IsGenerating
		{
			get
			{
				return this.thread != null && this.thread.IsAlive;
			}
		}

		private Cluster()
		{
		}

		public Cluster(string name, int seed, bool assertMissingTraits)
		{
			DebugUtil.Assert(!string.IsNullOrEmpty(name), "Cluster file is missing");
			this.seed = seed;
			WorldGen.LoadSettings();
			this.clusterLayout = SettingsCache.clusterLayouts.clusterCache[name];
			this.Id = name;
			for (int i = 0; i < this.clusterLayout.worldPlacements.Count; i++)
			{
				global::ProcGen.World worldData = SettingsCache.worlds.GetWorldData(this.clusterLayout.worldPlacements[i].world);
				if (worldData != null)
				{
					this.clusterLayout.worldPlacements[i].SetSize(worldData.worldsize);
					if (i == this.clusterLayout.startWorldIndex)
					{
						this.clusterLayout.worldPlacements[i].startWorld = true;
					}
				}
			}
			this.size = BestFit.BestFitWorlds(this.clusterLayout.worldPlacements, false);
			foreach (WorldPlacement worldPlacement in this.clusterLayout.worldPlacements)
			{
				List<string> list = new List<string>();
				if (seed > 0)
				{
					list = SettingsCache.GetRandomTraits(seed);
					seed++;
				}
				WorldGen worldGen = new WorldGen(worldPlacement.world, list, assertMissingTraits);
				Vector2I worldsize = worldGen.Settings.world.worldsize;
				worldGen.SetWorldSize(worldsize.x, worldsize.y);
				worldGen.SetPosition(new Vector2I(worldPlacement.x, worldPlacement.y));
				this.worlds.Add(worldGen);
				if (worldPlacement.startWorld)
				{
					this.currentWorld = worldGen;
					worldGen.isStartingWorld = true;
				}
			}
			if (this.currentWorld == null)
			{
				global::Debug.LogWarning(string.Format("Start world not set. Defaulting to first world {0}", this.worlds[0].Settings.world.name));
				this.currentWorld = this.worlds[0];
			}
			if (this.clusterLayout.numRings > 0)
			{
				this.numRings = this.clusterLayout.numRings;
			}
		}

		public void Reset()
		{
			this.worlds.Clear();
		}

		public void Generate(WorldGen.OfflineCallbackFunction callbackFn, Action<OfflineWorldGen.ErrorInfo> error_cb, int worldSeed = -1, int layoutSeed = -1, int terrainSeed = -1, int noiseSeed = -1, bool doSimSettle = true, bool debug = false)
		{
			this.doSimSettle = doSimSettle;
			for (int num = 0; num != this.worlds.Count; num++)
			{
				this.worlds[num].Initialise(callbackFn, error_cb, worldSeed + num, layoutSeed + num, terrainSeed + num, noiseSeed + num, debug);
			}
			this.IsGenerationComplete = false;
			this.thread = new Thread(new ThreadStart(this.ThreadMain));
			global::Util.ApplyInvariantCultureToThread(this.thread);
			this.thread.Start();
		}

		private void BeginGeneration()
		{
			Sim.Cell[] array = null;
			Sim.DiseaseCell[] array2 = null;
			for (int i = 0; i < this.worlds.Count; i++)
			{
				WorldGen worldGen = this.worlds[i];
				if (this.ShouldSkipWorldCallback != null && this.ShouldSkipWorldCallback(i, worldGen))
				{
					global::Debug.Log("Skipping worldgen for " + worldGen.Settings.world.name);
				}
				else
				{
					if (this.PerWorldGenBeginCallback != null)
					{
						this.PerWorldGenBeginCallback(i, worldGen);
					}
					GridSettings.Reset(worldGen.GetSize().x, worldGen.GetSize().y);
					worldGen.GenerateOffline();
					worldGen.FinalizeStartLocation();
					array = null;
					array2 = null;
					if (!worldGen.RenderOffline(this.doSimSettle, ref array, ref array2, i, worldGen.isStartingWorld))
					{
						this.thread = null;
						return;
					}
					if (this.PerWorldGenCompleteCallback != null)
					{
						this.PerWorldGenCompleteCallback(i, worldGen, array, array2);
					}
				}
			}
			this.AssignClusterLocations();
			this.Save();
			this.thread = null;
			this.IsGenerationComplete = true;
		}

		private bool IsValidHex(AxialI location)
		{
			return location.IsWithinRadius(AxialI.ZERO, this.numRings - 1);
		}

		public void AssignClusterLocations()
		{
			this.myRandom = new SeededRandom(this.seed);
			ClusterLayout clusterLayout = SettingsCache.clusterLayouts.clusterCache[this.Id];
			List<WorldPlacement> worldPlacements = clusterLayout.worldPlacements;
			List<SpaceMapPOIPlacement> list = clusterLayout.poiPlacements;
			this.currentWorld.SetClusterLocation(AxialI.ZERO);
			HashSet<AxialI> assignedLocations = new HashSet<AxialI>();
			HashSet<AxialI> worldForbiddenLocations = new HashSet<AxialI>();
			new HashSet<AxialI>();
			HashSet<AxialI> poiWorldAvoidance = new HashSet<AxialI>();
			int num = 2;
			for (int i = 0; i < this.worlds.Count; i++)
			{
				WorldGen worldGen = this.worlds[i];
				WorldPlacement worldPlacement = worldPlacements[i];
				DebugUtil.Assert(worldPlacement != null, "Somehow we're trying to generate a cluster with a world that isn't the cluster .yaml's world list!", worldGen.Settings.world.filePath);
				HashSet<AxialI> antiBuffer = new HashSet<AxialI>();
				foreach (AxialI axialI in assignedLocations)
				{
					antiBuffer.UnionWith(AxialUtil.GetRings(axialI, 1, worldPlacement.buffer));
				}
				List<AxialI> list2 = (from location in AxialUtil.GetRings(AxialI.ZERO, worldPlacement.allowedRings.min, Mathf.Min(worldPlacement.allowedRings.max, this.numRings - 1))
					where !assignedLocations.Contains(location) && !worldForbiddenLocations.Contains(location) && !antiBuffer.Contains(location)
					select location).ToList<AxialI>();
				if (list2.Count > 0)
				{
					AxialI axialI2 = list2[this.myRandom.RandomRange(0, list2.Count)];
					worldGen.SetClusterLocation(axialI2);
					assignedLocations.Add(axialI2);
					worldForbiddenLocations.UnionWith(AxialUtil.GetRings(axialI2, 1, worldPlacement.buffer));
					poiWorldAvoidance.UnionWith(AxialUtil.GetRings(axialI2, 1, num));
				}
				else
				{
					DebugUtil.DevLogError(string.Concat(new string[]
					{
						"Could not find a spot in the cluster for ",
						worldGen.Settings.world.filePath,
						". Check the placement settings in ",
						this.Id,
						".yaml to ensure there are no conflicts."
					}));
					HashSet<AxialI> minBuffers = new HashSet<AxialI>();
					foreach (AxialI axialI3 in assignedLocations)
					{
						minBuffers.UnionWith(AxialUtil.GetRings(axialI3, 1, 2));
					}
					list2 = (from location in AxialUtil.GetRings(AxialI.ZERO, worldPlacement.allowedRings.min, Mathf.Min(worldPlacement.allowedRings.max, this.numRings - 1))
						where !assignedLocations.Contains(location) && !minBuffers.Contains(location)
						select location).ToList<AxialI>();
					DebugUtil.Assert(list2.Count > 0, string.Concat(new string[]
					{
						"Could not find a spot in the cluster for ",
						worldGen.Settings.world.filePath,
						" EVEN AFTER REDUCING BUFFERS. Check the placement settings in ",
						this.Id,
						".yaml to ensure there are no conflicts."
					}));
					AxialI axialI4 = list2[this.myRandom.RandomRange(0, list2.Count)];
					worldGen.SetClusterLocation(axialI4);
					assignedLocations.Add(axialI4);
					worldForbiddenLocations.UnionWith(AxialUtil.GetRings(axialI4, 1, worldPlacement.buffer));
					poiWorldAvoidance.UnionWith(AxialUtil.GetRings(axialI4, 1, num));
				}
			}
			if (DlcManager.FeatureClusterSpaceEnabled() && list != null)
			{
				HashSet<AxialI> poiClumpLocations = new HashSet<AxialI>();
				HashSet<AxialI> poiForbiddenLocations = new HashSet<AxialI>();
				float num2 = 0.5f;
				int num3 = 3;
				int num4 = 0;
				Func<AxialI, bool> <>9__2;
				Func<AxialI, bool> <>9__3;
				foreach (SpaceMapPOIPlacement spaceMapPOIPlacement in list)
				{
					List<string> pois = spaceMapPOIPlacement.pois;
					for (int j = 0; j < spaceMapPOIPlacement.numToSpawn; j++)
					{
						bool flag = this.myRandom.RandomRange(0f, 1f) <= num2;
						List<AxialI> list3 = null;
						if (flag && num4 < num3 && !spaceMapPOIPlacement.avoidClumping)
						{
							num4++;
							IEnumerable<AxialI> rings = AxialUtil.GetRings(AxialI.ZERO, spaceMapPOIPlacement.allowedRings.min, Mathf.Min(spaceMapPOIPlacement.allowedRings.max, this.numRings - 1));
							Func<AxialI, bool> func;
							if ((func = <>9__2) == null)
							{
								func = (<>9__2 = (AxialI location) => !assignedLocations.Contains(location) && poiClumpLocations.Contains(location) && !poiWorldAvoidance.Contains(location));
							}
							list3 = rings.Where<AxialI>(func).ToList<AxialI>();
						}
						if (list3 == null || list3.Count <= 0)
						{
							num4 = 0;
							poiClumpLocations.Clear();
							IEnumerable<AxialI> rings2 = AxialUtil.GetRings(AxialI.ZERO, spaceMapPOIPlacement.allowedRings.min, Mathf.Min(spaceMapPOIPlacement.allowedRings.max, this.numRings - 1));
							Func<AxialI, bool> func2;
							if ((func2 = <>9__3) == null)
							{
								func2 = (<>9__3 = (AxialI location) => !assignedLocations.Contains(location) && !poiWorldAvoidance.Contains(location) && !poiForbiddenLocations.Contains(location));
							}
							list3 = rings2.Where<AxialI>(func2).ToList<AxialI>();
						}
						if (list3 != null && list3.Count > 0)
						{
							AxialI axialI5 = list3[this.myRandom.RandomRange(0, list3.Count)];
							string text = pois[this.myRandom.RandomRange(0, pois.Count)];
							if (!spaceMapPOIPlacement.canSpawnDuplicates)
							{
								pois.Remove(text);
							}
							this.poiPlacements[axialI5] = text;
							poiForbiddenLocations.UnionWith(AxialUtil.GetRings(axialI5, 1, 3));
							poiClumpLocations.UnionWith(AxialUtil.GetRings(axialI5, 1, 1));
							assignedLocations.Add(axialI5);
						}
						else
						{
							global::Debug.LogWarning(string.Format("There is no room for a Space POI in ring range [{0}, {1}]", spaceMapPOIPlacement.allowedRings.min, spaceMapPOIPlacement.allowedRings.max));
						}
					}
				}
			}
		}

		public void AbortGeneration()
		{
			if (this.thread != null && this.thread.IsAlive)
			{
				this.thread.Abort();
				this.thread = null;
			}
		}

		private void ThreadMain()
		{
			this.BeginGeneration();
		}

		private void Save()
		{
			try
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
					{
						try
						{
							Manager.Clear();
							ClusterLayoutSave clusterLayoutSave = new ClusterLayoutSave();
							clusterLayoutSave.version = new Vector2I(1, 1);
							clusterLayoutSave.size = this.size;
							clusterLayoutSave.ID = this.Id;
							clusterLayoutSave.numRings = this.numRings;
							clusterLayoutSave.poiLocations = this.poiLocations;
							clusterLayoutSave.poiPlacements = this.poiPlacements;
							for (int num = 0; num != this.worlds.Count; num++)
							{
								WorldGen worldGen = this.worlds[num];
								clusterLayoutSave.worlds.Add(new ClusterLayoutSave.World
								{
									data = worldGen.data,
									stats = worldGen.stats,
									name = worldGen.Settings.world.filePath,
									isDiscovered = worldGen.isStartingWorld,
									traits = worldGen.Settings.GetTraitIDs().ToList<string>()
								});
								if (worldGen == this.currentWorld)
								{
									clusterLayoutSave.currentWorldIdx = num;
								}
							}
							Serializer.Serialize(clusterLayoutSave, binaryWriter);
						}
						catch (Exception ex)
						{
							DebugUtil.LogErrorArgs(new object[] { "Couldn't serialize", ex.Message, ex.StackTrace });
						}
					}
					using (BinaryWriter binaryWriter2 = new BinaryWriter(File.Open(WorldGen.WORLDGEN_SAVE_FILENAME, FileMode.Create)))
					{
						Manager.SerializeDirectory(binaryWriter2);
						binaryWriter2.Write(memoryStream.ToArray());
					}
				}
			}
			catch (Exception ex2)
			{
				DebugUtil.LogErrorArgs(new object[] { "Couldn't write", ex2.Message, ex2.StackTrace });
			}
		}

		public static Cluster Load()
		{
			Cluster cluster = new Cluster();
			try
			{
				FastReader fastReader = new FastReader(File.ReadAllBytes(WorldGen.WORLDGEN_SAVE_FILENAME));
				Manager.DeserializeDirectory(fastReader);
				int position = fastReader.Position;
				ClusterLayoutSave clusterLayoutSave = new ClusterLayoutSave();
				if (!Deserializer.Deserialize(clusterLayoutSave, fastReader))
				{
					fastReader.Position = position;
					WorldGen worldGen = WorldGen.Load(fastReader, true);
					cluster.worlds.Add(worldGen);
					cluster.size = worldGen.GetSize();
					cluster.currentWorld = cluster.worlds[0] ?? null;
				}
				else
				{
					for (int num = 0; num != clusterLayoutSave.worlds.Count; num++)
					{
						ClusterLayoutSave.World world = clusterLayoutSave.worlds[num];
						WorldGen worldGen2 = new WorldGen(world.name, world.data, world.stats, world.traits, false);
						cluster.worlds.Add(worldGen2);
						if (num == clusterLayoutSave.currentWorldIdx)
						{
							cluster.currentWorld = worldGen2;
							cluster.worlds[num].isStartingWorld = true;
						}
					}
					cluster.size = clusterLayoutSave.size;
					cluster.Id = clusterLayoutSave.ID;
					cluster.numRings = clusterLayoutSave.numRings;
					cluster.poiLocations = clusterLayoutSave.poiLocations;
					cluster.poiPlacements = clusterLayoutSave.poiPlacements;
				}
				DebugUtil.Assert(cluster.currentWorld != null);
				if (cluster.currentWorld == null)
				{
					DebugUtil.Assert(0 < cluster.worlds.Count);
					cluster.currentWorld = cluster.worlds[0];
				}
			}
			catch (Exception ex)
			{
				DebugUtil.LogErrorArgs(new object[] { "SolarSystem.Load Error!\n", ex.Message, ex.StackTrace });
				cluster = null;
			}
			return cluster;
		}

		public void LoadClusterLayoutSim(List<SimSaveFileStructure> loadedWorlds)
		{
			for (int num = 0; num != this.worlds.Count; num++)
			{
				SimSaveFileStructure simSaveFileStructure = new SimSaveFileStructure();
				try
				{
					FastReader fastReader = new FastReader(File.ReadAllBytes(WorldGen.GetSIMSaveFilename(num)));
					Manager.DeserializeDirectory(fastReader);
					Deserializer.Deserialize(simSaveFileStructure, fastReader);
				}
				catch (Exception ex)
				{
					DebugUtil.LogErrorArgs(new object[] { "LoadSim Error!\n", ex.Message, ex.StackTrace });
					break;
				}
				if (simSaveFileStructure.worldDetail == null)
				{
					global::Debug.LogError("Detail is null for world " + num.ToString());
				}
				else
				{
					loadedWorlds.Add(simSaveFileStructure);
				}
			}
		}

		public void SetIsRunningDebug(bool isDebug)
		{
			foreach (WorldGen worldGen in this.worlds)
			{
				worldGen.isRunningDebugGen = isDebug;
			}
		}

		public void DEBUG_UpdateSeed(int seed)
		{
			this.seed = seed;
		}

		public List<WorldGen> worlds = new List<WorldGen>();

		public WorldGen currentWorld;

		public Vector2I size;

		public string Id;

		public int numRings = 5;

		private int seed;

		private SeededRandom myRandom;

		private bool doSimSettle = true;

		public Action<int, WorldGen> PerWorldGenBeginCallback;

		public Action<int, WorldGen, Sim.Cell[], Sim.DiseaseCell[]> PerWorldGenCompleteCallback;

		public Func<int, WorldGen, bool> ShouldSkipWorldCallback;

		public Dictionary<ClusterLayoutSave.POIType, List<AxialI>> poiLocations = new Dictionary<ClusterLayoutSave.POIType, List<AxialI>>();

		public Dictionary<AxialI, string> poiPlacements = new Dictionary<AxialI, string>();

		private Thread thread;
	}
}
