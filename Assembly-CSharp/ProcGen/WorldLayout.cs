using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Delaunay.Geo;
using KSerialization;
using ProcGen.Map;
using ProcGenGame;
using UnityEngine;

namespace ProcGen
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class WorldLayout
	{
		public WorldLayout(int seed)
		{
			this.localGraph = new MapGraph(seed);
			this.overworldGraph = new MapGraph(seed);
			this.SetSeed(seed);
		}

		public WorldLayout(int width, int height, int seed)
			: this(seed)
		{
			this.mapWidth = width;
			this.mapHeight = height;
		}

		[Serialize]
		public int mapWidth { get; private set; }

		[Serialize]
		public int mapHeight { get; private set; }

		public bool layoutOK { get; private set; }

		public static LevelLayer levelLayerGradient { get; private set; }

		public void SetSeed(int seed)
		{
			this.myRandom = new SeededRandom(seed);
			this.localGraph.SetSeed(seed);
			this.overworldGraph.SetSeed(seed);
		}

		public VoronoiTree GetVoronoiTree()
		{
			return this.voronoiTree;
		}

		public static void SetLayerGradient(LevelLayer newGradient)
		{
			WorldLayout.levelLayerGradient = newGradient;
		}

		public static string GetNodeTypeFromLayers(Vector2 point, float mapHeight, SeededRandom rnd)
		{
			string text = WorldGenTags.TheVoid.Name;
			int num = rnd.RandomRange(0, WorldLayout.levelLayerGradient[WorldLayout.levelLayerGradient.Count - 1].content.Count);
			text = WorldLayout.levelLayerGradient[WorldLayout.levelLayerGradient.Count - 1].content[num];
			for (int i = 0; i < WorldLayout.levelLayerGradient.Count; i++)
			{
				if (point.y < WorldLayout.levelLayerGradient[i].maxValue * mapHeight)
				{
					int num2 = rnd.RandomRange(0, WorldLayout.levelLayerGradient[i].content.Count);
					text = WorldLayout.levelLayerGradient[i].content[num2];
					break;
				}
			}
			return text;
		}

		public VoronoiTree GenerateOverworld()
		{
			VoronoiDiagram.Site site = new VoronoiDiagram.Site(0U, new Vector2((float)(this.mapWidth / 2), (float)(this.mapHeight / 2)), 1f);
			this.topEdge = new LineSegment(new Vector2?(new Vector2(0f, (float)(this.mapHeight - 5))), new Vector2?(new Vector2((float)this.mapWidth, (float)(this.mapHeight - 5))));
			this.bottomEdge = new LineSegment(new Vector2?(new Vector2(0f, 5f)), new Vector2?(new Vector2((float)this.mapWidth, 5f)));
			this.leftEdge = new LineSegment(new Vector2?(new Vector2(5f, 0f)), new Vector2?(new Vector2(5f, (float)this.mapHeight)));
			this.rightEdge = new LineSegment(new Vector2?(new Vector2((float)(this.mapWidth - 5), 0f)), new Vector2?(new Vector2((float)(this.mapWidth - 5), (float)this.mapHeight)));
			site.poly = new Polygon(new Rect(0f, 0f, (float)this.mapWidth, (float)this.mapHeight));
			this.voronoiTree = new VoronoiTree(site, null, this.myRandom.seed);
			VoronoiNode.maxIndex = 0U;
			float defaultFloat = WorldGen.Settings.GetDefaultFloat("OverworldDensityMin");
			float defaultFloat2 = WorldGen.Settings.GetDefaultFloat("OverworldDensityMax");
			float num = this.myRandom.RandomRange(defaultFloat, defaultFloat2);
			float defaultFloat3 = WorldGen.Settings.GetDefaultFloat("OverworldAvoidRadius");
			object obj = Enum.Parse(typeof(PointGenerator.SampleBehaviour), WorldGen.Settings.defaults.data["OverworldSampleBehaviour"] as string);
			PointGenerator.SampleBehaviour sampleBehaviour = PointGenerator.SampleBehaviour.PoissonDisk;
			if (obj != null)
			{
				sampleBehaviour = (PointGenerator.SampleBehaviour)((int)obj);
			}
			Node node = this.overworldGraph.AddNode(WorldGenTags.StartWorld.Name);
			node.SetPosition(new Vector2((float)(this.mapWidth / 2), (float)(this.mapHeight / 2)));
			List<Vector2> list = new List<Vector2>();
			list.Add(node.position);
			VoronoiNode voronoiNode = this.voronoiTree.AddSite(new VoronoiDiagram.Site((uint)node.node.Id, node.position, 1f), VoronoiNode.NodeType.Internal);
			List<Vector2> randomPoints = PointGenerator.GetRandomPoints(site.poly, num, defaultFloat3, list, sampleBehaviour, false, this.myRandom, false, true);
			int defaultInt = WorldGen.Settings.GetDefaultInt("OverworldMaxNodes");
			if (randomPoints.Count > defaultInt)
			{
				randomPoints.ShuffleSeeded<Vector2>(this.myRandom.RandomSource());
				randomPoints.RemoveRange(defaultInt, randomPoints.Count - defaultInt);
			}
			for (int i = 0; i < randomPoints.Count; i++)
			{
				Node node2 = this.overworldGraph.AddNode(WorldGenTags.UnassignedNode.Name);
				node2.SetPosition(randomPoints[i]);
				VoronoiNode voronoiNode2 = this.voronoiTree.AddSite(new VoronoiDiagram.Site((uint)node2.node.Id, node2.position, 1f), VoronoiNode.NodeType.Internal);
				voronoiNode2.tags.Add(WorldGenTags.UnassignedNode);
				node2.tags.Add(WorldGenTags.UnassignedNode);
			}
			this.voronoiTree.ComputeChildren(this.myRandom.seed + 1);
			this.voronoiTree.AddTagToChildren(WorldGenTags.Overworld);
			voronoiNode.AddTagToNeighbors(WorldGenTags.StartNear);
			List<VoronoiNode> siblings = voronoiNode.GetSiblings();
			List<VoronoiNode> neighbors = voronoiNode.GetNeighbors();
			for (int j = 0; j < neighbors.Count; j++)
			{
				VoronoiNode voronoiNode3 = neighbors[j];
				if (siblings.Contains(voronoiNode3))
				{
					siblings.Remove(voronoiNode3);
				}
				List<VoronoiNode> neighbors2 = voronoiNode3.GetNeighbors();
				for (int k = 0; k < neighbors2.Count; k++)
				{
					VoronoiNode voronoiNode4 = neighbors2[k];
					if (siblings.Contains(voronoiNode4))
					{
						siblings.Remove(voronoiNode4);
					}
					if (!voronoiNode4.tags.Contains(WorldGenTags.StartNear) && !voronoiNode4.tags.Contains(WorldGenTags.StartWorld))
					{
						voronoiNode4.AddTag(WorldGenTags.StartMedium);
					}
				}
			}
			for (int l = 0; l < siblings.Count; l++)
			{
				VoronoiNode voronoiNode5 = siblings[l];
				voronoiNode5.AddTag(WorldGenTags.StartFar);
			}
			int defaultInt2 = WorldGen.Settings.GetDefaultInt("OverworldRelaxIterations");
			float defaultFloat4 = WorldGen.Settings.GetDefaultFloat("OverworldRelaxEnergyMin");
			this.voronoiTree.RelaxRecursive(0, defaultInt2, defaultFloat4);
			for (int m = 0; m < this.voronoiTree.ChildCount(); m++)
			{
				VoronoiNode child = this.voronoiTree.GetChild(m);
				Node node3 = this.overworldGraph.FindNodeByID(child.site.id);
				node3.SetPosition(child.site.position);
			}
			this.TagTopAndBottomSites(WorldGenTags.NearSurface, WorldGenTags.NearDepths);
			this.ConvertUnknownCells();
			if (WorldGen.Settings.defaults.overworldAddTags != null)
			{
				foreach (string text in WorldGen.Settings.defaults.overworldAddTags)
				{
					int num2 = this.myRandom.RandomSource().Next(this.voronoiTree.ChildCount());
					VoronoiNode child2 = this.voronoiTree.GetChild(num2);
					child2.AddTag(new Tag(text));
				}
			}
			this.FlatternOverworld();
			this.AddSubworldChildren();
			this.ConvertNearStartAndHotEdgeToType(voronoiNode, "UNPASSABLE");
			this.GetStartLocation();
			return this.voronoiTree;
		}

		private void ConvertNearStartAndHotEdgeToType(VoronoiNode startArea, string newType)
		{
			TagSet tagSet = new TagSet();
			tagSet.Add(new Tag(Temperature.Range.Hot.ToString()));
			tagSet.Add(new Tag(Temperature.Range.VeryHot.ToString()));
			VoronoiNode.SplitCommand splitCommand = new VoronoiNode.SplitCommand();
			splitCommand.SplitFunction = new Action<VoronoiTree, VoronoiNode.SplitCommand>(this.SplitFunction);
			int num = 0;
			List<KeyValuePair<VoronoiNode, LineSegment>> neighborsByEdge = startArea.GetNeighborsByEdge();
			for (int i = 0; i < neighborsByEdge.Count; i++)
			{
				KeyValuePair<VoronoiNode, LineSegment> keyValuePair = neighborsByEdge[i];
				if (keyValuePair.Key == null)
				{
					global::Debug.Log("Weird, kvp.Key NULL", null);
				}
				else
				{
					VoronoiTree voronoiTree = (VoronoiTree)keyValuePair.Key;
					if (voronoiTree == null)
					{
						global::Debug.Log(string.Concat(new object[]
						{
							"Weird, VT null [",
							keyValuePair.Key.type,
							"] site ID: ",
							keyValuePair.Key.site.id
						}), null);
					}
					else
					{
						LineSegment lineSegment = keyValuePair.Value;
						Vector2 value = lineSegment.Center().Value;
						Vector2 vector = 2f * (value - startArea.site.poly.Centroid()).normalized;
						Vector2 vector2 = lineSegment.p1.Value - lineSegment.p0.Value;
						lineSegment = new LineSegment(new Vector2?(value + vector2 * 0.55f + vector * 2f), new Vector2?(value - vector2 * 0.55f + vector * 2f));
						if (voronoiTree.tags != null && voronoiTree.tags.ContainsOne(tagSet))
						{
							List<VoronoiLeaf> list = new List<VoronoiLeaf>();
							voronoiTree.GetIntersectingLeafNodes(lineSegment, list);
							for (int j = 0; j < list.Count; j++)
							{
								list[j].AddTag(WorldGenTags.HighDensitySplit);
								list[j].Split(splitCommand);
							}
							VoronoiNode.maxDepth = this.voronoiTree.MaxDepth(0);
							this.voronoiTree.ForceLowestToLeaf();
							list.Clear();
							voronoiTree.GetIntersectingLeafNodes(lineSegment, list);
							for (int k = 0; k < list.Count; k++)
							{
								this.localGraph.FindNodeByID(list[k].site.id).SetType(newType);
							}
							num++;
						}
					}
				}
			}
		}

		private void ConvertUnknownCells()
		{
			TagSet tagSet = new TagSet();
			tagSet.Add(WorldGenTags.NearDepths);
			tagSet.Add(WorldGenTags.StartFar);
			TagSet tagSet2 = new TagSet();
			tagSet2.Add(WorldGenTags.NearSurface);
			tagSet2.Add(WorldGenTags.StartFar);
			List<VoronoiNode> list = new List<VoronoiNode>();
			this.voronoiTree.GetNodesWithTag(WorldGenTags.UnassignedNode, list);
			List<SubWorld> list2 = new List<SubWorld>(WorldGen.Settings.subworlds.zones.Values);
			list2.Remove(WorldGen.Settings.subworlds.zones[WorldGenTags.StartWorld.Name]);
			Dictionary<Temperature.Range, List<SubWorld>> dictionary = new Dictionary<Temperature.Range, List<SubWorld>>();
			Temperature.Range range;
			foreach (object obj in Enum.GetValues(typeof(Temperature.Range)))
			{
				range = (Temperature.Range)((int)obj);
				dictionary.Add(range, list2.FindAll((SubWorld sw) => sw.temperatureRange == range));
			}
			foreach (VoronoiNode voronoiNode in list)
			{
				Node node = this.overworldGraph.FindNodeByID(voronoiNode.site.id);
				voronoiNode.tags.Remove(WorldGenTags.UnassignedNode);
				node.tags.Remove(WorldGenTags.UnassignedNode);
				HashSet<SubWorld> hashSet = new HashSet<SubWorld>();
				hashSet.UnionWith(dictionary[Temperature.Range.HumanWarm]);
				hashSet.UnionWith(dictionary[Temperature.Range.HumanHot]);
				if (voronoiNode.tags.Contains(WorldGenTags.StartNear))
				{
					hashSet.UnionWith(dictionary[Temperature.Range.HumanWarm]);
					hashSet.UnionWith(dictionary[Temperature.Range.Cool]);
					hashSet.UnionWith(dictionary[Temperature.Range.Mild]);
				}
				if (voronoiNode.tags.Contains(WorldGenTags.StartMedium))
				{
					hashSet.UnionWith(dictionary[Temperature.Range.Mild]);
					hashSet.UnionWith(dictionary[Temperature.Range.Cool]);
					hashSet.UnionWith(dictionary[Temperature.Range.Cold]);
					hashSet.UnionWith(dictionary[Temperature.Range.HumanWarm]);
					hashSet.UnionWith(dictionary[Temperature.Range.HumanHot]);
				}
				if (voronoiNode.tags.Contains(WorldGenTags.StartFar))
				{
					hashSet.UnionWith(dictionary[Temperature.Range.HumanWarm]);
					hashSet.UnionWith(dictionary[Temperature.Range.HumanHot]);
					hashSet.UnionWith(dictionary[Temperature.Range.Cool]);
					hashSet.UnionWith(dictionary[Temperature.Range.Cold]);
					hashSet.UnionWith(dictionary[Temperature.Range.Hot]);
				}
				if (voronoiNode.tags.ContainsAll(tagSet))
				{
					hashSet.UnionWith(dictionary[Temperature.Range.HumanWarm]);
					hashSet.UnionWith(dictionary[Temperature.Range.HumanHot]);
					hashSet.UnionWith(dictionary[Temperature.Range.VeryHot]);
				}
				else if (voronoiNode.tags.ContainsAll(tagSet2))
				{
					hashSet.UnionWith(dictionary[Temperature.Range.VeryCold]);
					hashSet.UnionWith(dictionary[Temperature.Range.Cool]);
					hashSet.UnionWith(dictionary[Temperature.Range.Cold]);
				}
				if (voronoiNode.tags.Contains(WorldGenTags.NearSurface) || voronoiNode.tags.Contains(WorldGenTags.AtSurface))
				{
					hashSet.Clear();
					hashSet.UnionWith(dictionary[Temperature.Range.VeryCold]);
					hashSet.UnionWith(dictionary[Temperature.Range.ExtremelyCold]);
				}
				else if (voronoiNode.tags.Contains(WorldGenTags.NearDepths) || voronoiNode.tags.Contains(WorldGenTags.AtDepths))
				{
					hashSet.Clear();
					hashSet.UnionWith(dictionary[Temperature.Range.VeryHot]);
					hashSet.UnionWith(dictionary[Temperature.Range.ExtremelyHot]);
				}
				List<SubWorld> list3 = new List<SubWorld>(hashSet);
				list3.ShuffleSeeded<SubWorld>(this.myRandom.RandomSource());
				string text = "NONE";
				foreach (KeyValuePair<string, SubWorld> keyValuePair in WorldGen.Settings.subworlds.zones)
				{
					if (list3.Count > 0)
					{
						if (keyValuePair.Value == list3[0])
						{
							text = keyValuePair.Key;
							break;
						}
					}
					else
					{
						global::Debug.LogWarning("No allowedSubworld types. Using default.", null);
						text = "subworldDefault";
					}
				}
				node.SetType(text);
				foreach (string text2 in list3[0].tags)
				{
					voronoiNode.AddTag(new Tag(text2));
				}
			}
		}

		public void ComputeSubWorlds(List<TemplateContainer> poi)
		{
			try
			{
				this.SplitTopAndBottomSites();
				this.SplitLargeStartingSites();
				this.PropagateStartTag();
				this.SetTemperatureTags();
				this.TagTopAndBottomSites(WorldGenTags.AtSurface, WorldGenTags.AtDepths);
				this.TagEdgeSites(WorldGenTags.AtLeftEdge, WorldGenTags.AtRightEdge);
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				string stackTrace = ex.StackTrace;
				Output.LogError(new object[] { "ex: " + message + " " + stackTrace });
			}
		}

		private void FlatternOverworld()
		{
			try
			{
				for (int i = 0; i < this.voronoiTree.ChildCount(); i++)
				{
					VoronoiNode child = this.voronoiTree.GetChild(i);
					if (child.type == VoronoiNode.NodeType.Internal)
					{
						VoronoiTree voronoiTree = child as VoronoiTree;
						Node node = this.overworldGraph.FindNodeByID(voronoiTree.site.id);
						Cell cell = this.overworldGraph.GetCell(node.position, node.node, true);
						cell.tags.Union(voronoiTree.tags);
					}
				}
				for (int j = 0; j < this.voronoiTree.ChildCount(); j++)
				{
					VoronoiNode child2 = this.voronoiTree.GetChild(j);
					if (child2.type == VoronoiNode.NodeType.Internal)
					{
						VoronoiTree voronoiTree2 = child2 as VoronoiTree;
						List<KeyValuePair<VoronoiNode, LineSegment>> neighborsByEdge = voronoiTree2.GetNeighborsByEdge();
						for (int k = 0; k < neighborsByEdge.Count; k++)
						{
							KeyValuePair<VoronoiNode, LineSegment> keyValuePair = neighborsByEdge[k];
							MapGraph mapGraph = this.overworldGraph;
							Vector2? p = keyValuePair.Value.p0;
							mapGraph.GetCorner(p.Value, true);
							MapGraph mapGraph2 = this.overworldGraph;
							Vector2? p2 = keyValuePair.Value.p1;
							mapGraph2.GetCorner(p2.Value, true);
						}
					}
				}
				TagSet tagSet = new TagSet();
				tagSet.Add(WorldGenTags.NearSurface);
				tagSet.Add(WorldGenTags.NearDepths);
				for (int l = 0; l < this.voronoiTree.ChildCount(); l++)
				{
					VoronoiNode child3 = this.voronoiTree.GetChild(l);
					if (child3.type == VoronoiNode.NodeType.Internal)
					{
						VoronoiTree voronoiTree3 = child3 as VoronoiTree;
						Node node2 = this.overworldGraph.FindNodeByID(voronoiTree3.site.id);
						Cell cell2 = this.overworldGraph.GetCell(node2.node);
						List<KeyValuePair<VoronoiNode, LineSegment>> neighborsByEdge2 = voronoiTree3.GetNeighborsByEdge();
						for (int m = 0; m < neighborsByEdge2.Count; m++)
						{
							KeyValuePair<VoronoiNode, LineSegment> keyValuePair2 = neighborsByEdge2[m];
							MapGraph mapGraph3 = this.overworldGraph;
							Vector2? p3 = keyValuePair2.Value.p0;
							Corner corner = mapGraph3.GetCorner(p3.Value, false);
							MapGraph mapGraph4 = this.overworldGraph;
							Vector2? p4 = keyValuePair2.Value.p1;
							Corner corner2 = mapGraph4.GetCorner(p4.Value, false);
							VoronoiNode key = keyValuePair2.Key;
							Edge edge;
							if (key != null)
							{
								Node node3 = this.overworldGraph.FindNodeByID(key.site.id);
								Cell cell3 = this.overworldGraph.GetCell(node3.node);
								edge = this.overworldGraph.GetEdge(corner, corner2, cell2, cell3, true);
								SubWorld subWorld = WorldGen.Settings.subworlds.GetSubWorld(node2.type);
								SubWorld subWorld2 = WorldGen.Settings.subworlds.GetSubWorld(node3.type);
								if (node2.type == node3.type || subWorld.zoneType == subWorld2.zoneType || (cell2.tags.ContainsOne(tagSet) && cell3.tags.ContainsOne(tagSet)))
								{
									edge.tags.Add(WorldGenTags.EdgeOpen);
								}
								else
								{
									edge.tags.Add(WorldGenTags.EdgeClosed);
								}
								cell3.Add(edge);
							}
							else
							{
								edge = this.overworldGraph.GetEdge(corner, corner2, cell2, cell2, true);
								edge.tags.Add(WorldGenTags.EdgeUnpassable);
							}
							cell2.Add(edge);
						}
					}
				}
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				string stackTrace = ex.StackTrace;
				Output.LogError(new object[] { "ex: " + message + " " + stackTrace });
			}
			this.UpdateEdgesAroundStart();
		}

		private void UpdateEdgesAroundStart()
		{
			Node node = this.overworldGraph.FindNode((Node n) => n.type == WorldGenTags.StartWorld.Name);
			Cell cell = this.overworldGraph.GetCell(node.node);
			foreach (Edge edge in cell.edges)
			{
				edge.tags.Add(WorldGenTags.RoomBorderMixed);
			}
		}

		private void AddSubworldChildren()
		{
			TagSet tagSet = new TagSet();
			tagSet.Add(WorldGenTags.Overworld);
			TagSet tagSet2 = new TagSet(WorldGen.Settings.defaults.defaultMoveTags);
			VoronoiNode.SplitCommand splitCommand = new VoronoiNode.SplitCommand();
			splitCommand.dontCopyTags = tagSet;
			splitCommand.moveTags = tagSet2;
			splitCommand.SplitFunction = new Action<VoronoiTree, VoronoiNode.SplitCommand>(this.SplitFunction);
			for (int i = 0; i < this.voronoiTree.ChildCount(); i++)
			{
				VoronoiNode child = this.voronoiTree.GetChild(i);
				if (child.type == VoronoiNode.NodeType.Internal)
				{
					VoronoiTree voronoiTree = child as VoronoiTree;
					Node node = this.overworldGraph.FindNodeByID(voronoiTree.site.id);
					SubWorld subWorld = WorldGen.Settings.subworlds.zones[node.type];
					voronoiTree.AddTag(new Tag(node.type));
					voronoiTree.AddTag(new Tag(subWorld.temperatureRange.ToString()));
					this.GenerateChildren(subWorld, voronoiTree, this.localGraph, (float)this.mapHeight, i + this.myRandom.seed);
					int num = voronoiTree.ChildCount();
					if (num < subWorld.minChildCount)
					{
						voronoiTree.AddTag(WorldGenTags.DEBUG_SplitForChildCount);
						splitCommand.dontCopyTags = tagSet;
						splitCommand.minChildCount = subWorld.minChildCount;
						voronoiTree.Split(splitCommand);
						if (subWorld.biomes != null && subWorld.biomes.Count > 0)
						{
							for (int j = num; j < voronoiTree.ChildCount(); j++)
							{
								WeightedBiome weightedBiome = WeightedRandom.Choose<WeightedBiome>(subWorld.biomes, this.myRandom);
								Node node2 = this.localGraph.FindNodeByID(voronoiTree.GetChild(j).site.id);
								node2.SetType(weightedBiome.name);
								voronoiTree.GetChild(j).AddTag(new Tag(node2.type));
							}
						}
						else
						{
							for (int k = num; k < voronoiTree.ChildCount(); k++)
							{
								Node node3 = this.localGraph.FindNodeByID(voronoiTree.GetChild(k).site.id);
								node3.SetType(WorldLayout.GetNodeTypeFromLayers(voronoiTree.site.position, (float)this.mapHeight, this.myRandom));
								voronoiTree.GetChild(k).AddTag(new Tag(node3.type));
							}
						}
					}
					voronoiTree.RelaxRecursive(0, 10, 1f);
					List<VoronoiNode> list = new List<VoronoiNode>();
					voronoiTree.GetNodesWithTag(WorldGenTags.Feature, list);
					splitCommand.dontCopyTags = new TagSet
					{
						WorldGenTags.Feature,
						WorldGenTags.SplitOnParentDensity
					};
					for (int l = 0; l < list.Count; l++)
					{
						if (!list[l].tags.Contains(WorldGenTags.CenteralFeature))
						{
							if (list[l].tags.Contains(WorldGenTags.SplitOnParentDensity))
							{
								list[l].Split(splitCommand);
							}
							if (list[l].tags.Contains(WorldGenTags.SplitTwice))
							{
								VoronoiTree voronoiTree2 = list[l].Split(splitCommand);
								if (voronoiTree2.ChildCount() <= 1)
								{
									global::Debug.LogError("split did not work.", null);
								}
								for (int m = 0; m < voronoiTree2.ChildCount(); m++)
								{
									VoronoiNode child2 = voronoiTree2.GetChild(m);
									child2.Split(splitCommand);
								}
							}
						}
					}
				}
			}
			VoronoiNode.maxDepth = this.voronoiTree.MaxDepth(0);
		}

		private List<Vector2> GetPoints(string name, LoggerSSF log, int minPointCount, Polygon boundingArea, float density, float avoidRadius, List<Vector2> avoidPoints, PointGenerator.SampleBehaviour sampleBehaviour, bool testInsideBounds, SeededRandom rnd, bool doShuffle = true, bool testAvoidPoints = true)
		{
			int num = 0;
			List<Vector2> randomPoints;
			do
			{
				randomPoints = PointGenerator.GetRandomPoints(boundingArea, density, avoidRadius, avoidPoints, sampleBehaviour, testInsideBounds, rnd, doShuffle, testAvoidPoints);
				if (randomPoints.Count < minPointCount)
				{
					density *= 0.8f;
					if (WorldGen.isRunningDebugGen)
					{
						global::Debug.LogWarning(string.Concat(new object[] { "Recaluclating points for ", name, " iter: ", num, " density:", density }), null);
					}
				}
				num++;
			}
			while (randomPoints.Count < minPointCount && num < 10);
			return randomPoints;
		}

		public void GenerateChildren(SubWorld sw, VoronoiTree node, Graph graph, float worldHeight, int seed)
		{
			SeededRandom seededRandom = new SeededRandom(seed);
			TagSet tagSet = new TagSet(WorldGen.Settings.defaults.defaultMoveTags);
			TagSet tagSet2 = new TagSet();
			if (tagSet != null)
			{
				for (int i = 0; i < tagSet.Count; i++)
				{
					Tag tag = tagSet[i];
					if (node.tags.Contains(tag))
					{
						node.tags.Remove(tag);
						tagSet2.Add(tag);
					}
				}
			}
			TagSet tagSet3 = new TagSet(node.tags);
			tagSet3.Remove(WorldGenTags.Overworld);
			for (int j = 0; j < sw.tags.Count; j++)
			{
				tagSet3.Add(new Tag(sw.tags[j]));
			}
			float randomValueWithinRange = sw.density.GetRandomValueWithinRange(seededRandom);
			Node node2 = sw.AddCenteralFeature(node, graph, tagSet3);
			List<Vector2> list = new List<Vector2>();
			if (node2 != null)
			{
				list.Add(node2.position);
				foreach (WeightedBiome weightedBiome in sw.biomes)
				{
					if (weightedBiome.name == node2.type)
					{
						TagSet tagSet4 = new TagSet(weightedBiome.tags);
						node2.tags.Union(tagSet4);
						break;
					}
				}
			}
			node.dontRelaxChildren = sw.dontRelaxChildren;
			int num = ((sw.features.Count <= 0) ? 2 : sw.features.Count);
			List<Vector2> points = this.GetPoints(sw.name, node.log, num, node.site.poly, randomValueWithinRange, sw.avoidRadius, list, sw.sampleBehaviour, true, seededRandom, true, sw.doAvoidPoints);
			for (int k = 0; k < sw.samplers.Count; k++)
			{
				list.AddRange(points);
				float randomValueWithinRange2 = sw.samplers[k].density.GetRandomValueWithinRange(seededRandom);
				List<Vector2> randomPoints = PointGenerator.GetRandomPoints(node.site.poly, randomValueWithinRange2, sw.samplers[k].avoidRadius, list, sw.samplers[k].sampleBehaviour, true, seededRandom, true, sw.samplers[k].doAvoidPoints);
				points.AddRange(randomPoints);
			}
			if (points.Count > 200)
			{
				points.RemoveRange(200, points.Count - 200);
			}
			if (points.Count < num)
			{
				string text = string.Empty;
				for (int l = 0; l < node.site.poly.Vertices.Count; l++)
				{
					text = text + node.site.poly.Vertices[l] + ", ";
				}
				if (WorldGen.isRunningDebugGen)
				{
				}
				return;
			}
			int m = 0;
			for (int n = 0; n < sw.features.Count; n++)
			{
				Feature feature = sw.features[n];
				TerrainFeature terrainFeature = null;
				TagSet tagSet5 = new TagSet(feature.tags.ToArray());
				if (WorldGen.Settings.features.TerrainFeatures.ContainsKey(feature.type) && WorldGen.Settings.features.TerrainFeatures[feature.type] != null)
				{
					terrainFeature = WorldGen.Settings.features.TerrainFeatures[feature.type];
					if (terrainFeature.tags != null)
					{
						tagSet5.Union(new TagSet(terrainFeature.tags.ToArray()));
					}
				}
				if (feature.excludesTags != null && feature.excludesTags.Count > 0)
				{
					tagSet5.Remove(new TagSet(feature.excludesTags.ToArray()));
				}
				tagSet5.Add(new Tag(feature.type));
				tagSet5.Add(WorldGenTags.Feature);
				TagSet tagSet6 = new TagSet();
				if (terrainFeature != null)
				{
					Feature defaultBiome = terrainFeature.defaultBiome;
					if (defaultBiome.tags != null)
					{
						TagSet tagSet7 = new TagSet(defaultBiome.tags);
						tagSet6.Union(tagSet7);
						tagSet5.Union(tagSet7);
					}
					foreach (WeightedBiome weightedBiome2 in sw.biomes)
					{
						if (weightedBiome2.name == defaultBiome.type)
						{
							tagSet5.Add(new Tag(defaultBiome.type));
							if (weightedBiome2.tags != null)
							{
								TagSet tagSet8 = new TagSet(weightedBiome2.tags);
								tagSet6.Union(tagSet8);
								tagSet5.Union(tagSet8);
							}
							break;
						}
					}
				}
				if (feature.type.Contains(WorldGenTags.River.Name) && m + 2 < points.Count)
				{
					Node node3 = graph.AddNode(feature.type);
					node3.biomeSpecificTags = new TagSet(tagSet6);
					VoronoiNode voronoiNode = node.AddSite(new VoronoiDiagram.Site((uint)node3.node.Id, node3.position, 1f), VoronoiNode.NodeType.Internal);
					voronoiNode.tags = new TagSet(tagSet5);
					node3.SetPosition(points[m++]);
					Node node4 = graph.AddNode(feature.type);
					node4.biomeSpecificTags = new TagSet(tagSet6);
					VoronoiNode voronoiNode2 = node.AddSite(new VoronoiDiagram.Site((uint)node4.node.Id, node4.position, 1f), VoronoiNode.NodeType.Internal);
					voronoiNode2.tags = new TagSet(tagSet5);
					node4.SetPosition(points[m++]);
					graph.AddArc(node3, node4, feature.type);
				}
				else if (m < points.Count)
				{
					Node node5 = graph.AddNode(feature.type);
					node5.biomeSpecificTags = new TagSet(tagSet6);
					node5.SetPosition((!(feature.type == WorldGenTags.StartLocation.Name)) ? points[m++] : node.site.poly.Centroid());
					VoronoiNode voronoiNode3 = node.AddSite(new VoronoiDiagram.Site((uint)node5.node.Id, node5.position, 1f), VoronoiNode.NodeType.Internal);
					voronoiNode3.tags = new TagSet(tagSet5);
				}
			}
			if (sw.features.Count > points.Count)
			{
			}
			while (m < points.Count)
			{
				TagSet tagSet9 = null;
				string text2;
				if (sw.biomes.Count > 0)
				{
					WeightedBiome weightedBiome3 = WeightedRandom.Choose<WeightedBiome>(sw.biomes, seededRandom);
					text2 = weightedBiome3.name;
					if (weightedBiome3.tags != null && weightedBiome3.tags.Count > 0)
					{
						tagSet9 = new TagSet(weightedBiome3.tags);
					}
				}
				else
				{
					text2 = WorldLayout.GetNodeTypeFromLayers(points[m], worldHeight, seededRandom);
				}
				Node node6 = graph.AddNode(text2);
				node6.biomeSpecificTags = tagSet9;
				node6.SetPosition(points[m]);
				VoronoiNode voronoiNode4 = node.AddSite(new VoronoiDiagram.Site((uint)node6.node.Id, node6.position, 1f), VoronoiNode.NodeType.Internal);
				voronoiNode4.tags = new TagSet(tagSet3);
				if (tagSet9 != null)
				{
					voronoiNode4.tags.Union(tagSet9);
				}
				voronoiNode4.AddTag(new Tag(text2));
				m++;
			}
			node.ComputeChildren(seededRandom.seed + 1);
			if (node.ChildCount() > 0)
			{
				for (int num2 = 0; num2 < tagSet2.Count; num2++)
				{
					global::Debug.Log(string.Format("Applying Moved Tag {0} to {1}", tagSet2[num2].Name, node.site.id), null);
					VoronoiNode child = node.GetChild(seededRandom.RandomSource().Next(node.ChildCount()));
					child.AddTag(tagSet2[num2]);
				}
			}
		}

		private void SplitTopAndBottomSites()
		{
			float defaultFloat = WorldGen.Settings.GetDefaultFloat("SplitTopAndBottomSitesMaxArea");
			TagSet tagSet = new TagSet();
			tagSet.Add(WorldGenTags.Overworld);
			TagSet tagSet2 = new TagSet(WorldGen.Settings.defaults.defaultMoveTags);
			List<VoronoiNode> list = new List<VoronoiNode>();
			this.voronoiTree.GetNodesWithTag(WorldGenTags.NearSurface, list);
			VoronoiNode.SplitCommand splitCommand = new VoronoiNode.SplitCommand();
			splitCommand.dontCopyTags = tagSet;
			splitCommand.moveTags = tagSet2;
			splitCommand.SplitFunction = new Action<VoronoiTree, VoronoiNode.SplitCommand>(this.SplitFunction);
			for (int i = 0; i < list.Count; i++)
			{
				VoronoiNode voronoiNode = list[i];
				if (voronoiNode.site.poly.Area() > defaultFloat)
				{
					voronoiNode.Split(splitCommand);
				}
			}
			List<VoronoiNode> list2 = new List<VoronoiNode>();
			this.voronoiTree.GetNodesWithTag(WorldGenTags.NearDepths, list2);
			for (int j = 0; j < list2.Count; j++)
			{
				VoronoiNode voronoiNode2 = list2[j];
				if (voronoiNode2.site.poly.Area() > defaultFloat)
				{
					voronoiNode2.Split(splitCommand);
				}
			}
			VoronoiNode.maxDepth = this.voronoiTree.MaxDepth(0);
			this.voronoiTree.ForceLowestToLeaf();
			list = new List<VoronoiNode>();
			this.voronoiTree.GetNodesWithTag(WorldGenTags.AtSurface, list);
			for (int k = 0; k < list.Count; k++)
			{
				VoronoiNode voronoiNode3 = list[k];
				voronoiNode3.tags.Remove(WorldGenTags.Geode);
				voronoiNode3.tags.Remove(WorldGenTags.Feature);
			}
		}

		private void SplitFunction(VoronoiTree tree, VoronoiNode.SplitCommand cmd)
		{
			Node node;
			if (tree.tags.Contains(WorldGenTags.Overworld))
			{
				node = WorldGen.WorldLayout.overworldGraph.FindNodeByID(tree.site.id);
			}
			else
			{
				node = WorldGen.WorldLayout.localGraph.FindNodeByID(tree.site.id);
			}
			TagSet tagSet = new TagSet(tree.tags);
			if (cmd.dontCopyTags != null)
			{
				tagSet.Remove(cmd.dontCopyTags);
				if (cmd.moveTags != null)
				{
					tagSet.Remove(cmd.moveTags);
				}
			}
			TagSet tagSet2 = new TagSet();
			if (cmd.moveTags != null)
			{
				for (int i = 0; i < cmd.moveTags.Count; i++)
				{
					Tag tag = cmd.moveTags[i];
					if (tree.tags.Contains(tag))
					{
						tree.tags.Remove(tag);
						tagSet2.Add(tag);
					}
				}
			}
			List<Vector2> list = new List<Vector2>();
			if (tagSet.Contains(WorldGenTags.Feature))
			{
				Node node2 = WorldGen.WorldLayout.localGraph.AddNode(node.type);
				node2.SetPosition((!tagSet.Contains(WorldGenTags.StartLocation)) ? tree.site.position : tree.site.poly.Centroid());
				VoronoiNode voronoiNode = tree.AddSite(new VoronoiDiagram.Site((uint)node2.node.Id, node2.position, 1f), VoronoiNode.NodeType.Leaf);
				if (tagSet != null && tagSet.Count != 0)
				{
					voronoiNode.SetTags(tagSet);
				}
				tagSet.Remove(WorldGenTags.Feature);
				tagSet.Remove(new Tag(node.type));
				list.Add(node2.position);
			}
			float num = WorldGen.Settings.GetDefaultFloat("SplitDensityMin");
			float num2 = WorldGen.Settings.GetDefaultFloat("SplitDensityMax");
			if (tree.tags.Contains(WorldGenTags.UltraHighDensitySplit))
			{
				num = WorldGen.Settings.GetDefaultFloat("UltraHighSplitDensityMin");
				num2 = WorldGen.Settings.GetDefaultFloat("UltraHighSplitDensityMax");
			}
			else if (tree.tags.Contains(WorldGenTags.VeryHighDensitySplit))
			{
				num = WorldGen.Settings.GetDefaultFloat("VeryHighSplitDensityMin");
				num2 = WorldGen.Settings.GetDefaultFloat("VeryHighSplitDensityMax");
			}
			else if (tree.tags.Contains(WorldGenTags.HighDensitySplit))
			{
				num = WorldGen.Settings.GetDefaultFloat("HighSplitDensityMin");
				num2 = WorldGen.Settings.GetDefaultFloat("HighSplitDensityMax");
			}
			else if (tree.tags.Contains(WorldGenTags.MediumDensitySplit))
			{
				num = WorldGen.Settings.GetDefaultFloat("MediumSplitDensityMin");
				num2 = WorldGen.Settings.GetDefaultFloat("MediumSplitDensityMax");
			}
			float num3 = tree.myRandom.RandomRange(num, num2);
			List<Vector2> points = this.GetPoints(tree.site.id.ToString(), tree.log, cmd.minChildCount, tree.site.poly, num3, 1f, list, PointGenerator.SampleBehaviour.PoissonDisk, true, tree.myRandom, true, true);
			if (points.Count < cmd.minChildCount)
			{
				if (WorldGen.isRunningDebugGen)
				{
				}
				if (points.Count == 0)
				{
					return;
				}
			}
			for (int j = 0; j < points.Count; j++)
			{
				Node node3 = WorldGen.WorldLayout.localGraph.AddNode((cmd.typeOverride != null) ? cmd.typeOverride(points[j]) : node.type);
				node3.SetPosition(points[j]);
				VoronoiNode voronoiNode2 = tree.AddSite(new VoronoiDiagram.Site((uint)node3.node.Id, node3.position, 1f), VoronoiNode.NodeType.Leaf);
				if (tagSet != null && tagSet.Count != 0)
				{
					voronoiNode2.SetTags(tagSet);
				}
			}
			for (int k = 0; k < tagSet2.Count; k++)
			{
				Tag tag2 = tagSet2[k];
				tree.GetChild(tree.myRandom.RandomRange(0, tree.ChildCount())).AddTag(tag2);
			}
		}

		private void SprinklePOI(List<TemplateContainer> poi)
		{
			List<VoronoiNode> leafNodesWithTag = this.GetLeafNodesWithTag(WorldGenTags.StartFar);
			leafNodesWithTag.RemoveAll((VoronoiNode vn) => vn.tags.Contains(WorldGenTags.AtDepths) || vn.tags.Contains(WorldGenTags.AtSurface));
			leafNodesWithTag.RemoveAll((VoronoiNode vn) => vn.tags.Contains(WorldGenTags.AtLeftEdge) || vn.tags.Contains(WorldGenTags.AtRightEdge));
			leafNodesWithTag.RemoveAll((VoronoiNode vn) => vn.tags.Contains(new Tag("EdgeOfVoid")));
			for (int i = 0; i < poi.Count; i++)
			{
				VoronoiNode voronoiNode = leafNodesWithTag.GetRandom<VoronoiNode>(this.myRandom);
				voronoiNode.AddTag(new Tag(poi[i].name));
				voronoiNode.AddTag(WorldGenTags.POI);
				leafNodesWithTag.Remove(voronoiNode);
				voronoiNode = leafNodesWithTag.GetRandom<VoronoiNode>(this.myRandom);
				voronoiNode.AddTag(new Tag(poi[i].name));
				voronoiNode.AddTag(WorldGenTags.POI);
				leafNodesWithTag.Remove(voronoiNode);
				voronoiNode = leafNodesWithTag.GetRandom<VoronoiNode>(this.myRandom);
				voronoiNode.AddTag(new Tag(poi[i].name));
				voronoiNode.AddTag(WorldGenTags.POI);
				leafNodesWithTag.Remove(voronoiNode);
			}
		}

		private void TagTopAndBottomSites(Tag topTag, Tag bottomTag)
		{
			List<VoronoiDiagram.Site> list = new List<VoronoiDiagram.Site>();
			List<VoronoiDiagram.Site> list2 = new List<VoronoiDiagram.Site>();
			this.voronoiTree.GetIntersectingLeafSites(this.topEdge, list);
			this.voronoiTree.GetIntersectingLeafSites(this.bottomEdge, list2);
			for (int i = 0; i < list.Count; i++)
			{
				VoronoiNode nodeForSite = this.voronoiTree.GetNodeForSite(list[i]);
				nodeForSite.AddTag(topTag);
			}
			for (int j = 0; j < list2.Count; j++)
			{
				VoronoiNode nodeForSite2 = this.voronoiTree.GetNodeForSite(list2[j]);
				nodeForSite2.AddTag(bottomTag);
			}
		}

		private void TagEdgeSites(Tag leftTag, Tag rightTag)
		{
			List<VoronoiDiagram.Site> list = new List<VoronoiDiagram.Site>();
			List<VoronoiDiagram.Site> list2 = new List<VoronoiDiagram.Site>();
			this.voronoiTree.GetIntersectingLeafSites(this.leftEdge, list);
			this.voronoiTree.GetIntersectingLeafSites(this.rightEdge, list2);
			for (int i = 0; i < list.Count; i++)
			{
				VoronoiNode nodeForSite = this.voronoiTree.GetNodeForSite(list[i]);
				nodeForSite.AddTag(leftTag);
			}
			for (int j = 0; j < list2.Count; j++)
			{
				VoronoiNode nodeForSite2 = this.voronoiTree.GetNodeForSite(list2[j]);
				nodeForSite2.AddTag(rightTag);
			}
		}

		private void SetTemperatureTags()
		{
			List<VoronoiLeaf> list = new List<VoronoiLeaf>();
			List<VoronoiLeaf> list2 = new List<VoronoiLeaf>();
			this.voronoiTree.GetIntersectingLeafNodes(this.topEdge, list);
			this.voronoiTree.GetIntersectingLeafNodes(this.bottomEdge, list2);
			TagSet tagSet = new TagSet();
			foreach (object obj in Enum.GetValues(typeof(Temperature.Range)))
			{
				Temperature.Range range = (Temperature.Range)((int)obj);
				tagSet.Add(new Tag(range.ToString()));
			}
			Tag tag = new Tag(Temperature.Range.VeryCold.ToString());
			for (int i = 0; i < list.Count; i++)
			{
				TagSet tagSet2 = new TagSet(list[i].tags);
				tagSet2.Remove(tagSet);
				tagSet2.Add(tag);
				list[i].SetTags(tagSet2);
			}
			Tag tag2 = new Tag(Temperature.Range.VeryHot.ToString());
			for (int j = 0; j < list2.Count; j++)
			{
				TagSet tagSet3 = new TagSet(list2[j].tags);
				tagSet3.Remove(tagSet);
				tagSet3.Add(tag2);
				list2[j].SetTags(tagSet3);
			}
		}

		private bool StartAreaTooLarge(VoronoiNode node)
		{
			bool flag = node.tags.Contains(WorldGenTags.StartWorld);
			if (flag)
			{
				float num = node.site.poly.Area();
				return num > 2000f;
			}
			return false;
		}

		private void SplitLargeStartingSites()
		{
			TagSet tagSet = new TagSet();
			tagSet.Add(WorldGenTags.Overworld);
			TagSet tagSet2 = new TagSet(WorldGen.Settings.defaults.defaultMoveTags);
			List<VoronoiNode> list = new List<VoronoiNode>();
			this.voronoiTree.GetLeafNodes(list, new VoronoiTree.LeafNodeTest(this.StartAreaTooLarge));
			VoronoiNode.SplitCommand splitCommand = new VoronoiNode.SplitCommand();
			splitCommand.dontCopyTags = tagSet;
			splitCommand.moveTags = tagSet2;
			splitCommand.SplitFunction = new Action<VoronoiTree, VoronoiNode.SplitCommand>(this.SplitFunction);
			while (list.Count > 0)
			{
				foreach (VoronoiNode voronoiNode in list)
				{
					voronoiNode.AddTag(WorldGenTags.DEBUG_SplitLargeStartingSites);
					voronoiNode.Split(splitCommand);
				}
				list.Clear();
				this.voronoiTree.GetLeafNodes(list, new VoronoiTree.LeafNodeTest(this.StartAreaTooLarge));
			}
		}

		private void PropagateStartTag()
		{
			List<VoronoiNode> startNodes = this.GetStartNodes();
			foreach (VoronoiNode voronoiNode in startNodes)
			{
				voronoiNode.AddTagToNeighbors(WorldGenTags.NearStartLocation);
				voronoiNode.AddTag(WorldGenTags.IgnoreCaveOverride);
			}
		}

		public List<VoronoiNode> GetStartNodes()
		{
			return this.GetLeafNodesWithTag(WorldGenTags.StartLocation);
		}

		public List<VoronoiNode> GetLeafNodesWithTag(Tag tag)
		{
			List<VoronoiNode> list = new List<VoronoiNode>();
			this.voronoiTree.GetLeafNodes(list, (VoronoiNode node) => node.tags != null && node.tags.Contains(tag));
			return list;
		}

		public List<Node> GetTerrainNodesForTag(Tag tag)
		{
			List<Node> list = new List<Node>();
			List<VoronoiNode> leafNodesWithTag = this.GetLeafNodesWithTag(tag);
			foreach (VoronoiNode voronoiNode in leafNodesWithTag)
			{
				Node node = this.localGraph.FindNodeByID(voronoiNode.site.id);
				if (node != null)
				{
					list.Add(node);
				}
			}
			return list;
		}

		private Node FindFirstNode(string nodeType)
		{
			return this.localGraph.FindNode((Node node) => node.type == nodeType);
		}

		private Node FindFirstNodeWithTag(Tag tag)
		{
			return this.localGraph.FindNode((Node node) => node.tags != null && node.tags.Contains(tag));
		}

		public Vector2I GetStartLocation()
		{
			Node node2 = this.FindFirstNodeWithTag(WorldGenTags.StartLocation);
			if (node2 == null)
			{
				List<VoronoiNode> nodes = this.GetStartNodes();
				if (nodes == null || nodes.Count == 0)
				{
					global::Debug.LogWarning("Couldnt find start node", null);
					return new Vector2I(this.mapWidth / 2, this.mapHeight / 2);
				}
				node2 = this.localGraph.FindNode((Node node) => (uint)node.node.Id == nodes[0].site.id);
				TagSet tagSet = new TagSet();
				tagSet.Add(WorldGenTags.StartLocation);
				for (int i = 1; i < nodes.Count; i++)
				{
					nodes[i].tags.Remove(tagSet);
				}
			}
			if (node2 == null)
			{
				global::Debug.LogWarning("Couldnt find start node", null);
				return new Vector2I(this.mapWidth / 2, this.mapHeight / 2);
			}
			return new Vector2I((int)node2.position.x, (int)node2.position.y);
		}

		private List<VoronoiDiagram.Site> GetIntersectingSites(VoronoiNode intersectingSiteSource, VoronoiTree sitesSource)
		{
			List<VoronoiDiagram.Site> list = new List<VoronoiDiagram.Site>();
			list = new List<VoronoiDiagram.Site>();
			LineSegment lineSegment;
			for (int i = 1; i < intersectingSiteSource.site.poly.Vertices.Count - 1; i++)
			{
				lineSegment = new LineSegment(new Vector2?(intersectingSiteSource.site.poly.Vertices[i - 1]), new Vector2?(intersectingSiteSource.site.poly.Vertices[i]));
				sitesSource.GetIntersectingLeafSites(lineSegment, list);
			}
			lineSegment = new LineSegment(new Vector2?(intersectingSiteSource.site.poly.Vertices[intersectingSiteSource.site.poly.Vertices.Count - 1]), new Vector2?(intersectingSiteSource.site.poly.Vertices[0]));
			sitesSource.GetIntersectingLeafSites(lineSegment, list);
			return list;
		}

		public void GetEdgeOfMapSites(VoronoiTree vt, List<VoronoiDiagram.Site> topSites, List<VoronoiDiagram.Site> bottomSites, List<VoronoiDiagram.Site> leftSites, List<VoronoiDiagram.Site> rightSites)
		{
			vt.GetIntersectingLeafSites(this.topEdge, topSites);
			vt.GetIntersectingLeafSites(this.bottomEdge, bottomSites);
			vt.GetIntersectingLeafSites(this.leftEdge, leftSites);
			vt.GetIntersectingLeafSites(this.rightEdge, rightSites);
		}

		public void ConvertEdgeCells(VoronoiTree vt)
		{
			List<VoronoiDiagram.Site> list = new List<VoronoiDiagram.Site>();
			List<VoronoiDiagram.Site> list2 = new List<VoronoiDiagram.Site>();
			List<VoronoiDiagram.Site> list3 = new List<VoronoiDiagram.Site>();
			List<VoronoiDiagram.Site> list4 = new List<VoronoiDiagram.Site>();
			this.GetEdgeOfMapSites(vt, list, list2, list3, list4);
			for (int i = 0; i < this.localGraph.nodes.Count; i++)
			{
				int num = -1;
				for (int j = 0; j < list.Count; j++)
				{
					if ((uint)this.localGraph.nodes[i].node.Id == list[j].id)
					{
						num = j;
						break;
					}
				}
				if (num != -1)
				{
					this.localGraph.nodes[i].SetType(WorldGenTags.TheVoid.Name);
					list.RemoveAt(num);
				}
				else
				{
					int num2 = 0;
					num2++;
				}
				num = -1;
				for (int k = 0; k < list2.Count; k++)
				{
					if ((uint)this.localGraph.nodes[i].node.Id == list2[k].id)
					{
						num = k;
						break;
					}
				}
				if (num != -1)
				{
					float num3 = this.myRandom.RandomValue();
					this.localGraph.nodes[i].SetType((num3 <= 0.33f) ? "MagmaLake" : ((num3 <= 0.66f) ? "MagmaBed" : "MagmaPool"));
					list2.RemoveAt(num);
				}
			}
		}

		[OnSerializing]
		internal void OnSerializingMethod()
		{
			try
			{
				this.extra = new WorldLayout.ExtraIO();
				if (this.voronoiTree != null)
				{
					this.extra.internals.Add(this.voronoiTree);
					this.voronoiTree.GetInternalNodes(this.extra.internals);
					List<VoronoiNode> list = new List<VoronoiNode>();
					this.voronoiTree.GetLeafNodes(list, null);
					VoronoiLeaf ln;
					foreach (VoronoiNode voronoiNode in list)
					{
						ln = (VoronoiLeaf)voronoiNode;
						if (ln != null)
						{
							this.extra.leafInternalParent.Add(new KeyValuePair<int, int>(this.extra.leafs.Count, this.extra.internals.FindIndex(0, (VoronoiTree n) => n == ln.parent)));
							this.extra.leafs.Add(ln);
						}
					}
					for (int i = 0; i < this.extra.internals.Count; i++)
					{
						VoronoiTree vt = this.extra.internals[i];
						if (vt.parent != null)
						{
							int num = this.extra.internals.FindIndex(0, (VoronoiTree n) => n == vt.parent);
							if (num >= 0)
							{
								this.extra.internalInternalParent.Add(new KeyValuePair<int, int>(i, num));
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				string stackTrace = ex.StackTrace;
				WorldGenLogger.LogException(message, stackTrace);
				global::Debug.Log("Error deserialising " + ex.Message, null);
			}
		}

		[OnSerialized]
		internal void OnSerializedMethod()
		{
			this.extra = null;
		}

		[OnDeserializing]
		internal void OnDeserializingMethod()
		{
			this.extra = new WorldLayout.ExtraIO();
		}

		[OnDeserialized]
		internal void OnDeserializedMethod()
		{
			try
			{
				this.voronoiTree = this.extra.internals[0];
				for (int i = 0; i < this.extra.internalInternalParent.Count; i++)
				{
					KeyValuePair<int, int> keyValuePair = this.extra.internalInternalParent[i];
					VoronoiTree voronoiTree = this.extra.internals[keyValuePair.Key];
					VoronoiTree voronoiTree2 = this.extra.internals[keyValuePair.Value];
					voronoiTree2.AddChild(voronoiTree);
				}
				for (int j = 0; j < this.extra.leafInternalParent.Count; j++)
				{
					KeyValuePair<int, int> keyValuePair2 = this.extra.leafInternalParent[j];
					VoronoiNode voronoiNode = this.extra.leafs[keyValuePair2.Key];
					VoronoiTree voronoiTree3 = this.extra.internals[keyValuePair2.Value];
					voronoiTree3.AddChild(voronoiNode);
				}
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				string stackTrace = ex.StackTrace;
				WorldGenLogger.LogException(message, stackTrace);
				global::Debug.Log("Error deserialising " + ex.Message, null);
			}
			this.extra = null;
		}

		private VoronoiTree voronoiTree;

		[Serialize]
		public MapGraph localGraph;

		[Serialize]
		public MapGraph overworldGraph;

		[EnumFlags]
		public static WorldLayout.DebugFlags drawOptions;

		private LineSegment topEdge;

		private LineSegment bottomEdge;

		private LineSegment leftEdge;

		private LineSegment rightEdge;

		private SeededRandom myRandom;

		[Serialize]
		private WorldLayout.ExtraIO extra;

		[Flags]
		public enum DebugFlags
		{
			LocalGraph = 1,
			OverworldGraph = 2,
			VoronoiTree = 4
		}

		[SerializationConfig(MemberSerialization.OptOut)]
		private class ExtraIO
		{
			[OnDeserializing]
			internal void OnDeserializingMethod()
			{
				this.leafs = new List<VoronoiLeaf>();
				this.internals = new List<VoronoiTree>();
				this.leafInternalParent = new List<KeyValuePair<int, int>>();
				this.internalInternalParent = new List<KeyValuePair<int, int>>();
			}

			public List<VoronoiLeaf> leafs = new List<VoronoiLeaf>();

			public List<VoronoiTree> internals = new List<VoronoiTree>();

			public List<KeyValuePair<int, int>> leafInternalParent = new List<KeyValuePair<int, int>>();

			public List<KeyValuePair<int, int>> internalInternalParent = new List<KeyValuePair<int, int>>();
		}
	}
}
