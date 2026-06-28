using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
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

		public Dictionary<string, object> Stats
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

		public static List<River> Rivers
		{
			get
			{
				return WorldGen.data.rivers;
			}
		}

		public static GameSpawnData SpawnData
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

		public static string GetPath()
		{
			if (WorldGen.PATH == null)
			{
				WorldGen.PATH = global::System.IO.Path.Combine(Application.streamingAssetsPath, "worldgen/");
			}
			return WorldGen.PATH;
		}

		public static void LoadSettings()
		{
			WorldGen.isRunningDebugGen = false;
			WorldGen.settings = WorldGenSettings.LoadFile(WorldGen.GetPath());
			if (WorldGen.settings == null)
			{
				return;
			}
			if (WorldGen.wasLoaded)
			{
				global::Debug.Log("Worldgen loaded, dont need to do anything else...", null);
				return;
			}
			WorldGen.data = new Data();
			WorldGen.data.chunkEdgeSize = WorldGen.settings.GetDefaultInt("ChunkEdgeSize");
			WorldGen.data.subWorldSize = new Vector2I(WorldGen.settings.GetDefaultInt("SubWorldWidth"), WorldGen.settings.GetDefaultInt("SubWorldHeight"));
			TemplateCache.Init();
			WorldGen.stats = new Dictionary<string, object>();
		}

		public static void SaveSettings(string newpath = null)
		{
			WorldGen.settings.Save((newpath != null) ? newpath : WorldGen.GetPath());
		}

		public void InitRandom(int worldSeed, int layoutSeed, int terrainSeed, int noiseSeed)
		{
			WorldGen.data.globalWorldSeed = worldSeed;
			WorldGen.data.globalWorldLayoutSeed = layoutSeed;
			WorldGen.data.globalTerrainSeed = terrainSeed;
			WorldGen.data.globalNoiseSeed = noiseSeed;
			Console.WriteLine(string.Format("Seeds are [{0}/{1}/{2}/{3}]", new object[] { worldSeed, layoutSeed, terrainSeed, noiseSeed }));
			this.myRandom = new SeededRandom(worldSeed);
		}

		public void Initialise(WorldGen.OfflineCallbackFunction callbackFn, Action<OfflineWorldGen.ErrorInfo> error_cb, int worldSeed = -1, int layoutSeed = -1, int terrainSeed = -1, int noiseSeed = -1)
		{
			if (WorldGen.wasLoaded)
			{
				global::Debug.LogError("Initialise called after load", null);
				return;
			}
			WorldGen.successCallbackFn = callbackFn;
			this.errorCallback = error_cb;
			WorldGen.running = false;
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
			Output.Log(new object[] { string.Format("World seeds: [{0}/{1}/{2}/{3}]", new object[] { worldSeed, layoutSeed, terrainSeed, noiseSeed }) });
			this.InitRandom(worldSeed, layoutSeed, terrainSeed, noiseSeed);
			WorldGen.data.gameSpawnData = new GameSpawnData();
			TerrainCell.ClearClaimedCells();
			WorldGen.successCallbackFn(UI.WORLDGEN.COMPLETE.key, 0f, WorldGenProgressStages.Stages.Failure);
			WorldGen.stats["GenerateTime"] = 0;
			WorldGen.stats["GenerateNoiseTime"] = 0;
			WorldGen.stats["GenerateLayoutTime"] = 0;
			WorldGen.stats["ConvertVoroToMapTime"] = 0;
			WorldLayout.SetLayerGradient(WorldGen.settings.layers.LevelLayers);
		}

		public void GenerateOfflineThreaded()
		{
			if (WorldGen.wasLoaded)
			{
				global::Debug.LogError("GenerateOfflineThreaded called after load", null);
				return;
			}
			if (WorldGen.Settings.GetWorld() == null)
			{
				return;
			}
			WorldGen.running = true;
			WorldGen.generateThread = new Thread(new ThreadStart(this.GenerateOffline));
			WorldGen.generateThread.Start();
		}

		public void RenderWorldThreaded()
		{
			if (WorldGen.wasLoaded)
			{
				global::Debug.LogError("RenderWorldThreaded called after load", null);
				return;
			}
			WorldGen.running = true;
			WorldGen.renderThread = new Thread(new ThreadStart(this.RenderOfflineThreadFn));
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

		public bool IsGenerateComplete()
		{
			return WorldGen.generateThread != null && !WorldGen.generateThread.IsAlive;
		}

		public bool IsRenderComplete()
		{
			return WorldGen.renderThread != null && !WorldGen.renderThread.IsAlive;
		}

		public void GenerateOffline()
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

		private void PlaceTemplateSpawners(Vector2I position, TemplateContainer template)
		{
			WorldGen.data.gameSpawnData.AddTemplate(template, position);
		}

		public bool IsSafeToSpawnPOI(TerrainCell tc)
		{
			using (List<uint>.Enumerator enumerator = tc.terrain_neighbors_idx.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					uint neighbor = enumerator.Current;
					TerrainCell terrainCell = WorldGen.data.terrainCells.Find((TerrainCell cell) => cell.site.id == neighbor);
					if (terrainCell.node.tags.Contains(WorldGenTags.POI))
					{
						return false;
					}
				}
			}
			return !tc.node.tags.Contains(WorldGenTags.StartLocation) && !tc.node.tags.Contains(WorldGenTags.NearStartLocation) && !tc.node.tags.Contains(WorldGenTags.POI) && !tc.node.tags.Contains(WorldGenTags.AtEdge) && !tc.node.tags.Contains(WorldGenTags.AtDepths) && !tc.node.tags.Contains(WorldGenTags.AtSurface);
		}

		public bool IsSafeToSpawnFeatureTemplate(TerrainCell tc)
		{
			return !tc.node.tags.Contains(WorldGenTags.StartLocation) && !tc.node.tags.Contains(WorldGenTags.NearStartLocation) && !tc.node.tags.Contains(WorldGenTags.POI) && !tc.node.tags.Contains(WorldGenTags.AtEdge) && !tc.node.tags.Contains(WorldGenTags.AtDepths) && !tc.node.tags.Contains(WorldGenTags.AtSurface);
		}

		public KeyValuePair<Vector2I, TemplateContainer> GetPOISpawnTarget(Sim.Cell[] cells, TerrainCell tc, List<TemplateContainer> poi)
		{
			KeyValuePair<Vector2I, TemplateContainer> keyValuePair = new KeyValuePair<Vector2I, TemplateContainer>(new Vector2I(-1, -1), null);
			using (List<uint>.Enumerator enumerator = tc.terrain_neighbors_idx.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					uint neighbor = enumerator.Current;
					TerrainCell terrainCell = WorldGen.data.terrainCells.Find((TerrainCell cell) => cell.site.id == neighbor);
					if (terrainCell.node.tags.Contains(WorldGenTags.POI))
					{
						return keyValuePair;
					}
				}
			}
			if (tc.node.tags.Contains(WorldGenTags.StartLocation) || tc.node.tags.Contains(WorldGenTags.NearStartLocation) || tc.node.tags.Contains(WorldGenTags.POI) || tc.node.tags.Contains(WorldGenTags.AtEdge) || tc.node.tags.Contains(WorldGenTags.AtDepths) || tc.node.tags.Contains(WorldGenTags.AtSurface))
			{
				return keyValuePair;
			}
			for (int i = 0; i < poi.Count; i++)
			{
				bool flag = tc.node.tags.Contains(new Tag(poi[i].name));
				if (flag)
				{
					tc.node.tags.Add(WorldGenTags.POI);
					Vector2I vector2I = new Vector2I((int)tc.poly.Centroid().x, (int)tc.poly.Centroid().y);
					keyValuePair = new KeyValuePair<Vector2I, TemplateContainer>(vector2I, poi[i]);
					poi.RemoveAt(i);
					break;
				}
			}
			return keyValuePair;
		}

		private void RenderOfflineThreadFn()
		{
			Sim.DiseaseCell[] array = null;
			this.RenderOffline(true, ref array);
		}

		public static int GetDiseaseIdx(string disease)
		{
			for (int i = 0; i < WorldGen.diseaseIds.Count; i++)
			{
				if (disease == WorldGen.diseaseIds[i])
				{
					return i;
				}
			}
			return 255;
		}

		public Sim.Cell[] RenderOffline(bool doSettle, ref Sim.DiseaseCell[] dc)
		{
			Sim.Cell[] array = null;
			float[] array2 = null;
			dc = null;
			HashSet<int> hashSet = new HashSet<int>();
			this.CompleteLayout(WorldGen.successCallbackFn);
			WorldGen.WriteOverWorldNoise(WorldGen.successCallbackFn);
			if (!WorldGen.RenderToMap(WorldGen.successCallbackFn, ref array, ref array2, ref dc, ref hashSet))
			{
				WorldGen.successCallbackFn(UI.WORLDGEN.FAILED.key, -100f, WorldGenProgressStages.Stages.Failure);
				return null;
			}
			WorldGen.EnsureEnoughAlgaeInStartingBiome(array);
			List<KeyValuePair<Vector2I, TemplateContainer>> list = new List<KeyValuePair<Vector2I, TemplateContainer>>();
			TemplateContainer baseStartingTemplate = TemplateCache.GetBaseStartingTemplate();
			List<TerrainCell> terrainCellsForTag = WorldGen.GetTerrainCellsForTag(WorldGenTags.StartLocation);
			foreach (TerrainCell terrainCell in terrainCellsForTag)
			{
				list.Add(new KeyValuePair<Vector2I, TemplateContainer>(new Vector2I((int)terrainCell.poly.Centroid().x, (int)terrainCell.poly.Centroid().y), baseStartingTemplate));
			}
			List<TemplateContainer> list2 = TemplateCache.CollectBaseTemplateAssets("poi/");
			foreach (SubWorld subWorld in WorldGen.Settings.GetSubWorldList())
			{
				if (subWorld.pointsOfInterest != null)
				{
					foreach (KeyValuePair<string, string[]> keyValuePair in subWorld.pointsOfInterest)
					{
						List<TerrainCell> terrainCellsForTag2 = WorldGen.GetTerrainCellsForTag(subWorld.name.ToTag());
						for (int i = terrainCellsForTag2.Count - 1; i >= 0; i--)
						{
							if (!this.IsSafeToSpawnPOI(terrainCellsForTag2[i]))
							{
								terrainCellsForTag2.Remove(terrainCellsForTag2[i]);
							}
						}
						if (terrainCellsForTag2.Count > 0)
						{
							string template2 = null;
							TemplateContainer templateContainer = null;
							int num = 0;
							while (templateContainer == null && num < keyValuePair.Value.Length)
							{
								template2 = keyValuePair.Value[this.myRandom.RandomRange(0, keyValuePair.Value.Length)];
								templateContainer = list2.Find((TemplateContainer value) => value.name == template2);
								num++;
							}
							if (templateContainer != null)
							{
								list2.Remove(templateContainer);
								TerrainCell terrainCell2 = terrainCellsForTag2[this.myRandom.RandomRange(0, terrainCellsForTag2.Count)];
								list.Add(new KeyValuePair<Vector2I, TemplateContainer>(new Vector2I((int)terrainCell2.poly.Centroid().x, (int)terrainCell2.poly.Centroid().y), templateContainer));
								terrainCell2.node.tags.Add(template2.ToTag());
								terrainCell2.node.tags.Add(WorldGenTags.POI);
							}
						}
					}
				}
			}
			List<TemplateContainer> list3 = TemplateCache.CollectBaseTemplateAssets("features/");
			foreach (SubWorld subWorld2 in WorldGen.Settings.GetSubWorldList())
			{
				if (subWorld2.featureTemplates != null && subWorld2.featureTemplates.Count > 0)
				{
					List<string> list4 = new List<string>();
					foreach (KeyValuePair<string, int> keyValuePair2 in subWorld2.featureTemplates)
					{
						for (int j = 0; j < keyValuePair2.Value; j++)
						{
							list4.Add(keyValuePair2.Key);
						}
					}
					list4.ShuffleSeeded<string>(this.myRandom.RandomSource());
					List<TerrainCell> terrainCellsForTag3 = WorldGen.GetTerrainCellsForTag(subWorld2.name.ToTag());
					terrainCellsForTag3.ShuffleSeeded<TerrainCell>(this.myRandom.RandomSource());
					foreach (TerrainCell terrainCell3 in terrainCellsForTag3)
					{
						if (list4.Count == 0)
						{
							break;
						}
						if (this.IsSafeToSpawnFeatureTemplate(terrainCell3))
						{
							string template = list4[list4.Count - 1];
							list4.RemoveAt(list4.Count - 1);
							TemplateContainer templateContainer2 = list3.Find((TemplateContainer value) => value.name == template);
							if (templateContainer2 != null)
							{
								list.Add(new KeyValuePair<Vector2I, TemplateContainer>(new Vector2I((int)terrainCell3.poly.Centroid().x, (int)terrainCell3.poly.Centroid().y), templateContainer2));
								terrainCell3.node.tags.Add(template.ToTag());
								terrainCell3.node.tags.Add(WorldGenTags.POI);
							}
						}
					}
				}
			}
			if (doSettle)
			{
				foreach (int num2 in hashSet)
				{
					array[num2].SetValues(WorldGen.unobtaniumElement, ElementLoader.elements);
				}
				WorldGen.running = WorldGenSimUtil.DoSettleSim(array, array2, dc, WorldGen.successCallbackFn, WorldGen.data, list, this.errorCallback);
			}
			MobSpawning.DetectNaturalCavities(WorldGen.successCallbackFn);
			SeededRandom seededRandom = new SeededRandom(WorldGen.data.globalTerrainSeed);
			for (int k = 0; k < WorldGen.TerrainCells.Count; k++)
			{
				float num3 = (float)k / (float)WorldGen.TerrainCells.Count * 100f;
				WorldGen.successCallbackFn(UI.WORLDGEN.PLACINGCREATURES.key, num3, WorldGenProgressStages.Stages.PlacingCreatures);
				TerrainCell terrainCell4 = WorldGen.TerrainCells[k];
				Dictionary<int, string> dictionary = MobSpawning.PlaceAmbientMobs(terrainCell4, seededRandom, array, array2, dc, hashSet);
				if (dictionary != null)
				{
					WorldGen.data.gameSpawnData.AddRange(dictionary);
				}
			}
			WorldGen.successCallbackFn(UI.WORLDGEN.PLACINGCREATURES.key, 100f, WorldGenProgressStages.Stages.PlacingCreatures);
			foreach (KeyValuePair<Vector2I, TemplateContainer> keyValuePair3 in list)
			{
				this.PlaceTemplateSpawners(keyValuePair3.Key, keyValuePair3.Value);
			}
			for (int l = WorldGen.data.gameSpawnData.buildings.Count - 1; l >= 0; l--)
			{
				int num4 = Grid.XYToCell(WorldGen.data.gameSpawnData.buildings[l].location_x, WorldGen.data.gameSpawnData.buildings[l].location_y);
				if (hashSet.Contains(num4))
				{
					WorldGen.data.gameSpawnData.buildings.RemoveAt(l);
				}
			}
			for (int m = WorldGen.data.gameSpawnData.elementalOres.Count - 1; m >= 0; m--)
			{
				int num5 = Grid.XYToCell(WorldGen.data.gameSpawnData.elementalOres[m].location_x, WorldGen.data.gameSpawnData.elementalOres[m].location_y);
				if (hashSet.Contains(num5))
				{
					WorldGen.data.gameSpawnData.elementalOres.RemoveAt(m);
				}
			}
			for (int n = WorldGen.data.gameSpawnData.otherEntities.Count - 1; n >= 0; n--)
			{
				int num6 = Grid.XYToCell(WorldGen.data.gameSpawnData.otherEntities[n].location_x, WorldGen.data.gameSpawnData.otherEntities[n].location_y);
				if (hashSet.Contains(num6))
				{
					WorldGen.data.gameSpawnData.otherEntities.RemoveAt(n);
				}
			}
			for (int num7 = WorldGen.data.gameSpawnData.pickupables.Count - 1; num7 >= 0; num7--)
			{
				int num8 = Grid.XYToCell(WorldGen.data.gameSpawnData.pickupables[num7].location_x, WorldGen.data.gameSpawnData.pickupables[num7].location_y);
				if (hashSet.Contains(num8))
				{
					WorldGen.data.gameSpawnData.pickupables.RemoveAt(num7);
				}
			}
			WorldGen.SaveWorldGen();
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
			WorldGen.stats["GenerateNoiseTime"] = global::System.DateTime.Now.Ticks;
			try
			{
				WorldGen.SetWorldSize(Grid.WidthInCells, Grid.HeightInCells);
				WorldGen.running = updateProgressFn(UI.WORLDGEN.SETUPNOISE.key, 0f, WorldGenProgressStages.Stages.SetupNoise);
				if (!WorldGen.running)
				{
					WorldGen.stats["GenerateNoiseTime"] = 0;
					return false;
				}
				WorldGen.SetupNoise(updateProgressFn);
				WorldGen.running = updateProgressFn(UI.WORLDGEN.SETUPNOISE.key, 100f, WorldGenProgressStages.Stages.SetupNoise);
				if (!WorldGen.running)
				{
					WorldGen.stats["GenerateNoiseTime"] = 0;
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
			WorldGen.stats["GenerateNoiseTime"] = global::System.DateTime.Now.Ticks - (long)WorldGen.stats["GenerateNoiseTime"];
			return true;
		}

		public static bool GenerateLayout(WorldGen.OfflineCallbackFunction updateProgressFn)
		{
			WorldGen.stats["GenerateLayoutTime"] = global::System.DateTime.Now.Ticks;
			try
			{
				WorldGen.running = updateProgressFn(UI.WORLDGEN.WORLDLAYOUT.key, 0f, WorldGenProgressStages.Stages.WorldLayout);
				if (!WorldGen.running)
				{
					return false;
				}
				WorldGen.data.worldLayout = new WorldLayout(WorldGen.data.world.size.x, WorldGen.data.world.size.y, WorldGen.data.globalWorldLayoutSeed);
				WorldGen.running = updateProgressFn(UI.WORLDGEN.WORLDLAYOUT.key, 5f, WorldGenProgressStages.Stages.WorldLayout);
				WorldGen.data.voronoiTree = null;
				try
				{
					WorldGen.data.voronoiTree = WorldGen.WorldLayout.GenerateOverworld(WorldGen.settings.GetWorld().layoutMethod == global::ProcGen.World.LayoutMethod.PowerTree);
					WorldGen.WorldLayout.PopulateSubworlds();
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
					global::VoronoiTree.Tree tree = WorldGen.data.voronoiTree.GetChild(i) as global::VoronoiTree.Tree;
					global::ProcGen.Node node = WorldGen.data.worldLayout.overworldGraph.FindNodeByID(tree.site.id);
					WorldGen.data.overworldCells.Add(new TerrainCellLogged(node, tree.site));
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
			WorldGen.stats["GenerateLayoutTime"] = global::System.DateTime.Now.Ticks - (long)WorldGen.stats["GenerateLayoutTime"];
			return true;
		}

		public bool CompleteLayout(WorldGen.OfflineCallbackFunction updateProgressFn)
		{
			long num = global::System.DateTime.Now.Ticks;
			try
			{
				WorldGen.running = updateProgressFn(UI.WORLDGEN.COMPLETELAYOUT.key, 0f, WorldGenProgressStages.Stages.CompleteLayout);
				if (!WorldGen.running)
				{
					return false;
				}
				List<TemplateContainer> list = TemplateCache.CollectBaseTemplateAssets("poi/");
				WorldGen.WorldLayout.ComputeSubWorlds(list);
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
				this.ApplyStartNode();
				WorldGen.data.terrainCells = new List<TerrainCell>(4000);
				List<global::VoronoiTree.Node> list2 = new List<global::VoronoiTree.Node>();
				WorldGen.data.voronoiTree.ForceLowestToLeaf();
				WorldGen.data.voronoiTree.VisitAll(new Action<global::VoronoiTree.Node>(this.UpdateVoronoiNodeTags));
				WorldGen.data.voronoiTree.GetLeafNodes(list2, null);
				for (int i = 0; i < list2.Count; i++)
				{
					global::VoronoiTree.Node node = list2[i];
					global::ProcGen.Node tn = WorldGen.data.worldLayout.localGraph.FindNodeByID(node.site.id);
					if (tn != null)
					{
						TerrainCell terrainCell = WorldGen.data.terrainCells.Find((TerrainCell c) => c.node == tn);
						if (terrainCell == null)
						{
							TerrainCell terrainCell2 = new TerrainCellLogged(tn, node.site);
							WorldGen.data.terrainCells.Add(terrainCell2);
						}
						else
						{
							global::Debug.LogWarning("Duplicate cell found" + terrainCell.node.node.Id, null);
						}
					}
				}
				for (int j = 0; j < WorldGen.data.terrainCells.Count; j++)
				{
					TerrainCell terrainCell3 = WorldGen.data.terrainCells[j];
					foreach (KeyValuePair<uint, int> keyValuePair in terrainCell3.site.neighbours)
					{
						for (int k = 0; k < WorldGen.data.terrainCells.Count; k++)
						{
							if (WorldGen.data.terrainCells[k].site.id == keyValuePair.Key)
							{
								terrainCell3.terrain_neighbors_idx.Add(keyValuePair.Key);
							}
						}
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
			WorldGen.stats["GenerateLayoutTime"] = (long)WorldGen.stats["GenerateLayoutTime"] + num;
			return true;
		}

		public void UpdateVoronoiNodeTags(global::VoronoiTree.Node node)
		{
			global::ProcGen.Node node2;
			if (node.tags.Contains(WorldGenTags.Overworld))
			{
				node2 = WorldGen.WorldLayout.overworldGraph.FindNodeByID(node.site.id);
			}
			else
			{
				node2 = WorldGen.WorldLayout.localGraph.FindNodeByID(node.site.id);
			}
			if (node2 != null)
			{
				node2.tags.Union(node.tags);
			}
		}

		public static bool GenerateWorldData()
		{
			WorldGen.stats["GenerateDataTime"] = global::System.DateTime.Now.Ticks;
			if (!WorldGen.GenerateNoiseData(WorldGen.successCallbackFn))
			{
				return false;
			}
			if (!WorldGen.GenerateLayout(WorldGen.successCallbackFn))
			{
				return false;
			}
			WorldGen.stats["GenerateDataTime"] = global::System.DateTime.Now.Ticks - (long)WorldGen.stats["GenerateDataTime"];
			return true;
		}

		public static void EnsureEnoughAlgaeInStartingBiome(Sim.Cell[] cells)
		{
			List<TerrainCell> terrainCellsForTag = WorldGen.GetTerrainCellsForTag(WorldGenTags.StartWorld);
			float num = 8200f;
			float num2 = 0f;
			int num3 = 0;
			foreach (TerrainCell terrainCell in terrainCellsForTag)
			{
				foreach (int num4 in terrainCell.GetAllCells())
				{
					if (ElementLoader.GetElementIndex(SimHashes.Algae) == (int)cells[num4].elementIdx)
					{
						num3++;
						num2 += cells[num4].mass;
					}
				}
			}
			if (num2 < num)
			{
				float num5 = (num - num2) / (float)num3;
				foreach (TerrainCell terrainCell2 in terrainCellsForTag)
				{
					foreach (int num6 in terrainCell2.GetAllCells())
					{
						if (ElementLoader.GetElementIndex(SimHashes.Algae) == (int)cells[num6].elementIdx)
						{
							int num7 = num6;
							cells[num7].mass = cells[num7].mass + num5;
						}
					}
				}
			}
		}

		public static bool RenderToMap(WorldGen.OfflineCallbackFunction updateProgressFn, ref Sim.Cell[] cells, ref float[] bgTemp, ref Sim.DiseaseCell[] dcs, ref HashSet<int> borderCells)
		{
			borderCells = new HashSet<int>();
			cells = new Sim.Cell[Grid.CellCount];
			bgTemp = new float[Grid.CellCount];
			dcs = new Sim.DiseaseCell[Grid.CellCount];
			WorldGen.running = updateProgressFn(UI.WORLDGEN.CLEARINGLEVEL.key, 0f, WorldGenProgressStages.Stages.ClearingLevel);
			if (!WorldGen.running)
			{
				return false;
			}
			for (int i = 0; i < cells.Length; i++)
			{
				cells[i].SetValues(WorldGen.katairiteElement, ElementLoader.elements);
				bgTemp[i] = -1f;
				dcs[i] = default(Sim.DiseaseCell);
				dcs[i].diseaseIdx = byte.MaxValue;
				WorldGen.running = updateProgressFn(UI.WORLDGEN.CLEARINGLEVEL.key, 100f * ((float)i / (float)Grid.CellCount), WorldGenProgressStages.Stages.ClearingLevel);
				if (!WorldGen.running)
				{
					return false;
				}
			}
			updateProgressFn(UI.WORLDGEN.CLEARINGLEVEL.key, 100f, WorldGenProgressStages.Stages.ClearingLevel);
			try
			{
				WorldGen.ProcessByTerrainCell(cells, bgTemp, dcs, updateProgressFn);
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				string stackTrace = ex.StackTrace;
				WorldGenLogger.LogException(message, stackTrace);
				WorldGen.running = updateProgressFn(new StringKey("Exception in ProcessByTerrainCell"), -1f, WorldGenProgressStages.Stages.Failure);
				return false;
			}
			if (bool.Parse(WorldGen.settings.defaults.data["DrawWorldBorder"] as string))
			{
				SeededRandom seededRandom = new SeededRandom(0);
				updateProgressFn(UI.WORLDGEN.DRAWWORLDBORDER.key, 0f, WorldGenProgressStages.Stages.DrawWorldBorder);
				WorldGen.DrawWorldBorder(cells, WorldGen.data.world, seededRandom, borderCells);
				updateProgressFn(UI.WORLDGEN.DRAWWORLDBORDER.key, 100f, WorldGenProgressStages.Stages.DrawWorldBorder);
			}
			WorldGen.data.gameSpawnData.baseStartPos = WorldGen.data.worldLayout.GetStartLocation();
			return true;
		}

		public static SubWorld GetSubWorldForNode(global::VoronoiTree.Tree node)
		{
			global::ProcGen.Node node2 = WorldGen.WorldLayout.overworldGraph.FindNodeByID(node.site.id);
			if (node2 == null)
			{
				return null;
			}
			if (!WorldGen.Settings.GetSubWorlds().ContainsKey(node2.type))
			{
				return null;
			}
			return WorldGen.Settings.GetSubWorld(node2.type);
		}

		public static global::VoronoiTree.Tree GetOverworldForNode(Leaf leaf)
		{
			if (leaf == null)
			{
				return null;
			}
			return WorldGen.data.worldLayout.GetVoronoiTree().GetChildContainingLeaf(leaf);
		}

		public static Leaf GetLeafForTerrainCell(TerrainCell cell)
		{
			if (cell == null)
			{
				return null;
			}
			return WorldGen.data.worldLayout.GetVoronoiTree().GetNodeForSite(cell.site) as Leaf;
		}

		public static List<TerrainCell> GetTerrainCellsForTag(Tag tag)
		{
			List<TerrainCell> list = new List<TerrainCell>();
			List<global::VoronoiTree.Node> leafNodesWithTag = WorldGen.WorldLayout.GetLeafNodesWithTag(tag);
			for (int i = 0; i < leafNodesWithTag.Count; i++)
			{
				global::VoronoiTree.Node node = leafNodesWithTag[i];
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

		public static void ChooseBaseLocation(global::VoronoiTree.Node startNode)
		{
			TagSet tagSet = new TagSet();
			tagSet.Add(WorldGenTags.StartLocation);
			List<global::VoronoiTree.Node> startNodes = WorldGen.WorldLayout.GetStartNodes();
			for (int i = 0; i < startNodes.Count; i++)
			{
				if (startNodes[i] != startNode)
				{
					startNodes[i].tags.Remove(tagSet);
				}
			}
		}

		private static void SwitchNodes(global::VoronoiTree.Node n1, global::VoronoiTree.Node n2)
		{
			if (n1 is global::VoronoiTree.Tree || n2 is global::VoronoiTree.Tree)
			{
				global::Debug.Log("WorldGen::SwitchNodes() Skipping tree node", null);
				return;
			}
			global::ProcGen.Node node = WorldGen.data.worldLayout.localGraph.FindNodeByID(n1.site.id);
			global::ProcGen.Node node2 = WorldGen.data.worldLayout.localGraph.FindNodeByID(n2.site.id);
			Diagram.Site site = n1.site;
			n1.site = n2.site;
			n2.site = site;
			string type = node.type;
			node.SetType(node2.type);
			node2.SetType(type);
		}

		public void ApplyStartNode()
		{
			global::VoronoiTree.Node node3 = WorldGen.data.worldLayout.GetLeafNodesWithTag(WorldGenTags.StartLocation)[0];
			node3.parent.AddTagToChildren(WorldGenTags.IgnoreCaveOverride);
			node3.parent.tags.Remove(WorldGenTags.StartLocation);
			List<global::VoronoiTree.Node> siblings = node3.GetSiblings();
			List<global::VoronoiTree.Node> neighbors = node3.GetNeighbors();
			siblings.RemoveAll((global::VoronoiTree.Node node) => neighbors.Contains(node));
			if (neighbors.Count > 0)
			{
				neighbors.ShuffleSeeded<global::VoronoiTree.Node>(this.myRandom.RandomSource());
				List<global::VoronoiTree.Node> list = new List<global::VoronoiTree.Node>();
				List<global::VoronoiTree.Node> list2 = new List<global::VoronoiTree.Node>();
				for (int i = 0; i < neighbors.Count; i++)
				{
					global::VoronoiTree.Node node2 = neighbors[i];
					bool flag = !node2.tags.Contains(WorldGenTags.Wet);
					bool flag2 = node2.site.poly.Centroid().y > node3.site.poly.Centroid().y;
					if (!flag && flag2)
					{
						if (list2.Count > 0)
						{
							WorldGen.SwitchNodes(node2, list2[0]);
							list2.RemoveAt(0);
						}
						else
						{
							list.Add(node2);
						}
					}
					else if (flag && !flag2)
					{
						if (list.Count > 0)
						{
							WorldGen.SwitchNodes(node2, list[0]);
							list.RemoveAt(0);
						}
						else
						{
							list2.Add(node2);
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
			}
		}

		public static void ReplayGenerate(WorldGen.ResetFunction Reset)
		{
			Reset(WorldGen.data.gameSpawnData);
		}

		public static void GetElementForBiome(Chunk chunk, string nt, Vector2I pos, out Element element, out Sim.PhysicsData pd, out Sim.DiseaseCell dc, float erode)
		{
			dc = Sim.DiseaseCell.Invalid;
			TerrainCell.ElementOverride elementOverride = TerrainCell.GetElementOverride(WorldGen.voidElement.tag.ToString(), null);
			if (WorldGen.settings.biomes.BiomeBackgroundElementBandConfigurations.ContainsKey(nt))
			{
				elementOverride = WorldGen.GetElementFromBiomeElementTable(chunk, pos, WorldGen.settings.biomes.BiomeBackgroundElementBandConfigurations[nt], erode);
			}
			else if (WorldGen.settings.features.TerrainFeatures.ContainsKey(nt))
			{
				if (WorldGen.settings.features.TerrainFeatures[nt] == null)
				{
					global::Debug.LogError("TerrainFeatureLookupTable is null for [" + nt + "]", null);
				}
				string defaultBiome = WorldGen.settings.GetDefaultBiome(nt);
				if (!WorldGen.settings.biomes.BiomeBackgroundElementBandConfigurations.ContainsKey(defaultBiome))
				{
					global::Debug.LogError(string.Concat(new string[] { "No biome lookup table of type ", defaultBiome, " is loaded. nt [", nt, "]" }), null);
					throw new Exception(string.Concat(new string[] { "No biome lookup table of type ", defaultBiome, " is loaded. nt [", nt, "]" }));
				}
				ElementBandConfiguration elementBandConfiguration = WorldGen.settings.biomes.BiomeBackgroundElementBandConfigurations[defaultBiome];
				elementOverride = WorldGen.GetElementFromBiomeElementTable(chunk, pos, elementBandConfiguration, erode);
			}
			element = elementOverride.element;
			pd = elementOverride.pdelement;
			dc = elementOverride.dc;
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
			List<Vector2I> line = global::ProcGen.Util.GetLine(segment.First, segment.Second);
			for (int i = 0; i < WorldGen.data.terrainCells.Count; i++)
			{
				if (WorldGen.data.terrainCells[i].node.type != type)
				{
					for (int j = 0; j < line.Count; j++)
					{
						if (WorldGen.data.terrainCells[i].poly.Contains(line[j]))
						{
							WorldGen.data.terrainCells[i].node.SetType(type);
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

		public static List<global::VoronoiTree.Node> GetNodesForStartAreas()
		{
			List<global::VoronoiTree.Node> list = new List<global::VoronoiTree.Node>();
			global::VoronoiTree.Tree voronoiTree = WorldGen.data.worldLayout.GetVoronoiTree();
			if (voronoiTree != null)
			{
				voronoiTree.GetNodesWithTag(WorldGenTags.StartLocation, list);
			}
			return list;
		}

		private static void ProcessByTerrainCell(Sim.Cell[] map_cells, float[] bgTemp, Sim.DiseaseCell[] dcs, WorldGen.OfflineCallbackFunction updateProgressFn)
		{
			updateProgressFn(UI.WORLDGEN.PROCESSING.key, 0f, WorldGenProgressStages.Stages.Processing);
			SeededRandom seededRandom = new SeededRandom(WorldGen.data.globalTerrainSeed);
			try
			{
				for (int i = 0; i < WorldGen.data.terrainCells.Count; i++)
				{
					updateProgressFn(UI.WORLDGEN.PROCESSING.key, 100f * ((float)i / (float)WorldGen.data.terrainCells.Count), WorldGenProgressStages.Stages.Processing);
					WorldGen.data.terrainCells[i].Process(map_cells, bgTemp, dcs, WorldGen.data.world, seededRandom);
				}
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				string stackTrace = ex.StackTrace;
				updateProgressFn(new StringKey("Exception in TerrainCell.Process"), -1f, WorldGenProgressStages.Stages.Failure);
				global::Debug.LogError("Error:" + message + "\n" + stackTrace, null);
			}
			List<WeightedSimHash> list = new List<WeightedSimHash>
			{
				new WeightedSimHash(SimHashes.Granite.ToString(), 10f, null),
				new WeightedSimHash(SimHashes.IgneousRock.ToString(), 4f, null),
				new WeightedSimHash(SimHashes.Obsidian.ToString(), 5f, null)
			};
			List<WeightedSimHash> list2 = new List<WeightedSimHash>
			{
				new WeightedSimHash(SimHashes.Katairite.ToString(), 10f, null)
			};
			List<WeightedSimHash> list3 = new List<WeightedSimHash>
			{
				new WeightedSimHash(SimHashes.Unobtanium.ToString(), 1f, null)
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
						list4.Add(new Border(new Neighbors(terrainCell, terrainCell2), edge2.corner0.position, edge2.corner1.position)
						{
							element = list3,
							width = (float)seededRandom.RandomRange(2, 3)
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
							Border border = new Border(new Neighbors(terrainCell3, terrainCell4), edge.corner0.position, edge.corner1.position);
							border.element = list2;
							if (edge.tags.Contains(WorldGenTags.RoomBorderMixed))
							{
								border.element = list;
							}
							border.width = (float)seededRandom.RandomRange(2, 3);
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
				global::Debug.LogError("Error:" + message2 + " " + stackTrace2, null);
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
				global::Debug.LogError("Error:" + message3 + " " + stackTrace3, null);
			}
			try
			{
				TerrainCell.SetValuesFunction setValuesFunction = delegate(int index, object elem, Sim.PhysicsData pd, Sim.DiseaseCell dc)
				{
					if (Grid.IsValidCell(index))
					{
						if ((elem as Element).HasTag(GameTags.Special))
						{
							pd = (elem as Element).defaultValues;
						}
						map_cells[index].SetValues(elem as Element, pd, ElementLoader.elements);
						dcs[index] = dc;
					}
					else
					{
						global::Debug.LogError(string.Concat(new object[] { "Process::SetValuesFunction Index [", index, "] is not valid. cells.Length [", map_cells.Length, "]" }), null);
					}
				};
				for (int m = 0; m < list4.Count; m++)
				{
					Border border2 = list4[m];
					SubWorld subWorld = WorldGen.Settings.GetSubWorld(border2.neighbors.n0.node.type);
					SubWorld subWorld2 = WorldGen.Settings.GetSubWorld(border2.neighbors.n1.node.type);
					float num = Mathf.Min(WorldGen.Settings.temperatures.ranges[subWorld.temperatureRange].min, WorldGen.Settings.temperatures.ranges[subWorld2.temperatureRange].min);
					float num2 = Mathf.Max(WorldGen.Settings.temperatures.ranges[subWorld.temperatureRange].max, WorldGen.Settings.temperatures.ranges[subWorld2.temperatureRange].max);
					float num3 = num2 - num;
					border2.Stagger(seededRandom, (float)seededRandom.RandomRange(8, 13), (float)seededRandom.RandomRange(2, 5));
					border2.ConvertToMap(WorldGen.data.world, setValuesFunction, num, num3, seededRandom);
				}
			}
			catch (Exception ex4)
			{
				string message4 = ex4.Message;
				string stackTrace4 = ex4.StackTrace;
				updateProgressFn(new StringKey("Exception in border.ConvertToMap"), -1f, WorldGenProgressStages.Stages.Failure);
				global::Debug.LogError("Error:" + message4 + " " + stackTrace4, null);
			}
		}

		private static void DrawBorder(Chunk chunk, int thickness, int range, SeededRandom rnd)
		{
			if (Mathf.Abs(chunk.offset.x % WorldGen.SubWorldSize.x) == 0)
			{
				int num = 0;
				for (int i = WorldGen.ChunkEdgeSize - 1; i >= 0; i--)
				{
					num = Mathf.Max(-range, Mathf.Min(num + rnd.RandomRange(-2, 2), range));
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
					num2 = Mathf.Max(-range, Mathf.Min(num2 + rnd.RandomRange(-2, 2), range));
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
					num3 = Mathf.Max(-range, Mathf.Min(num3 + rnd.RandomRange(-2, 2), range));
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
					num4 = Mathf.Max(-range, Mathf.Min(num4 + rnd.RandomRange(-2, 2), range));
					for (int num6 = 0; num6 < thickness + num4; num6++)
					{
						chunk.overrides[num5 + WorldGen.ChunkEdgeSize * (WorldGen.ChunkEdgeSize - 1 - num6)] = 100f;
					}
				}
			}
		}

		private static void DrawWorldBorder(Sim.Cell[] cells, Chunk world, SeededRandom rnd, HashSet<int> borderCells)
		{
			int num = int.Parse(WorldGen.settings.defaults.data["WorldBorderThickness"] as string);
			int num2 = int.Parse(WorldGen.settings.defaults.data["WorldBorderRange"] as string);
			int num3 = 0;
			int num4 = 0;
			for (int i = world.size.y - 1; i >= 0; i--)
			{
				num3 = Mathf.Max(-num2, Mathf.Min(num3 + rnd.RandomRange(-2, 2), num2));
				for (int j = 0; j < num + num3; j++)
				{
					int num5 = Grid.XYToCell(j, i);
					borderCells.Add(num5);
					cells[num5].SetValues(WorldGen.unobtaniumElement, ElementLoader.elements);
				}
				num4 = Mathf.Max(-num2, Mathf.Min(num4 + rnd.RandomRange(-2, 2), num2));
				for (int k = 0; k < num + num4; k++)
				{
					int num6 = Grid.XYToCell(world.size.x - 1 - k, i);
					borderCells.Add(num6);
					cells[num6].SetValues(WorldGen.unobtaniumElement, ElementLoader.elements);
				}
			}
			int num7 = 0;
			int num8 = 0;
			bool flag = bool.Parse(WorldGen.settings.defaults.data["DrawWorldBorderTop"] as string);
			for (int l = 0; l < world.size.x; l++)
			{
				num7 = Mathf.Max(-num2, Mathf.Min(num7 + rnd.RandomRange(-2, 2), num2));
				for (int m = 0; m < num + num7; m++)
				{
					int num9 = Grid.XYToCell(l, m);
					borderCells.Add(num9);
					cells[num9].SetValues(WorldGen.unobtaniumElement, ElementLoader.elements);
				}
				num8 = Mathf.Max(-num2, Mathf.Min(num8 + rnd.RandomRange(-2, 2), num2));
				for (int n = 0; n < num + num8; n++)
				{
					int num10 = Grid.XYToCell(l, world.size.y - 1 - n);
					borderCells.Add(num10);
					cells[num10].SetValues((!flag) ? WorldGen.voidElement : WorldGen.unobtaniumElement, ElementLoader.elements);
				}
			}
		}

		private static void SetupNoise(WorldGen.OfflineCallbackFunction updateProgressFn)
		{
			updateProgressFn(UI.WORLDGEN.BUILDNOISESOURCE.key, 0f, WorldGenProgressStages.Stages.SetupNoise);
			WorldGen.heatSource = WorldGen.BuildNoiseSource(WorldGen.data.world.size.x, WorldGen.data.world.size.y, "noise/Heat");
			updateProgressFn(UI.WORLDGEN.BUILDNOISESOURCE.key, 100f, WorldGenProgressStages.Stages.SetupNoise);
		}

		public static NoiseMapBuilderPlane BuildNoiseSource(int width, int height, string name)
		{
			global::ProcGen.Noise.Tree tree = WorldGen.settings.noise.GetTree(name, WorldGen.PATH);
			return WorldGen.BuildNoiseSource(width, height, tree);
		}

		public static NoiseMapBuilderPlane BuildNoiseSource(int width, int height, global::ProcGen.Noise.Tree tree)
		{
			Vector2f lowerBound = tree.settings.lowerBound;
			Vector2f upperBound = tree.settings.upperBound;
			NoiseMapBuilderPlane noiseMapBuilderPlane = new NoiseMapBuilderPlane(lowerBound.x, upperBound.x, lowerBound.y, upperBound.y, false);
			noiseMapBuilderPlane.SetSize(width, height);
			noiseMapBuilderPlane.SourceModule = tree.BuildFinalModule(WorldGen.data.globalNoiseSeed);
			return noiseMapBuilderPlane;
		}

		private static void GetMinMaxDataValues(float[] data, int width, int height)
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
				updateProgressFn(UI.WORLDGEN.GENERATENOISE.key, (float)((int)(25.0 * (double)((float)line / (float)WorldGen.data.world.size.y))), WorldGenProgressStages.Stages.GenerateNoise);
			};
			noiseMapBuilderCallback = delegate(int line)
			{
				updateProgressFn(UI.WORLDGEN.GENERATENOISE.key, (float)((int)(25.0 + 25.0 * (double)((float)line / (float)WorldGen.data.world.size.y))), WorldGenProgressStages.Stages.GenerateNoise);
			};
			if (noiseMapBuilderCallback == null)
			{
				global::Debug.LogError("nupd is null", null);
			}
			WorldGen.data.world.heatOffset = WorldGen.GenerateNoise(vector, WorldGen.settings.noise.GetZoomForTree("noise/Heat"), WorldGen.heatSource, WorldGen.data.world.size.x, WorldGen.data.world.size.y, noiseMapBuilderCallback);
			WorldGen.data.world.data = new float[WorldGen.data.world.heatOffset.Length];
			WorldGen.data.world.density = new float[WorldGen.data.world.heatOffset.Length];
			WorldGen.data.world.overrides = new float[WorldGen.data.world.heatOffset.Length];
			updateProgressFn(UI.WORLDGEN.NORMALISENOISE.key, 50f, WorldGenProgressStages.Stages.GenerateNoise);
			if (WorldGen.settings.noise.ShouldNormaliseTree("noise/Heat"))
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
				global::ProcGen.Noise.Tree tree = WorldGen.Settings.noise.GetTree("noise/Default", WorldGen.PATH);
				global::ProcGen.Noise.Tree tree2 = WorldGen.Settings.noise.GetTree("noise/DefaultCave", WorldGen.PATH);
				global::ProcGen.Noise.Tree tree3 = WorldGen.Settings.noise.GetTree("noise/DefaultDensity", WorldGen.PATH);
				SubWorld subWorld = WorldGen.Settings.GetSubWorld(terrainCell.node.type);
				if (subWorld == null)
				{
					global::Debug.Log("Couldnt find Subworld for overworld node [" + terrainCell.node.type + "] using defaults", null);
				}
				else
				{
					if (subWorld.biomeNoise != null)
					{
						global::ProcGen.Noise.Tree tree4 = WorldGen.Settings.noise.GetTree(subWorld.biomeNoise);
						if (tree4 != null)
						{
							tree = tree4;
						}
					}
					if (subWorld.overrideNoise != null)
					{
						global::ProcGen.Noise.Tree tree5 = WorldGen.Settings.noise.GetTree(subWorld.overrideNoise);
						if (tree5 != null)
						{
							tree2 = tree5;
						}
					}
					if (subWorld.densityNoise != null)
					{
						global::ProcGen.Noise.Tree tree6 = WorldGen.Settings.noise.GetTree(subWorld.densityNoise);
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
			float num3 = (float)(pos.y + chunk.offset.y);
			float num4 = (float)WorldGen.data.world.size.y * 0.8f;
			if (num3 > num4)
			{
				num2 *= ((float)WorldGen.data.world.size.y - num3) / ((float)WorldGen.data.world.size.y - num4);
				noDiamond = true;
			}
			return num2;
		}

		public static bool InChunkRange(Chunk chunk, Vector2I pos)
		{
			int num = pos.x + WorldGen.data.world.size.x * pos.y;
			return num >= 0 && num < chunk.data.Length;
		}

		private static TerrainCell.ElementOverride GetElementFromBiomeElementTable(Chunk chunk, Vector2I pos, List<ElementGradient> table, float erode)
		{
			bool flag;
			float num = WorldGen.GetValue(chunk, pos, out flag) * erode;
			TerrainCell.ElementOverride elementOverride = TerrainCell.GetElementOverride(WorldGen.voidElement.tag.ToString(), null);
			if (table.Count == 0)
			{
				return elementOverride;
			}
			for (int i = 0; i < table.Count; i++)
			{
				if (table[i] == null || table[i].content == null || table[i].content.Length == 0)
				{
					int num2 = 0;
					num2++;
				}
				if (num < table[i].maxValue)
				{
					return TerrainCell.GetElementOverride(table[i].content, table[i].overrides);
				}
			}
			return TerrainCell.GetElementOverride(table[table.Count - 1].content, table[table.Count - 1].overrides);
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
				flag = false;
			}
			catch (Exception ex)
			{
				Output.LogWarning(new object[] { "Failed to read " + fileName + "\n" + ex.ToString() });
				flag = false;
			}
			return flag;
		}

		public static void SaveWorldGen()
		{
			try
			{
				Manager.Clear();
				WorldGenSave worldGenSave = new WorldGenSave();
				worldGenSave.version = new Vector2I(1, 1);
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
				if (worldGenSave.version.x != 1 || worldGenSave.version.y > 1)
				{
					Output.LogError(new object[] { string.Concat(new object[]
					{
						"LoadWorldGenSim Error! Wrong save version Current: [",
						1,
						".",
						1,
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
				global::Debug.LogError("Detail is null", null);
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

		public static bool isRunningDebugGen;

		private const string _SIM_SAVE_FILENAME = "WorldGenSimSave.dat";

		private const string _WORLDGEN_SAVE_FILENAME = "WorldGenDataSave.dat";

		private static Dictionary<string, object> stats = null;

		private static Data data = null;

		private SeededRandom myRandom;

		private static WorldGen.OfflineCallbackFunction successCallbackFn;

		private Action<OfflineWorldGen.ErrorInfo> errorCallback;

		private static bool running = true;

		private static Thread generateThread;

		private static Thread renderThread;

		private const int heatScale = 2;

		public static string STAGE_COMPLETE = "Complete";

		public static string CATASTROPHIC_FAIL = "FAILED";

		public static Element voidElement;

		public static Element vacuumElement;

		public static Element katairiteElement;

		public static Element unobtaniumElement;

		private static WorldGenSettings settings;

		private static bool wasLoaded = false;

		private static string PATH = null;

		public static List<string> diseaseIds = new List<string>
		{
			"FoodPoisoning",
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			"SlimeLung"
		};

		public const int WORLD_OFFSET_Y = 0;

		private const int UNPASSABLE_EDGE_COUNT = 4;

		private const string heat_noise_name = "noise/Heat";

		private const string base_noise_name = "noise/Default";

		private const string cave_noise_name = "noise/DefaultCave";

		private const string density_noise_name = "noise/DefaultDensity";

		private static NoiseMapBuilderPlane heatSource = null;

		public const int WORLDGEN_SAVE_MAJOR_VERSION = 1;

		public const int WORLDGEN_SAVE_MINOR_VERSION = 1;

		public static int polyIndex = 0;

		[EnumFlags]
		public static WorldGen.DebugFlags drawOptions;

		public delegate void ResetFunction(GameSpawnData gsd);

		public delegate bool OfflineCallbackFunction(StringKey stringKeyRoot, float completePercent, WorldGenProgressStages.Stages stage);

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
			SitePoly = 4
		}
	}
}
