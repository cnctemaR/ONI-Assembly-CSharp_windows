using System;
using System.Collections.Generic;
using System.IO;
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
		public WorldGen(string worldName = "worlds/Default")
		{
			WorldGen.LoadSettings();
			this.Settings = new WorldGenSettings(worldName);
			this.data = new Data();
			this.data.chunkEdgeSize = this.Settings.GetIntSetting("ChunkEdgeSize");
			this.data.subWorldSize = new Vector2I(this.Settings.GetIntSetting("SubWorldWidth"), this.Settings.GetIntSetting("SubWorldHeight"));
			this.stats = new Dictionary<string, object>();
		}

		public WorldGen(int worldNameIndex)
		{
			WorldGen.LoadSettings();
			string text = ((worldNameIndex >= SettingsCache.GetWorldNames().Count) ? "worlds/Default" : SettingsCache.GetWorldNames()[worldNameIndex]);
			this.Settings = new WorldGenSettings(text);
			this.data = new Data();
			this.data.chunkEdgeSize = this.Settings.GetIntSetting("ChunkEdgeSize");
			this.data.subWorldSize = new Vector2I(this.Settings.GetIntSetting("SubWorldWidth"), this.Settings.GetIntSetting("SubWorldHeight"));
			this.stats = new Dictionary<string, object>();
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

		public Vector2I SubWorldSize
		{
			get
			{
				return this.data.subWorldSize;
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

		public WorldGenSettings Settings { get; private set; }

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
			IFileSystem fileSystem;
			if (Global.Instance != null && Global.Instance.layeredFileSystem != null)
			{
				IFileSystem layeredFileSystem = Global.Instance.layeredFileSystem;
				fileSystem = layeredFileSystem;
			}
			else
			{
				fileSystem = new StandardFileSystem();
			}
			IFileSystem fileSystem2 = fileSystem;
			if (SettingsCache.LoadFiles(fileSystem2))
			{
				TemplateCache.Init();
			}
			if (CustomGameSettings.Instance != null)
			{
			}
		}

		public void SaveSettings(string newpath = null)
		{
			SettingsCache.Save((newpath != null) ? newpath : SettingsCache.GetPath());
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

		public void Initialise(WorldGen.OfflineCallbackFunction callbackFn, Action<OfflineWorldGen.ErrorInfo> error_cb, int worldSeed = -1, int layoutSeed = -1, int terrainSeed = -1, int noiseSeed = -1)
		{
			if (this.wasLoaded)
			{
				global::Debug.LogError("Initialise called after load", null);
				return;
			}
			this.successCallbackFn = callbackFn;
			this.errorCallback = error_cb;
			this.isRunningDebugGen = false;
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
			Output.Log(new object[] { string.Format("World seeds: [{0}/{1}/{2}/{3}]", new object[] { worldSeed, layoutSeed, terrainSeed, noiseSeed }) });
			this.InitRandom(worldSeed, layoutSeed, terrainSeed, noiseSeed);
			TerrainCell.ClearClaimedCells();
			this.successCallbackFn(UI.WORLDGEN.COMPLETE.key, 0f, WorldGenProgressStages.Stages.Failure);
			this.stats["GenerateTime"] = 0;
			this.stats["GenerateNoiseTime"] = 0;
			this.stats["GenerateLayoutTime"] = 0;
			this.stats["ConvertVoroToMapTime"] = 0;
			WorldLayout.SetLayerGradient(SettingsCache.layers.LevelLayers);
		}

		public void GenerateOfflineThreaded()
		{
			if (this.wasLoaded)
			{
				global::Debug.LogError("GenerateOfflineThreaded called after load", null);
				return;
			}
			if (this.Settings.world == null)
			{
				return;
			}
			this.running = true;
			this.generateThread = new Thread(new ThreadStart(this.GenerateOffline));
			global::Util.ApplyInvariantCultureToThread(this.generateThread);
			this.generateThread.Start();
		}

		public void RenderWorldThreaded()
		{
			if (this.wasLoaded)
			{
				global::Debug.LogError("RenderWorldThreaded called after load", null);
				return;
			}
			this.running = true;
			this.renderThread = new Thread(new ThreadStart(this.RenderOfflineThreadFn));
			global::Util.ApplyInvariantCultureToThread(this.renderThread);
			this.renderThread.Start();
		}

		public void Quit()
		{
			if (this.generateThread != null && this.generateThread.IsAlive)
			{
				this.generateThread.Abort();
			}
			if (this.renderThread != null && this.renderThread.IsAlive)
			{
				this.renderThread.Abort();
			}
			this.running = false;
		}

		public bool IsGenerateComplete()
		{
			return this.generateThread != null && !this.generateThread.IsAlive;
		}

		public bool IsRenderComplete()
		{
			return this.renderThread != null && !this.renderThread.IsAlive;
		}

		public void GenerateOffline()
		{
			for (int i = 0; i < 10; i++)
			{
				if (this.GenerateWorldData())
				{
					break;
				}
				this.successCallbackFn(UI.WORLDGEN.RETRYCOUNT.key, (float)i, WorldGenProgressStages.Stages.Failure);
			}
		}

		private void PlaceTemplateSpawners(Vector2I position, TemplateContainer template)
		{
			this.data.gameSpawnData.AddTemplate(template, position);
		}

		public bool IsSafeToSpawnPOI(TerrainCell tc)
		{
			using (List<uint>.Enumerator enumerator = tc.terrain_neighbors_idx.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					uint neighbor = enumerator.Current;
					TerrainCell terrainCell = this.data.terrainCells.Find((TerrainCell cell) => cell.site.id == neighbor);
					if (terrainCell.node.tags.Contains(WorldGenTags.POI))
					{
						return false;
					}
				}
			}
			return !tc.node.tags.Contains(WorldGenTags.StartLocation) && !tc.node.tags.Contains(WorldGenTags.NearStartLocation) && !tc.node.tags.Contains(WorldGenTags.POI) && !tc.node.tags.Contains(WorldGenTags.AtEdge) && !tc.node.tags.Contains(WorldGenTags.AtDepths);
		}

		public KeyValuePair<Vector2I, TemplateContainer> GetPOISpawnTarget(Sim.Cell[] cells, TerrainCell tc, List<TemplateContainer> poi)
		{
			KeyValuePair<Vector2I, TemplateContainer> keyValuePair = new KeyValuePair<Vector2I, TemplateContainer>(new Vector2I(-1, -1), null);
			using (List<uint>.Enumerator enumerator = tc.terrain_neighbors_idx.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					uint neighbor = enumerator.Current;
					TerrainCell terrainCell = this.data.terrainCells.Find((TerrainCell cell) => cell.site.id == neighbor);
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
			HashSet<int> borderCells = new HashSet<int>();
			this.CompleteLayout(this.successCallbackFn);
			this.WriteOverWorldNoise(this.successCallbackFn);
			if (!this.RenderToMap(this.successCallbackFn, ref array, ref array2, ref dc, ref borderCells))
			{
				this.successCallbackFn(UI.WORLDGEN.FAILED.key, -100f, WorldGenProgressStages.Stages.Failure);
				return null;
			}
			this.EnsureEnoughAlgaeInStartingBiome(array);
			List<KeyValuePair<Vector2I, TemplateContainer>> list = new List<KeyValuePair<Vector2I, TemplateContainer>>();
			TemplateContainer baseStartingTemplate = TemplateCache.GetBaseStartingTemplate();
			List<TerrainCell> terrainCellsForTag = this.GetTerrainCellsForTag(WorldGenTags.StartLocation);
			foreach (TerrainCell terrainCell in terrainCellsForTag)
			{
				list.Add(new KeyValuePair<Vector2I, TemplateContainer>(new Vector2I((int)terrainCell.poly.Centroid().x, (int)terrainCell.poly.Centroid().y), baseStartingTemplate));
			}
			List<TemplateContainer> list2 = TemplateCache.CollectBaseTemplateAssets("poi/");
			foreach (SubWorld subWorld in this.Settings.GetSubWorldList())
			{
				if (subWorld.pointsOfInterest != null)
				{
					foreach (KeyValuePair<string, string[]> keyValuePair in subWorld.pointsOfInterest)
					{
						List<TerrainCell> terrainCellsForTag2 = this.GetTerrainCellsForTag(subWorld.name.ToTag());
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
								for (int j = 0; j < terrainCellsForTag2.Count; j++)
								{
									TerrainCell terrainCell2 = terrainCellsForTag2[this.myRandom.RandomRange(0, terrainCellsForTag2.Count)];
									if (!terrainCell2.node.tags.Contains(WorldGenTags.POI))
									{
										if (templateContainer.info.size.Y <= terrainCell2.poly.MaxY - terrainCell2.poly.MinY)
										{
											list.Add(new KeyValuePair<Vector2I, TemplateContainer>(new Vector2I((int)terrainCell2.poly.Centroid().x, (int)terrainCell2.poly.Centroid().y), templateContainer));
											terrainCell2.node.tags.Add(template2.ToTag());
											terrainCell2.node.tags.Add(WorldGenTags.POI);
											break;
										}
										float num2 = templateContainer.info.size.Y - (terrainCell2.poly.MaxY - terrainCell2.poly.MinY);
										float num3 = templateContainer.info.size.X - (terrainCell2.poly.MaxX - terrainCell2.poly.MinX);
										if (terrainCell2.poly.MaxY + num2 < (float)Grid.HeightInCells && terrainCell2.poly.MinY - num2 > 0f && terrainCell2.poly.MaxX + num3 < (float)Grid.WidthInCells && terrainCell2.poly.MinX - num3 > 0f)
										{
											list.Add(new KeyValuePair<Vector2I, TemplateContainer>(new Vector2I((int)terrainCell2.poly.Centroid().x, (int)terrainCell2.poly.Centroid().y), templateContainer));
											terrainCell2.node.tags.Add(template2.ToTag());
											terrainCell2.node.tags.Add(WorldGenTags.POI);
											break;
										}
									}
								}
							}
						}
					}
				}
			}
			List<TemplateContainer> list3 = TemplateCache.CollectBaseTemplateAssets("features/");
			foreach (SubWorld subWorld2 in this.Settings.GetSubWorldList())
			{
				if (subWorld2.featureTemplates != null && subWorld2.featureTemplates.Count > 0)
				{
					List<string> list4 = new List<string>();
					foreach (KeyValuePair<string, int> keyValuePair2 in subWorld2.featureTemplates)
					{
						for (int k = 0; k < keyValuePair2.Value; k++)
						{
							list4.Add(keyValuePair2.Key);
						}
					}
					list4.ShuffleSeeded<string>(this.myRandom.RandomSource());
					List<TerrainCell> terrainCellsForTag3 = this.GetTerrainCellsForTag(subWorld2.name.ToTag());
					terrainCellsForTag3.ShuffleSeeded<TerrainCell>(this.myRandom.RandomSource());
					foreach (TerrainCell terrainCell3 in terrainCellsForTag3)
					{
						if (list4.Count == 0)
						{
							break;
						}
						if (terrainCell3.IsSafeToSpawnFeatureTemplate())
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
			foreach (int num4 in borderCells)
			{
				array[num4].SetValues(WorldGen.unobtaniumElement, ElementLoader.elements);
			}
			if (doSettle)
			{
				this.running = WorldGenSimUtil.DoSettleSim(this.Settings, array, array2, dc, this.successCallbackFn, this.data, list, this.errorCallback, delegate(Sim.Cell[] updatedCells, float[] updatedBGTemp, Sim.DiseaseCell[] updatedDisease)
				{
					this.SpawnMobsAndTemplates(updatedCells, updatedBGTemp, updatedDisease, borderCells);
				});
			}
			foreach (KeyValuePair<Vector2I, TemplateContainer> keyValuePair3 in list)
			{
				this.PlaceTemplateSpawners(keyValuePair3.Key, keyValuePair3.Value);
			}
			for (int l = this.data.gameSpawnData.buildings.Count - 1; l >= 0; l--)
			{
				int num5 = Grid.XYToCell(this.data.gameSpawnData.buildings[l].location_x, this.data.gameSpawnData.buildings[l].location_y);
				if (borderCells.Contains(num5))
				{
					this.data.gameSpawnData.buildings.RemoveAt(l);
				}
			}
			for (int m = this.data.gameSpawnData.elementalOres.Count - 1; m >= 0; m--)
			{
				int num6 = Grid.XYToCell(this.data.gameSpawnData.elementalOres[m].location_x, this.data.gameSpawnData.elementalOres[m].location_y);
				if (borderCells.Contains(num6))
				{
					this.data.gameSpawnData.elementalOres.RemoveAt(m);
				}
			}
			for (int n = this.data.gameSpawnData.otherEntities.Count - 1; n >= 0; n--)
			{
				int num7 = Grid.XYToCell(this.data.gameSpawnData.otherEntities[n].location_x, this.data.gameSpawnData.otherEntities[n].location_y);
				if (borderCells.Contains(num7))
				{
					this.data.gameSpawnData.otherEntities.RemoveAt(n);
				}
			}
			for (int num8 = this.data.gameSpawnData.pickupables.Count - 1; num8 >= 0; num8--)
			{
				int num9 = Grid.XYToCell(this.data.gameSpawnData.pickupables[num8].location_x, this.data.gameSpawnData.pickupables[num8].location_y);
				if (borderCells.Contains(num9))
				{
					this.data.gameSpawnData.pickupables.RemoveAt(num8);
				}
			}
			this.SaveWorldGen();
			this.successCallbackFn(UI.WORLDGEN.COMPLETE.key, 101f, WorldGenProgressStages.Stages.Complete);
			this.running = false;
			return array;
		}

		private void SpawnMobsAndTemplates(Sim.Cell[] cells, float[] bgTemp, Sim.DiseaseCell[] dc, HashSet<int> borderCells)
		{
			MobSpawning.DetectNaturalCavities(this.TerrainCells, this.successCallbackFn);
			SeededRandom seededRandom = new SeededRandom(this.data.globalTerrainSeed);
			for (int i = 0; i < this.TerrainCells.Count; i++)
			{
				float num = (float)i / (float)this.TerrainCells.Count * 100f;
				this.successCallbackFn(UI.WORLDGEN.PLACINGCREATURES.key, num, WorldGenProgressStages.Stages.PlacingCreatures);
				TerrainCell terrainCell = this.TerrainCells[i];
				Dictionary<int, string> dictionary = MobSpawning.PlaceAmbientMobs(this.Settings, terrainCell, seededRandom, cells, bgTemp, dc, borderCells, this.isRunningDebugGen);
				if (dictionary != null)
				{
					this.data.gameSpawnData.AddRange(dictionary);
				}
			}
			this.successCallbackFn(UI.WORLDGEN.PLACINGCREATURES.key, 100f, WorldGenProgressStages.Stages.PlacingCreatures);
		}

		public void SetWorldSize(int width, int height)
		{
			this.data.world = new Chunk(0, 0, width, height);
		}

		public bool GenerateNoiseData(WorldGen.OfflineCallbackFunction updateProgressFn)
		{
			this.stats["GenerateNoiseTime"] = global::System.DateTime.Now.Ticks;
			try
			{
				this.SetWorldSize(Grid.WidthInCells, Grid.HeightInCells);
				this.running = updateProgressFn(UI.WORLDGEN.SETUPNOISE.key, 0f, WorldGenProgressStages.Stages.SetupNoise);
				if (!this.running)
				{
					this.stats["GenerateNoiseTime"] = 0;
					return false;
				}
				this.SetupNoise(updateProgressFn);
				this.running = updateProgressFn(UI.WORLDGEN.SETUPNOISE.key, 100f, WorldGenProgressStages.Stages.SetupNoise);
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
				this.data.worldLayout = new WorldLayout(this, this.data.world.size.x, this.data.world.size.y, this.data.globalWorldLayoutSeed);
				this.running = updateProgressFn(UI.WORLDGEN.WORLDLAYOUT.key, 5f, WorldGenProgressStages.Stages.WorldLayout);
				this.data.voronoiTree = null;
				try
				{
					this.data.voronoiTree = this.WorldLayout.GenerateOverworld(this.Settings.world.layoutMethod == global::ProcGen.World.LayoutMethod.PowerTree);
					this.WorldLayout.PopulateSubworlds();
				}
				catch (Exception ex)
				{
					string message = ex.Message;
					string stackTrace = ex.StackTrace;
					WorldGenLogger.LogException(message, stackTrace);
					this.running = updateProgressFn(new StringKey("Exception in InitVoronoiTree"), -1f, WorldGenProgressStages.Stages.Failure);
					return false;
				}
				this.data.overworldCells = new List<TerrainCell>(40);
				for (int i = 0; i < this.data.voronoiTree.ChildCount(); i++)
				{
					global::VoronoiTree.Tree tree = this.data.voronoiTree.GetChild(i) as global::VoronoiTree.Tree;
					global::ProcGen.Node node = this.data.worldLayout.overworldGraph.FindNodeByID(tree.site.id);
					this.data.overworldCells.Add(new TerrainCellLogged(node, tree.site));
				}
				this.running = updateProgressFn(UI.WORLDGEN.WORLDLAYOUT.key, 100f, WorldGenProgressStages.Stages.WorldLayout);
			}
			catch (Exception ex2)
			{
				string message2 = ex2.Message;
				string stackTrace2 = ex2.StackTrace;
				WorldGenLogger.LogException(message2, stackTrace2);
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
				List<TemplateContainer> list = TemplateCache.CollectBaseTemplateAssets("poi/");
				this.WorldLayout.ComputeSubWorlds(list);
				this.data.terrainCells = null;
				this.running = updateProgressFn(UI.WORLDGEN.COMPLETELAYOUT.key, 65f, WorldGenProgressStages.Stages.CompleteLayout);
				if (!this.running)
				{
					return false;
				}
				this.running = updateProgressFn(UI.WORLDGEN.COMPLETELAYOUT.key, 75f, WorldGenProgressStages.Stages.CompleteLayout);
				if (!this.running)
				{
					return false;
				}
				this.ApplyStartNode();
				this.data.terrainCells = new List<TerrainCell>(4000);
				List<global::VoronoiTree.Node> list2 = new List<global::VoronoiTree.Node>();
				this.data.voronoiTree.ForceLowestToLeaf();
				this.data.voronoiTree.VisitAll(new Action<global::VoronoiTree.Node>(this.UpdateVoronoiNodeTags));
				this.data.voronoiTree.GetLeafNodes(list2, null);
				for (int i = 0; i < list2.Count; i++)
				{
					global::VoronoiTree.Node node = list2[i];
					global::ProcGen.Node tn = this.data.worldLayout.localGraph.FindNodeByID(node.site.id);
					if (tn != null)
					{
						TerrainCell terrainCell = this.data.terrainCells.Find((TerrainCell c) => c.node == tn);
						if (terrainCell == null)
						{
							TerrainCell terrainCell2 = new TerrainCellLogged(tn, node.site);
							this.data.terrainCells.Add(terrainCell2);
						}
						else
						{
							global::Debug.LogWarning("Duplicate cell found" + terrainCell.node.node.Id, null);
						}
					}
				}
				for (int j = 0; j < this.data.terrainCells.Count; j++)
				{
					TerrainCell terrainCell3 = this.data.terrainCells[j];
					foreach (KeyValuePair<uint, int> keyValuePair in terrainCell3.site.neighbours)
					{
						for (int k = 0; k < this.data.terrainCells.Count; k++)
						{
							if (this.data.terrainCells[k].site.id == keyValuePair.Key)
							{
								terrainCell3.terrain_neighbors_idx.Add(keyValuePair.Key);
							}
						}
					}
				}
				this.running = updateProgressFn(UI.WORLDGEN.COMPLETELAYOUT.key, 100f, WorldGenProgressStages.Stages.CompleteLayout);
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
			if (!this.GenerateNoiseData(this.successCallbackFn))
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

		public void EnsureEnoughAlgaeInStartingBiome(Sim.Cell[] cells)
		{
			List<TerrainCell> terrainCellsForTag = this.GetTerrainCellsForTag(WorldGenTags.StartWorld);
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

		public bool RenderToMap(WorldGen.OfflineCallbackFunction updateProgressFn, ref Sim.Cell[] cells, ref float[] bgTemp, ref Sim.DiseaseCell[] dcs, ref HashSet<int> borderCells)
		{
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
				this.running = updateProgressFn(UI.WORLDGEN.CLEARINGLEVEL.key, 100f * ((float)i / (float)Grid.CellCount), WorldGenProgressStages.Stages.ClearingLevel);
				if (!this.running)
				{
					return false;
				}
			}
			updateProgressFn(UI.WORLDGEN.CLEARINGLEVEL.key, 100f, WorldGenProgressStages.Stages.ClearingLevel);
			try
			{
				this.ProcessByTerrainCell(cells, bgTemp, dcs, updateProgressFn);
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
				updateProgressFn(UI.WORLDGEN.DRAWWORLDBORDER.key, 0f, WorldGenProgressStages.Stages.DrawWorldBorder);
				this.DrawWorldBorder(cells, this.data.world, seededRandom, borderCells);
				updateProgressFn(UI.WORLDGEN.DRAWWORLDBORDER.key, 100f, WorldGenProgressStages.Stages.DrawWorldBorder);
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
			if (!this.Settings.GetSubWorlds().ContainsKey(node2.type))
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

		public void ChooseBaseLocation(global::VoronoiTree.Node startNode)
		{
			TagSet tagSet = new TagSet();
			tagSet.Add(WorldGenTags.StartLocation);
			List<global::VoronoiTree.Node> startNodes = this.WorldLayout.GetStartNodes();
			for (int i = 0; i < startNodes.Count; i++)
			{
				if (startNodes[i] != startNode)
				{
					startNodes[i].tags.Remove(tagSet);
				}
			}
		}

		private void SwitchNodes(global::VoronoiTree.Node n1, global::VoronoiTree.Node n2)
		{
			if (n1 is global::VoronoiTree.Tree || n2 is global::VoronoiTree.Tree)
			{
				global::Debug.Log("WorldGen::SwitchNodes() Skipping tree node", null);
				return;
			}
			global::ProcGen.Node node = this.data.worldLayout.localGraph.FindNodeByID(n1.site.id);
			global::ProcGen.Node node2 = this.data.worldLayout.localGraph.FindNodeByID(n2.site.id);
			Diagram.Site site = n1.site;
			n1.site = n2.site;
			n2.site = site;
			string type = node.type;
			node.SetType(node2.type);
			node2.SetType(type);
		}

		public void ApplyStartNode()
		{
			global::VoronoiTree.Node node3 = this.data.worldLayout.GetLeafNodesWithTag(WorldGenTags.StartLocation)[0];
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
							this.SwitchNodes(node2, list2[0]);
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
							this.SwitchNodes(node2, list[0]);
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
						this.SwitchNodes(list[num], list2[0]);
						list2.RemoveAt(0);
						num++;
					}
				}
			}
		}

		public void ReplayGenerate(WorldGen.ResetFunction Reset)
		{
			Reset(this.data.gameSpawnData);
		}

		public void GetElementForBiome(Chunk chunk, string nt, Vector2I pos, out Element element, out Sim.PhysicsData pd, out Sim.DiseaseCell dc, float erode)
		{
			dc = Sim.DiseaseCell.Invalid;
			TerrainCell.ElementOverride elementOverride = TerrainCell.GetElementOverride(WorldGen.voidElement.tag.ToString(), null);
			if (SettingsCache.biomes.BiomeBackgroundElementBandConfigurations.ContainsKey(nt))
			{
				elementOverride = this.GetElementFromBiomeElementTable(chunk, pos, SettingsCache.biomes.BiomeBackgroundElementBandConfigurations[nt], erode);
			}
			else if (SettingsCache.features.TerrainFeatures.ContainsKey(nt))
			{
				if (SettingsCache.features.TerrainFeatures[nt] == null)
				{
					global::Debug.LogError("TerrainFeatureLookupTable is null for [" + nt + "]", null);
				}
				string defaultBiome = SettingsCache.GetDefaultBiome(nt);
				if (!SettingsCache.biomes.BiomeBackgroundElementBandConfigurations.ContainsKey(defaultBiome))
				{
					global::Debug.LogError(string.Concat(new string[] { "No biome lookup table of type ", defaultBiome, " is loaded. nt [", nt, "]" }), null);
					throw new Exception(string.Concat(new string[] { "No biome lookup table of type ", defaultBiome, " is loaded. nt [", nt, "]" }));
				}
				ElementBandConfiguration elementBandConfiguration = SettingsCache.biomes.BiomeBackgroundElementBandConfigurations[defaultBiome];
				elementOverride = this.GetElementFromBiomeElementTable(chunk, pos, elementBandConfiguration, erode);
			}
			element = elementOverride.element;
			pd = elementOverride.pdelement;
			dc = elementOverride.dc;
		}

		private bool ConvertTerrainCellsToEdges(WorldGen.OfflineCallbackFunction updateProgress)
		{
			for (int i = 0; i < this.data.overworldCells.Count; i++)
			{
				this.running = updateProgress(UI.WORLDGEN.CONVERTTERRAINCELLSTOEDGES.key, (float)i / (float)this.data.overworldCells.Count, WorldGenProgressStages.Stages.ConvertCellsToEdges);
				if (!this.running)
				{
					return this.running;
				}
				List<Vector2> vertices = this.data.overworldCells[i].poly.Vertices;
				for (int j = 0; j < vertices.Count; j++)
				{
					if (j < vertices.Count - 1)
					{
						this.ConvertIntersectingCellsToType(new MathUtil.Pair<Vector2, Vector2>(vertices[j], vertices[j + 1]), "EDGE");
					}
					else
					{
						this.ConvertIntersectingCellsToType(new MathUtil.Pair<Vector2, Vector2>(vertices[j], vertices[0]), (vertices.Count <= 4) ? "EDGE" : "UNPASSABLE");
					}
				}
			}
			return true;
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

		private List<Polygon> GetOverworldPolygons()
		{
			List<Polygon> list = new List<Polygon>();
			for (int i = 0; i < this.data.overworldCells.Count; i++)
			{
				list.Add(this.data.overworldCells[i].poly);
			}
			return list;
		}

		private List<Border> GetBorders(List<TerrainCell> cells)
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

		public List<global::VoronoiTree.Node> GetNodesForStartAreas()
		{
			List<global::VoronoiTree.Node> list = new List<global::VoronoiTree.Node>();
			global::VoronoiTree.Tree voronoiTree = this.data.worldLayout.GetVoronoiTree();
			if (voronoiTree != null)
			{
				voronoiTree.GetNodesWithTag(WorldGenTags.StartLocation, list);
			}
			return list;
		}

		private void ProcessByTerrainCell(Sim.Cell[] map_cells, float[] bgTemp, Sim.DiseaseCell[] dcs, WorldGen.OfflineCallbackFunction updateProgressFn)
		{
			updateProgressFn(UI.WORLDGEN.PROCESSING.key, 0f, WorldGenProgressStages.Stages.Processing);
			SeededRandom seededRandom = new SeededRandom(this.data.globalTerrainSeed);
			try
			{
				for (int i = 0; i < this.data.terrainCells.Count; i++)
				{
					updateProgressFn(UI.WORLDGEN.PROCESSING.key, 100f * ((float)i / (float)this.data.terrainCells.Count), WorldGenProgressStages.Stages.Processing);
					this.data.terrainCells[i].Process(this, map_cells, bgTemp, dcs, this.data.world, seededRandom);
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
				List<Edge> edgesWithTag = this.data.worldLayout.overworldGraph.GetEdgesWithTag(WorldGenTags.EdgeUnpassable);
				for (int j = 0; j < edgesWithTag.Count; j++)
				{
					Edge edge2 = edgesWithTag[j];
					if (edge2.site0 != edge2.site1)
					{
						TerrainCell terrainCell = this.data.overworldCells.Find((TerrainCell c) => c.node.node == edge2.site0.node);
						TerrainCell terrainCell2 = this.data.overworldCells.Find((TerrainCell c) => c.node.node == edge2.site1.node);
						list4.Add(new Border(new Neighbors(terrainCell, terrainCell2), edge2.corner0.position, edge2.corner1.position)
						{
							element = list3,
							width = (float)seededRandom.RandomRange(2, 3)
						});
					}
				}
				List<Edge> edgesWithTag2 = this.data.worldLayout.overworldGraph.GetEdgesWithTag(WorldGenTags.EdgeClosed);
				for (int k = 0; k < edgesWithTag2.Count; k++)
				{
					Edge edge = edgesWithTag2[k];
					if (edge.site0 != edge.site1)
					{
						if (!edgesWithTag.Contains(edge))
						{
							TerrainCell terrainCell3 = this.data.overworldCells.Find((TerrainCell c) => c.node.node == edge.site0.node);
							TerrainCell terrainCell4 = this.data.overworldCells.Find((TerrainCell c) => c.node.node == edge.site1.node);
							Border border = new Border(new Neighbors(terrainCell3, terrainCell4), edge.corner0.position, edge.corner1.position);
							border.element = list2;
							if (edge.tags.Contains(WorldGenTags.RoomBorderMixed))
							{
								border.element = list;
							}
							border.width = seededRandom.RandomRange(1f, 2.5f);
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
					SubWorld subWorld = this.Settings.GetSubWorld(border2.neighbors.n0.node.type);
					SubWorld subWorld2 = this.Settings.GetSubWorld(border2.neighbors.n1.node.type);
					float num = Mathf.Min(SettingsCache.temperatures.ranges[subWorld.temperatureRange].min, SettingsCache.temperatures.ranges[subWorld2.temperatureRange].min);
					float num2 = Mathf.Max(SettingsCache.temperatures.ranges[subWorld.temperatureRange].max, SettingsCache.temperatures.ranges[subWorld2.temperatureRange].max);
					float num3 = num2 - num;
					border2.Stagger(seededRandom, (float)seededRandom.RandomRange(8, 13), (float)seededRandom.RandomRange(2, 5));
					border2.ConvertToMap(this.data.world, setValuesFunction, num, num3, seededRandom);
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

		private void DrawBorder(Chunk chunk, int thickness, int range, SeededRandom rnd)
		{
			if (Mathf.Abs(chunk.offset.x % this.SubWorldSize.x) == 0)
			{
				int num = 0;
				for (int i = this.ChunkEdgeSize - 1; i >= 0; i--)
				{
					num = Mathf.Max(-range, Mathf.Min(num + rnd.RandomRange(-2, 2), range));
					for (int j = 0; j < thickness + num; j++)
					{
						chunk.overrides[j + this.ChunkEdgeSize * i] = 100f;
					}
				}
			}
			if (Mathf.Abs((chunk.offset.x + this.ChunkEdgeSize) % this.SubWorldSize.x) == 0)
			{
				int num2 = 0;
				for (int k = this.ChunkEdgeSize - 1; k >= 0; k--)
				{
					num2 = Mathf.Max(-range, Mathf.Min(num2 + rnd.RandomRange(-2, 2), range));
					for (int l = 0; l < thickness + num2; l++)
					{
						chunk.overrides[this.ChunkEdgeSize - 1 - l + this.ChunkEdgeSize * k] = 100f;
					}
				}
			}
			if (Mathf.Abs(chunk.offset.y % this.SubWorldSize.y) == 0)
			{
				int num3 = 0;
				for (int m = 0; m < this.ChunkEdgeSize; m++)
				{
					num3 = Mathf.Max(-range, Mathf.Min(num3 + rnd.RandomRange(-2, 2), range));
					for (int n = 0; n < thickness + num3; n++)
					{
						chunk.overrides[m + this.ChunkEdgeSize * n] = 100f;
					}
				}
			}
			if (Mathf.Abs((chunk.offset.y + this.ChunkEdgeSize) % this.SubWorldSize.y) == 0)
			{
				int num4 = 0;
				for (int num5 = 0; num5 < this.ChunkEdgeSize; num5++)
				{
					num4 = Mathf.Max(-range, Mathf.Min(num4 + rnd.RandomRange(-2, 2), range));
					for (int num6 = 0; num6 < thickness + num4; num6++)
					{
						chunk.overrides[num5 + this.ChunkEdgeSize * (this.ChunkEdgeSize - 1 - num6)] = 100f;
					}
				}
			}
		}

		private void DrawWorldBorder(Sim.Cell[] cells, Chunk world, SeededRandom rnd, HashSet<int> borderCells)
		{
			bool boolSetting = this.Settings.GetBoolSetting("DrawWorldBorderTop");
			int intSetting = this.Settings.GetIntSetting("WorldBorderThickness");
			int intSetting2 = this.Settings.GetIntSetting("WorldBorderRange");
			byte b = (byte)ElementLoader.elements.IndexOf(WorldGen.unobtaniumElement);
			float temperature = WorldGen.unobtaniumElement.defaultValues.temperature;
			float mass = WorldGen.unobtaniumElement.defaultValues.mass;
			int num = 0;
			int num2 = 0;
			int num3 = world.size.y - 32;
			if (!boolSetting)
			{
				num3 = Math.Max(0, num3 - intSetting - 2 * intSetting2);
				num = -intSetting2;
				num2 = -intSetting2;
			}
			for (int i = num3; i >= 0; i--)
			{
				num = Mathf.Max(-intSetting2, Mathf.Min(num + rnd.RandomRange(-2, 2), intSetting2));
				for (int j = 0; j < intSetting + num; j++)
				{
					int num4 = Grid.XYToCell(j, i);
					borderCells.Add(num4);
					cells[num4].SetValues(b, temperature, mass);
				}
				num2 = Mathf.Max(-intSetting2, Mathf.Min(num2 + rnd.RandomRange(-2, 2), intSetting2));
				for (int k = 0; k < intSetting + num2; k++)
				{
					int num5 = Grid.XYToCell(world.size.x - 1 - k, i);
					borderCells.Add(num5);
					cells[num5].SetValues(b, temperature, mass);
				}
			}
			int num6 = 0;
			for (int l = 0; l < world.size.x; l++)
			{
				num6 = Mathf.Max(-intSetting2, Mathf.Min(num6 + rnd.RandomRange(-2, 2), intSetting2));
				for (int m = 0; m < intSetting + num6; m++)
				{
					int num7 = Grid.XYToCell(l, m);
					borderCells.Add(num7);
					cells[num7].SetValues(b, temperature, mass);
				}
			}
			if (boolSetting)
			{
				int num8 = 0;
				for (int n = 0; n < world.size.x; n++)
				{
					num8 = Mathf.Max(-intSetting2, Mathf.Min(num8 + rnd.RandomRange(-2, 2), intSetting2));
					for (int num9 = 0; num9 < intSetting + num8; num9++)
					{
						int num10 = Grid.XYToCell(n, world.size.y - 1 - num9);
						borderCells.Add(num10);
						cells[num10].SetValues(b, temperature, mass);
					}
				}
			}
		}

		private void SetupNoise(WorldGen.OfflineCallbackFunction updateProgressFn)
		{
			updateProgressFn(UI.WORLDGEN.BUILDNOISESOURCE.key, 0f, WorldGenProgressStages.Stages.SetupNoise);
			this.heatSource = this.BuildNoiseSource(this.data.world.size.x, this.data.world.size.y, "noise/Heat");
			updateProgressFn(UI.WORLDGEN.BUILDNOISESOURCE.key, 100f, WorldGenProgressStages.Stages.SetupNoise);
		}

		public NoiseMapBuilderPlane BuildNoiseSource(int width, int height, string name)
		{
			global::ProcGen.Noise.Tree tree = SettingsCache.noise.GetTree(name, SettingsCache.GetPath());
			return this.BuildNoiseSource(width, height, tree);
		}

		public NoiseMapBuilderPlane BuildNoiseSource(int width, int height, global::ProcGen.Noise.Tree tree)
		{
			Vector2f lowerBound = tree.settings.lowerBound;
			Vector2f upperBound = tree.settings.upperBound;
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
				updateProgressFn(UI.WORLDGEN.GENERATENOISE.key, (float)((int)(25.0 * (double)((float)line / (float)this.data.world.size.y))), WorldGenProgressStages.Stages.GenerateNoise);
			};
			noiseMapBuilderCallback = delegate(int line)
			{
				updateProgressFn(UI.WORLDGEN.GENERATENOISE.key, (float)((int)(25.0 + 25.0 * (double)((float)line / (float)this.data.world.size.y))), WorldGenProgressStages.Stages.GenerateNoise);
			};
			if (noiseMapBuilderCallback == null)
			{
				global::Debug.LogError("nupd is null", null);
			}
			this.data.world.heatOffset = WorldGen.GenerateNoise(vector, SettingsCache.noise.GetZoomForTree("noise/Heat"), this.heatSource, this.data.world.size.x, this.data.world.size.y, noiseMapBuilderCallback);
			this.data.world.data = new float[this.data.world.heatOffset.Length];
			this.data.world.density = new float[this.data.world.heatOffset.Length];
			this.data.world.overrides = new float[this.data.world.heatOffset.Length];
			updateProgressFn(UI.WORLDGEN.NORMALISENOISE.key, 50f, WorldGenProgressStages.Stages.GenerateNoise);
			if (SettingsCache.noise.ShouldNormaliseTree("noise/Heat"))
			{
				WorldGen.Normalise(this.data.world.heatOffset);
			}
			updateProgressFn(UI.WORLDGEN.NORMALISENOISE.key, 100f, WorldGenProgressStages.Stages.GenerateNoise);
		}

		public void WriteOverWorldNoise(WorldGen.OfflineCallbackFunction updateProgressFn)
		{
			float num = (float)this.OverworldCells.Count;
			float perCell = 100f / num;
			float currentProgress = 0f;
			foreach (TerrainCell terrainCell in this.OverworldCells)
			{
				global::ProcGen.Noise.Tree tree = SettingsCache.noise.GetTree("noise/Default", SettingsCache.GetPath());
				global::ProcGen.Noise.Tree tree2 = SettingsCache.noise.GetTree("noise/DefaultCave", SettingsCache.GetPath());
				global::ProcGen.Noise.Tree tree3 = SettingsCache.noise.GetTree("noise/DefaultDensity", SettingsCache.GetPath());
				SubWorld subWorld = this.Settings.GetSubWorld(terrainCell.node.type);
				if (subWorld == null)
				{
					global::Debug.Log("Couldnt find Subworld for overworld node [" + terrainCell.node.type + "] using defaults", null);
				}
				else
				{
					if (subWorld.biomeNoise != null)
					{
						global::ProcGen.Noise.Tree tree4 = SettingsCache.noise.GetTree(subWorld.biomeNoise);
						if (tree4 != null)
						{
							tree = tree4;
						}
					}
					if (subWorld.overrideNoise != null)
					{
						global::ProcGen.Noise.Tree tree5 = SettingsCache.noise.GetTree(subWorld.overrideNoise);
						if (tree5 != null)
						{
							tree2 = tree5;
						}
					}
					if (subWorld.densityNoise != null)
					{
						global::ProcGen.Noise.Tree tree6 = SettingsCache.noise.GetTree(subWorld.densityNoise);
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
				NoiseMapBuilderPlane noiseMapBuilderPlane = this.BuildNoiseSource(num2, height, tree);
				NoiseMap noiseMap = WorldGen.BuildNoiseMap(vector, tree.settings.zoom, noiseMapBuilderPlane, num2, height, noiseMapBuilderCallback);
				NoiseMapBuilderPlane noiseMapBuilderPlane2 = this.BuildNoiseSource(num2, height, tree2);
				NoiseMap noiseMap2 = WorldGen.BuildNoiseMap(vector, tree2.settings.zoom, noiseMapBuilderPlane2, num2, height, noiseMapBuilderCallback);
				NoiseMapBuilderPlane noiseMapBuilderPlane3 = this.BuildNoiseSource(num2, height, tree3);
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
							this.BaseNoiseMap[num11] = noiseMap.GetValue(num12, num13);
							this.OverrideMap[num11] = noiseMap2.GetValue(num12, num13);
							this.DensityMap[num11] = noiseMap3.GetValue(num12, num13);
							num5 = Mathf.Min(this.BaseNoiseMap[num11], num5);
							num6 = Mathf.Max(this.BaseNoiseMap[num11], num6);
							num7 = Mathf.Min(this.OverrideMap[num11], num7);
							num8 = Mathf.Max(this.OverrideMap[num11], num8);
							num9 = Mathf.Min(this.DensityMap[num11], num9);
							num10 = Mathf.Max(this.DensityMap[num11], num10);
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
						this.BaseNoiseMap[list[i]] = (this.BaseNoiseMap[list[i]] - num5) / num14;
					}
				}
				if (tree2.settings.normalise)
				{
					float num15 = num8 - num7;
					for (int j = 0; j < list.Count; j++)
					{
						this.OverrideMap[list[j]] = (this.OverrideMap[list[j]] - num7) / num15;
					}
				}
				if (tree3.settings.normalise)
				{
					float num16 = num10 - num9;
					for (int k = 0; k < list.Count; k++)
					{
						this.DensityMap[list[k]] = (this.DensityMap[list[k]] - num9) / num16;
					}
				}
				currentProgress += perCell;
			}
		}

		private float GetValue(Chunk chunk, Vector2I pos)
		{
			int num = pos.x + this.data.world.size.x * pos.y;
			if (num < 0 || num >= chunk.data.Length)
			{
				throw new ArgumentOutOfRangeException("chunkDataIndex [" + num + "]", "chunk data length [" + chunk.data.Length + "]");
			}
			float num2 = chunk.data[num];
			float num3 = (float)(pos.y + chunk.offset.y);
			float num4 = num3 / (float)this.data.world.size.y;
			if (num4 > 0.9f)
			{
				num2 = 0f;
			}
			else if (num4 > 0.85f)
			{
				float num5 = Mathf.Clamp01((0.9f - num4) / 0.049999952f);
				num2 *= num5;
			}
			return num2;
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

		public void SaveWorldGen()
		{
			try
			{
				Manager.Clear();
				WorldGenSave worldGenSave = new WorldGenSave();
				worldGenSave.version = new Vector2I(1, 1);
				worldGenSave.stats = this.stats;
				worldGenSave.data = this.data;
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

		public bool LoadWorldGen()
		{
			try
			{
				WorldGenSave worldGenSave = new WorldGenSave();
				FastReader fastReader = new FastReader(File.ReadAllBytes(WorldGen.WORLDGEN_SAVE_FILENAME));
				Manager.DeserializeDirectory(fastReader);
				Deserializer.Deserialize(worldGenSave, fastReader);
				this.stats = worldGenSave.stats;
				this.data = worldGenSave.data;
				if (worldGenSave.version.x != 1 || worldGenSave.version.y > 1)
				{
					Output.LogError(string.Concat(new object[]
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
					}));
					this.wasLoaded = false;
				}
				else
				{
					this.wasLoaded = true;
				}
			}
			catch (Exception ex)
			{
				Output.LogError(new object[] { "LoadWorldGenSim Error!\n", ex.Message, ex.StackTrace });
				this.wasLoaded = false;
			}
			return this.wasLoaded;
		}

		public SimSaveFileStructure LoadWorldGenSim()
		{
			this.LoadWorldGen();
			SimSaveFileStructure simSaveFileStructure = new SimSaveFileStructure();
			try
			{
				FastReader fastReader = new FastReader(File.ReadAllBytes(WorldGen.SIM_SAVE_FILENAME));
				Manager.DeserializeDirectory(fastReader);
				Deserializer.Deserialize(simSaveFileStructure, fastReader);
			}
			catch (Exception ex)
			{
				Output.LogError(new object[] { "LoadWorldGenSim Error!\n", ex.Message, ex.StackTrace });
				this.wasLoaded = false;
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
			return simSaveFileStructure;
		}

		public void DrawDebug()
		{
		}

		private const string _SIM_SAVE_FILENAME = "WorldGenSimSave.dat";

		private const string _WORLDGEN_SAVE_FILENAME = "WorldGenDataSave.dat";

		private const int heatScale = 2;

		private const int UNPASSABLE_EDGE_COUNT = 4;

		private const string heat_noise_name = "noise/Heat";

		private const string base_noise_name = "noise/Default";

		private const string cave_noise_name = "noise/DefaultCave";

		private const string density_noise_name = "noise/DefaultDensity";

		public const int WORLDGEN_SAVE_MAJOR_VERSION = 1;

		public const int WORLDGEN_SAVE_MINOR_VERSION = 1;

		public const int WORLD_OFFSET_Y = 0;

		public static Element voidElement;

		public static Element vacuumElement;

		public static Element katairiteElement;

		public static Element unobtaniumElement;

		public static List<string> diseaseIds = new List<string>
		{
			"FoodPoisoning",
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			"SlimeLung"
		};

		public bool isRunningDebugGen;

		private Data data;

		private WorldGen.OfflineCallbackFunction successCallbackFn;

		private bool running = true;

		private Thread generateThread;

		private Thread renderThread;

		private Action<OfflineWorldGen.ErrorInfo> errorCallback;

		private SeededRandom myRandom;

		private NoiseMapBuilderPlane heatSource;

		private bool wasLoaded;

		public int polyIndex;

		[EnumFlags]
		public WorldGen.DebugFlags drawOptions;

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
