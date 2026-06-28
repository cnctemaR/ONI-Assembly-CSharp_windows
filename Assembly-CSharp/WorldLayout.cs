using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Delaunay.Geo;
using Generated;
using Klei;
using Klei.Map;
using KSerialization;
using Satsuma;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class WorldLayout
{
	public WorldLayout()
	{
		this.localGraph = new MapGraph();
		this.overworldGraph = new MapGraph();
	}

	public WorldLayout(int width, int height)
		: this()
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

	public VoronoiTree GetVoronoiTree()
	{
		return this.voronoiTree;
	}

	public static void SetLayerGradient(LevelLayer newGradient)
	{
		WorldLayout.levelLayerGradient = newGradient;
	}

	public static string GetNodeTypeFromLayers(Vector2 point, float mapHeight)
	{
		string text = WorldGenTags.TheVoid.Name;
		int num = (int)WorldGen.RandomRange(0f, (float)WorldLayout.levelLayerGradient[WorldLayout.levelLayerGradient.Count - 1].content.Count);
		text = WorldLayout.levelLayerGradient[WorldLayout.levelLayerGradient.Count - 1].content[num];
		for (int i = 0; i < WorldLayout.levelLayerGradient.Count; i++)
		{
			if (point.y < WorldLayout.levelLayerGradient[i].maxValue * mapHeight)
			{
				int num2 = (int)WorldGen.RandomRange(0f, (float)WorldLayout.levelLayerGradient[i].content.Count);
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
		site.poly = new Polygon(new Rect(0f, 0f, (float)this.mapWidth, (float)this.mapHeight));
		this.voronoiTree = new VoronoiTree(site, null);
		VoronoiNode.maxIndex = 0U;
		float @float = WorldGen.Settings.defaults.GetFloat("OverworldDensityMin");
		float float2 = WorldGen.Settings.defaults.GetFloat("OverworldDensityMax");
		float num = WorldGen.RandomRange(@float, float2);
		float float3 = WorldGen.Settings.defaults.GetFloat("OverworldAvoidRadius");
		object obj = Enum.Parse(typeof(PointGenerator.SampleBehaviour), WorldGen.Settings.defaults.data["OverworldSampleBehaviour"] as string);
		PointGenerator.SampleBehaviour sampleBehaviour = PointGenerator.SampleBehaviour.PoissonDisk;
		if (obj != null)
		{
			sampleBehaviour = (PointGenerator.SampleBehaviour)((int)obj);
		}
		global::Klei.Node node = this.overworldGraph.AddNode(WorldGenTags.StartWorld.Name);
		node.SetPosition(new Vector2((float)(this.mapWidth / 2), (float)(this.mapHeight / 2)));
		List<Vector2> list = new List<Vector2>();
		list.Add(node.position);
		VoronoiNode voronoiNode = this.voronoiTree.AddSite(new VoronoiDiagram.Site((uint)node.node.Id, node.position, 1f), VoronoiNode.NodeType.Internal);
		List<Vector2> randomPoints = PointGenerator.GetRandomPoints(site.poly, num, float3, list, sampleBehaviour, false, false, true);
		int @int = WorldGen.Settings.defaults.GetInt("OverworldMaxNodes");
		if (randomPoints.Count > @int)
		{
			randomPoints.ShuffleSeeded<Vector2>(WorldGen.RandomSource());
			randomPoints.RemoveRange(@int, randomPoints.Count - @int);
		}
		for (int i = 0; i < randomPoints.Count; i++)
		{
			global::Klei.Node node2 = this.overworldGraph.AddNode(WorldGenTags.UnassignedNode.Name);
			node2.SetPosition(randomPoints[i]);
			VoronoiNode voronoiNode2 = this.voronoiTree.AddSite(new VoronoiDiagram.Site((uint)node2.node.Id, node2.position, 1f), VoronoiNode.NodeType.Internal);
			voronoiNode2.tags.Add(WorldGenTags.UnassignedNode);
			node2.tags.Add(WorldGenTags.UnassignedNode);
		}
		this.voronoiTree.ComputeChildren();
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
		int int2 = WorldGen.Settings.defaults.GetInt("OverworldRelaxIterations");
		float float4 = WorldGen.Settings.defaults.GetFloat("OverworldRelaxEnergyMin");
		this.voronoiTree.RelaxRecursive(0, int2, float4);
		for (int m = 0; m < this.voronoiTree.ChildCount(); m++)
		{
			VoronoiNode child = this.voronoiTree.GetChild(m);
			global::Klei.Node node3 = this.overworldGraph.FindNodeByID(child.site.id);
			node3.SetPosition(child.site.position);
		}
		this.TagTopAndBottomSites(WorldGenTags.NearSurface, WorldGenTags.NearDepths);
		this.ConvertUnknownCells();
		List<VoronoiNode> neighbors3 = voronoiNode.GetNeighbors();
		bool flag = false;
		if (flag)
		{
			neighbors3.ShuffleSeeded<VoronoiNode>(WorldGen.RandomSource());
			global::Klei.Node node4 = this.overworldGraph.FindNodeByID(neighbors3[0].site.id);
			node4.SetType(WorldGenTags.StartWorld.Name);
			neighbors3[0].AddTag(WorldGenTags.FakeStart);
			global::Klei.Node node5 = this.overworldGraph.FindNodeByID(neighbors3[1].site.id);
			node5.SetType(WorldGenTags.StartWorld.Name);
			neighbors3[1].AddTag(WorldGenTags.FakeStart);
		}
		if (WorldGen.Settings.defaults.overworldAddTags != null)
		{
			foreach (string text in WorldGen.Settings.defaults.overworldAddTags)
			{
				int num2 = WorldGen.RandomSource().Next(this.voronoiTree.ChildCount());
				VoronoiNode child2 = this.voronoiTree.GetChild(num2);
				child2.AddTag(new Tag(text));
				global::Debug.Log("Applying Overworld Add Tag " + text, null);
			}
		}
		this.FlatternOverworld();
		this.AddSubworldChildren();
		this.ConvertNearStartAndHotEdgeToType(voronoiNode, "UNPASSABLE");
		List<VoronoiNode> startNodes = this.GetStartNodes();
		for (int n = 0; n < startNodes.Count; n++)
		{
			if (startNodes[n].tags.Contains(WorldGenTags.FakeStart))
			{
				startNodes[n].tags.Remove(WorldGenTags.StartLocation);
			}
		}
		return this.voronoiTree;
	}

	private void ConvertNearStartAndHotEdgeToType(VoronoiNode startArea, string newType)
	{
		TagSet tagSet = new TagSet();
		tagSet.Add(new Tag(Temperature.Range.Hot.ToString()));
		tagSet.Add(new Tag(Temperature.Range.VeryHot.ToString()));
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
							list[j].Split((VoronoiNode.SplitType)0, null, null, null);
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
			global::Klei.Node node = this.overworldGraph.FindNodeByID(voronoiNode.site.id);
			voronoiNode.tags.Remove(WorldGenTags.UnassignedNode);
			node.tags.Remove(WorldGenTags.UnassignedNode);
			HashSet<SubWorld> hashSet = new HashSet<SubWorld>();
			hashSet.UnionWith(dictionary[Temperature.Range.HumanWarm]);
			hashSet.UnionWith(dictionary[Temperature.Range.HumanHot]);
			if (voronoiNode.tags.Contains(WorldGenTags.StartNear))
			{
				hashSet.UnionWith(dictionary[Temperature.Range.HumanWarm]);
				hashSet.UnionWith(dictionary[Temperature.Range.HumanHot]);
				hashSet.UnionWith(dictionary[Temperature.Range.Cool]);
				hashSet.UnionWith(dictionary[Temperature.Range.Cold]);
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
				hashSet.UnionWith(dictionary[Temperature.Range.Cool]);
				hashSet.UnionWith(dictionary[Temperature.Range.Cold]);
			}
			else if (voronoiNode.tags.ContainsAll(tagSet2))
			{
				hashSet.UnionWith(dictionary[Temperature.Range.HumanWarm]);
				hashSet.UnionWith(dictionary[Temperature.Range.HumanHot]);
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
			list3.ShuffleSeeded<SubWorld>(WorldGen.RandomSource());
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

	public void ComputeSubWorlds()
	{
		try
		{
			this.SplitTopAndBottomSites();
			this.SplitLargeStartingSites();
			this.PropagateStartTag();
			this.SetTemperatureTags();
			this.TagTopAndBottomSites(WorldGenTags.AtSurface, WorldGenTags.AtDepths);
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
					global::Klei.Node node = this.overworldGraph.FindNodeByID(voronoiTree.site.id);
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
					global::Klei.Node node2 = this.overworldGraph.FindNodeByID(voronoiTree3.site.id);
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
							global::Klei.Node node3 = this.overworldGraph.FindNodeByID(key.site.id);
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
		global::Klei.Node node = this.overworldGraph.FindNode((global::Klei.Node n) => n.type == WorldGenTags.StartWorld.Name);
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
		for (int i = 0; i < this.voronoiTree.ChildCount(); i++)
		{
			VoronoiNode child = this.voronoiTree.GetChild(i);
			if (child.type == VoronoiNode.NodeType.Internal)
			{
				VoronoiTree voronoiTree = child as VoronoiTree;
				global::Klei.Node node = this.overworldGraph.FindNodeByID(voronoiTree.site.id);
				SubWorld subWorld = WorldGen.Settings.subworlds.zones[node.type];
				voronoiTree.AddTag(new Tag(node.type));
				voronoiTree.AddTag(new Tag(subWorld.temperatureRange.ToString()));
				subWorld.GenerateChildren(voronoiTree, this.localGraph, (float)this.mapHeight);
				int num = voronoiTree.ChildCount();
				if (num < subWorld.minChildCount)
				{
					voronoiTree.AddTag(WorldGenTags.DEBUG_SplitForChildCount);
					VoronoiTree voronoiTree2 = voronoiTree;
					TagSet tagSet3 = tagSet;
					voronoiTree2.Split((VoronoiNode.SplitType)0, tagSet3, tagSet2, null);
					if (subWorld.biomes != null && subWorld.biomes.Count > 0)
					{
						for (int j = num; j < voronoiTree.ChildCount(); j++)
						{
							WeightedBiome weightedBiome = global::Generated.Util.WeightedRandom.Choose<WeightedBiome>(subWorld.biomes);
							global::Klei.Node node2 = this.localGraph.FindNodeByID(voronoiTree.GetChild(j).site.id);
							node2.SetType(weightedBiome.name);
							voronoiTree.GetChild(j).AddTag(new Tag(node2.type));
						}
					}
					else
					{
						for (int k = num; k < voronoiTree.ChildCount(); k++)
						{
							global::Klei.Node node3 = this.localGraph.FindNodeByID(voronoiTree.GetChild(k).site.id);
							node3.SetType(WorldLayout.GetNodeTypeFromLayers(voronoiTree.site.position, (float)this.mapHeight));
							voronoiTree.GetChild(k).AddTag(new Tag(node3.type));
						}
					}
				}
				voronoiTree.RelaxRecursive(0, 10, 1f);
				List<VoronoiNode> list = new List<VoronoiNode>();
				voronoiTree.GetNodesWithTag(WorldGenTags.Feature, list);
				TagSet tagSet4 = new TagSet();
				tagSet4.Add(WorldGenTags.Feature);
				tagSet4.Add(WorldGenTags.SplitOnParentDensity);
				for (int l = 0; l < list.Count; l++)
				{
					if (!list[l].tags.Contains(WorldGenTags.CenteralFeature))
					{
						if (list[l].tags.Contains(WorldGenTags.SplitOnParentDensity))
						{
							VoronoiNode voronoiNode = list[l];
							TagSet tagSet3 = tagSet4;
							voronoiNode.Split((VoronoiNode.SplitType)0, tagSet3, tagSet2, null);
						}
						if (list[l].tags.Contains(WorldGenTags.SplitTwice))
						{
							VoronoiNode voronoiNode2 = list[l];
							TagSet tagSet3 = tagSet4;
							VoronoiTree voronoiTree3 = voronoiNode2.Split((VoronoiNode.SplitType)0, tagSet3, tagSet2, null);
							if (voronoiTree3.ChildCount() <= 1)
							{
								global::Debug.LogError("split did not work.", null);
							}
							for (int m = 0; m < voronoiTree3.ChildCount(); m++)
							{
								VoronoiNode child2 = voronoiTree3.GetChild(m);
								VoronoiNode voronoiNode3 = child2;
								tagSet3 = tagSet4;
								voronoiNode3.Split((VoronoiNode.SplitType)0, tagSet3, tagSet2, null);
							}
						}
					}
				}
			}
		}
		VoronoiNode.maxDepth = this.voronoiTree.MaxDepth(0);
	}

	private void SplitTopAndBottomSites()
	{
		float @float = WorldGen.Settings.defaults.GetFloat("SplitTopAndBottomSitesMaxArea");
		TagSet tagSet = new TagSet();
		tagSet.Add(WorldGenTags.Overworld);
		TagSet tagSet2 = new TagSet(WorldGen.Settings.defaults.defaultMoveTags);
		List<VoronoiNode> list = new List<VoronoiNode>();
		this.voronoiTree.GetNodesWithTag(WorldGenTags.NearSurface, list);
		for (int i = 0; i < list.Count; i++)
		{
			VoronoiNode voronoiNode = list[i];
			if (voronoiNode.site.poly.Area() > @float)
			{
				VoronoiNode voronoiNode2 = voronoiNode;
				TagSet tagSet3 = tagSet;
				voronoiNode2.Split((VoronoiNode.SplitType)0, tagSet3, tagSet2, null);
			}
		}
		List<VoronoiNode> list2 = new List<VoronoiNode>();
		this.voronoiTree.GetNodesWithTag(WorldGenTags.NearDepths, list2);
		for (int j = 0; j < list2.Count; j++)
		{
			VoronoiNode voronoiNode3 = list2[j];
			if (voronoiNode3.site.poly.Area() > @float)
			{
				VoronoiNode voronoiNode4 = voronoiNode3;
				TagSet tagSet3 = tagSet;
				voronoiNode4.Split((VoronoiNode.SplitType)0, tagSet3, tagSet2, null);
			}
		}
		VoronoiNode.maxDepth = this.voronoiTree.MaxDepth(0);
		this.voronoiTree.ForceLowestToLeaf();
		list = new List<VoronoiNode>();
		this.voronoiTree.GetNodesWithTag(WorldGenTags.AtSurface, list);
		for (int k = 0; k < list.Count; k++)
		{
			VoronoiNode voronoiNode5 = list[k];
			voronoiNode5.tags.Remove(WorldGenTags.Geode);
			voronoiNode5.tags.Remove(WorldGenTags.Feature);
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
		float num = node.site.poly.Area();
		bool flag2 = num > 2000f;
		return flag && flag2;
	}

	private void SplitLargeStartingSites()
	{
		TagSet tagSet = new TagSet();
		tagSet.Add(WorldGenTags.Overworld);
		TagSet tagSet2 = new TagSet(WorldGen.Settings.defaults.defaultMoveTags);
		List<VoronoiNode> list = new List<VoronoiNode>();
		this.voronoiTree.GetLeafNodes(list, new VoronoiTree.LeafNodeTest(this.StartAreaTooLarge));
		while (list.Count > 0)
		{
			foreach (VoronoiNode voronoiNode in list)
			{
				voronoiNode.AddTag(WorldGenTags.DEBUG_SplitLargeStartingSites);
				VoronoiNode voronoiNode2 = voronoiNode;
				TagSet tagSet3 = tagSet;
				voronoiNode2.Split((VoronoiNode.SplitType)0, tagSet3, tagSet2, null);
			}
			list.Clear();
			this.voronoiTree.GetLeafNodes(list, new VoronoiTree.LeafNodeTest(this.StartAreaTooLarge));
		}
	}

	public List<VoronoiNode> GetStartNodes()
	{
		return this.GetNodesWithTag(WorldGenTags.StartLocation);
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

	public List<VoronoiNode> GetNodesWithTag(Tag tag)
	{
		List<VoronoiNode> list = new List<VoronoiNode>();
		this.voronoiTree.GetLeafNodes(list, (VoronoiNode node) => node.tags != null && node.tags.Contains(tag));
		return list;
	}

	public List<global::Klei.Node> GetTerrainNodesForTag(Tag tag)
	{
		List<global::Klei.Node> list = new List<global::Klei.Node>();
		List<VoronoiNode> nodesWithTag = this.GetNodesWithTag(tag);
		foreach (VoronoiNode voronoiNode in nodesWithTag)
		{
			global::Klei.Node node = this.localGraph.FindNodeByID(voronoiNode.site.id);
			if (node != null)
			{
				list.Add(node);
			}
		}
		return list;
	}

	private global::Klei.Node FindFirstNode(string nodeType)
	{
		return this.localGraph.FindNode((global::Klei.Node node) => node.type == nodeType);
	}

	private global::Klei.Node FindFirstNodeWithTag(Tag tag)
	{
		return this.localGraph.FindNode((global::Klei.Node node) => node.tags != null && node.tags.Contains(tag));
	}

	public Vector2I GetStartLocation()
	{
		global::Klei.Node node2 = this.FindFirstNodeWithTag(WorldGenTags.StartLocation);
		if (node2 == null)
		{
			List<VoronoiNode> nodes = this.GetStartNodes();
			if (nodes == null || nodes.Count == 0)
			{
				global::Debug.LogWarning("Couldnt find start node", null);
				return new Vector2I(this.mapWidth / 2, this.mapHeight / 2);
			}
			node2 = this.localGraph.FindNode((global::Klei.Node node) => (uint)node.node.Id == nodes[0].site.id);
		}
		if (node2 == null)
		{
			global::Debug.LogWarning("Couldnt find start node", null);
			return new Vector2I(this.mapWidth / 2, this.mapHeight / 2);
		}
		return new Vector2I((int)node2.position.x, (int)node2.position.y);
	}

	public List<River> GetRivers()
	{
		List<River> list = new List<River>();
		foreach (global::Satsuma.Arc arc in this.localGraph.baseGraph.Arcs(ArcFilter.All))
		{
			global::Satsuma.Node n0 = this.localGraph.baseGraph.U(arc);
			global::Satsuma.Node n1 = this.localGraph.baseGraph.V(arc);
			global::Klei.Node tn0 = this.localGraph.FindNode((global::Klei.Node n) => n.node == n0);
			global::Klei.Node tn1 = this.localGraph.FindNode((global::Klei.Node n) => n.node == n1);
			if (tn0 != null && tn1 != null && !(tn0.type != tn1.type) && tn0.type.Contains(WorldGenTags.River.Name))
			{
				if (list.Find((River r) => r.SinkPosition() == tn0.position && r.SourcePosition() == tn1.position) == null)
				{
					River river;
					if (WorldGen.Settings.rivers.rivers.ContainsKey(tn0.type))
					{
						river = new River(WorldGen.Settings.rivers.rivers[tn0.type], false);
						river.AddSection(tn0, tn1);
					}
					else
					{
						river = new River(tn0, tn1, SimHashes.Water, SimHashes.Granite, 373f, 2000f, 1000f, 100f, 1.5f, 1.5f);
					}
					river.widthCenter = WorldGen.RandomRange(1f, river.widthCenter + 0.5f);
					river.widthBorder = WorldGen.RandomRange(1f, river.widthBorder + 0.5f);
					river.Stagger(WorldGen.RandomRange(8f, 20f), WorldGen.RandomRange(1f, 3f));
					list.Add(river);
				}
			}
		}
		return list;
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

	public void GetTopAndBottomSites(VoronoiTree vt, List<VoronoiDiagram.Site> topSites, List<VoronoiDiagram.Site> bottomSites)
	{
		vt.GetIntersectingLeafSites(this.topEdge, topSites);
		vt.GetIntersectingLeafSites(this.bottomEdge, bottomSites);
	}

	public void ConvertEdgeCells(VoronoiTree vt)
	{
		List<VoronoiDiagram.Site> list = new List<VoronoiDiagram.Site>();
		List<VoronoiDiagram.Site> list2 = new List<VoronoiDiagram.Site>();
		this.GetTopAndBottomSites(vt, list, list2);
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
				float num3 = WorldGen.RandomValue();
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

	public void Draw()
	{
		if ((WorldLayout.drawOptions & WorldLayout.DebugFlags.LocalGraph) != (WorldLayout.DebugFlags)0 && this.localGraph != null)
		{
			this.localGraph.Draw();
		}
		if ((WorldLayout.drawOptions & WorldLayout.DebugFlags.OverworldGraph) != (WorldLayout.DebugFlags)0 && this.overworldGraph != null)
		{
			this.overworldGraph.Draw();
		}
		if ((WorldLayout.drawOptions & WorldLayout.DebugFlags.VoronoiTree) != (WorldLayout.DebugFlags)0 && this.voronoiTree != null)
		{
			this.voronoiTree.Draw(0);
		}
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
