using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Database;
using Delaunay.Geo;
using Klei;
using KSerialization;
using LibNoiseDotNet.Graphics.Tools.Noise.Builder;
using ProcGen;
using ProcGen.Map;
using ProcGen.Noise;
using STRINGS;
using UnityEngine;
using VoronoiTree;

namespace ProcGenGame
{
	[Serializable]
	public class WorldGen
	{
		public static string GetSIMSaveFilename(int baseID = -1)
		{
			return global::System.IO.Path.Combine(global::Util.RootFolder(), (baseID == -1) ? "WorldGenSimSave.dat" : string.Format("{0}{1}{2}", "WorldGenSimSave", baseID, ".dat"));
		}

		public static string WORLDGEN_SAVE_FILENAME
		{
			get
			{
				return global::System.IO.Path.Combine(global::Util.RootFolder(), "WorldGenDataSave.dat");
			}
		}

		public static Diseases diseaseStats
		{
			get
			{
				if (WorldGen.m_diseasesDb == null)
				{
					WorldGen.m_diseasesDb = new Diseases(null, true);
				}
				return WorldGen.m_diseasesDb;
			}
		}

		public int BaseLeft
		{
			get
			{
				return this.Settings.GetBaseLocation().left;
			}
		}

		public int BaseRight
		{
			get
			{
				return this.Settings.GetBaseLocation().right;
			}
		}

		public int BaseTop
		{
			get
			{
				return this.Settings.GetBaseLocation().top;
			}
		}

		public int BaseBot
		{
			get
			{
				return this.Settings.GetBaseLocation().bottom;
			}
		}

		public Dictionary<string, object> stats { get; private set; }

		public Data data { get; private set; }

		public bool HasData
		{
			get
			{
				return this.data != null;
			}
		}

		public bool HasNoiseData
		{
			get
			{
				return this.HasData && this.data.world != null;
			}
		}

		public float[] DensityMap
		{
			get
			{
				return this.data.world.density;
			}
		}

		public float[] HeatMap
		{
			get
			{
				return this.data.world.heatOffset;
			}
		}

		public float[] OverrideMap
		{
			get
			{
				return this.data.world.overrides;
			}
		}

		public float[] BaseNoiseMap
		{
			get
			{
				return this.data.world.data;
			}
		}

		public float[] DefaultTendMap
		{
			get
			{
				return this.data.world.defaultTemp;
			}
		}

		public Chunk World
		{
			get
			{
				return this.data.world;
			}
		}

		public Vector2I WorldSize
		{
			get
			{
				return this.data.world.size;
			}
		}

		public Vector2I WorldOffset
		{
			get
			{
				return this.data.world.offset;
			}
		}

		public WorldLayout WorldLayout
		{
			get
			{
				return this.data.worldLayout;
			}
		}

		public List<TerrainCell> OverworldCells
		{
			get
			{
				return this.data.overworldCells;
			}
		}

		public List<TerrainCell> TerrainCells
		{
			get
			{
				return this.data.terrainCells;
			}
		}

		public List<River> Rivers
		{
			get
			{
				return this.data.rivers;
			}
		}

		public GameSpawnData SpawnData
		{
			get
			{
				return this.data.gameSpawnData;
			}
		}

		public int ChunkEdgeSize
		{
			get
			{
				return this.data.chunkEdgeSize;
			}
		}

		public HashSet<int> ClaimedCells
		{
			get
			{
				return this.claimedCells;
			}
		}

		public HashSet<int> HighPriorityClaimedCells
		{
			get
			{
				return this.highPriorityClaims;
			}
		}

		public void ClearClaimedCells()
		{
			this.claimedCells.Clear();
			this.highPriorityClaims.Clear();
		}

		public void AddHighPriorityCells(HashSet<int> cells)
		{
			this.highPriorityClaims.Union<int>(cells);
		}

		public WorldGenSettings Settings { get; private set; }

		public WorldGen(string worldName, List<string> chosenTraits, bool assertMissingTraits)
		{
			WorldGen.LoadSettings();
			this.Settings = new WorldGenSettings(worldName, chosenTraits, assertMissingTraits);
			this.data = new Data();
			this.data.chunkEdgeSize = this.Settings.GetIntSetting("ChunkEdgeSize");
			this.stats = new Dictionary<string, object>();
		}

		public WorldGen(string worldName, Data data, Dictionary<string, object> stats, List<string> chosenTraits, bool assertMissingTraits)
		{
			WorldGen.LoadSettings();
			this.Settings = new WorldGenSettings(worldName, chosenTraits, assertMissingTraits);
			this.data = data;
			this.stats = stats;
		}

		public static void SetupDefaultElements()
		{
			WorldGen.voidElement = ElementLoader.FindElementByHash(SimHashes.Void);
			WorldGen.vacuumElement = ElementLoader.FindElementByHash(SimHashes.Vacuum);
			WorldGen.katairiteElement = ElementLoader.FindElementByHash(SimHashes.Katairite);
			WorldGen.unobtaniumElement = ElementLoader.FindElementByHash(SimHashes.Unobtanium);
		}

		public void Reset()
		{
			this.wasLoaded = false;
		}

		public static void LoadSettings()
		{
			ListPool<YamlIO.Error, WorldGen>.PooledList pooledList = ListPool<YamlIO.Error, WorldGen>.Allocate();
			if (SettingsCache.LoadFiles(pooledList))
			{
				TemplateCache.Init();
			}
			CustomGameSettings.Instance != null;
			if (Application.isPlaying)
			{
				Global.Instance.modManager.HandleErrors(pooledList);
			}
			else
			{
				foreach (YamlIO.Error error in pooledList)
				{
					YamlIO.LogError(error, false);
				}
			}
			pooledList.Recycle();
		}

		public void InitRandom(int worldSeed, int layoutSeed, int terrainSeed, int noiseSeed)
		{
			this.data.globalWorldSeed = worldSeed;
			this.data.globalWorldLayoutSeed = layoutSeed;
			this.data.globalTerrainSeed = terrainSeed;
			this.data.globalNoiseSeed = noiseSeed;
			Console.WriteLine(string.Format("Seeds are [{0}/{1}/{2}/{3}]", new object[] { worldSeed, layoutSeed, terrainSeed, noiseSeed }));
			this.myRandom = new SeededRandom(worldSeed);
		}

		public void Initialise(WorldGen.OfflineCallbackFunction callbackFn, Action<OfflineWorldGen.ErrorInfo> error_cb, int worldSeed = -1, int layoutSeed = -1, int terrainSeed = -1, int noiseSeed = -1, bool debug = false)
		{
			if (this.wasLoaded)
			{
				global::Debug.LogError("Initialise called after load");
				return;
			}
			this.successCallbackFn = callbackFn;
			this.errorCallback = error_cb;
			global::Debug.Assert(this.successCallbackFn != null);
			this.isRunningDebugGen = debug;
			this.running = false;
			int num = global::UnityEngine.Random.Range(0, int.MaxValue);
			if (worldSeed == -1)
			{
				worldSeed = num;
			}
			if (layoutSeed == -1)
			{
				layoutSeed = num;
			}
			if (terrainSeed == -1)
			{
				terrainSeed = num;
			}
			if (noiseSeed == -1)
			{
				noiseSeed = num;
			}
			this.data.gameSpawnData = new GameSpawnData();
			DebugUtil.LogArgs(new object[] { string.Format("World Seeds: {0} for world {1} [{2}/{3}/{4}/{5}]", new object[]
			{
				worldSeed,
				this.Settings.world.name.Replace("STRINGS.WORLDS.", "").Replace(".NAME", ""),
				worldSeed,
				layoutSeed,
				terrainSeed,
				noiseSeed
			}) });
			this.InitRandom(worldSeed, layoutSeed, terrainSeed, noiseSeed);
			this.successCallbackFn(UI.WORLDGEN.COMPLETE.key, 0f, WorldGenProgressStages.Stages.Failure);
			this.stats["GenerateTime"] = 0;
			this.stats["GenerateNoiseTime"] = 0;
			this.stats["GenerateLayoutTime"] = 0;
			this.stats["ConvertVoroToMapTime"] = 0;
			WorldLayout.SetLayerGradient(SettingsCache.layers.LevelLayers);
		}

		public void DontGenerateNoiseData()
		{
			this.generateNoiseData = false;
		}

		public void GenerateOffline()
		{
			int num = 1;
			int num2 = 0;
			while (num2 < num && !this.GenerateWorldData())
			{
				DebugUtil.DevLogError("Failed worldgen");
				this.successCallbackFn(UI.WORLDGEN.RETRYCOUNT.key, (float)num2, WorldGenProgressStages.Stages.Failure);
				num2++;
			}
		}

		private void PlaceTemplateSpawners(Vector2I position, TemplateContainer template, ref Dictionary<int, int> claimedCells)
		{
			this.data.gameSpawnData.AddTemplate(template, position, ref claimedCells);
		}

		public bool RenderOffline(bool doSettle, ref Sim.Cell[] cells, ref Sim.DiseaseCell[] dc, int baseId, bool isStartingWorld = false)
		{
			float[] array = null;
			dc = null;
			HashSet<int> hashSet = new HashSet<int>();
			this.POIBounds = new List<RectInt>();
			this.WriteOverWorldNoise(this.successCallbackFn);
			if (!this.RenderToMap(this.successCallbackFn, ref cells, ref array, ref dc, ref hashSet, ref this.POIBounds))
			{
				this.successCallbackFn(UI.WORLDGEN.FAILED.key, -100f, WorldGenProgressStages.Stages.Failure);
				if (!this.isRunningDebugGen)
				{
					return false;
				}
			}
			foreach (int num in hashSet)
			{
				cells[num].SetValues(WorldGen.unobtaniumElement, ElementLoader.elements);
				this.claimedPOICells[num] = 1;
			}
			List<KeyValuePair<Vector2I, TemplateContainer>> list = null;
			try
			{
				list = TemplateSpawning.DetermineTemplatesForWorld(this.Settings, this.data.terrainCells, this.myRandom, ref this.POIBounds, this.isRunningDebugGen, this.successCallbackFn);
			}
			catch (Exception ex)
			{
				if (!this.isRunningDebugGen)
				{
					this.ReportWorldGenError(ex);
					return false;
				}
			}
			if (isStartingWorld)
			{
				this.EnsureEnoughElementsInStartingBiome(cells);
			}
			List<TerrainCell> terrainCellsForTag = this.GetTerrainCellsForTag(WorldGenTags.StartWorld);
			foreach (TerrainCell terrainCell in this.OverworldCells)
			{
				foreach (TerrainCell terrainCell2 in terrainCellsForTag)
				{
					if (terrainCell.poly.PointInPolygon(terrainCell2.poly.Centroid()))
					{
						terrainCell.node.tags.Add(WorldGenTags.StartWorld);
						break;
					}
				}
			}
			if (doSettle)
			{
				this.running = WorldGenSimUtil.DoSettleSim(this.Settings, ref cells, ref array, ref dc, this.successCallbackFn, this.data, list, this.errorCallback, baseId);
			}
			foreach (KeyValuePair<Vector2I, TemplateContainer> keyValuePair in list)
			{
				this.PlaceTemplateSpawners(keyValuePair.Key, keyValuePair.Value, ref this.claimedPOICells);
			}
			if (doSettle)
			{
				this.SpawnMobsAndTemplates(cells, array, dc, new HashSet<int>(this.claimedPOICells.Keys));
			}
			this.successCallbackFn(UI.WORLDGEN.COMPLETE.key, 1f, WorldGenProgressStages.Stages.Complete);
			this.running = false;
			return true;
		}

		private void SpawnMobsAndTemplates(Sim.Cell[] cells, float[] bgTemp, Sim.DiseaseCell[] dc, HashSet<int> claimedCells)
		{
			MobSpawning.DetectNaturalCavities(this.TerrainCells, this.successCallbackFn, cells);
			SeededRandom seededRandom = new SeededRandom(this.data.globalTerrainSeed);
			for (int i = 0; i < this.TerrainCells.Count; i++)
			{
				float num = (float)i / (float)this.TerrainCells.Count;
				this.successCallbackFn(UI.WORLDGEN.PLACINGCREATURES.key, num, WorldGenProgressStages.Stages.PlacingCreatures);
				TerrainCell terrainCell = this.TerrainCells[i];
				Dictionary<int, string> dictionary = MobSpawning.PlaceFeatureAmbientMobs(this.Settings, terrainCell, seededRandom, cells, bgTemp, dc, claimedCells, this.isRunningDebugGen);
				if (dictionary != null)
				{
					this.data.gameSpawnData.AddRange(dictionary);
				}
				dictionary = MobSpawning.PlaceBiomeAmbientMobs(this.Settings, terrainCell, seededRandom, cells, bgTemp, dc, claimedCells, this.isRunningDebugGen);
				if (dictionary != null)
				{
					this.data.gameSpawnData.AddRange(dictionary);
				}
			}
			this.successCallbackFn(UI.WORLDGEN.PLACINGCREATURES.key, 1f, WorldGenProgressStages.Stages.PlacingCreatures);
		}

		private void ReportWorldGenError(Exception e)
		{
			string settingsCoordinate = CustomGameSettings.Instance.GetSettingsCoordinate();
			global::Debug.LogWarning("Worldgen Failure on seed " + settingsCoordinate);
			this.errorCallback(new OfflineWorldGen.ErrorInfo
			{
				errorDesc = string.Format(UI.FRONTEND.SUPPORTWARNINGS.WORLD_GEN_FAILURE, settingsCoordinate),
				exception = e
			});
			KCrashReporter.ReportErrorDevNotification("WorldgenFailure", e.StackTrace, string.Format("{0} - {1} [Build: {2}]", settingsCoordinate, e.Message, 471618U));
		}

		public void SetWorldSize(int width, int height)
		{
			if (this.data.world != null && this.data.world.offset != Vector2I.zero)
			{
				global::Debug.LogWarning("Resetting world chunk to defaults.");
			}
			this.data.world = new Chunk(0, 0, width, height);
		}

		public Vector2I GetSize()
		{
			return this.data.world.size;
		}

		public void SetPosition(Vector2I position)
		{
			this.data.world.offset = position;
		}

		public Vector2I GetPosition()
		{
			return this.data.world.offset;
		}

		public void SetClusterLocation(AxialI location)
		{
			this.data.clusterLocation = location;
		}

		public AxialI GetClusterLocation()
		{
			return this.data.clusterLocation;
		}

		public bool GenerateNoiseData(WorldGen.OfflineCallbackFunction updateProgressFn)
		{
			this.stats["GenerateNoiseTime"] = global::System.DateTime.Now.Ticks;
			try
			{
				this.running = updateProgressFn(UI.WORLDGEN.SETUPNOISE.key, 0f, WorldGenProgressStages.Stages.SetupNoise);
				if (!this.running)
				{
					this.stats["GenerateNoiseTime"] = 0;
					return false;
				}
				this.SetupNoise(updateProgressFn);
				this.running = updateProgressFn(UI.WORLDGEN.SETUPNOISE.key, 1f, WorldGenProgressStages.Stages.SetupNoise);
				if (!this.running)
				{
					this.stats["GenerateNoiseTime"] = 0;
					return false;
				}
				this.GenerateUnChunkedNoise(updateProgressFn);
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				string stackTrace = ex.StackTrace;
				this.ReportWorldGenError(ex);
				WorldGenLogger.LogException(message, stackTrace);
				this.running = this.successCallbackFn(new StringKey("Exception in GenerateNoiseData"), -1f, WorldGenProgressStages.Stages.Failure);
				return false;
			}
			this.stats["GenerateNoiseTime"] = global::System.DateTime.Now.Ticks - (long)this.stats["GenerateNoiseTime"];
			return true;
		}

		public bool GenerateLayout(WorldGen.OfflineCallbackFunction updateProgressFn)
		{
			this.stats["GenerateLayoutTime"] = global::System.DateTime.Now.Ticks;
			try
			{
				this.running = updateProgressFn(UI.WORLDGEN.WORLDLAYOUT.key, 0f, WorldGenProgressStages.Stages.WorldLayout);
				if (!this.running)
				{
					return false;
				}
				global::Debug.Assert(this.data.world.size.x != 0 && this.data.world.size.y != 0, "Map size has not been set");
				this.data.worldLayout = new WorldLayout(this, this.data.world.size.x, this.data.world.size.y, this.data.globalWorldLayoutSeed);
				this.running = updateProgressFn(UI.WORLDGEN.WORLDLAYOUT.key, 1f, WorldGenProgressStages.Stages.WorldLayout);
				this.data.voronoiTree = null;
				try
				{
					this.data.voronoiTree = this.WorldLayout.GenerateOverworld(this.Settings.world.layoutMethod == global::ProcGen.World.LayoutMethod.PowerTree, this.isRunningDebugGen);
					this.WorldLayout.PopulateSubworlds();
					this.CompleteLayout(updateProgressFn);
				}
				catch (Exception ex)
				{
					string message = ex.Message;
					string stackTrace = ex.StackTrace;
					WorldGenLogger.LogException(message, stackTrace);
					this.ReportWorldGenError(ex);
					this.running = updateProgressFn(new StringKey("Exception in InitVoronoiTree"), -1f, WorldGenProgressStages.Stages.Failure);
					return false;
				}
				this.data.overworldCells = new List<TerrainCell>(40);
				for (int i = 0; i < this.data.voronoiTree.ChildCount(); i++)
				{
					global::VoronoiTree.Tree tree = this.data.voronoiTree.GetChild(i) as global::VoronoiTree.Tree;
					Cell cell = this.data.worldLayout.overworldGraph.FindNodeByID(tree.site.id);
					this.data.overworldCells.Add(new TerrainCellLogged(cell, tree.site, tree.minDistanceToTag));
				}
				this.running = updateProgressFn(UI.WORLDGEN.WORLDLAYOUT.key, 1f, WorldGenProgressStages.Stages.WorldLayout);
			}
			catch (Exception ex2)
			{
				string message2 = ex2.Message;
				string stackTrace2 = ex2.StackTrace;
				WorldGenLogger.LogException(message2, stackTrace2);
				this.ReportWorldGenError(ex2);
				this.successCallbackFn(new StringKey("Exception in GenerateLayout"), -1f, WorldGenProgressStages.Stages.Failure);
				return false;
			}
			this.stats["GenerateLayoutTime"] = global::System.DateTime.Now.Ticks - (long)this.stats["GenerateLayoutTime"];
			return true;
		}

		public bool CompleteLayout(WorldGen.OfflineCallbackFunction updateProgressFn)
		{
			long num = global::System.DateTime.Now.Ticks;
			try
			{
				this.running = updateProgressFn(UI.WORLDGEN.COMPLETELAYOUT.key, 0f, WorldGenProgressStages.Stages.CompleteLayout);
				if (!this.running)
				{
					return false;
				}
				this.data.terrainCells = null;
				this.running = updateProgressFn(UI.WORLDGEN.COMPLETELAYOUT.key, 0.65f, WorldGenProgressStages.Stages.CompleteLayout);
				if (!this.running)
				{
					return false;
				}
				this.running = updateProgressFn(UI.WORLDGEN.COMPLETELAYOUT.key, 0.75f, WorldGenProgressStages.Stages.CompleteLayout);
				if (!this.running)
				{
					return false;
				}
				this.data.terrainCells = new List<TerrainCell>(4000);
				List<global::VoronoiTree.Node> list = new List<global::VoronoiTree.Node>();
				this.data.voronoiTree.ForceLowestToLeaf();
				this.ApplyStartNode();
				this.ApplySwapTags();
				this.data.voronoiTree.GetLeafNodes(list, null);
				WorldLayout.ResetMapGraphFromVoronoiTree(list, this.WorldLayout.localGraph, true);
				for (int i = 0; i < list.Count; i++)
				{
					global::VoronoiTree.Node node = list[i];
					Cell tn = this.data.worldLayout.localGraph.FindNodeByID(node.site.id);
					if (tn != null)
					{
						TerrainCell terrainCell = this.data.terrainCells.Find((TerrainCell c) => c.node == tn);
						if (terrainCell == null)
						{
							TerrainCell terrainCell2 = new TerrainCellLogged(tn, node.site, node.parent.minDistanceToTag);
							this.data.terrainCells.Add(terrainCell2);
						}
						else
						{
							global::Debug.LogWarning("Duplicate cell found" + terrainCell.node.NodeId.ToString());
						}
					}
				}
				for (int j = 0; j < this.data.terrainCells.Count; j++)
				{
					TerrainCell terrainCell3 = this.data.terrainCells[j];
					for (int k = j + 1; k < this.data.terrainCells.Count; k++)
					{
						int num2 = 0;
						TerrainCell terrainCell4 = this.data.terrainCells[k];
						LineSegment lineSegment;
						if (terrainCell4.poly.SharesEdge(terrainCell3.poly, ref num2, out lineSegment) == Polygon.Commonality.Edge)
						{
							terrainCell3.neighbourTerrainCells.Add(k);
							terrainCell4.neighbourTerrainCells.Add(j);
						}
					}
				}
				this.running = updateProgressFn(UI.WORLDGEN.COMPLETELAYOUT.key, 1f, WorldGenProgressStages.Stages.CompleteLayout);
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				string stackTrace = ex.StackTrace;
				WorldGenLogger.LogException(message, stackTrace);
				this.successCallbackFn(new StringKey("Exception in CompleteLayout"), -1f, WorldGenProgressStages.Stages.Failure);
				return false;
			}
			num = global::System.DateTime.Now.Ticks - num;
			this.stats["GenerateLayoutTime"] = (long)this.stats["GenerateLayoutTime"] + num;
			return true;
		}

		public void UpdateVoronoiNodeTags(global::VoronoiTree.Node node)
		{
			global::ProcGen.Node node2;
			if (node.tags.Contains(WorldGenTags.Overworld))
			{
				node2 = this.WorldLayout.overworldGraph.FindNodeByID(node.site.id);
			}
			else
			{
				node2 = this.WorldLayout.localGraph.FindNodeByID(node.site.id);
			}
			if (node2 != null)
			{
				node2.tags.Union(node.tags);
			}
		}

		public bool GenerateWorldData()
		{
			this.stats["GenerateDataTime"] = global::System.DateTime.Now.Ticks;
			if (this.generateNoiseData && !this.GenerateNoiseData(this.successCallbackFn))
			{
				return false;
			}
			if (!this.GenerateLayout(this.successCallbackFn))
			{
				return false;
			}
			this.stats["GenerateDataTime"] = global::System.DateTime.Now.Ticks - (long)this.stats["GenerateDataTime"];
			return true;
		}

		public void EnsureEnoughElementsInStartingBiome(Sim.Cell[] cells)
		{
			List<StartingWorldElementSetting> defaultStartingElements = this.Settings.GetDefaultStartingElements();
			List<TerrainCell> terrainCellsForTag = this.GetTerrainCellsForTag(WorldGenTags.StartWorld);
			foreach (StartingWorldElementSetting startingWorldElementSetting in defaultStartingElements)
			{
				float amount = startingWorldElementSetting.amount;
				Element element = ElementLoader.GetElement(new Tag(((SimHashes)Enum.Parse(typeof(SimHashes), startingWorldElementSetting.element, true)).ToString()));
				float num = 0f;
				int num2 = 0;
				foreach (TerrainCell terrainCell in terrainCellsForTag)
				{
					foreach (int num3 in terrainCell.GetAllCells())
					{
						if (element.idx == cells[num3].elementIdx)
						{
							num2++;
							num += cells[num3].mass;
						}
					}
				}
				DebugUtil.DevAssert(num2 > 0, string.Format("No {0} found in starting biome and trying to ensure at least {1}. Skipping.", element.id, amount), null);
				if (num < amount && num2 > 0)
				{
					float num4 = num / (float)num2;
					float num5 = (amount - num) / (float)num2;
					DebugUtil.DevAssert(num4 + num5 <= 2f * element.maxMass, string.Format("Number of cells ({0}) of {1} in the starting biome is insufficient, this will result in extremely dense cells. {2} but expecting less than {3}", new object[]
					{
						num2,
						element.id,
						num4 + num5,
						2f * element.maxMass
					}), null);
					foreach (TerrainCell terrainCell2 in terrainCellsForTag)
					{
						foreach (int num6 in terrainCell2.GetAllCells())
						{
							if (element.idx == cells[num6].elementIdx)
							{
								int num7 = num6;
								cells[num7].mass = cells[num7].mass + num5;
							}
						}
					}
				}
			}
		}

		public bool RenderToMap(WorldGen.OfflineCallbackFunction updateProgressFn, ref Sim.Cell[] cells, ref float[] bgTemp, ref Sim.DiseaseCell[] dcs, ref HashSet<int> borderCells, ref List<RectInt> poiBounds)
		{
			global::Debug.Assert(Grid.WidthInCells == this.Settings.world.worldsize.x);
			global::Debug.Assert(Grid.HeightInCells == this.Settings.world.worldsize.y);
			global::Debug.Assert(Grid.CellCount == Grid.WidthInCells * Grid.HeightInCells);
			global::Debug.Assert(Grid.CellSizeInMeters != 0f);
			borderCells = new HashSet<int>();
			cells = new Sim.Cell[Grid.CellCount];
			bgTemp = new float[Grid.CellCount];
			dcs = new Sim.DiseaseCell[Grid.CellCount];
			this.running = updateProgressFn(UI.WORLDGEN.CLEARINGLEVEL.key, 0f, WorldGenProgressStages.Stages.ClearingLevel);
			if (!this.running)
			{
				return false;
			}
			for (int i = 0; i < cells.Length; i++)
			{
				cells[i].SetValues(WorldGen.katairiteElement, ElementLoader.elements);
				bgTemp[i] = -1f;
				dcs[i] = default(Sim.DiseaseCell);
				dcs[i].diseaseIdx = byte.MaxValue;
				this.running = updateProgressFn(UI.WORLDGEN.CLEARINGLEVEL.key, (float)i / (float)Grid.CellCount, WorldGenProgressStages.Stages.ClearingLevel);
				if (!this.running)
				{
					return false;
				}
			}
			updateProgressFn(UI.WORLDGEN.CLEARINGLEVEL.key, 1f, WorldGenProgressStages.Stages.ClearingLevel);
			try
			{
				this.ProcessByTerrainCell(cells, bgTemp, dcs, updateProgressFn, this.highPriorityClaims);
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				string stackTrace = ex.StackTrace;
				WorldGenLogger.LogException(message, stackTrace);
				this.running = updateProgressFn(new StringKey("Exception in ProcessByTerrainCell"), -1f, WorldGenProgressStages.Stages.Failure);
				return false;
			}
			if (this.Settings.GetBoolSetting("DrawWorldBorder"))
			{
				SeededRandom seededRandom = new SeededRandom(0);
				this.DrawWorldBorder(cells, this.data.world, seededRandom, ref borderCells, ref poiBounds, updateProgressFn);
				updateProgressFn(UI.WORLDGEN.DRAWWORLDBORDER.key, 1f, WorldGenProgressStages.Stages.DrawWorldBorder);
			}
			this.data.gameSpawnData.baseStartPos = this.data.worldLayout.GetStartLocation();
			return true;
		}

		public SubWorld GetSubWorldForNode(global::VoronoiTree.Tree node)
		{
			global::ProcGen.Node node2 = this.WorldLayout.overworldGraph.FindNodeByID(node.site.id);
			if (node2 == null)
			{
				return null;
			}
			if (!this.Settings.HasSubworld(node2.type))
			{
				return null;
			}
			return this.Settings.GetSubWorld(node2.type);
		}

		public global::VoronoiTree.Tree GetOverworldForNode(Leaf leaf)
		{
			if (leaf == null)
			{
				return null;
			}
			return this.data.worldLayout.GetVoronoiTree().GetChildContainingLeaf(leaf);
		}

		public Leaf GetLeafForTerrainCell(TerrainCell cell)
		{
			if (cell == null)
			{
				return null;
			}
			return this.data.worldLayout.GetVoronoiTree().GetNodeForSite(cell.site) as Leaf;
		}

		public List<TerrainCell> GetTerrainCellsForTag(Tag tag)
		{
			List<TerrainCell> list = new List<TerrainCell>();
			List<global::VoronoiTree.Node> leafNodesWithTag = this.WorldLayout.GetLeafNodesWithTag(tag);
			for (int i = 0; i < leafNodesWithTag.Count; i++)
			{
				global::VoronoiTree.Node node = leafNodesWithTag[i];
				TerrainCell terrainCell = this.data.terrainCells.Find((TerrainCell cell) => cell.site.id == node.site.id);
				if (terrainCell != null)
				{
					list.Add(terrainCell);
				}
			}
			return list;
		}

		private void GetStartCells(out int baseX, out int baseY)
		{
			Vector2I startLocation = new Vector2I(this.data.world.size.x / 2, (int)((float)this.data.world.size.y * 0.7f));
			if (this.data.worldLayout != null)
			{
				startLocation = this.data.worldLayout.GetStartLocation();
			}
			baseX = startLocation.x;
			baseY = startLocation.y;
		}

		public void FinalizeStartLocation()
		{
			if (string.IsNullOrEmpty(this.Settings.world.startSubworldName))
			{
				return;
			}
			List<global::VoronoiTree.Node> startNodes = this.WorldLayout.GetStartNodes();
			global::Debug.Assert(startNodes.Count > 0, "Couldn't find a start node on a world that expects it!!");
			TagSet tagSet = new TagSet();
			tagSet.Add(WorldGenTags.StartLocation);
			for (int i = 1; i < startNodes.Count; i++)
			{
				startNodes[i].tags.Remove(tagSet);
			}
		}

		private void SwitchNodes(global::VoronoiTree.Node n1, global::VoronoiTree.Node n2)
		{
			if (n1 is global::VoronoiTree.Tree || n2 is global::VoronoiTree.Tree)
			{
				global::Debug.Log("WorldGen::SwitchNodes() Skipping tree node");
				return;
			}
			Diagram.Site site = n1.site;
			n1.site = n2.site;
			n2.site = site;
			Cell cell = this.data.worldLayout.localGraph.FindNodeByID(n1.site.id);
			global::ProcGen.Node node = this.data.worldLayout.localGraph.FindNodeByID(n2.site.id);
			string type = cell.type;
			cell.SetType(node.type);
			node.SetType(type);
		}

		private void ApplyStartNode()
		{
			List<global::VoronoiTree.Node> leafNodesWithTag = this.data.worldLayout.GetLeafNodesWithTag(WorldGenTags.StartLocation);
			if (leafNodesWithTag.Count == 0)
			{
				return;
			}
			global::VoronoiTree.Node node = leafNodesWithTag[0];
			global::VoronoiTree.Tree parent = node.parent;
			node.parent.AddTagToChildren(WorldGenTags.IgnoreCaveOverride);
			node.parent.tags.Remove(WorldGenTags.StartLocation);
		}

		private void ApplySwapTags()
		{
			List<global::VoronoiTree.Node> list = new List<global::VoronoiTree.Node>();
			for (int i = 0; i < this.data.voronoiTree.ChildCount(); i++)
			{
				if (this.data.voronoiTree.GetChild(i).tags.Contains(WorldGenTags.SwapLakesToBelow))
				{
					list.Add(this.data.voronoiTree.GetChild(i));
				}
			}
			foreach (global::VoronoiTree.Node node in list)
			{
				if (!node.tags.Contains(WorldGenTags.CenteralFeature))
				{
					List<global::VoronoiTree.Node> list2 = new List<global::VoronoiTree.Node>();
					((global::VoronoiTree.Tree)node).GetNodesWithoutTag(WorldGenTags.CenteralFeature, list2);
					this.SwapNodesAround(WorldGenTags.Wet, true, list2, node.site.poly.Centroid());
				}
			}
		}

		private void SwapNodesAround(Tag swapTag, bool sendTagToBottom, List<global::VoronoiTree.Node> nodes, Vector2 pivot)
		{
			nodes.ShuffleSeeded<global::VoronoiTree.Node>(this.myRandom.RandomSource());
			List<global::VoronoiTree.Node> list = new List<global::VoronoiTree.Node>();
			List<global::VoronoiTree.Node> list2 = new List<global::VoronoiTree.Node>();
			foreach (global::VoronoiTree.Node node in nodes)
			{
				bool flag = node.tags.Contains(swapTag);
				bool flag2 = node.site.poly.Centroid().y > pivot.y;
				bool flag3 = (flag2 && sendTagToBottom) || (!flag2 && !sendTagToBottom);
				if (flag && flag3)
				{
					if (list2.Count > 0)
					{
						this.SwitchNodes(node, list2[0]);
						list2.RemoveAt(0);
					}
					else
					{
						list.Add(node);
					}
				}
				else if (!flag && !flag3)
				{
					if (list.Count > 0)
					{
						this.SwitchNodes(node, list[0]);
						list.RemoveAt(0);
					}
					else
					{
						list2.Add(node);
					}
				}
			}
			if (list2.Count > 0)
			{
				int num = 0;
				while (num < list.Count && list2.Count > 0)
				{
					this.SwitchNodes(list[num], list2[0]);
					list2.RemoveAt(0);
					num++;
				}
			}
		}

		public void GetElementForBiomePoint(Chunk chunk, ElementBandConfiguration elementBands, Vector2I pos, out Element element, out Sim.PhysicsData pd, out Sim.DiseaseCell dc, float erode)
		{
			TerrainCell.ElementOverride elementOverride = TerrainCell.GetElementOverride(WorldGen.voidElement.tag.ToString(), null);
			elementOverride = this.GetElementFromBiomeElementTable(chunk, pos, elementBands, erode);
			element = elementOverride.element;
			pd = elementOverride.pdelement;
			dc = elementOverride.dc;
		}

		public void ConvertIntersectingCellsToType(MathUtil.Pair<Vector2, Vector2> segment, string type)
		{
			List<Vector2I> line = global::ProcGen.Util.GetLine(segment.First, segment.Second);
			for (int i = 0; i < this.data.terrainCells.Count; i++)
			{
				if (this.data.terrainCells[i].node.type != type)
				{
					for (int j = 0; j < line.Count; j++)
					{
						if (this.data.terrainCells[i].poly.Contains(line[j]))
						{
							this.data.terrainCells[i].node.SetType(type);
						}
					}
				}
			}
		}

		public string GetSubWorldType(Vector2I pos)
		{
			for (int i = 0; i < this.data.overworldCells.Count; i++)
			{
				if (this.data.overworldCells[i].poly.Contains(pos))
				{
					return this.data.overworldCells[i].node.type;
				}
			}
			return null;
		}

		private void ProcessByTerrainCell(Sim.Cell[] map_cells, float[] bgTemp, Sim.DiseaseCell[] dcs, WorldGen.OfflineCallbackFunction updateProgressFn, HashSet<int> hightPriorityCells)
		{
			updateProgressFn(UI.WORLDGEN.PROCESSING.key, 0f, WorldGenProgressStages.Stages.Processing);
			SeededRandom seededRandom = new SeededRandom(this.data.globalTerrainSeed);
			try
			{
				for (int i = 0; i < this.data.terrainCells.Count; i++)
				{
					updateProgressFn(UI.WORLDGEN.PROCESSING.key, (float)i / (float)this.data.terrainCells.Count, WorldGenProgressStages.Stages.Processing);
					this.data.terrainCells[i].Process(this, map_cells, bgTemp, dcs, this.data.world, seededRandom);
				}
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				string stackTrace = ex.StackTrace;
				updateProgressFn(new StringKey("Exception in TerrainCell.Process"), -1f, WorldGenProgressStages.Stages.Failure);
				global::Debug.LogError("Error:" + message + "\n" + stackTrace);
			}
			List<Border> list = new List<Border>();
			updateProgressFn(UI.WORLDGEN.BORDERS.key, 0f, WorldGenProgressStages.Stages.Borders);
			try
			{
				List<Edge> edgesWithTag = this.data.worldLayout.overworldGraph.GetEdgesWithTag(WorldGenTags.EdgeUnpassable);
				for (int j = 0; j < edgesWithTag.Count; j++)
				{
					Edge edge = edgesWithTag[j];
					List<Cell> cells2 = this.data.worldLayout.overworldGraph.GetNodes(edge);
					global::Debug.Assert(cells2[0] != cells2[1], "Both nodes on an arc were the same. Allegedly this means it was a world border but I don't think we do that anymore.");
					TerrainCell terrainCell = this.data.overworldCells.Find((TerrainCell c) => c.node == cells2[0]);
					TerrainCell terrainCell2 = this.data.overworldCells.Find((TerrainCell c) => c.node == cells2[1]);
					global::Debug.Assert(terrainCell != null && terrainCell2 != null, "NULL Terrainell nodes with EdgeUnpassable");
					terrainCell.LogInfo("BORDER WITH " + terrainCell2.site.id.ToString(), "UNPASSABLE", 0f);
					terrainCell2.LogInfo("BORDER WITH " + terrainCell.site.id.ToString(), "UNPASSABLE", 0f);
					list.Add(new Border(new Neighbors(terrainCell, terrainCell2), edge.corner0.position, edge.corner1.position)
					{
						element = SettingsCache.borders["impenetrable"],
						width = (float)seededRandom.RandomRange(2, 3)
					});
				}
				List<Edge> edgesWithTag2 = this.data.worldLayout.overworldGraph.GetEdgesWithTag(WorldGenTags.EdgeClosed);
				for (int k = 0; k < edgesWithTag2.Count; k++)
				{
					Edge edge2 = edgesWithTag2[k];
					if (!edgesWithTag.Contains(edge2))
					{
						List<Cell> cells = this.data.worldLayout.overworldGraph.GetNodes(edge2);
						global::Debug.Assert(cells[0] != cells[1], "Both nodes on an arc were the same. Allegedly this means it was a world border but I don't think we do that anymore.");
						TerrainCell terrainCell3 = this.data.overworldCells.Find((TerrainCell c) => c.node == cells[0]);
						TerrainCell terrainCell4 = this.data.overworldCells.Find((TerrainCell c) => c.node == cells[1]);
						global::Debug.Assert(terrainCell3 != null && terrainCell4 != null, "NULL Terraincell nodes with EdgeClosed");
						string borderOverride = this.Settings.GetSubWorld(terrainCell3.node.type).borderOverride;
						string borderOverride2 = this.Settings.GetSubWorld(terrainCell4.node.type).borderOverride;
						string text;
						if (!string.IsNullOrEmpty(borderOverride2) && !string.IsNullOrEmpty(borderOverride))
						{
							text = ((seededRandom.RandomValue() > 0.5f) ? borderOverride2 : borderOverride);
							terrainCell3.LogInfo("BORDER WITH " + terrainCell4.site.id.ToString(), "Picked Random:" + text, 0f);
							terrainCell4.LogInfo("BORDER WITH " + terrainCell3.site.id.ToString(), "Picked Random:" + text, 0f);
						}
						else if (string.IsNullOrEmpty(borderOverride2) && string.IsNullOrEmpty(borderOverride))
						{
							text = "hardToDig";
							terrainCell3.LogInfo("BORDER WITH " + terrainCell4.site.id.ToString(), "Both null", 0f);
							terrainCell4.LogInfo("BORDER WITH " + terrainCell3.site.id.ToString(), "Both null", 0f);
						}
						else
						{
							text = ((!string.IsNullOrEmpty(borderOverride2)) ? borderOverride2 : borderOverride);
							terrainCell3.LogInfo("BORDER WITH " + terrainCell4.site.id.ToString(), "Picked specific " + text, 0f);
							terrainCell4.LogInfo("BORDER WITH " + terrainCell3.site.id.ToString(), "Picked specific " + text, 0f);
						}
						if (!(text == "NONE"))
						{
							Border border = new Border(new Neighbors(terrainCell3, terrainCell4), edge2.corner0.position, edge2.corner1.position);
							border.element = SettingsCache.borders[text];
							MinMax minMax = new MinMax(1.5f, 2f);
							MinMax borderSizeOverride = this.Settings.GetSubWorld(terrainCell3.node.type).borderSizeOverride;
							MinMax borderSizeOverride2 = this.Settings.GetSubWorld(terrainCell4.node.type).borderSizeOverride;
							bool flag = borderSizeOverride.min != 0f || borderSizeOverride.max != 0f;
							bool flag2 = borderSizeOverride2.min != 0f || borderSizeOverride2.max != 0f;
							if (flag && flag2)
							{
								minMax = ((borderSizeOverride.max > borderSizeOverride2.max) ? borderSizeOverride : borderSizeOverride2);
							}
							else if (flag)
							{
								minMax = borderSizeOverride;
							}
							else if (flag2)
							{
								minMax = borderSizeOverride2;
							}
							border.width = seededRandom.RandomRange(minMax.min, minMax.max);
							list.Add(border);
						}
					}
				}
			}
			catch (Exception ex2)
			{
				string message2 = ex2.Message;
				string stackTrace2 = ex2.StackTrace;
				updateProgressFn(new StringKey("Exception in Border creation"), -1f, WorldGenProgressStages.Stages.Failure);
				global::Debug.LogError("Error:" + message2 + " " + stackTrace2);
			}
			try
			{
				if (this.data.world.defaultTemp == null)
				{
					this.data.world.defaultTemp = new float[this.data.world.density.Length];
				}
				for (int l = 0; l < this.data.world.defaultTemp.Length; l++)
				{
					this.data.world.defaultTemp[l] = bgTemp[l];
				}
			}
			catch (Exception ex3)
			{
				string message3 = ex3.Message;
				string stackTrace3 = ex3.StackTrace;
				updateProgressFn(new StringKey("Exception in border.defaultTemp"), -1f, WorldGenProgressStages.Stages.Failure);
				global::Debug.LogError("Error:" + message3 + " " + stackTrace3);
			}
			try
			{
				TerrainCell.SetValuesFunction setValuesFunction = delegate(int index, object elem, Sim.PhysicsData pd, Sim.DiseaseCell dc)
				{
					if (!Grid.IsValidCell(index))
					{
						global::Debug.LogError(string.Concat(new string[]
						{
							"Process::SetValuesFunction Index [",
							index.ToString(),
							"] is not valid. cells.Length [",
							map_cells.Length.ToString(),
							"]"
						}));
						return;
					}
					if (this.highPriorityClaims.Contains(index))
					{
						return;
					}
					if ((elem as Element).HasTag(GameTags.Special))
					{
						pd = (elem as Element).defaultValues;
					}
					map_cells[index].SetValues(elem as Element, pd, ElementLoader.elements);
					dcs[index] = dc;
				};
				for (int m = 0; m < list.Count; m++)
				{
					Border border2 = list[m];
					SubWorld subWorld = this.Settings.GetSubWorld(border2.neighbors.n0.node.type);
					SubWorld subWorld2 = this.Settings.GetSubWorld(border2.neighbors.n1.node.type);
					float num = (SettingsCache.temperatures[subWorld.temperatureRange].min + SettingsCache.temperatures[subWorld.temperatureRange].max) / 2f;
					float num2 = (SettingsCache.temperatures[subWorld2.temperatureRange].min + SettingsCache.temperatures[subWorld2.temperatureRange].max) / 2f;
					float num3 = Mathf.Min(SettingsCache.temperatures[subWorld.temperatureRange].min, SettingsCache.temperatures[subWorld2.temperatureRange].min);
					float num4 = Mathf.Max(SettingsCache.temperatures[subWorld.temperatureRange].max, SettingsCache.temperatures[subWorld2.temperatureRange].max);
					float num5 = (num + num2) / 2f;
					float num6 = num4 - num3;
					float num7 = 2f;
					float num8 = 5f;
					int num9 = 1;
					if (num6 >= 150f)
					{
						num7 = 0f;
						num8 = border2.width * 0.2f;
						num9 = 2;
						border2.width = Mathf.Max(border2.width, 2f);
						float num10 = num - 273.15f;
						float num11 = num2 - 273.15f;
						if (Mathf.Abs(num10) < Mathf.Abs(num11))
						{
							num5 = num;
						}
						else
						{
							num5 = num2;
						}
					}
					border2.Stagger(seededRandom, (float)seededRandom.RandomRange(8, 13), seededRandom.RandomRange(num7, num8));
					border2.ConvertToMap(this.data.world, setValuesFunction, num, num2, num5, seededRandom, num9);
				}
			}
			catch (Exception ex4)
			{
				string message4 = ex4.Message;
				string stackTrace4 = ex4.StackTrace;
				updateProgressFn(new StringKey("Exception in border.ConvertToMap"), -1f, WorldGenProgressStages.Stages.Failure);
				global::Debug.LogError("Error:" + message4 + " " + stackTrace4);
			}
		}

		private void DrawWorldBorder(Sim.Cell[] cells, Chunk world, SeededRandom rnd, ref HashSet<int> borderCells, ref List<RectInt> poiBounds, WorldGen.OfflineCallbackFunction updateProgressFn)
		{
			bool boolSetting = this.Settings.GetBoolSetting("DrawWorldBorderOverVacuum");
			int intSetting = this.Settings.GetIntSetting("WorldBorderThickness");
			int intSetting2 = this.Settings.GetIntSetting("WorldBorderRange");
			byte b = (byte)ElementLoader.elements.IndexOf(WorldGen.vacuumElement);
			byte b2 = (byte)ElementLoader.elements.IndexOf(WorldGen.voidElement);
			byte b3 = (byte)ElementLoader.elements.IndexOf(WorldGen.unobtaniumElement);
			float temperature = WorldGen.unobtaniumElement.defaultValues.temperature;
			float mass = WorldGen.unobtaniumElement.defaultValues.mass;
			int num = 0;
			int num2 = 0;
			updateProgressFn(UI.WORLDGEN.DRAWWORLDBORDER.key, 0f, WorldGenProgressStages.Stages.DrawWorldBorder);
			int num3 = world.size.y - 1;
			int num4 = 0;
			int num5 = world.size.x - 1;
			for (int i = num3; i >= 0; i--)
			{
				updateProgressFn(UI.WORLDGEN.DRAWWORLDBORDER.key, (float)i / (float)num3 * 0.33f, WorldGenProgressStages.Stages.DrawWorldBorder);
				num = Mathf.Max(-intSetting2, Mathf.Min(num + rnd.RandomRange(-2, 2), intSetting2));
				for (int j = 0; j < intSetting + num; j++)
				{
					int num6 = Grid.XYToCell(j, i);
					if (boolSetting || (cells[num6].elementIdx != b && cells[num6].elementIdx != b2))
					{
						borderCells.Add(num6);
						cells[num6].SetValues(b3, temperature, mass);
						num4 = Mathf.Max(num4, j);
					}
				}
				num2 = Mathf.Max(-intSetting2, Mathf.Min(num2 + rnd.RandomRange(-2, 2), intSetting2));
				for (int k = 0; k < intSetting + num2; k++)
				{
					int num7 = world.size.x - 1 - k;
					int num8 = Grid.XYToCell(num7, i);
					if (boolSetting || (cells[num8].elementIdx != b && cells[num8].elementIdx != b2))
					{
						borderCells.Add(num8);
						cells[num8].SetValues(b3, temperature, mass);
						num5 = Mathf.Min(num5, num7);
					}
				}
			}
			this.POIBounds.Add(new RectInt(0, 0, num4 + 1, this.World.size.y));
			this.POIBounds.Add(new RectInt(num5, 0, world.size.x - num5, this.World.size.y));
			int num9 = 0;
			int num10 = 0;
			int num11 = 0;
			int num12 = this.World.size.y - 1;
			for (int l = 0; l < world.size.x; l++)
			{
				updateProgressFn(UI.WORLDGEN.DRAWWORLDBORDER.key, (float)l / (float)world.size.x * 0.66f + 0.33f, WorldGenProgressStages.Stages.DrawWorldBorder);
				num9 = Mathf.Max(-intSetting2, Mathf.Min(num9 + rnd.RandomRange(-2, 2), intSetting2));
				for (int m = 0; m < intSetting + num9; m++)
				{
					int num13 = Grid.XYToCell(l, m);
					if (boolSetting || (cells[num13].elementIdx != b && cells[num13].elementIdx != b2))
					{
						borderCells.Add(num13);
						cells[num13].SetValues(b3, temperature, mass);
						num11 = Mathf.Max(num11, m);
					}
				}
				num10 = Mathf.Max(-intSetting2, Mathf.Min(num10 + rnd.RandomRange(-2, 2), intSetting2));
				for (int n = 0; n < intSetting + num10; n++)
				{
					int num14 = world.size.y - 1 - n;
					int num15 = Grid.XYToCell(l, num14);
					if (boolSetting || (cells[num15].elementIdx != b && cells[num15].elementIdx != b2))
					{
						borderCells.Add(num15);
						cells[num15].SetValues(b3, temperature, mass);
						num12 = Mathf.Min(num12, num14);
					}
				}
			}
			this.POIBounds.Add(new RectInt(0, 0, this.World.size.x, num11 + 1));
			this.POIBounds.Add(new RectInt(0, num12, this.World.size.x, this.World.size.y - num12));
		}

		private void SetupNoise(WorldGen.OfflineCallbackFunction updateProgressFn)
		{
			updateProgressFn(UI.WORLDGEN.BUILDNOISESOURCE.key, 0f, WorldGenProgressStages.Stages.SetupNoise);
			this.heatSource = this.BuildNoiseSource(this.data.world.size.x, this.data.world.size.y, "noise/Heat");
			updateProgressFn(UI.WORLDGEN.BUILDNOISESOURCE.key, 1f, WorldGenProgressStages.Stages.SetupNoise);
		}

		public NoiseMapBuilderPlane BuildNoiseSource(int width, int height, string name)
		{
			global::ProcGen.Noise.Tree tree = SettingsCache.noise.GetTree(name);
			global::Debug.Assert(tree != null, name);
			return this.BuildNoiseSource(width, height, tree);
		}

		public NoiseMapBuilderPlane BuildNoiseSource(int width, int height, global::ProcGen.Noise.Tree tree)
		{
			Vector2f lowerBound = tree.settings.lowerBound;
			Vector2f upperBound = tree.settings.upperBound;
			global::Debug.Assert(lowerBound.x < upperBound.x, string.Concat(new string[]
			{
				"BuildNoiseSource X range broken [l: ",
				lowerBound.x.ToString(),
				" h: ",
				upperBound.x.ToString(),
				"]"
			}));
			global::Debug.Assert(lowerBound.y < upperBound.y, string.Concat(new string[]
			{
				"BuildNoiseSource Y range broken [l: ",
				lowerBound.y.ToString(),
				" h: ",
				upperBound.y.ToString(),
				"]"
			}));
			global::Debug.Assert(width > 0, "BuildNoiseSource width <=0: [" + width.ToString() + "]");
			global::Debug.Assert(height > 0, "BuildNoiseSource height <=0: [" + height.ToString() + "]");
			NoiseMapBuilderPlane noiseMapBuilderPlane = new NoiseMapBuilderPlane(lowerBound.x, upperBound.x, lowerBound.y, upperBound.y, false);
			noiseMapBuilderPlane.SetSize(width, height);
			noiseMapBuilderPlane.SourceModule = tree.BuildFinalModule(this.data.globalNoiseSeed);
			return noiseMapBuilderPlane;
		}

		private void GetMinMaxDataValues(float[] data, int width, int height)
		{
		}

		public static NoiseMap BuildNoiseMap(Vector2 offset, float zoom, NoiseMapBuilderPlane nmbp, int width, int height, NoiseMapBuilderCallback cb = null)
		{
			double num = (double)offset.x;
			double num2 = (double)offset.y;
			if (zoom == 0f)
			{
				zoom = 0.01f;
			}
			double num3 = num * (double)zoom;
			double num4 = (num + (double)width) * (double)zoom;
			double num5 = num2 * (double)zoom;
			double num6 = (num2 + (double)height) * (double)zoom;
			NoiseMap noiseMap = new NoiseMap(width, height);
			nmbp.NoiseMap = noiseMap;
			nmbp.SetBounds((float)num3, (float)num4, (float)num5, (float)num6);
			nmbp.CallBack = cb;
			nmbp.Build();
			return noiseMap;
		}

		public static float[] GenerateNoise(Vector2 offset, float zoom, NoiseMapBuilderPlane nmbp, int width, int height, NoiseMapBuilderCallback cb = null)
		{
			NoiseMap noiseMap = WorldGen.BuildNoiseMap(offset, zoom, nmbp, width, height, cb);
			float[] array = new float[noiseMap.Width * noiseMap.Height];
			noiseMap.CopyTo(ref array);
			return array;
		}

		public static void Normalise(float[] data)
		{
			global::Debug.Assert(data != null && data.Length != 0, "MISSING DATA FOR NORMALIZE");
			float num = float.MaxValue;
			float num2 = float.MinValue;
			for (int i = 0; i < data.Length; i++)
			{
				num = Mathf.Min(data[i], num);
				num2 = Mathf.Max(data[i], num2);
			}
			float num3 = num2 - num;
			for (int j = 0; j < data.Length; j++)
			{
				data[j] = (data[j] - num) / num3;
			}
		}

		private void GenerateUnChunkedNoise(WorldGen.OfflineCallbackFunction updateProgressFn)
		{
			Vector2 vector = new Vector2(0f, 0f);
			updateProgressFn(UI.WORLDGEN.GENERATENOISE.key, 0f, WorldGenProgressStages.Stages.GenerateNoise);
			NoiseMapBuilderCallback noiseMapBuilderCallback = delegate(int line)
			{
				updateProgressFn(UI.WORLDGEN.GENERATENOISE.key, (float)((int)(0f + 0.25f * ((float)line / (float)this.data.world.size.y))), WorldGenProgressStages.Stages.GenerateNoise);
			};
			noiseMapBuilderCallback = delegate(int line)
			{
				updateProgressFn(UI.WORLDGEN.GENERATENOISE.key, (float)((int)(0.25f + 0.25f * ((float)line / (float)this.data.world.size.y))), WorldGenProgressStages.Stages.GenerateNoise);
			};
			if (noiseMapBuilderCallback == null)
			{
				global::Debug.LogError("nupd is null");
			}
			this.data.world.heatOffset = WorldGen.GenerateNoise(vector, SettingsCache.noise.GetZoomForTree("noise/Heat"), this.heatSource, this.data.world.size.x, this.data.world.size.y, noiseMapBuilderCallback);
			this.data.world.data = new float[this.data.world.heatOffset.Length];
			this.data.world.density = new float[this.data.world.heatOffset.Length];
			this.data.world.overrides = new float[this.data.world.heatOffset.Length];
			updateProgressFn(UI.WORLDGEN.NORMALISENOISE.key, 0.5f, WorldGenProgressStages.Stages.GenerateNoise);
			if (SettingsCache.noise.ShouldNormaliseTree("noise/Heat"))
			{
				WorldGen.Normalise(this.data.world.heatOffset);
			}
			updateProgressFn(UI.WORLDGEN.NORMALISENOISE.key, 1f, WorldGenProgressStages.Stages.GenerateNoise);
		}

		public void WriteOverWorldNoise(WorldGen.OfflineCallbackFunction updateProgressFn)
		{
			Dictionary<HashedString, WorldGen.NoiseNormalizationStats> dictionary = new Dictionary<HashedString, WorldGen.NoiseNormalizationStats>();
			float num = (float)this.OverworldCells.Count;
			float perCell = 1f / num;
			float currentProgress = 0f;
			foreach (TerrainCell terrainCell in this.OverworldCells)
			{
				global::ProcGen.Noise.Tree tree = SettingsCache.noise.GetTree("noise/Default");
				global::ProcGen.Noise.Tree tree2 = SettingsCache.noise.GetTree("noise/DefaultCave");
				global::ProcGen.Noise.Tree tree3 = SettingsCache.noise.GetTree("noise/DefaultDensity");
				string text = "noise/Default";
				string text2 = "noise/DefaultCave";
				string text3 = "noise/DefaultDensity";
				SubWorld subWorld = this.Settings.GetSubWorld(terrainCell.node.type);
				if (subWorld == null)
				{
					global::Debug.Log("Couldnt find Subworld for overworld node [" + terrainCell.node.type + "] using defaults");
				}
				else
				{
					if (subWorld.biomeNoise != null)
					{
						global::ProcGen.Noise.Tree tree4 = SettingsCache.noise.GetTree(subWorld.biomeNoise);
						if (tree4 != null)
						{
							tree = tree4;
							text = subWorld.biomeNoise;
						}
					}
					if (subWorld.overrideNoise != null)
					{
						global::ProcGen.Noise.Tree tree5 = SettingsCache.noise.GetTree(subWorld.overrideNoise);
						if (tree5 != null)
						{
							tree2 = tree5;
							text2 = subWorld.overrideNoise;
						}
					}
					if (subWorld.densityNoise != null)
					{
						global::ProcGen.Noise.Tree tree6 = SettingsCache.noise.GetTree(subWorld.densityNoise);
						if (tree6 != null)
						{
							tree3 = tree6;
							text3 = subWorld.densityNoise;
						}
					}
				}
				WorldGen.NoiseNormalizationStats noiseNormalizationStats;
				if (!dictionary.TryGetValue(text, out noiseNormalizationStats))
				{
					noiseNormalizationStats = new WorldGen.NoiseNormalizationStats(this.BaseNoiseMap);
					dictionary.Add(text, noiseNormalizationStats);
				}
				WorldGen.NoiseNormalizationStats noiseNormalizationStats2;
				if (!dictionary.TryGetValue(text2, out noiseNormalizationStats2))
				{
					noiseNormalizationStats2 = new WorldGen.NoiseNormalizationStats(this.OverrideMap);
					dictionary.Add(text2, noiseNormalizationStats2);
				}
				WorldGen.NoiseNormalizationStats noiseNormalizationStats3;
				if (!dictionary.TryGetValue(text3, out noiseNormalizationStats3))
				{
					noiseNormalizationStats3 = new WorldGen.NoiseNormalizationStats(this.DensityMap);
					dictionary.Add(text3, noiseNormalizationStats3);
				}
				int num2 = (int)Mathf.Ceil(terrainCell.poly.bounds.width + 2f);
				int height = (int)Mathf.Ceil(terrainCell.poly.bounds.height + 2f);
				int num3 = (int)Mathf.Floor(terrainCell.poly.bounds.xMin - 1f);
				int num4 = (int)Mathf.Floor(terrainCell.poly.bounds.yMin - 1f);
				Vector2 vector2;
				Vector2 vector = (vector2 = new Vector2((float)num3, (float)num4));
				NoiseMapBuilderCallback noiseMapBuilderCallback = delegate(int line)
				{
					updateProgressFn(UI.WORLDGEN.GENERATENOISE.key, (float)((int)(currentProgress + perCell * ((float)line / (float)height))), WorldGenProgressStages.Stages.NoiseMapBuilder);
				};
				NoiseMapBuilderPlane noiseMapBuilderPlane = this.BuildNoiseSource(num2, height, tree);
				NoiseMap noiseMap = WorldGen.BuildNoiseMap(vector, tree.settings.zoom, noiseMapBuilderPlane, num2, height, noiseMapBuilderCallback);
				NoiseMapBuilderPlane noiseMapBuilderPlane2 = this.BuildNoiseSource(num2, height, tree2);
				NoiseMap noiseMap2 = WorldGen.BuildNoiseMap(vector, tree2.settings.zoom, noiseMapBuilderPlane2, num2, height, noiseMapBuilderCallback);
				NoiseMapBuilderPlane noiseMapBuilderPlane3 = this.BuildNoiseSource(num2, height, tree3);
				NoiseMap noiseMap3 = WorldGen.BuildNoiseMap(vector, tree3.settings.zoom, noiseMapBuilderPlane3, num2, height, noiseMapBuilderCallback);
				vector2.x = (float)((int)Mathf.Floor(terrainCell.poly.bounds.xMin));
				while (vector2.x <= (float)((int)Mathf.Ceil(terrainCell.poly.bounds.xMax)))
				{
					vector2.y = (float)((int)Mathf.Floor(terrainCell.poly.bounds.yMin));
					while (vector2.y <= (float)((int)Mathf.Ceil(terrainCell.poly.bounds.yMax)))
					{
						if (terrainCell.poly.PointInPolygon(vector2))
						{
							int num5 = Grid.XYToCell((int)vector2.x, (int)vector2.y);
							if (tree.settings.normalise)
							{
								noiseNormalizationStats.cells.Add(num5);
							}
							if (tree2.settings.normalise)
							{
								noiseNormalizationStats2.cells.Add(num5);
							}
							if (tree3.settings.normalise)
							{
								noiseNormalizationStats3.cells.Add(num5);
							}
							int num6 = (int)vector2.x - num3;
							int num7 = (int)vector2.y - num4;
							this.BaseNoiseMap[num5] = noiseMap.GetValue(num6, num7);
							this.OverrideMap[num5] = noiseMap2.GetValue(num6, num7);
							this.DensityMap[num5] = noiseMap3.GetValue(num6, num7);
							noiseNormalizationStats.min = Mathf.Min(this.BaseNoiseMap[num5], noiseNormalizationStats.min);
							noiseNormalizationStats.max = Mathf.Max(this.BaseNoiseMap[num5], noiseNormalizationStats.max);
							noiseNormalizationStats2.min = Mathf.Min(this.OverrideMap[num5], noiseNormalizationStats2.min);
							noiseNormalizationStats2.max = Mathf.Max(this.OverrideMap[num5], noiseNormalizationStats2.max);
							noiseNormalizationStats3.min = Mathf.Min(this.DensityMap[num5], noiseNormalizationStats3.min);
							noiseNormalizationStats3.max = Mathf.Max(this.DensityMap[num5], noiseNormalizationStats3.max);
						}
						vector2.y += 1f;
					}
					vector2.x += 1f;
				}
			}
			foreach (KeyValuePair<HashedString, WorldGen.NoiseNormalizationStats> keyValuePair in dictionary)
			{
				float num8 = keyValuePair.Value.max - keyValuePair.Value.min;
				foreach (int num9 in keyValuePair.Value.cells)
				{
					keyValuePair.Value.noise[num9] = (keyValuePair.Value.noise[num9] - keyValuePair.Value.min) / num8;
				}
			}
		}

		private float GetValue(Chunk chunk, Vector2I pos)
		{
			int num = pos.x + this.data.world.size.x * pos.y;
			if (num < 0 || num >= chunk.data.Length)
			{
				throw new ArgumentOutOfRangeException("chunkDataIndex [" + num.ToString() + "]", "chunk data length [" + chunk.data.Length.ToString() + "]");
			}
			return chunk.data[num];
		}

		public bool InChunkRange(Chunk chunk, Vector2I pos)
		{
			int num = pos.x + this.data.world.size.x * pos.y;
			return num >= 0 && num < chunk.data.Length;
		}

		private TerrainCell.ElementOverride GetElementFromBiomeElementTable(Chunk chunk, Vector2I pos, List<ElementGradient> table, float erode)
		{
			float num = this.GetValue(chunk, pos) * erode;
			TerrainCell.ElementOverride elementOverride = TerrainCell.GetElementOverride(WorldGen.voidElement.tag.ToString(), null);
			if (table.Count == 0)
			{
				return elementOverride;
			}
			for (int i = 0; i < table.Count; i++)
			{
				global::Debug.Assert(table[i].content != null, i.ToString());
				if (num < table[i].maxValue)
				{
					return TerrainCell.GetElementOverride(table[i].content, table[i].overrides);
				}
			}
			return TerrainCell.GetElementOverride(table[table.Count - 1].content, table[table.Count - 1].overrides);
		}

		public static bool CanLoad(string fileName)
		{
			if (fileName == null || fileName == "")
			{
				return false;
			}
			bool flag;
			try
			{
				if (File.Exists(fileName))
				{
					using (BinaryReader binaryReader = new BinaryReader(File.Open(fileName, FileMode.Open)))
					{
						return binaryReader.BaseStream.CanRead;
					}
				}
				flag = false;
			}
			catch (FileNotFoundException)
			{
				flag = false;
			}
			catch (Exception ex)
			{
				DebugUtil.LogWarningArgs(new object[] { "Failed to read " + fileName + "\n" + ex.ToString() });
				flag = false;
			}
			return flag;
		}

		public void SaveWorldGen()
		{
			try
			{
				Manager.Clear();
				WorldGenSave worldGenSave = new WorldGenSave();
				worldGenSave.version = new Vector2I(1, 1);
				worldGenSave.stats = this.stats;
				worldGenSave.data = this.data;
				worldGenSave.worldID = this.Settings.world.filePath;
				worldGenSave.traitIDs = new List<string>(this.Settings.GetTraitIDs());
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
					{
						try
						{
							Serializer.Serialize(worldGenSave, binaryWriter);
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

		public static WorldGen Load(IReader reader, bool defaultDiscovered)
		{
			WorldGen worldGen2;
			try
			{
				WorldGenSave worldGenSave = new WorldGenSave();
				Deserializer.Deserialize(worldGenSave, reader);
				WorldGen worldGen = new WorldGen(worldGenSave.worldID, worldGenSave.data, worldGenSave.stats, worldGenSave.traitIDs, false);
				worldGen.isStartingWorld = true;
				if (worldGenSave.version.x != 1 || worldGenSave.version.y > 1)
				{
					DebugUtil.LogErrorArgs(new object[] { string.Concat(new string[]
					{
						"LoadWorldGenSim Error! Wrong save version Current: [",
						1.ToString(),
						".",
						1.ToString(),
						"] File: [",
						worldGenSave.version.x.ToString(),
						".",
						worldGenSave.version.y.ToString(),
						"]"
					}) });
					worldGen.wasLoaded = false;
				}
				else
				{
					worldGen.wasLoaded = true;
				}
				worldGen2 = worldGen;
			}
			catch (Exception ex)
			{
				DebugUtil.LogErrorArgs(new object[] { "WorldGen.Load Error!\n", ex.Message, ex.StackTrace });
				worldGen2 = null;
			}
			return worldGen2;
		}

		public void DrawDebug()
		{
		}

		private const string _SIM_SAVE_FILENAME = "WorldGenSimSave";

		private const string _SIM_SAVE_EXTENSION = ".dat";

		private const string _WORLDGEN_SAVE_FILENAME = "WorldGenDataSave.dat";

		private const int heatScale = 2;

		private const int UNPASSABLE_EDGE_COUNT = 4;

		private const string heat_noise_name = "noise/Heat";

		private const string default_base_noise_name = "noise/Default";

		private const string default_cave_noise_name = "noise/DefaultCave";

		private const string default_density_noise_name = "noise/DefaultDensity";

		public const int WORLDGEN_SAVE_MAJOR_VERSION = 1;

		public const int WORLDGEN_SAVE_MINOR_VERSION = 1;

		private const float EXTREME_TEMPERATURE_BORDER_RANGE = 150f;

		private const float EXTREME_TEMPERATURE_BORDER_MIN_WIDTH = 2f;

		public static Element voidElement;

		public static Element vacuumElement;

		public static Element katairiteElement;

		public static Element unobtaniumElement;

		private static Diseases m_diseasesDb;

		public bool isRunningDebugGen;

		private bool generateNoiseData = true;

		private HashSet<int> claimedCells = new HashSet<int>();

		public Dictionary<int, int> claimedPOICells = new Dictionary<int, int>();

		private HashSet<int> highPriorityClaims = new HashSet<int>();

		public List<RectInt> POIBounds = new List<RectInt>();

		private WorldGen.OfflineCallbackFunction successCallbackFn;

		private bool running = true;

		private Action<OfflineWorldGen.ErrorInfo> errorCallback;

		private SeededRandom myRandom;

		private NoiseMapBuilderPlane heatSource;

		private bool wasLoaded;

		public int polyIndex = -1;

		public bool isStartingWorld;

		public bool isModuleInterior;

		public delegate bool OfflineCallbackFunction(StringKey stringKeyRoot, float completePercent, WorldGenProgressStages.Stages stage);

		public enum GenerateSection
		{
			SolarSystem,
			WorldNoise,
			WorldLayout,
			RenderToMap,
			CollectSpawners
		}

		private class NoiseNormalizationStats
		{
			public NoiseNormalizationStats(float[] noise)
			{
				this.noise = noise;
			}

			public float[] noise;

			public float min = float.MaxValue;

			public float max = float.MinValue;

			public HashSet<int> cells = new HashSet<int>();
		}
	}
}
