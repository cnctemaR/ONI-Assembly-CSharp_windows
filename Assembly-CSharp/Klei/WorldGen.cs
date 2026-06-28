using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Delaunay.Geo;
using Generated;
using Klei.Map;
using Klei.Noise;
using KSerialization;
using LibNoiseDotNet.Graphics.Tools.Noise.Builder;
using STRINGS;
using UnityEngine;

namespace Klei
{
	[Serializable]
	public class WorldGen
	{
		public static int BaseLeft
		{
			get
			{
				return WorldGen.settings.defaults.baseData.left;
			}
		}

		public static int BaseRight
		{
			get
			{
				return WorldGen.settings.defaults.baseData.right;
			}
		}

		public static int BaseTop
		{
			get
			{
				return WorldGen.settings.defaults.baseData.top;
			}
		}

		public static int BaseBot
		{
			get
			{
				return WorldGen.settings.defaults.baseData.bottom;
			}
		}

		public static string SIM_SAVE_FILENAME
		{
			get
			{
				return global::System.IO.Path.Combine(global::Util.RootFolder(), "WorldGenSimSave.dat");
			}
		}

		public static string WORLDGEN_SAVE_FILENAME
		{
			get
			{
				return global::System.IO.Path.Combine(global::Util.RootFolder(), "WorldGenDataSave.dat");
			}
		}

		public static WorldGenStats Stats
		{
			get
			{
				return WorldGen.stats;
			}
		}

		public static bool HasData
		{
			get
			{
				return WorldGen.data != null;
			}
		}

		public static bool HasNoiseData
		{
			get
			{
				return WorldGen.HasData && WorldGen.data.world != null;
			}
		}

		public static float[] DensityMap
		{
			get
			{
				return WorldGen.data.world.density;
			}
		}

		public static float[] HeatMap
		{
			get
			{
				return WorldGen.data.world.heatOffset;
			}
		}

		public static float[] OverrideMap
		{
			get
			{
				return WorldGen.data.world.overrides;
			}
		}

		public static float[] BaseNoiseMap
		{
			get
			{
				return WorldGen.data.world.data;
			}
		}

		public static float[] DefaultTendMap
		{
			get
			{
				return WorldGen.data.world.defaultTemp;
			}
		}

		public static int GlobalWorldSeed
		{
			get
			{
				if (WorldGen.data == null)
				{
					return 0;
				}
				return WorldGen.data.globalWorldSeed;
			}
		}

		public static Vector2I SubWorldSize
		{
			get
			{
				return WorldGen.data.subWorldSize;
			}
		}

		public static WorldLayout WorldLayout
		{
			get
			{
				return WorldGen.data.worldLayout;
			}
		}

		public static List<TerrainCell> OverworldCells
		{
			get
			{
				return WorldGen.data.overworldCells;
			}
		}

		public static List<TerrainCell> TerrainCells
		{
			get
			{
				return WorldGen.data.terrainCells;
			}
		}

		public static List<Cloud> GasClouds
		{
			get
			{
				return WorldGen.data.clouds;
			}
		}

		public static WorldGen.GameSpawnData SpawnData
		{
			get
			{
				return WorldGen.data.gameSpawnData;
			}
		}

		public static int ChunkEdgeSize
		{
			get
			{
				return WorldGen.data.chunkEdgeSize;
			}
		}

		public static void SetupDefaultElements()
		{
			WorldGen.voidElement = ElementLoader.FindElementByHash(SimHashes.Void);
			WorldGen.vacuumElement = ElementLoader.FindElementByHash(SimHashes.Vacuum);
			WorldGen.katairiteElement = ElementLoader.FindElementByHash(SimHashes.Katairite);
			WorldGen.unobtaniumElement = ElementLoader.FindElementByHash(SimHashes.Unobtanium);
		}

		public static WorldGenSettings Settings
		{
			get
			{
				return WorldGen.settings;
			}
		}

		public static void Reset()
		{
			WorldGen.wasLoaded = false;
		}

		public static void LoadSettings()
		{
			string text = global::System.IO.Path.Combine(Application.streamingAssetsPath, "worldgen/");
			WorldGen.settings = WorldGenSettings.LoadFile(text);
			if (WorldGen.settings == null)
			{
				return;
			}
			if (WorldGen.wasLoaded)
			{
				Debug.Log("Worldgen loaded, dont need to do anything else...");
				return;
			}
			WorldGen.data = new Data();
			WorldGen.data.chunkEdgeSize = WorldGen.settings.defaults.GetInt("ChunkEdgeSize");
			WorldGen.data.subWorldSize = new Vector2I(WorldGen.settings.defaults.GetInt("SubWorldWidth"), WorldGen.settings.defaults.GetInt("SubWorldHeight"));
			WorldGen.stats = new WorldGenStats();
		}

		public static void SaveSettings(string newpath = null)
		{
			string text = global::System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Assets\\tuning\\WorldGenSettings.yaml");
			WorldGen.settings.Save((newpath != null) ? newpath : text);
		}

		public static void InitRandom(int seed)
		{
			WorldGen.data.globalWorldSeed = seed;
			WorldGen.worldGenRandom = new global::System.Random(seed);
		}

		public static global::System.Random RandomSource()
		{
			return WorldGen.worldGenRandom;
		}

		public static float RandomValue()
		{
			if (WorldGen.worldGenRandom == null)
			{
				WorldGen.InitRandom(0);
			}
			return (float)WorldGen.worldGenRandom.NextDouble();
		}

		public static float RandomRange(float rangeLow, float rangeHigh)
		{
			if (WorldGen.worldGenRandom == null)
			{
				WorldGen.InitRandom(0);
			}
			float num = rangeHigh - rangeLow;
			return rangeLow + (float)(WorldGen.worldGenRandom.NextDouble() * (double)num);
		}

		public static void Initialise(WorldGen.OfflineCallbackFunction callbackFn, int seed = -1)
		{
			if (WorldGen.wasLoaded)
			{
				Debug.LogError("Initialise called after load");
				return;
			}
			WorldGen.successCallbackFn = callbackFn;
			WorldGen.running = false;
			if (seed == -1)
			{
				seed = global::UnityEngine.Random.Range(0, int.MaxValue);
			}
			Console.WriteLine(string.Format("World seed is {0}", seed));
			WorldGen.InitRandom(seed);
			WorldGen.successCallbackFn(UI.WORLDGEN.COMPLETE.key, 0f, WorldGenProgressStages.Stages.Failure);
			WorldGen.stats.GenerateTime = 0L;
			WorldGen.stats.GenerateNoiseTime = 0L;
			WorldGen.stats.GenerateLayoutTime = 0L;
			WorldGen.stats.ConvertVoroToMapTime = 0L;
			WorldLayout.SetLayerGradient(WorldGen.settings.layers.LevelLayers);
		}

		public static void GenerateOfflineThreaded()
		{
			if (WorldGen.wasLoaded)
			{
				Debug.LogError("GenerateOfflineThreaded called after load");
				return;
			}
			WorldGen.running = true;
			WorldGen.generateThread = new Thread(new ThreadStart(WorldGen.GenerateOffline));
			WorldGen.generateThread.Start();
		}

		public static void RenderWorldThreaded()
		{
			if (WorldGen.wasLoaded)
			{
				Debug.LogError("RenderWorldThreaded called after load");
				return;
			}
			WorldGen.running = true;
			WorldGen.renderThread = new Thread(new ThreadStart(WorldGen.RenderOfflineThreadFn));
			WorldGen.renderThread.Start();
		}

		public static void Quit()
		{
			if (WorldGen.generateThread != null && WorldGen.generateThread.IsAlive)
			{
				WorldGen.generateThread.Abort();
			}
			if (WorldGen.renderThread != null && WorldGen.renderThread.IsAlive)
			{
				WorldGen.renderThread.Abort();
			}
			WorldGen.running = false;
		}

		public static bool IsGenerateComplete()
		{
			return WorldGen.generateThread != null && !WorldGen.generateThread.IsAlive;
		}

		public static bool IsRenderComplete()
		{
			return WorldGen.renderThread != null && !WorldGen.renderThread.IsAlive;
		}

		public static void GenerateOffline()
		{
			for (int i = 0; i < 10; i++)
			{
				if (WorldGen.GenerateWorldData())
				{
					break;
				}
				WorldGen.successCallbackFn(UI.WORLDGEN.RETRYCOUNT.key, (float)i, WorldGenProgressStages.Stages.Failure);
			}
		}

		public unsafe static void DoSettleSim(Sim.Cell[] cells, float[] bgTemp, WorldGen.OfflineCallbackFunction updateProgressFn)
		{
			SimMessages.SimDataInitializeFromCells(Grid.WidthInCells, Grid.HeightInCells, cells, bgTemp, ElementLoader.elements);
			int num = 300;
			updateProgressFn(UI.WORLDGEN.SETTLESIM.key, 0f, WorldGenProgressStages.Stages.SettleSim);
			Vector2I vector2I = new Vector2I(0, 0);
			Vector2I vector2I2 = new Vector2I(Grid.WidthInCells, Grid.HeightInCells);
			byte[] array = null;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					try
					{
						Sim.Save(binaryWriter);
					}
					catch (Exception ex)
					{
						string message = ex.Message;
						string stackTrace = ex.StackTrace;
						WorldGenLogger.LogException(message, stackTrace);
						WorldGen.running = WorldGen.successCallbackFn(new StringKey("Exception in Sim Save"), -1f, WorldGenProgressStages.Stages.Failure);
						return;
					}
				}
				array = memoryStream.ToArray();
			}
			FastReader fastReader = new FastReader(array);
			if (Sim.Load(fastReader) != 0)
			{
				updateProgressFn(UI.WORLDGEN.FAILED.key, -1f, WorldGenProgressStages.Stages.Failure);
				return;
			}
			byte[] array2 = new byte[Grid.CellCount];
			for (int i = 0; i < Grid.CellCount; i++)
			{
				array2[i] = byte.MaxValue;
			}
			for (int j = 0; j < num; j++)
			{
				SimMessages.NewGameFrame(vector2I, vector2I2);
				IntPtr intPtr = Sim.HandleMessage(SimMessageHashes.PrepareGameData, 0, array2);
				updateProgressFn(UI.WORLDGEN.SETTLESIM.key, (float)j / (float)num * 100f, WorldGenProgressStages.Stages.SettleSim);
				if (!(intPtr == IntPtr.Zero))
				{
					Sim.GameDataUpdate* ptr = (Sim.GameDataUpdate*)(void*)intPtr;
					for (int k = 0; k < ptr->numSubstanceChangeInfo; k++)
					{
						Sim.SubstanceChangeInfo substanceChangeInfo = ptr->substanceChangeInfo[k];
						int cellIdx = substanceChangeInfo.cellIdx;
						cells[cellIdx].elementIdx = ptr->cells[cellIdx].elementIdx;
					}
				}
			}
		}

		private static void PlaceAmbientMobs(TerrainCell tc)
		{
			Dictionary<int, Tag> dictionary = new Dictionary<int, Tag>();
			Node node = tc.node;
			List<Tag> list = new List<Tag>();
			bool flag = false;
			int num = 0;
			if (node.tags == null)
			{
				return;
			}
			if (node.biomeSpecificTags == null)
			{
				return;
			}
			foreach (Tag tag in node.biomeSpecificTags)
			{
				if (WorldGen.Settings.mobs.MobLookupTable.ContainsKey(tag.Name) && WorldGen.Settings.mobs.MobLookupTable[tag.Name] != null)
				{
					list.Add(tag);
					num++;
					flag = true;
				}
			}
			if (!flag)
			{
				return;
			}
			List<int> availableCells = tc.GetAvailableCells();
			for (int i = 0; i < MobSettings.AmbientMobDensity; i++)
			{
				list.Shuffle<Tag>();
				if (availableCells.Count <= 0)
				{
					break;
				}
				for (int j = 0; j < list.Count; j++)
				{
					if (!WorldGen.Settings.mobs.GetMobTags().Contains(list[j]))
					{
						Debug.LogError("Missing sample description for tag [" + list[j].Name + "]");
					}
					else
					{
						Mob mob = WorldGen.settings.mobs.MobLookupTable[list[j].Name];
						List<int> list2 = availableCells.FindAll((int cell) => WorldGen.isSuitableMobSpawnPoint(cell, mob));
						list2.Shuffle<int>();
						float num2 = mob.density.GetValue();
						if (num2 > 1f)
						{
							Debug.LogWarning("Got a mob density greater than 1.0 for " + list[j].Name + ". Probably using density as spacing!");
							num2 = 1f;
						}
						int num3 = Mathf.RoundToInt((float)list2.Count * num2);
						for (int k = 0; k < num3; k++)
						{
							if (list2 != null && list2.Count != 0)
							{
								while (dictionary.ContainsKey(list2[0]))
								{
									list2.RemoveAt(0);
								}
								if (list2.Count > 0)
								{
									dictionary.Add(list2[0], list[j]);
									list2.RemoveAt(0);
								}
							}
						}
					}
				}
			}
			List<KeyValuePair<int, Tag>> list3 = new List<KeyValuePair<int, Tag>>();
			foreach (int num4 in dictionary.Keys)
			{
				list3.Add(new KeyValuePair<int, Tag>(num4, dictionary[num4]));
			}
			if (tc.mobs == null)
			{
				tc.mobs = new List<KeyValuePair<int, Tag>>();
			}
			tc.mobs.AddRange(list3);
			WorldGen.data.gameSpawnData.AddRange(list3);
		}

		private static bool isSuitableMobSpawnPoint(int cell, Mob mob)
		{
			switch (mob.location)
			{
			case Mob.Location.Floor:
				return WorldGen.isNaturalCavity(cell) && !Grid.Solid[cell] && !Grid.Solid[Grid.CellAbove(cell)] && Grid.Solid[Grid.CellBelow(cell)];
			case Mob.Location.Ceiling:
				return WorldGen.isNaturalCavity(cell) && !Grid.Solid[cell] && Grid.Solid[Grid.CellAbove(cell)] && !Grid.Solid[Grid.CellBelow(cell)];
			case Mob.Location.Air:
				return !Grid.Solid[cell] && !Grid.Solid[Grid.CellAbove(cell)];
			case Mob.Location.Solid:
				return !WorldGen.isNaturalCavity(cell) && Grid.Solid[cell];
			}
			return WorldGen.isNaturalCavity(cell) && !Grid.Solid[cell];
		}

		public static void DetectNaturalCavities(WorldGen.OfflineCallbackFunction updateProgressFn)
		{
			updateProgressFn(UI.WORLDGEN.ANALYZINGWORLD.key, 0.8f, WorldGenProgressStages.Stages.DetectNaturalCavities);
			HashSet<int> invalidCells = new HashSet<int>();
			for (int i = 0; i < WorldGen.TerrainCells.Count; i++)
			{
				TerrainCell terrainCell = WorldGen.TerrainCells[i];
				float num = (float)i / (float)WorldGen.TerrainCells.Count * 100f;
				updateProgressFn(UI.WORLDGEN.ANALYZINGWORLDCOMPLETE.key, num, WorldGenProgressStages.Stages.DetectNaturalCavities);
				WorldGen.NaturalCavities.Add(terrainCell, new List<HashSet<int>>());
				invalidCells.Clear();
				for (int j = 0; j < terrainCell.GetAllCells().Count; j++)
				{
					int num2 = terrainCell.GetAllCells()[j];
					if (!Grid.Solid[num2] && !invalidCells.Contains(num2))
					{
						HashSet<int> hashSet = GameUtil.FloodCollectCells(num2, (int checkCell) => !invalidCells.Contains(checkCell) && !Grid.Solid[checkCell], 300, invalidCells);
						if (hashSet != null && hashSet.Count > 0)
						{
							WorldGen.NaturalCavities[terrainCell].Add(hashSet);
							WorldGen.allNaturalCavityCells.UnionWith(hashSet);
						}
					}
				}
			}
			updateProgressFn(UI.WORLDGEN.ANALYZINGWORLDCOMPLETE.key, 100f, WorldGenProgressStages.Stages.DetectNaturalCavities);
		}

		private static void RenderOfflineThreadFn()
		{
			WorldGen.RenderOffline();
		}

		public static Sim.Cell[] RenderOffline()
		{
			Sim.Cell[] array = null;
			float[] array2 = null;
			WorldGen.CompleteLayout(WorldGen.successCallbackFn);
			WorldGen.WriteOverWorldNoise(WorldGen.successCallbackFn);
			if (!WorldGen.RenderToMap(WorldGen.successCallbackFn, ref array, ref array2))
			{
				WorldGen.successCallbackFn(UI.WORLDGEN.FAILED.key, -100f, WorldGenProgressStages.Stages.Failure);
				return null;
			}
			Sim.SIM_Initialize(null);
			SimMessages.CreateSimElementsTable(ElementLoader.elements);
			WorldGen.DoSettleSim(array, array2, WorldGen.successCallbackFn);
			WorldGen.DetectNaturalCavities(WorldGen.successCallbackFn);
			for (int i = 0; i < WorldGen.TerrainCells.Count; i++)
			{
				float num = (float)i / (float)WorldGen.TerrainCells.Count * 100f;
				WorldGen.successCallbackFn(UI.WORLDGEN.PLACINGCREATURES.key, num, WorldGenProgressStages.Stages.PlacingCreatures);
				TerrainCell terrainCell = WorldGen.TerrainCells[i];
				WorldGen.PlaceAmbientMobs(terrainCell);
			}
			WorldGen.successCallbackFn(UI.WORLDGEN.PLACINGCREATURES.key, 100f, WorldGenProgressStages.Stages.PlacingCreatures);
			WorldGen.SaveSim();
			Sim.Shutdown();
			WorldGen.successCallbackFn(UI.WORLDGEN.COMPLETE.key, 101f, WorldGenProgressStages.Stages.Complete);
			WorldGen.running = false;
			return array;
		}

		public static void SetWorldSize(int width, int height)
		{
			WorldGen.data.world = new Chunk(0, 0, width, height);
		}

		public static bool GenerateNoiseData(WorldGen.OfflineCallbackFunction updateProgressFn)
		{
			WorldGen.stats.GenerateNoiseTime = global::System.DateTime.Now.Ticks;
			try
			{
				WorldGen.SetWorldSize(Grid.WidthInCells, Grid.HeightInCells);
				WorldGen.running = updateProgressFn(UI.WORLDGEN.SETUPNOISE.key, 0f, WorldGenProgressStages.Stages.SetupNoise);
				if (!WorldGen.running)
				{
					WorldGen.stats.GenerateNoiseTime = 0L;
					return false;
				}
				WorldGen.SetupNoise(updateProgressFn);
				WorldGen.running = updateProgressFn(UI.WORLDGEN.SETUPNOISE.key, 100f, WorldGenProgressStages.Stages.SetupNoise);
				if (!WorldGen.running)
				{
					WorldGen.stats.GenerateNoiseTime = 0L;
					return false;
				}
				WorldGen.GenerateUnChunkedNoise(updateProgressFn);
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				string stackTrace = ex.StackTrace;
				WorldGenLogger.LogException(message, stackTrace);
				WorldGen.running = WorldGen.successCallbackFn(new StringKey("Exception in GenerateNoiseData"), -1f, WorldGenProgressStages.Stages.Failure);
				return false;
			}
			WorldGen.stats.GenerateNoiseTime = global::System.DateTime.Now.Ticks - WorldGen.stats.GenerateNoiseTime;
			return true;
		}

		public static bool GenerateSolarSystem(WorldGen.OfflineCallbackFunction updateProgressFn)
		{
			try
			{
				WorldGen.running = updateProgressFn(UI.WORLDGEN.GENERATESOLARSYSTEM.key, 0f, WorldGenProgressStages.Stages.GenerateSolarSystem);
				if (!WorldGen.running)
				{
					return false;
				}
				WorldGen.GenerateClouds();
				WorldGen.running = updateProgressFn(UI.WORLDGEN.GENERATESOLARSYSTEM.key, 100f, WorldGenProgressStages.Stages.GenerateSolarSystem);
				if (!WorldGen.running)
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				string stackTrace = ex.StackTrace;
				WorldGenLogger.LogException(message, stackTrace);
				WorldGen.successCallbackFn(new StringKey("Exception in GenerateLayout"), -1f, WorldGenProgressStages.Stages.Failure);
				return false;
			}
			return true;
		}

		public static bool GenerateLayout(WorldGen.OfflineCallbackFunction updateProgressFn)
		{
			WorldGen.stats.GenerateLayoutTime = global::System.DateTime.Now.Ticks;
			try
			{
				WorldGen.running = updateProgressFn(UI.WORLDGEN.WORLDLAYOUT.key, 0f, WorldGenProgressStages.Stages.WorldLayout);
				if (!WorldGen.running)
				{
					return false;
				}
				Debug.Assert(WorldGen.data.world.size.x != 0 && WorldGen.data.world.size.y != 0, "Map size has not been set");
				WorldGen.data.worldLayout = new WorldLayout(WorldGen.data.world.size.x, WorldGen.data.world.size.y);
				WorldGen.running = updateProgressFn(UI.WORLDGEN.WORLDLAYOUT.key, 5f, WorldGenProgressStages.Stages.WorldLayout);
				WorldGen.data.voronoiTree = null;
				try
				{
					WorldGen.data.voronoiTree = WorldGen.WorldLayout.GenerateOverworld();
				}
				catch (Exception ex)
				{
					string message = ex.Message;
					string stackTrace = ex.StackTrace;
					WorldGenLogger.LogException(message, stackTrace);
					WorldGen.running = updateProgressFn(new StringKey("Exception in InitVoronoiTree"), -1f, WorldGenProgressStages.Stages.Failure);
					return false;
				}
				WorldGen.data.overworldCells = new List<TerrainCell>(40);
				for (int i = 0; i < WorldGen.data.voronoiTree.ChildCount(); i++)
				{
					VoronoiTree voronoiTree = WorldGen.data.voronoiTree.GetChild(i) as VoronoiTree;
					Node node = WorldGen.data.worldLayout.overworldGraph.FindNodeByID(voronoiTree.site.id);
					WorldGen.data.overworldCells.Add(new TerrainCell(node, voronoiTree.site));
				}
				WorldGen.running = updateProgressFn(UI.WORLDGEN.WORLDLAYOUT.key, 100f, WorldGenProgressStages.Stages.WorldLayout);
			}
			catch (Exception ex2)
			{
				string message2 = ex2.Message;
				string stackTrace2 = ex2.StackTrace;
				WorldGenLogger.LogException(message2, stackTrace2);
				WorldGen.successCallbackFn(new StringKey("Exception in GenerateLayout"), -1f, WorldGenProgressStages.Stages.Failure);
				return false;
			}
			WorldGen.stats.GenerateLayoutTime = global::System.DateTime.Now.Ticks - WorldGen.stats.GenerateLayoutTime;
			return true;
		}

		public static bool CompleteLayout(WorldGen.OfflineCallbackFunction updateProgressFn)
		{
			long num = global::System.DateTime.Now.Ticks;
			try
			{
				WorldGen.running = updateProgressFn(UI.WORLDGEN.COMPLETELAYOUT.key, 0f, WorldGenProgressStages.Stages.CompleteLayout);
				if (!WorldGen.running)
				{
					return false;
				}
				WorldGen.WorldLayout.ComputeSubWorlds();
				WorldGen.data.terrainCells = null;
				WorldGen.running = updateProgressFn(UI.WORLDGEN.COMPLETELAYOUT.key, 65f, WorldGenProgressStages.Stages.CompleteLayout);
				if (!WorldGen.running)
				{
					return false;
				}
				WorldGen.running = updateProgressFn(UI.WORLDGEN.COMPLETELAYOUT.key, 75f, WorldGenProgressStages.Stages.CompleteLayout);
				if (!WorldGen.running)
				{
					return false;
				}
				WorldGen.ApplyStartNode();
				WorldGen.data.terrainCells = new List<TerrainCell>(4000);
				List<VoronoiNode> list = new List<VoronoiNode>();
				WorldGen.data.voronoiTree.ForceLowestToLeaf();
				WorldGen.data.voronoiTree.UpdateTags();
				WorldGen.data.voronoiTree.GetLeafNodes(list, null);
				for (int i = 0; i < list.Count; i++)
				{
					VoronoiNode voronoiNode = list[i];
					Node node = WorldGen.data.worldLayout.localGraph.FindNodeByID(voronoiNode.site.id);
					if (node != null)
					{
						TerrainCell terrainCell = new TerrainCell(node, voronoiNode.site);
						WorldGen.data.terrainCells.Add(terrainCell);
					}
				}
				WorldGen.running = updateProgressFn(UI.WORLDGEN.COMPLETELAYOUT.key, 100f, WorldGenProgressStages.Stages.CompleteLayout);
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				string stackTrace = ex.StackTrace;
				WorldGenLogger.LogException(message, stackTrace);
				WorldGen.successCallbackFn(new StringKey("Exception in CompleteLayout"), -1f, WorldGenProgressStages.Stages.Failure);
				return false;
			}
			num = global::System.DateTime.Now.Ticks - num;
			WorldGen.stats.GenerateLayoutTime += num;
			return true;
		}

		public static bool GenerateWorldData()
		{
			WorldGen.stats.GenerateDataTime = global::System.DateTime.Now.Ticks;
			if (!WorldGen.GenerateNoiseData(WorldGen.successCallbackFn))
			{
				return false;
			}
			if (!WorldGen.GenerateSolarSystem(WorldGen.successCallbackFn))
			{
				return false;
			}
			if (!WorldGen.GenerateLayout(WorldGen.successCallbackFn))
			{
				return false;
			}
			WorldGen.stats.GenerateDataTime = global::System.DateTime.Now.Ticks - WorldGen.stats.GenerateDataTime;
			return true;
		}

		public static bool RenderToMap(WorldGen.OfflineCallbackFunction updateProgressFn, ref Sim.Cell[] cells, ref float[] bgTemp)
		{
			WorldGen.stats.ConvertVoroToMapTime = global::System.DateTime.Now.Ticks;
			Debug.Assert(Grid.CellCount == Grid.WidthInCells * Grid.HeightInCells);
			Debug.Assert(Grid.CellSizeInMeters != 0f);
			cells = new Sim.Cell[Grid.CellCount];
			bgTemp = new float[Grid.CellCount];
			WorldGen.running = updateProgressFn(UI.WORLDGEN.CLEARINGLEVEL.key, 0f, WorldGenProgressStages.Stages.ClearingLevel);
			if (!WorldGen.running)
			{
				return false;
			}
			for (int i = 0; i < cells.Length; i++)
			{
				cells[i].SetValues(WorldGen.voidElement, ElementLoader.elements);
				bgTemp[i] = -1f;
				WorldGen.running = updateProgressFn(UI.WORLDGEN.CLEARINGLEVEL.key, 100f * ((float)i / (float)Grid.CellCount), WorldGenProgressStages.Stages.ClearingLevel);
				if (!WorldGen.running)
				{
					return false;
				}
			}
			updateProgressFn(UI.WORLDGEN.CLEARINGLEVEL.key, 100f, WorldGenProgressStages.Stages.ClearingLevel);
			WorldGen.stats.MissedCellCount = 0;
			try
			{
				WorldGen.ProcessByTerrainCell(cells, bgTemp, updateProgressFn);
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				string stackTrace = ex.StackTrace;
				WorldGenLogger.LogException(message, stackTrace);
				WorldGen.running = updateProgressFn(new StringKey("Exception in ProcessByTerrainCell"), -1f, WorldGenProgressStages.Stages.Failure);
				return false;
			}
			WorldGen.data.gameSpawnData = new WorldGen.GameSpawnData();
			WorldGen.data.gameSpawnData.clouds = WorldGen.data.clouds;
			for (int j = 0; j < WorldGen.data.terrainCells.Count; j++)
			{
				if (WorldGen.data.terrainCells[j].HasMobs)
				{
					WorldGen.data.gameSpawnData.AddRange(WorldGen.data.terrainCells[j].mobs);
				}
			}
			try
			{
				updateProgressFn(UI.WORLDGEN.PROCESSRIVERS.key, 0f, WorldGenProgressStages.Stages.ProcessRivers);
				WorldGen.data.rivers = WorldGen.data.worldLayout.GetRivers();
				if (WorldGen.data.rivers.Count > 0)
				{
					WorldGen.ProcessRivers(cells);
					WorldGen.GetRiverLocation(ref WorldGen.data.gameSpawnData);
				}
			}
			catch (Exception ex2)
			{
				string message2 = ex2.Message;
				string stackTrace2 = ex2.StackTrace;
				WorldGenLogger.LogException(message2, stackTrace2);
				WorldGen.running = updateProgressFn(new StringKey("Exception in ProcessRivers"), -1f, WorldGenProgressStages.Stages.Failure);
				return false;
			}
			if (bool.Parse(WorldGen.settings.defaults.data["DrawWorldBorder"] as string))
			{
				updateProgressFn(UI.WORLDGEN.DRAWWORLDBORDER.key, 0f, WorldGenProgressStages.Stages.DrawWorldBorder);
				WorldGen.DrawWorldBorder(cells, WorldGen.data.world);
				updateProgressFn(UI.WORLDGEN.DRAWWORLDBORDER.key, 100f, WorldGenProgressStages.Stages.DrawWorldBorder);
			}
			WorldGen.data.gameSpawnData.baseStartPos = WorldGen.data.worldLayout.GetStartLocation();
			WorldGen.stats.ConvertVoroToMapTime = global::System.DateTime.Now.Ticks - WorldGen.stats.ConvertVoroToMapTime;
			return true;
		}

		public static void GenerateLayoutTree()
		{
			WorldGen.stats.GenerateLayoutTime = global::System.DateTime.Now.Ticks;
			WorldGen.data.worldLayout = new WorldLayout(WorldGen.data.world.size.x, WorldGen.data.world.size.y);
		}

		public static SubWorld GetSubWorldForNode(VoronoiTree node)
		{
			Node node2 = WorldGen.WorldLayout.overworldGraph.FindNodeByID(node.site.id);
			if (node2 == null)
			{
				return null;
			}
			if (!WorldGen.Settings.subworlds.zones.ContainsKey(node2.type))
			{
				return null;
			}
			return WorldGen.Settings.subworlds.zones[node2.type];
		}

		public static VoronoiTree GetOverworldForNode(VoronoiLeaf leaf)
		{
			if (leaf == null)
			{
				return null;
			}
			return WorldGen.data.worldLayout.GetVoronoiTree().GetChildContainingLeaf(leaf);
		}

		public static VoronoiLeaf GetLeafForTerrainCell(TerrainCell cell)
		{
			if (cell == null)
			{
				return null;
			}
			return WorldGen.data.worldLayout.GetVoronoiTree().GetNodeForSite(cell.site) as VoronoiLeaf;
		}

		public static List<TerrainCell> GetTerrainCellsForTag(Tag tag)
		{
			List<TerrainCell> list = new List<TerrainCell>();
			List<VoronoiNode> nodesWithTag = WorldGen.WorldLayout.GetNodesWithTag(tag);
			for (int i = 0; i < nodesWithTag.Count; i++)
			{
				VoronoiNode node = nodesWithTag[i];
				TerrainCell terrainCell = WorldGen.data.terrainCells.Find((TerrainCell cell) => cell.site.id == node.site.id);
				if (terrainCell != null)
				{
					list.Add(terrainCell);
				}
			}
			return list;
		}

		private static void GetStartCells(out int baseX, out int baseY)
		{
			Vector2I startLocation = new Vector2I(WorldGen.data.world.size.x / 2, (int)((float)WorldGen.data.world.size.y * 0.7f));
			if (WorldGen.data.worldLayout != null)
			{
				startLocation = WorldGen.data.worldLayout.GetStartLocation();
			}
			baseX = startLocation.x;
			baseY = startLocation.y;
		}

		public static void ChooseBaseLocation(VoronoiNode startNode)
		{
			TagSet tagSet = new TagSet();
			tagSet.Add(WorldGenTags.StartLocation);
			List<VoronoiNode> startNodes = WorldGen.WorldLayout.GetStartNodes();
			for (int i = 0; i < startNodes.Count; i++)
			{
				if (startNodes[i] != startNode)
				{
					startNodes[i].tags.Remove(tagSet);
				}
			}
		}

		private static void SwitchNodes(VoronoiNode n1, VoronoiNode n2)
		{
			if (n1 is VoronoiTree || n2 is VoronoiTree)
			{
				Debug.Log("WorldGen::SwitchNodes() Skipping tree node");
				return;
			}
			Node node = WorldGen.data.worldLayout.localGraph.FindNodeByID(n1.site.id);
			Node node2 = WorldGen.data.worldLayout.localGraph.FindNodeByID(n2.site.id);
			VoronoiDiagram.Site site = n1.site;
			n1.site = n2.site;
			n2.site = site;
			string type = node.type;
			node.type = node2.type;
			node2.type = type;
		}

		public static void ApplyStartNode()
		{
			VoronoiNode voronoiNode = WorldGen.data.worldLayout.GetNodesWithTag(WorldGenTags.StartLocation)[0];
			voronoiNode.parent.AddTagToChildren(WorldGenTags.IgnoreCaveOverride);
			voronoiNode.parent.tags.Remove(WorldGenTags.StartLocation);
			List<VoronoiNode> siblings = voronoiNode.GetSiblings();
			List<VoronoiNode> neighbors = voronoiNode.GetNeighbors();
			siblings.RemoveAll((VoronoiNode node) => neighbors.Contains(node));
			if (neighbors.Count > 0)
			{
				neighbors.ShuffleSeeded<VoronoiNode>(WorldGen.RandomSource());
				List<VoronoiNode> list = new List<VoronoiNode>();
				List<VoronoiNode> list2 = new List<VoronoiNode>();
				for (int i = 0; i < neighbors.Count; i++)
				{
					VoronoiNode voronoiNode2 = neighbors[i];
					bool flag = !voronoiNode2.tags.Contains(WorldGenTags.Wet);
					bool flag2 = voronoiNode2.site.poly.Centroid().y > voronoiNode.site.poly.Centroid().y;
					if (!flag && flag2)
					{
						if (list2.Count > 0)
						{
							WorldGen.SwitchNodes(voronoiNode2, list2[0]);
							list2.RemoveAt(0);
						}
						else
						{
							list.Add(voronoiNode2);
						}
					}
					else if (flag && !flag2)
					{
						if (list.Count > 0)
						{
							WorldGen.SwitchNodes(voronoiNode2, list[0]);
							list.RemoveAt(0);
						}
						else
						{
							list2.Add(voronoiNode2);
						}
					}
				}
				if (list2.Count > 0)
				{
					int num = 0;
					while (num < list.Count && list2.Count > 0)
					{
						WorldGen.SwitchNodes(list[num], list2[0]);
						list2.RemoveAt(0);
						num++;
					}
				}
				if (neighbors.Count > 1)
				{
					neighbors[1].AddTag(WorldGenTags.OxySpace);
				}
			}
		}

		public static void ReplayGenerate(WorldGen.ResetFunction Reset)
		{
			Reset(WorldGen.data.gameSpawnData);
		}

		private static List<SimHashes> GetNonSolidAtTargetTemperature(float temperature)
		{
			List<SimHashes> list = new List<SimHashes>();
			for (int i = 0; i < ElementLoader.elements.Count; i++)
			{
				Element element = ElementLoader.elements[i];
				if (!element.IsSolid && !element.IsVacuum && element.lowTemp < temperature)
				{
					list.Add(element.id);
				}
			}
			return list;
		}

		private static void GenerateClouds()
		{
			int num = int.Parse(WorldGen.Settings.defaults.data["CloudCountMin"] as string);
			int num2 = int.Parse(WorldGen.Settings.defaults.data["CloudCountRange"] as string);
			float num3 = float.Parse(WorldGen.Settings.defaults.data["CloudRampMin"] as string);
			float num4 = float.Parse(WorldGen.Settings.defaults.data["CloudRampRange"] as string);
			float num5 = float.Parse(WorldGen.Settings.defaults.data["CloudSizeMin"] as string);
			float num6 = float.Parse(WorldGen.Settings.defaults.data["CloudSizeRange"] as string);
			float num7 = float.Parse(WorldGen.Settings.defaults.data["CloudTemperatureMin"] as string);
			float num8 = float.Parse(WorldGen.Settings.defaults.data["CloudTemperatureRange"] as string);
			float num9 = float.Parse(WorldGen.Settings.defaults.data["CloudMassMin"] as string);
			float num10 = float.Parse(WorldGen.Settings.defaults.data["CloudMassRange"] as string);
			int num11 = num + (int)((float)num2 * WorldGen.RandomValue());
			WorldGen.data.clouds = new List<Cloud>();
			for (int i = 0; i < num11; i++)
			{
				Cloud cloud = new Cloud();
				cloud.rampTime = num3 + num4 * WorldGen.RandomValue();
				cloud.onLength = num5 + num6 * WorldGen.RandomValue();
				cloud.externalTemperatureK = num7 + num8 * WorldGen.RandomValue();
				List<SimHashes> nonSolidAtTargetTemperature = WorldGen.GetNonSolidAtTargetTemperature(cloud.externalTemperatureK);
				nonSolidAtTargetTemperature.ShuffleSeeded<SimHashes>(WorldGen.RandomSource());
				cloud.element = nonSolidAtTargetTemperature[0];
				cloud.externalMassKg = num9 + num10 * WorldGen.RandomValue();
				WorldGen.data.clouds.Add(cloud);
			}
		}

		private static void ProcessRivers(Sim.Cell[] cells)
		{
			TerrainCell.SetValuesFunction setValuesFunction = delegate(int index, Element elem, Sim.PhysicsData pd)
			{
				if (Grid.IsValidCell(index))
				{
					cells[index].SetValues(elem, pd, ElementLoader.elements);
				}
				else
				{
					Debug.LogError(string.Concat(new object[] { "Process::SetValuesFunction Index [", index, "] is not valid. cells.Length [", cells.Length, "]" }));
				}
			};
			float num = 265f;
			float num2 = 30f;
			for (int i = 0; i < WorldGen.data.rivers.Count; i++)
			{
				WorldGen.data.rivers[i].ConvertToMap(WorldGen.data.world, setValuesFunction, num, num2);
			}
		}

		public static River GetRiverForCell(int cell)
		{
			return WorldGen.data.rivers.Find((River river) => Grid.PosToCell(river.SourcePosition()) == cell || Grid.PosToCell(river.SinkPosition()) == cell);
		}

		private static void GetRiverLocation(ref WorldGen.GameSpawnData gsd)
		{
			if (WorldGen.data.rivers != null)
			{
				for (int i = 0; i < WorldGen.data.rivers.Count; i++)
				{
					Vector2 vector = WorldGen.data.rivers[i].SourcePosition();
					Vector2 vector2 = WorldGen.data.rivers[i].SinkPosition();
					if (vector.y < vector2.y)
					{
						Vector2 vector3 = vector2;
						vector2 = vector;
						vector = vector3;
					}
					int num = Grid.PosToCell(vector);
					int num2 = Grid.PosToCell(vector2);
					gsd.Add(num, GameTags.RiverSource);
					gsd.Add(num2, GameTags.RiverSink);
				}
			}
		}

		private static Rect GetBoxAroundStart(float radius)
		{
			int num;
			int num2;
			WorldGen.GetStartCells(out num, out num2);
			return new Rect((float)num - radius, (float)num2 - radius, 2f * radius, 2f * radius);
		}

		private static void AddStartLocation(Sim.Cell[] cells)
		{
			int num;
			int num2;
			WorldGen.GetStartCells(out num, out num2);
			float @float = WorldGen.settings.defaults.GetFloat("StartAreaPressureMultiplier");
			float float2 = WorldGen.settings.defaults.GetFloat("StartAreaTemperatureOffset");
			Element element = ElementLoader.FindElementByHash(SimHashes.Oxygen);
			Element element2 = ElementLoader.FindElementByHash(SimHashes.Ice);
			for (int i = WorldGen.BaseLeft - 1; i <= WorldGen.BaseRight + 1; i++)
			{
				for (int j = WorldGen.BaseBot; j <= WorldGen.BaseTop; j++)
				{
					int num3 = Grid.XYToCell(i + num, j + num2);
					if ((i >= WorldGen.BaseLeft && i <= WorldGen.BaseRight && (j == WorldGen.BaseBot || j == WorldGen.BaseTop)) || ((i == WorldGen.BaseLeft - 1 || i == WorldGen.BaseRight + 1) && j >= WorldGen.BaseBot && j <= WorldGen.BaseTop))
					{
						cells[num3].SetValues(element2, ElementLoader.elements);
					}
					else
					{
						Sim.PhysicsData defaultValues = element.defaultValues;
						defaultValues.pressure *= @float;
						defaultValues.temperature = 273.15f + float2;
						cells[num3].SetValues(element, defaultValues, ElementLoader.elements);
					}
				}
			}
		}

		public static void GetElementForBiome(Chunk chunk, string nt, Vector2I pos, out Element element, out Sim.PhysicsData pd, float erode)
		{
			element = WorldGen.voidElement;
			if (WorldGen.settings.biomes.TerrainBiomeLookupTable.ContainsKey(nt))
			{
				element = WorldGen.GetElementFromBiomeElementTable(chunk, pos, WorldGen.settings.biomes.TerrainBiomeLookupTable[nt], erode);
			}
			else if (WorldGen.settings.features.TerrainFeatures.ContainsKey(nt))
			{
				if (WorldGen.settings.features.TerrainFeatures[nt] == null)
				{
					Debug.LogError("TerrainFeatureLookupTable is null for [" + nt + "]");
				}
				string type = WorldGen.settings.features.TerrainFeatures[nt].defaultBiome.type;
				if (WorldGen.settings.biomes.TerrainBiomeLookupTable.ContainsKey(type))
				{
					Biome biome = WorldGen.settings.biomes.TerrainBiomeLookupTable[type];
					element = WorldGen.GetElementFromBiomeElementTable(chunk, pos, biome, erode);
				}
				else
				{
					Debug.LogError("No biome lookup table of type " + type + " is loaded.");
				}
			}
			pd = element.defaultValues;
		}

		public static bool isNaturalCavity(int cell)
		{
			return WorldGen.NaturalCavities != null && WorldGen.allNaturalCavityCells.Contains(cell);
		}

		private static bool ConvertTerrainCellsToEdges(WorldGen.OfflineCallbackFunction updateProgress)
		{
			for (int i = 0; i < WorldGen.data.overworldCells.Count; i++)
			{
				WorldGen.running = updateProgress(UI.WORLDGEN.CONVERTTERRAINCELLSTOEDGES.key, (float)i / (float)WorldGen.data.overworldCells.Count, WorldGenProgressStages.Stages.ConvertCellsToEdges);
				if (!WorldGen.running)
				{
					return WorldGen.running;
				}
				List<Vector2> vertices = WorldGen.data.overworldCells[i].poly.Vertices;
				for (int j = 0; j < vertices.Count; j++)
				{
					if (j < vertices.Count - 1)
					{
						WorldGen.ConvertIntersectingCellsToType(new MathUtil.Pair<Vector2, Vector2>(vertices[j], vertices[j + 1]), "EDGE");
					}
					else
					{
						WorldGen.ConvertIntersectingCellsToType(new MathUtil.Pair<Vector2, Vector2>(vertices[j], vertices[0]), (vertices.Count <= 4) ? "EDGE" : "UNPASSABLE");
					}
				}
			}
			return true;
		}

		public static void ConvertIntersectingCellsToType(MathUtil.Pair<Vector2, Vector2> segment, string type)
		{
			List<Vector2I> line = global::Generated.Util.GetLine(segment.First, segment.Second);
			for (int i = 0; i < WorldGen.data.terrainCells.Count; i++)
			{
				if (WorldGen.data.terrainCells[i].node.type != type)
				{
					for (int j = 0; j < line.Count; j++)
					{
						if (WorldGen.data.terrainCells[i].poly.Contains(line[j]))
						{
							WorldGen.data.terrainCells[i].node.type = type;
						}
					}
				}
			}
		}

		public static string GetSubWorldType(Vector2I pos)
		{
			for (int i = 0; i < WorldGen.data.overworldCells.Count; i++)
			{
				if (WorldGen.data.overworldCells[i].poly.Contains(pos))
				{
					return WorldGen.data.overworldCells[i].node.type;
				}
			}
			return null;
		}

		private static List<Polygon> GetOverworldPolygons()
		{
			List<Polygon> list = new List<Polygon>();
			for (int i = 0; i < WorldGen.data.overworldCells.Count; i++)
			{
				list.Add(WorldGen.data.overworldCells[i].poly);
			}
			return list;
		}

		private static List<Border> GetBorders(List<TerrainCell> cells)
		{
			List<Border> list = new List<Border>();
			HashSet<TerrainCell> hashSet = new HashSet<TerrainCell>();
			for (int i = 0; i < cells.Count; i++)
			{
				TerrainCell terrainCell = cells[i];
				hashSet.Add(terrainCell);
				HashSet<KeyValuePair<uint, int>>.Enumerator enumerator = terrainCell.site.neighbours.GetEnumerator();
				int num = 0;
				while (enumerator.MoveNext())
				{
					KeyValuePair<uint, int> neighborId = enumerator.Current;
					TerrainCell terrainCell2 = cells.Find((TerrainCell n) => n.site.id == neighborId.Key);
					if (terrainCell2 == null || hashSet.Contains(terrainCell2))
					{
					}
					num++;
				}
			}
			return list;
		}

		public static List<VoronoiNode> GetNodesForStartAreas()
		{
			List<VoronoiNode> list = new List<VoronoiNode>();
			VoronoiTree voronoiTree = WorldGen.data.worldLayout.GetVoronoiTree();
			if (voronoiTree != null)
			{
				voronoiTree.GetNodesWithTag(WorldGenTags.StartLocation, list);
			}
			return list;
		}

		private static void ProcessByTerrainCell(Sim.Cell[] map_cells, float[] bgTemp, WorldGen.OfflineCallbackFunction updateProgressFn)
		{
			updateProgressFn(UI.WORLDGEN.PROCESSING.key, 0f, WorldGenProgressStages.Stages.Processing);
			try
			{
				for (int i = 0; i < WorldGen.data.terrainCells.Count; i++)
				{
					updateProgressFn(UI.WORLDGEN.PROCESSING.key, 100f * ((float)i / (float)WorldGen.data.terrainCells.Count), WorldGenProgressStages.Stages.Processing);
					WorldGen.data.terrainCells[i].Process(map_cells, bgTemp, WorldGen.data.world);
				}
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				string stackTrace = ex.StackTrace;
				updateProgressFn(new StringKey("Exception in TerrainCell.Process"), -1f, WorldGenProgressStages.Stages.Failure);
				Debug.LogError("Error:" + message + "\n" + stackTrace);
			}
			List<WeightedSimHash> list = new List<WeightedSimHash>
			{
				new WeightedSimHash(SimHashes.Granite, 10f),
				new WeightedSimHash(SimHashes.IgneousRock, 4f),
				new WeightedSimHash(SimHashes.Obsidian, 5f)
			};
			List<WeightedSimHash> list2 = new List<WeightedSimHash>
			{
				new WeightedSimHash(SimHashes.Katairite, 10f)
			};
			List<WeightedSimHash> list3 = new List<WeightedSimHash>
			{
				new WeightedSimHash(SimHashes.Unobtanium, 1f)
			};
			List<Border> list4 = new List<Border>();
			updateProgressFn(UI.WORLDGEN.BORDERS.key, 0f, WorldGenProgressStages.Stages.Borders);
			try
			{
				List<Edge> edgesWithTag = WorldGen.data.worldLayout.overworldGraph.GetEdgesWithTag(WorldGenTags.EdgeUnpassable);
				for (int j = 0; j < edgesWithTag.Count; j++)
				{
					Edge edge2 = edgesWithTag[j];
					if (edge2.site0 != edge2.site1)
					{
						TerrainCell terrainCell = WorldGen.data.overworldCells.Find((TerrainCell c) => c.node.node == edge2.site0.node);
						TerrainCell terrainCell2 = WorldGen.data.overworldCells.Find((TerrainCell c) => c.node.node == edge2.site1.node);
						Debug.Assert(terrainCell != null && terrainCell2 != null, "NULL Terraincell nodes with EdgeUnpassable");
						list4.Add(new Border(new Neighbors(terrainCell, terrainCell2), edge2.corner0.position, edge2.corner1.position)
						{
							element = list3,
							width = WorldGen.RandomRange(2f, 3f)
						});
					}
				}
				List<Edge> edgesWithTag2 = WorldGen.data.worldLayout.overworldGraph.GetEdgesWithTag(WorldGenTags.EdgeClosed);
				for (int k = 0; k < edgesWithTag2.Count; k++)
				{
					Edge edge = edgesWithTag2[k];
					if (edge.site0 != edge.site1)
					{
						if (!edgesWithTag.Contains(edge))
						{
							TerrainCell terrainCell3 = WorldGen.data.overworldCells.Find((TerrainCell c) => c.node.node == edge.site0.node);
							TerrainCell terrainCell4 = WorldGen.data.overworldCells.Find((TerrainCell c) => c.node.node == edge.site1.node);
							Debug.Assert(terrainCell3 != null && terrainCell4 != null, "NULL Terraincell nodes with EdgeClosed");
							Border border = new Border(new Neighbors(terrainCell3, terrainCell4), edge.corner0.position, edge.corner1.position);
							border.element = list2;
							if (edge.tags.Contains(WorldGenTags.RoomBorderMixed))
							{
								border.element = list;
							}
							border.width = WorldGen.RandomRange(2f, 3f);
							list4.Add(border);
						}
					}
				}
			}
			catch (Exception ex2)
			{
				string message2 = ex2.Message;
				string stackTrace2 = ex2.StackTrace;
				updateProgressFn(new StringKey("Exception in Border creation"), -1f, WorldGenProgressStages.Stages.Failure);
				Debug.LogError("Error:" + message2 + " " + stackTrace2);
			}
			try
			{
				if (WorldGen.data.world.defaultTemp == null)
				{
					WorldGen.data.world.defaultTemp = new float[WorldGen.data.world.density.Length];
				}
				for (int l = 0; l < WorldGen.data.world.defaultTemp.Length; l++)
				{
					WorldGen.data.world.defaultTemp[l] = bgTemp[l];
				}
			}
			catch (Exception ex3)
			{
				string message3 = ex3.Message;
				string stackTrace3 = ex3.StackTrace;
				updateProgressFn(new StringKey("Exception in border.defaultTemp"), -1f, WorldGenProgressStages.Stages.Failure);
				Debug.LogError("Error:" + message3 + " " + stackTrace3);
			}
			try
			{
				TerrainCell.SetValuesFunction setValuesFunction = delegate(int index, Element elem, Sim.PhysicsData pd)
				{
					if (Grid.IsValidCell(index))
					{
						if (elem.HasTag(GameTags.Special))
						{
							pd = elem.defaultValues;
						}
						map_cells[index].SetValues(elem, pd, ElementLoader.elements);
					}
					else
					{
						Debug.LogError(string.Concat(new object[] { "Process::SetValuesFunction Index [", index, "] is not valid. cells.Length [", map_cells.Length, "]" }));
					}
				};
				for (int m = 0; m < list4.Count; m++)
				{
					Border border2 = list4[m];
					SubWorld subWorld = WorldGen.Settings.subworlds.GetSubWorld(border2.neighbors.n0.node.type);
					SubWorld subWorld2 = WorldGen.Settings.subworlds.GetSubWorld(border2.neighbors.n1.node.type);
					float num = Mathf.Min(WorldGen.Settings.temperatures.ranges[subWorld.temperatureRange].min, WorldGen.Settings.temperatures.ranges[subWorld2.temperatureRange].min);
					float num2 = Mathf.Max(WorldGen.Settings.temperatures.ranges[subWorld.temperatureRange].max, WorldGen.Settings.temperatures.ranges[subWorld2.temperatureRange].max);
					float num3 = num2 - num;
					border2.Stagger(WorldGen.RandomRange(8f, 13f), WorldGen.RandomRange(2f, 5f));
					border2.ConvertToMap(WorldGen.data.world, setValuesFunction, num, num3);
				}
			}
			catch (Exception ex4)
			{
				string message4 = ex4.Message;
				string stackTrace4 = ex4.StackTrace;
				updateProgressFn(new StringKey("Exception in border.ConvertToMap"), -1f, WorldGenProgressStages.Stages.Failure);
				Debug.LogError("Error:" + message4 + " " + stackTrace4);
			}
		}

		private static void DrawBorder(Chunk chunk, int thickness, int range)
		{
			if (Mathf.Abs(chunk.offset.x % WorldGen.SubWorldSize.x) == 0)
			{
				int num = 0;
				for (int i = WorldGen.ChunkEdgeSize - 1; i >= 0; i--)
				{
					num = (int)Mathf.Max((float)(-(float)range), Mathf.Min((float)num + WorldGen.RandomRange(-2f, 2f), (float)range));
					for (int j = 0; j < thickness + num; j++)
					{
						chunk.overrides[j + WorldGen.ChunkEdgeSize * i] = 100f;
					}
				}
			}
			if (Mathf.Abs((chunk.offset.x + WorldGen.ChunkEdgeSize) % WorldGen.SubWorldSize.x) == 0)
			{
				int num2 = 0;
				for (int k = WorldGen.ChunkEdgeSize - 1; k >= 0; k--)
				{
					num2 = (int)Mathf.Max((float)(-(float)range), Mathf.Min((float)num2 + WorldGen.RandomRange(-2f, 2f), (float)range));
					for (int l = 0; l < thickness + num2; l++)
					{
						chunk.overrides[WorldGen.ChunkEdgeSize - 1 - l + WorldGen.ChunkEdgeSize * k] = 100f;
					}
				}
			}
			if (Mathf.Abs(chunk.offset.y % WorldGen.SubWorldSize.y) == 0)
			{
				int num3 = 0;
				for (int m = 0; m < WorldGen.ChunkEdgeSize; m++)
				{
					num3 = (int)Mathf.Max((float)(-(float)range), Mathf.Min((float)num3 + WorldGen.RandomRange(-2f, 2f), (float)range));
					for (int n = 0; n < thickness + num3; n++)
					{
						chunk.overrides[m + WorldGen.ChunkEdgeSize * n] = 100f;
					}
				}
			}
			if (Mathf.Abs((chunk.offset.y + WorldGen.ChunkEdgeSize) % WorldGen.SubWorldSize.y) == 0)
			{
				int num4 = 0;
				for (int num5 = 0; num5 < WorldGen.ChunkEdgeSize; num5++)
				{
					num4 = (int)Mathf.Max((float)(-(float)range), Mathf.Min((float)num4 + WorldGen.RandomRange(-2f, 2f), (float)range));
					for (int num6 = 0; num6 < thickness + num4; num6++)
					{
						chunk.overrides[num5 + WorldGen.ChunkEdgeSize * (WorldGen.ChunkEdgeSize - 1 - num6)] = 100f;
					}
				}
			}
		}

		private static void DrawWorldBorder(Sim.Cell[] cells, Chunk world)
		{
			int num = int.Parse(WorldGen.settings.defaults.data["WorldBorderThickness"] as string);
			int num2 = int.Parse(WorldGen.settings.defaults.data["WorldBorderRange"] as string);
			int num3 = 0;
			int num4 = 0;
			for (int i = world.size.y - 1; i >= 0; i--)
			{
				num3 = (int)Mathf.Max((float)(-(float)num2), Mathf.Min((float)num3 + WorldGen.RandomRange(-2f, 2f), (float)num2));
				for (int j = 0; j < num + num3; j++)
				{
					cells[Grid.XYToCell(j, i)].SetValues(WorldGen.unobtaniumElement, ElementLoader.elements);
				}
				num4 = (int)Mathf.Max((float)(-(float)num2), Mathf.Min((float)num4 + WorldGen.RandomRange(-2f, 2f), (float)num2));
				for (int k = 0; k < num + num4; k++)
				{
					cells[Grid.XYToCell(world.size.x - 1 - k, i)].SetValues(WorldGen.unobtaniumElement, ElementLoader.elements);
				}
			}
			int num5 = 0;
			int num6 = 0;
			bool flag = bool.Parse(WorldGen.settings.defaults.data["DrawWorldBorderTop"] as string);
			for (int l = 0; l < world.size.x; l++)
			{
				num5 = (int)Mathf.Max((float)(-(float)num2), Mathf.Min((float)num5 + WorldGen.RandomRange(-2f, 2f), (float)num2));
				for (int m = 0; m < num + num5; m++)
				{
					cells[Grid.XYToCell(l, m)].SetValues(WorldGen.unobtaniumElement, ElementLoader.elements);
				}
				num6 = (int)Mathf.Max((float)(-(float)num2), Mathf.Min((float)num6 + WorldGen.RandomRange(-2f, 2f), (float)num2));
				for (int n = 0; n < num + num6; n++)
				{
					cells[Grid.XYToCell(l, world.size.y - 1 - n)].SetValues((!flag) ? WorldGen.voidElement : WorldGen.unobtaniumElement, ElementLoader.elements);
				}
			}
		}

		private static void SetupNoise(WorldGen.OfflineCallbackFunction updateProgressFn)
		{
			WorldGen.stats.MinDataValue = float.MaxValue;
			WorldGen.stats.MaxDataValue = float.MinValue;
			WorldGen.stats.MinHeatValue = float.MaxValue;
			WorldGen.stats.MaxHeatValue = float.MinValue;
			updateProgressFn(UI.WORLDGEN.BUILDNOISESOURCE.key, 0f, WorldGenProgressStages.Stages.SetupNoise);
			WorldGen.heatSource = WorldGen.BuildNoiseSource(WorldGen.data.world.size.x, WorldGen.data.world.size.y, "Heat");
			updateProgressFn(UI.WORLDGEN.BUILDNOISESOURCE.key, 100f, WorldGenProgressStages.Stages.SetupNoise);
		}

		public static NoiseMapBuilderPlane BuildNoiseSource(int width, int height, string name)
		{
			Klei.Noise.Tree tree = WorldGen.settings.noise.GetTree(name);
			return WorldGen.BuildNoiseSource(width, height, tree);
		}

		public static NoiseMapBuilderPlane BuildNoiseSource(int width, int height, Klei.Noise.Tree tree)
		{
			Vector2f lowerBound = tree.settings.lowerBound;
			Vector2f upperBound = tree.settings.upperBound;
			Debug.Assert(lowerBound.x < upperBound.x, string.Concat(new object[] { "BuildNoiseSource X range broken [l: ", lowerBound.x, " h: ", upperBound.x, "]" }));
			Debug.Assert(lowerBound.y < upperBound.y, string.Concat(new object[] { "BuildNoiseSource Y range broken [l: ", lowerBound.y, " h: ", upperBound.y, "]" }));
			Debug.Assert(width > 0, "BuildNoiseSource width <=0: [" + width + "]");
			Debug.Assert(height > 0, "BuildNoiseSource height <=0: [" + height + "]");
			NoiseMapBuilderPlane noiseMapBuilderPlane = new NoiseMapBuilderPlane(lowerBound.x, upperBound.x, lowerBound.y, upperBound.y, false);
			noiseMapBuilderPlane.SetSize(width, height);
			noiseMapBuilderPlane.SourceModule = tree.BuildFinalModule();
			return noiseMapBuilderPlane;
		}

		private static void GetMinMaxDataValues(float[] data, int width, int height)
		{
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					WorldGen.stats.MinDataValue = Mathf.Min(data[i + j * width], WorldGen.stats.MinDataValue);
					WorldGen.stats.MaxDataValue = Mathf.Max(data[i + j * width], WorldGen.stats.MaxDataValue);
				}
			}
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
			Debug.Assert(data != null && data.Length > 0, "MISSING DATA FOR NORMALIZE");
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

		private static void GenerateUnChunkedNoise(WorldGen.OfflineCallbackFunction updateProgressFn)
		{
			Vector2 vector = new Vector2(0f, 0f);
			updateProgressFn(UI.WORLDGEN.GENERATENOISE.key, 0f, WorldGenProgressStages.Stages.GenerateNoise);
			NoiseMapBuilderCallback noiseMapBuilderCallback = delegate(int line)
			{
				updateProgressFn(UI.WORLDGEN.GENERATENOISE.key, (float)((int)(0.0 + 25.0 * (double)((float)line / (float)WorldGen.data.world.size.y))), WorldGenProgressStages.Stages.GenerateNoise);
			};
			noiseMapBuilderCallback = delegate(int line)
			{
				updateProgressFn(UI.WORLDGEN.GENERATENOISE.key, (float)((int)(25.0 + 25.0 * (double)((float)line / (float)WorldGen.data.world.size.y))), WorldGenProgressStages.Stages.GenerateNoise);
			};
			if (noiseMapBuilderCallback == null)
			{
				Debug.LogError("nupd is null");
			}
			WorldGen.data.world.heatOffset = WorldGen.GenerateNoise(vector, WorldGen.settings.noise.GetZoomForTree("Heat"), WorldGen.heatSource, WorldGen.data.world.size.x, WorldGen.data.world.size.y, noiseMapBuilderCallback);
			WorldGen.data.world.data = new float[WorldGen.data.world.heatOffset.Length];
			WorldGen.data.world.density = new float[WorldGen.data.world.heatOffset.Length];
			WorldGen.data.world.overrides = new float[WorldGen.data.world.heatOffset.Length];
			updateProgressFn(UI.WORLDGEN.NORMALISENOISE.key, 50f, WorldGenProgressStages.Stages.GenerateNoise);
			if (WorldGen.settings.noise.ShouldNormaliseTree("Heat"))
			{
				WorldGen.Normalise(WorldGen.data.world.heatOffset);
			}
			updateProgressFn(UI.WORLDGEN.NORMALISENOISE.key, 100f, WorldGenProgressStages.Stages.GenerateNoise);
		}

		public static void WriteOverWorldNoise(WorldGen.OfflineCallbackFunction updateProgressFn)
		{
			float num = (float)WorldGen.OverworldCells.Count;
			float perCell = 100f / num;
			float currentProgress = 0f;
			foreach (TerrainCell terrainCell in WorldGen.OverworldCells)
			{
				string text = "Default";
				string text2 = "DefaultCave";
				string text3 = "DefaultDensity";
				Klei.Noise.Tree tree = WorldGen.Settings.noise.GetTree(text);
				Klei.Noise.Tree tree2 = WorldGen.Settings.noise.GetTree(text2);
				Klei.Noise.Tree tree3 = WorldGen.Settings.noise.GetTree(text3);
				SubWorld subWorld = WorldGen.Settings.subworlds.GetSubWorld(terrainCell.node.type);
				if (subWorld == null)
				{
					Debug.Log("Couldnt find Subworld for overworld node [" + terrainCell.node.type + "] using defaults");
				}
				else
				{
					if (subWorld.biomeNoise != null)
					{
						Klei.Noise.Tree tree4 = WorldGen.Settings.noise.GetTree(subWorld.biomeNoise);
						if (tree4 != null)
						{
							tree = tree4;
						}
					}
					if (subWorld.overrideNoise != null)
					{
						Klei.Noise.Tree tree5 = WorldGen.Settings.noise.GetTree(subWorld.overrideNoise);
						if (tree5 != null)
						{
							tree2 = tree5;
						}
					}
					if (subWorld.densityNoise != null)
					{
						Klei.Noise.Tree tree6 = WorldGen.Settings.noise.GetTree(subWorld.densityNoise);
						if (tree6 != null)
						{
							tree3 = tree6;
						}
					}
				}
				int num2 = (int)Mathf.Ceil(terrainCell.poly.bounds.width + 1f);
				int height = (int)Mathf.Ceil(terrainCell.poly.bounds.height + 1f);
				int num3 = (int)Mathf.Floor(terrainCell.poly.bounds.xMin - 1f);
				int num4 = (int)Mathf.Floor(terrainCell.poly.bounds.yMin - 1f);
				Vector2 vector = new Vector2((float)num3, (float)num4);
				Vector2 vector2 = vector;
				NoiseMapBuilderCallback noiseMapBuilderCallback = delegate(int line)
				{
					updateProgressFn(UI.WORLDGEN.GENERATENOISE.key, (float)((int)(currentProgress + perCell * ((float)line / (float)height))), WorldGenProgressStages.Stages.NoiseMapBuilder);
				};
				NoiseMapBuilderPlane noiseMapBuilderPlane = WorldGen.BuildNoiseSource(num2, height, tree);
				NoiseMap noiseMap = WorldGen.BuildNoiseMap(vector, tree.settings.zoom, noiseMapBuilderPlane, num2, height, noiseMapBuilderCallback);
				NoiseMapBuilderPlane noiseMapBuilderPlane2 = WorldGen.BuildNoiseSource(num2, height, tree2);
				NoiseMap noiseMap2 = WorldGen.BuildNoiseMap(vector, tree2.settings.zoom, noiseMapBuilderPlane2, num2, height, noiseMapBuilderCallback);
				NoiseMapBuilderPlane noiseMapBuilderPlane3 = WorldGen.BuildNoiseSource(num2, height, tree3);
				NoiseMap noiseMap3 = WorldGen.BuildNoiseMap(vector, tree3.settings.zoom, noiseMapBuilderPlane3, num2, height, noiseMapBuilderCallback);
				float num5 = float.MaxValue;
				float num6 = float.MinValue;
				float num7 = float.MaxValue;
				float num8 = float.MinValue;
				float num9 = float.MaxValue;
				float num10 = float.MinValue;
				List<int> list = new List<int>();
				vector2.x = (float)((int)Mathf.Floor(terrainCell.poly.bounds.xMin));
				while (vector2.x <= (float)((int)Mathf.Ceil(terrainCell.poly.bounds.xMax)))
				{
					vector2.y = (float)((int)Mathf.Floor(terrainCell.poly.bounds.yMin));
					while (vector2.y <= (float)((int)Mathf.Ceil(terrainCell.poly.bounds.yMax)))
					{
						if (terrainCell.poly.PointInPolygon(vector2))
						{
							int num11 = Grid.XYToCell((int)vector2.x, (int)vector2.y);
							list.Add(num11);
							int num12 = (int)vector2.x - num3;
							int num13 = (int)vector2.y - num4;
							WorldGen.BaseNoiseMap[num11] = noiseMap.GetValue(num12, num13);
							WorldGen.OverrideMap[num11] = noiseMap2.GetValue(num12, num13);
							WorldGen.DensityMap[num11] = noiseMap3.GetValue(num12, num13);
							num5 = Mathf.Min(WorldGen.BaseNoiseMap[num11], num5);
							num6 = Mathf.Max(WorldGen.BaseNoiseMap[num11], num6);
							num7 = Mathf.Min(WorldGen.OverrideMap[num11], num7);
							num8 = Mathf.Max(WorldGen.OverrideMap[num11], num8);
							num9 = Mathf.Min(WorldGen.DensityMap[num11], num9);
							num10 = Mathf.Max(WorldGen.DensityMap[num11], num10);
						}
						vector2.y += 1f;
					}
					vector2.x += 1f;
				}
				if (tree.settings.normalise)
				{
					float num14 = num6 - num5;
					for (int i = 0; i < list.Count; i++)
					{
						WorldGen.BaseNoiseMap[list[i]] = (WorldGen.BaseNoiseMap[list[i]] - num5) / num14;
					}
				}
				if (tree2.settings.normalise)
				{
					float num15 = num8 - num7;
					for (int j = 0; j < list.Count; j++)
					{
						WorldGen.OverrideMap[list[j]] = (WorldGen.OverrideMap[list[j]] - num7) / num15;
					}
				}
				if (tree3.settings.normalise)
				{
					float num16 = num10 - num9;
					for (int k = 0; k < list.Count; k++)
					{
						WorldGen.DensityMap[list[k]] = (WorldGen.DensityMap[list[k]] - num9) / num16;
					}
				}
				currentProgress += perCell;
			}
		}

		private static float GetValue(Chunk chunk, Vector2I pos, out bool noDiamond)
		{
			noDiamond = false;
			int num = pos.x + WorldGen.data.world.size.x * pos.y;
			if (num < 0 || num >= chunk.data.Length)
			{
				throw new ArgumentOutOfRangeException("chunkDataIndex [" + num + "]", "chunk data length [" + chunk.data.Length + "]");
			}
			float num2 = chunk.data[num];
			WorldGen.stats.MinDataValue = Mathf.Min(num2, WorldGen.stats.MinDataValue);
			WorldGen.stats.MaxDataValue = Mathf.Max(num2, WorldGen.stats.MaxDataValue);
			float num3 = chunk.heatOffset[num];
			WorldGen.stats.MinHeatValue = Mathf.Min(num3, WorldGen.stats.MinHeatValue);
			WorldGen.stats.MaxHeatValue = Mathf.Max(num3, WorldGen.stats.MaxHeatValue);
			float num4 = (float)(pos.y + chunk.offset.y);
			float num5 = (float)WorldGen.data.world.size.y * 0.8f;
			if (num4 > num5)
			{
				num2 *= ((float)WorldGen.data.world.size.y - num4) / ((float)WorldGen.data.world.size.y - num5);
				noDiamond = true;
			}
			return num2;
		}

		public static bool InChunkRange(Chunk chunk, Vector2I pos)
		{
			int num = pos.x + WorldGen.data.world.size.x * pos.y;
			return num >= 0 && num < chunk.data.Length;
		}

		private static Element GetElementFromBiomeElementTable(Chunk chunk, Vector2I pos, List<ElementGradient> table, float erode)
		{
			bool flag;
			float num = WorldGen.GetValue(chunk, pos, out flag) * erode;
			Element element = WorldGen.voidElement;
			if (table.Count == 0)
			{
				return element;
			}
			for (int i = 0; i < table.Count; i++)
			{
				if (num < table[i].maxValue)
				{
					return ElementLoader.FindElementByHash(table[i].content);
				}
			}
			return ElementLoader.FindElementByHash(table[table.Count - 1].content);
		}

		public static bool CanLoad(string fileName)
		{
			if (fileName == null || fileName == string.Empty)
			{
				return false;
			}
			bool flag;
			try
			{
				using (BinaryReader binaryReader = new BinaryReader(File.Open(fileName, FileMode.Open)))
				{
					flag = binaryReader.BaseStream.CanRead;
				}
			}
			catch (FileNotFoundException)
			{
				Output.LogWarning(new object[] { "File [" + fileName + "] not found" });
				flag = false;
			}
			catch (Exception ex)
			{
				Output.LogWarning(new object[] { "Failed to read " + fileName + "\n" + ex.ToString() });
				flag = false;
			}
			return flag;
		}

		private static void SaveSim()
		{
			try
			{
				Manager.Clear();
				SimSaveFileStructure simSaveFileStructure = new SimSaveFileStructure();
				for (int i = 0; i < WorldGen.data.overworldCells.Count; i++)
				{
					simSaveFileStructure.worldDetail.overworldCells.Add(new WorldDetailSave.OverworldCell(WorldGen.data.overworldCells[i]));
				}
				simSaveFileStructure.WidthInCells = Grid.WidthInCells;
				simSaveFileStructure.HeightInCells = Grid.HeightInCells;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
					{
						Sim.Save(binaryWriter);
					}
					simSaveFileStructure.Sim = memoryStream.ToArray();
				}
				using (MemoryStream memoryStream2 = new MemoryStream())
				{
					using (BinaryWriter binaryWriter2 = new BinaryWriter(memoryStream2))
					{
						try
						{
							Serializer.Serialize(simSaveFileStructure, binaryWriter2);
						}
						catch (Exception ex)
						{
							Output.LogError(new object[] { "Couldn't serialize", ex.Message, ex.StackTrace });
						}
					}
					using (BinaryWriter binaryWriter3 = new BinaryWriter(File.Open(WorldGen.SIM_SAVE_FILENAME, FileMode.Create)))
					{
						Manager.SerializeDirectory(binaryWriter3);
						binaryWriter3.Write(memoryStream2.ToArray());
					}
				}
			}
			catch (Exception ex2)
			{
				Output.LogError(new object[] { "Couldn't write", ex2.Message, ex2.StackTrace });
				return;
			}
			WorldGen.SaveWorldGen();
		}

		public static void SaveWorldGen()
		{
			try
			{
				Manager.Clear();
				WorldGenSave worldGenSave = new WorldGenSave();
				worldGenSave.version = new Vector2I(1, 0);
				worldGenSave.stats = WorldGen.stats;
				worldGenSave.data = WorldGen.data;
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
							Output.LogError(new object[] { "Couldn't serialize", ex.Message, ex.StackTrace });
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
				Output.LogError(new object[] { "Couldn't write", ex2.Message, ex2.StackTrace });
			}
		}

		public static bool LoadWorldGen()
		{
			try
			{
				Manager.assemblies = new Assembly[]
				{
					typeof(WorldGen).Assembly,
					typeof(Polygon).Assembly,
					typeof(Vector2).Assembly
				};
				WorldGenSave worldGenSave = new WorldGenSave();
				FastReader fastReader = new FastReader(File.ReadAllBytes(WorldGen.WORLDGEN_SAVE_FILENAME));
				Manager.DeserializeDirectory(fastReader);
				Deserializer.Deserialize(worldGenSave, fastReader);
				WorldGen.stats = worldGenSave.stats;
				WorldGen.data = worldGenSave.data;
				if (worldGenSave.version.x != 1 || worldGenSave.version.y > 0)
				{
					Output.LogError(new object[] { string.Concat(new object[]
					{
						"LoadWorldGenSim Error! Wrong save version Current: [",
						1,
						".",
						0,
						"] File: [",
						worldGenSave.version.x,
						".",
						worldGenSave.version.y,
						"]"
					}) });
					WorldGen.wasLoaded = false;
				}
				else
				{
					WorldGen.wasLoaded = true;
				}
			}
			catch (Exception ex)
			{
				Output.LogError(new object[] { "LoadWorldGenSim Error!\n", ex.Message, ex.StackTrace });
				WorldGen.wasLoaded = false;
			}
			return WorldGen.wasLoaded;
		}

		public static SimSaveFileStructure LoadWorldGenSim()
		{
			WorldGen.LoadWorldGen();
			SimSaveFileStructure simSaveFileStructure = new SimSaveFileStructure();
			try
			{
				Manager.assemblies = new Assembly[]
				{
					typeof(WorldGen).Assembly,
					typeof(Polygon).Assembly,
					typeof(Vector2).Assembly
				};
				FastReader fastReader = new FastReader(File.ReadAllBytes(WorldGen.SIM_SAVE_FILENAME));
				Manager.DeserializeDirectory(fastReader);
				Deserializer.Deserialize(simSaveFileStructure, fastReader);
			}
			catch (Exception ex)
			{
				Output.LogError(new object[] { "LoadWorldGenSim Error!\n", ex.Message, ex.StackTrace });
				WorldGen.wasLoaded = false;
				return null;
			}
			if (simSaveFileStructure.worldDetail == null)
			{
				Debug.LogError("Detail is null");
			}
			else
			{
				SaveLoader.Instance.SetWorldDetail(simSaveFileStructure.worldDetail);
			}
			if (!WorldGen.LoadWorldGen())
			{
				return null;
			}
			return simSaveFileStructure;
		}

		public static void DrawDebug()
		{
		}

		private const string _SIM_SAVE_FILENAME = "WorldGenSimSave.dat";

		private const string _WORLDGEN_SAVE_FILENAME = "WorldGenDataSave.dat";

		private const int heatScale = 2;

		public const int WORLD_OFFSET_Y = 0;

		private const int UNPASSABLE_EDGE_COUNT = 4;

		public const int WORLDGEN_SAVE_MAJOR_VERSION = 1;

		public const int WORLDGEN_SAVE_MINOR_VERSION = 0;

		private static WorldGenStats stats;

		private static Data data;

		public static Dictionary<TerrainCell, List<HashSet<int>>> NaturalCavities = new Dictionary<TerrainCell, List<HashSet<int>>>();

		public static HashSet<int> allNaturalCavityCells = new HashSet<int>();

		private static global::System.Random worldGenRandom = null;

		private static WorldGen.OfflineCallbackFunction successCallbackFn;

		private static bool running = true;

		private static Thread generateThread;

		private static Thread renderThread;

		public static string STAGE_COMPLETE = "Complete";

		public static string CATASTROPHIC_FAIL = "FAILED";

		public static Element voidElement;

		public static Element vacuumElement;

		public static Element katairiteElement;

		public static Element unobtaniumElement;

		private static WorldGenSettings settings;

		private static bool wasLoaded = false;

		private static NoiseMapBuilderPlane heatSource = null;

		public static int polyIndex = 0;

		[EnumFlags]
		public static WorldGen.DebugFlags drawOptions;

		[SerializationConfig(MemberSerialization.OptOut)]
		public class GameSpawnData
		{
			public List<KeyValuePair<int, Tag>> GetDrops()
			{
				return this.drops;
			}

			public void Add(int cell, Tag tag)
			{
				if (this.drops == null)
				{
					this.drops = new List<KeyValuePair<int, Tag>>();
				}
				this.drops.Add(new KeyValuePair<int, Tag>(cell, tag));
			}

			public void AddRange(List<KeyValuePair<int, Tag>> newItems)
			{
				if (this.drops == null)
				{
					this.drops = new List<KeyValuePair<int, Tag>>();
				}
				this.drops.AddRange(newItems);
			}

			public Vector2I baseStartPos;

			public List<Cloud> clouds;

			public List<KeyValuePair<int, Tag>> drops = new List<KeyValuePair<int, Tag>>();
		}

		public enum GenerateSection
		{
			SolarSystem,
			WorldNoise,
			WorldLayout,
			RenderToMap,
			CollectSpawners
		}

		[Flags]
		public enum DebugFlags
		{
			Site = 1,
			Centroid = 2,
			SitePoly = 4
		}

		public delegate void ResetFunction(WorldGen.GameSpawnData gsd);

		public delegate bool OfflineCallbackFunction(StringKey stringKeyRoot, float completePercent, WorldGenProgressStages.Stages stage);
	}
}
