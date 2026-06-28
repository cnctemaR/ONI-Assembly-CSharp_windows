using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Delaunay.Geo;
using KSerialization;
using ProcGen.Map;
using ProcGenGame;
using Satsuma;
using UnityEngine;
using VoronoiTree;

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

		public global::VoronoiTree.Tree GetVoronoiTree()
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

		public global::VoronoiTree.Tree GenerateOverworld(bool usePD)
		{
			Diagram.Site site = new Diagram.Site(0U, new Vector2((float)(this.mapWidth / 2), (float)(this.mapHeight / 2)), 1f);
			this.topEdge = new LineSegment(new Vector2?(new Vector2(0f, (float)(this.mapHeight - 5))), new Vector2?(new Vector2((float)this.mapWidth, (float)(this.mapHeight - 5))));
			this.bottomEdge = new LineSegment(new Vector2?(new Vector2(0f, 5f)), new Vector2?(new Vector2((float)this.mapWidth, 5f)));
			this.leftEdge = new LineSegment(new Vector2?(new Vector2(5f, 0f)), new Vector2?(new Vector2(5f, (float)this.mapHeight)));
			this.rightEdge = new LineSegment(new Vector2?(new Vector2((float)(this.mapWidth - 5), 0f)), new Vector2?(new Vector2((float)(this.mapWidth - 5), (float)this.mapHeight)));
			site.poly = new Polygon(new Rect(0f, 0f, (float)this.mapWidth, (float)this.mapHeight));
			this.voronoiTree = new global::VoronoiTree.Tree(site, null, this.myRandom.seed);
			global::VoronoiTree.Node.maxIndex = 0U;
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
			ProcGen.Node node = this.overworldGraph.AddNode(WorldGenTags.StartWorld.Name);
			node.SetPosition(new Vector2((float)(this.mapWidth / 2), (float)(this.mapHeight / 2)));
			List<Vector2> list = new List<Vector2>();
			list.Add(node.position);
			global::VoronoiTree.Node node2 = this.voronoiTree.AddSite(new Diagram.Site((uint)node.node.Id, node.position, 1f), global::VoronoiTree.Node.NodeType.Internal);
			List<Vector2> randomPoints = PointGenerator.GetRandomPoints(site.poly, num, defaultFloat3, list, sampleBehaviour, false, this.myRandom, false, true);
			int defaultInt = WorldGen.Settings.GetDefaultInt("OverworldMaxNodes");
			if (randomPoints.Count > defaultInt)
			{
				randomPoints.ShuffleSeeded<Vector2>(this.myRandom.RandomSource());
				randomPoints.RemoveRange(defaultInt, randomPoints.Count - defaultInt);
			}
			for (int i = 0; i < randomPoints.Count; i++)
			{
				ProcGen.Node node3 = this.overworldGraph.AddNode(WorldGenTags.UnassignedNode.Name);
				node3.SetPosition(randomPoints[i]);
				global::VoronoiTree.Node node4 = this.voronoiTree.AddSite(new Diagram.Site((uint)node3.node.Id, node3.position, 1f), global::VoronoiTree.Node.NodeType.Internal);
				node4.tags.Add(WorldGenTags.UnassignedNode);
				node3.tags.Add(WorldGenTags.UnassignedNode);
			}
			if (usePD)
			{
				List<Diagram.Site> list2 = new List<Diagram.Site>();
				for (int j = 0; j < this.voronoiTree.ChildCount(); j++)
				{
					list2.Add(this.voronoiTree.GetChild(j).site);
				}
				this.voronoiTree.ComputeNode(list2);
				this.voronoiTree.ComputeNodePD(list2, 500, 0.2f);
			}
			else
			{
				this.voronoiTree.ComputeChildren(this.myRandom.seed + 1, false, false);
			}
			this.voronoiTree.AddTagToChildren(WorldGenTags.Overworld);
			node2.AddTag(WorldGenTags.StartWorld);
			node2.AddTagToNeighbors(WorldGenTags.StartNear);
			List<global::VoronoiTree.Node> siblings = node2.GetSiblings();
			List<global::VoronoiTree.Node> neighbors = node2.GetNeighbors();
			for (int k = 0; k < neighbors.Count; k++)
			{
				global::VoronoiTree.Node node5 = neighbors[k];
				if (siblings.Contains(node5))
				{
					siblings.Remove(node5);
				}
				List<global::VoronoiTree.Node> neighbors2 = node5.GetNeighbors();
				for (int l = 0; l < neighbors2.Count; l++)
				{
					global::VoronoiTree.Node node6 = neighbors2[l];
					if (siblings.Contains(node6))
					{
						siblings.Remove(node6);
					}
					if (!node6.tags.Contains(WorldGenTags.StartNear) && !node6.tags.Contains(WorldGenTags.StartWorld))
					{
						node6.AddTag(WorldGenTags.StartMedium);
					}
				}
			}
			for (int m = 0; m < siblings.Count; m++)
			{
				global::VoronoiTree.Node node7 = siblings[m];
				if (!node7.tags.Contains(WorldGenTags.StartNear) && !node7.tags.Contains(WorldGenTags.StartWorld) && !node7.tags.Contains(WorldGenTags.StartMedium))
				{
					node7.AddTag(WorldGenTags.StartFar);
				}
			}
			int defaultInt2 = WorldGen.Settings.GetDefaultInt("OverworldRelaxIterations");
			float defaultFloat4 = WorldGen.Settings.GetDefaultFloat("OverworldRelaxEnergyMin");
			this.voronoiTree.RelaxRecursive(0, defaultInt2, defaultFloat4, usePD);
			this.TagTopAndBottomSites(WorldGenTags.NearSurface, WorldGenTags.NearDepths);
			this.TagEdgeSites(WorldGenTags.NearEdge, WorldGenTags.NearEdge);
			for (int n = 0; n < this.voronoiTree.ChildCount(); n++)
			{
				global::VoronoiTree.Node child = this.voronoiTree.GetChild(n);
				ProcGen.Node node8 = this.overworldGraph.FindNodeByID(child.site.id);
				node8.tags.Union(child.tags);
				node8.SetPosition(child.site.position);
				List<global::VoronoiTree.Node> neighbors3 = child.GetNeighbors();
				for (int num2 = 0; num2 < neighbors3.Count; num2++)
				{
					ProcGen.Node node9 = this.overworldGraph.FindNodeByID(neighbors3[num2].site.id);
					this.overworldGraph.AddArc(node8, node9, "Neighbor");
				}
			}
			this.PropegateOverworldTags(this.voronoiTree, WorldGenTags.DistanceTags);
			this.ConvertUnknownCells();
			if (WorldGen.Settings.defaults.overworldAddTags != null)
			{
				foreach (string text in WorldGen.Settings.defaults.overworldAddTags)
				{
					int num3 = this.myRandom.RandomSource().Next(this.voronoiTree.ChildCount());
					global::VoronoiTree.Node child2 = this.voronoiTree.GetChild(num3);
					child2.AddTag(new Tag(text));
				}
			}
			this.FlatternOverworld();
			return this.voronoiTree;
		}

		public void PopulateSubworlds()
		{
			this.AddSubworldChildren();
			this.GetStartLocation();
		}

		private void PropegateOverworldTags(global::VoronoiTree.Tree tree, TagSet tags)
		{
			foreach (Tag tag in tags)
			{
				Dictionary<uint, int> distanceToTag = this.overworldGraph.GetDistanceToTag(tag);
				if (distanceToTag != null)
				{
					for (int i = 0; i < tree.ChildCount(); i++)
					{
						global::VoronoiTree.Node child = tree.GetChild(i);
						uint id = child.site.id;
						if (distanceToTag.ContainsKey(id) && distanceToTag[id] > 0)
						{
							child.minDistaceToTag.Add(tag, distanceToTag[id]);
						}
					}
				}
			}
		}

		private void ConvertNearStartAndHotEdgeToType(global::VoronoiTree.Node startArea, string newType)
		{
			TagSet tagSet = new TagSet();
			tagSet.Add(new Tag(Temperature.Range.Hot.ToString()));
			tagSet.Add(new Tag(Temperature.Range.VeryHot.ToString()));
			global::VoronoiTree.Node.SplitCommand splitCommand = new global::VoronoiTree.Node.SplitCommand();
			splitCommand.SplitFunction = new Action<global::VoronoiTree.Tree, global::VoronoiTree.Node.SplitCommand>(this.SplitFunction);
			int num = 0;
			List<KeyValuePair<global::VoronoiTree.Node, LineSegment>> neighborsByEdge = startArea.GetNeighborsByEdge();
			for (int i = 0; i < neighborsByEdge.Count; i++)
			{
				KeyValuePair<global::VoronoiTree.Node, LineSegment> keyValuePair = neighborsByEdge[i];
				if (keyValuePair.Key == null)
				{
					global::Debug.Log("Weird, kvp.Key NULL", null);
				}
				else
				{
					global::VoronoiTree.Tree tree = (global::VoronoiTree.Tree)keyValuePair.Key;
					if (tree == null)
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
						if (tree.tags != null && tree.tags.ContainsOne(tagSet))
						{
							List<Leaf> list = new List<Leaf>();
							tree.GetIntersectingLeafNodes(lineSegment, list);
							for (int j = 0; j < list.Count; j++)
							{
								list[j].AddTag(WorldGenTags.HighDensitySplit);
								list[j].Split(splitCommand);
							}
							global::VoronoiTree.Node.maxDepth = this.voronoiTree.MaxDepth(0);
							this.voronoiTree.ForceLowestToLeaf();
							list.Clear();
							tree.GetIntersectingLeafNodes(lineSegment, list);
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

		private char ConvertSignToCmp(int val)
		{
			if (val > 0)
			{
				return '>';
			}
			if (val < 0)
			{
				return '<';
			}
			return '=';
		}

		private HashSet<SubWorld> GetDistanceFilterSet(global::VoronoiTree.Node vn, Dictionary<string, TagSet> tagsets, World.AllowedCellsFilter filter, List<SubWorld> subworlds)
		{
			HashSet<SubWorld> hashSet = new HashSet<SubWorld>();
			ProcGen.Node node = this.overworldGraph.FindNodeByID(vn.site.id);
			int distanceToTagSetFromNode = this.overworldGraph.GetDistanceToTagSetFromNode(node, tagsets[filter.tagset]);
			if (distanceToTagSetFromNode >= 0)
			{
				int num = distanceToTagSetFromNode.CompareTo(filter.distance);
				if (num == filter.distCmp && distanceToTagSetFromNode < filter.maxDistance)
				{
					int i;
					for (i = 0; i < filter.subworldNames.Count; i++)
					{
						hashSet.UnionWith(subworlds.FindAll((SubWorld f) => f.name == filter.subworldNames[i]));
					}
				}
			}
			return hashSet;
		}

		private HashSet<SubWorld> GetNameFilterSet(global::VoronoiTree.Node vn, Dictionary<string, TagSet> tagsets, World.AllowedCellsFilter filter, List<SubWorld> subworlds)
		{
			HashSet<SubWorld> hashSet = new HashSet<SubWorld>();
			switch (filter.tagcommand)
			{
			case World.AllowedCellsFilter.TagCommand.Default:
			{
				int j;
				for (j = 0; j < filter.subworldNames.Count; j++)
				{
					hashSet.UnionWith(subworlds.FindAll((SubWorld f) => f.name == filter.subworldNames[j]));
				}
				break;
			}
			case World.AllowedCellsFilter.TagCommand.ContainsOne:
				if (vn.tags.ContainsOne(tagsets[filter.tagset]))
				{
					int k;
					for (k = 0; k < filter.subworldNames.Count; k++)
					{
						hashSet.UnionWith(subworlds.FindAll((SubWorld f) => f.name == filter.subworldNames[k]));
					}
				}
				break;
			case World.AllowedCellsFilter.TagCommand.ContainsAll:
				if (vn.tags.ContainsAll(tagsets[filter.tagset]))
				{
					int l;
					for (l = 0; l < filter.subworldNames.Count; l++)
					{
						hashSet.UnionWith(subworlds.FindAll((SubWorld f) => f.name == filter.subworldNames[l]));
					}
				}
				break;
			case World.AllowedCellsFilter.TagCommand.ContainsNone:
				if (!vn.tags.ContainsOne(tagsets[filter.tagset]))
				{
					int i;
					for (i = 0; i < filter.subworldNames.Count; i++)
					{
						hashSet.UnionWith(subworlds.FindAll((SubWorld f) => f.name == filter.subworldNames[i]));
					}
				}
				break;
			}
			return hashSet;
		}

		private HashSet<SubWorld> GetZoneTypeFilterSet(global::VoronoiTree.Node vn, Dictionary<string, TagSet> tagsets, World.AllowedCellsFilter filter, Dictionary<string, List<SubWorld>> subworldsByZoneType)
		{
			HashSet<SubWorld> hashSet = new HashSet<SubWorld>();
			switch (filter.tagcommand)
			{
			case World.AllowedCellsFilter.TagCommand.Default:
			{
				for (int i = 0; i < filter.zoneTypes.Count; i++)
				{
					hashSet.UnionWith(subworldsByZoneType[filter.zoneTypes[i].ToString()]);
				}
				break;
			}
			case World.AllowedCellsFilter.TagCommand.ContainsOne:
				if (vn.tags.ContainsOne(tagsets[filter.tagset]))
				{
					for (int j = 0; j < filter.zoneTypes.Count; j++)
					{
						hashSet.UnionWith(subworldsByZoneType[filter.zoneTypes[j].ToString()]);
					}
				}
				break;
			case World.AllowedCellsFilter.TagCommand.ContainsAll:
				if (vn.tags.ContainsAll(tagsets[filter.tagset]))
				{
					for (int k = 0; k < filter.zoneTypes.Count; k++)
					{
						hashSet.UnionWith(subworldsByZoneType[filter.zoneTypes[k].ToString()]);
					}
				}
				break;
			case World.AllowedCellsFilter.TagCommand.ContainsNone:
				if (!vn.tags.ContainsOne(tagsets[filter.tagset]))
				{
					for (int l = 0; l < filter.zoneTypes.Count; l++)
					{
						hashSet.UnionWith(subworldsByZoneType[filter.zoneTypes[l].ToString()]);
					}
				}
				break;
			}
			return hashSet;
		}

		private HashSet<SubWorld> GetTemperatureFilterSet(global::VoronoiTree.Node vn, Dictionary<string, TagSet> tagsets, World.AllowedCellsFilter filter, Dictionary<string, List<SubWorld>> subworldsByTemperature)
		{
			HashSet<SubWorld> hashSet = new HashSet<SubWorld>();
			switch (filter.tagcommand)
			{
			case World.AllowedCellsFilter.TagCommand.Default:
			{
				for (int i = 0; i < filter.temperatureRanges.Count; i++)
				{
					hashSet.UnionWith(subworldsByTemperature[filter.temperatureRanges[i].ToString()]);
				}
				break;
			}
			case World.AllowedCellsFilter.TagCommand.ContainsOne:
				if (vn.tags.ContainsOne(tagsets[filter.tagset]))
				{
					for (int j = 0; j < filter.temperatureRanges.Count; j++)
					{
						hashSet.UnionWith(subworldsByTemperature[filter.temperatureRanges[j].ToString()]);
					}
				}
				break;
			case World.AllowedCellsFilter.TagCommand.ContainsAll:
				if (vn.tags.ContainsAll(tagsets[filter.tagset]))
				{
					for (int k = 0; k < filter.temperatureRanges.Count; k++)
					{
						hashSet.UnionWith(subworldsByTemperature[filter.temperatureRanges[k].ToString()]);
					}
				}
				break;
			case World.AllowedCellsFilter.TagCommand.ContainsNone:
				if (!vn.tags.ContainsOne(tagsets[filter.tagset]))
				{
					for (int l = 0; l < filter.temperatureRanges.Count; l++)
					{
						hashSet.UnionWith(subworldsByTemperature[filter.temperatureRanges[l].ToString()]);
					}
				}
				break;
			}
			return hashSet;
		}

		private void RunFilterClearCommand(global::VoronoiTree.Node vn, Dictionary<string, TagSet> tagsets, World.AllowedCellsFilter filter, HashSet<SubWorld> allowedSubworldsSet)
		{
			switch (filter.tagcommand)
			{
			case World.AllowedCellsFilter.TagCommand.Default:
				allowedSubworldsSet.Clear();
				break;
			case World.AllowedCellsFilter.TagCommand.ContainsOne:
				if (vn.tags.ContainsOne(tagsets[filter.tagset]))
				{
					allowedSubworldsSet.Clear();
				}
				break;
			case World.AllowedCellsFilter.TagCommand.ContainsAll:
				if (vn.tags.ContainsAll(tagsets[filter.tagset]))
				{
					allowedSubworldsSet.Clear();
				}
				break;
			case World.AllowedCellsFilter.TagCommand.ContainsNone:
				if (!vn.tags.ContainsOne(tagsets[filter.tagset]))
				{
					allowedSubworldsSet.Clear();
				}
				break;
			}
		}

		private HashSet<SubWorld> Filter(global::VoronoiTree.Node vn, Dictionary<string, TagSet> tagsets, List<SubWorld> allSubWorlds, Dictionary<string, List<SubWorld>> subworldsByTemperature, Dictionary<string, List<SubWorld>> subworldsByZoneType)
		{
			HashSet<SubWorld> hashSet = new HashSet<SubWorld>();
			World world = WorldGen.Settings.GetWorld();
			foreach (World.AllowedCellsFilter allowedCellsFilter in world.UnknownCellsAllowedSubworlds)
			{
				HashSet<SubWorld> hashSet2 = new HashSet<SubWorld>();
				if (allowedCellsFilter.subworldNames != null && allowedCellsFilter.subworldNames.Count > 0)
				{
					hashSet2.UnionWith(this.GetNameFilterSet(vn, tagsets, allowedCellsFilter, allSubWorlds));
				}
				if (allowedCellsFilter.temperatureRanges != null && allowedCellsFilter.temperatureRanges.Count > 0)
				{
					hashSet2.UnionWith(this.GetTemperatureFilterSet(vn, tagsets, allowedCellsFilter, subworldsByTemperature));
				}
				if (allowedCellsFilter.zoneTypes != null && allowedCellsFilter.zoneTypes.Count > 0)
				{
					hashSet2.UnionWith(this.GetZoneTypeFilterSet(vn, tagsets, allowedCellsFilter, subworldsByZoneType));
				}
				if (allowedCellsFilter.tagcommand == World.AllowedCellsFilter.TagCommand.DistanceFrom)
				{
					hashSet2.UnionWith(this.GetDistanceFilterSet(vn, tagsets, allowedCellsFilter, allSubWorlds));
				}
				switch (allowedCellsFilter.command)
				{
				case World.AllowedCellsFilter.Command.Clear:
					this.RunFilterClearCommand(vn, tagsets, allowedCellsFilter, hashSet);
					break;
				case World.AllowedCellsFilter.Command.Replace:
					if (hashSet2.Count > 0)
					{
						hashSet.Clear();
						hashSet.UnionWith(hashSet2);
					}
					break;
				case World.AllowedCellsFilter.Command.UnionWith:
					hashSet.UnionWith(hashSet2);
					break;
				case World.AllowedCellsFilter.Command.IntersectWith:
					hashSet.IntersectWith(hashSet2);
					break;
				case World.AllowedCellsFilter.Command.ExceptWith:
					hashSet.ExceptWith(hashSet2);
					break;
				case World.AllowedCellsFilter.Command.SymmetricExceptWith:
					hashSet.SymmetricExceptWith(hashSet2);
					break;
				}
			}
			return hashSet;
		}

		private void ConvertUnknownCells()
		{
			List<global::VoronoiTree.Node> list = new List<global::VoronoiTree.Node>();
			this.voronoiTree.GetNodesWithTag(WorldGenTags.UnassignedNode, list);
			List<SubWorld> subWorldList = WorldGen.Settings.GetSubWorldList();
			subWorldList.Remove(WorldGen.Settings.GetSubWorld(WorldGenTags.StartWorld.Name));
			Dictionary<string, List<SubWorld>> dictionary = new Dictionary<string, List<SubWorld>>();
			Temperature.Range range;
			foreach (object obj in Enum.GetValues(typeof(Temperature.Range)))
			{
				range = (Temperature.Range)((int)obj);
				dictionary.Add(range.ToString(), subWorldList.FindAll((SubWorld sw) => sw.temperatureRange == range));
			}
			Dictionary<string, List<SubWorld>> dictionary2 = new Dictionary<string, List<SubWorld>>();
			SubWorld.ZoneType zt;
			foreach (object obj2 in Enum.GetValues(typeof(SubWorld.ZoneType)))
			{
				zt = (SubWorld.ZoneType)((int)obj2);
				dictionary2.Add(zt.ToString(), subWorldList.FindAll((SubWorld sw) => sw.zoneType == zt));
			}
			Dictionary<string, TagSet> dictionary3 = new Dictionary<string, TagSet>();
			World world = WorldGen.Settings.GetWorld();
			foreach (KeyValuePair<string, List<string>> keyValuePair in world.DefineTagSet)
			{
				dictionary3.Add(keyValuePair.Key, new TagSet(keyValuePair.Value));
			}
			foreach (World.AllowedCellsFilter allowedCellsFilter in world.UnknownCellsAllowedSubworlds)
			{
				if (allowedCellsFilter.tagset != null && !dictionary3.ContainsKey(allowedCellsFilter.tagset))
				{
					dictionary3.Add(allowedCellsFilter.tagset, new TagSet(new string[] { allowedCellsFilter.tagset }));
				}
			}
			foreach (global::VoronoiTree.Node node in list)
			{
				ProcGen.Node node2 = this.overworldGraph.FindNodeByID(node.site.id);
				node.tags.Remove(WorldGenTags.UnassignedNode);
				node2.tags.Remove(WorldGenTags.UnassignedNode);
				HashSet<SubWorld> hashSet = this.Filter(node, dictionary3, subWorldList, dictionary, dictionary2);
				List<SubWorld> list2 = new List<SubWorld>(hashSet);
				list2.ShuffleSeeded<SubWorld>(this.myRandom.RandomSource());
				string text = "NONE";
				foreach (KeyValuePair<string, SubWorld> keyValuePair2 in WorldGen.Settings.GetSubWorlds())
				{
					if (list2.Count > 0)
					{
						if (keyValuePair2.Value == list2[0])
						{
							text = keyValuePair2.Key;
							break;
						}
					}
					else
					{
						global::Debug.LogWarning("No allowedSubworld types. Using default.", null);
						text = "Default";
					}
				}
				node2.SetType(text);
				if (list2.Count > 0)
				{
					foreach (string text2 in list2[0].tags)
					{
						node.AddTag(new Tag(text2));
					}
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
				this.TagEdgeSites(WorldGenTags.AtEdge, WorldGenTags.AtEdge);
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
					global::VoronoiTree.Node child = this.voronoiTree.GetChild(i);
					if (child.type == global::VoronoiTree.Node.NodeType.Internal)
					{
						global::VoronoiTree.Tree tree = child as global::VoronoiTree.Tree;
						ProcGen.Node node = this.overworldGraph.FindNodeByID(tree.site.id);
						node.tags.Union(tree.tags);
						Cell cell = this.overworldGraph.GetCell(node.position, node.node, true);
						cell.tags.Union(tree.tags);
					}
				}
				for (int j = 0; j < this.voronoiTree.ChildCount(); j++)
				{
					global::VoronoiTree.Node child2 = this.voronoiTree.GetChild(j);
					if (child2.type == global::VoronoiTree.Node.NodeType.Internal)
					{
						global::VoronoiTree.Tree tree2 = child2 as global::VoronoiTree.Tree;
						List<KeyValuePair<global::VoronoiTree.Node, LineSegment>> neighborsByEdge = tree2.GetNeighborsByEdge();
						for (int k = 0; k < neighborsByEdge.Count; k++)
						{
							KeyValuePair<global::VoronoiTree.Node, LineSegment> keyValuePair = neighborsByEdge[k];
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
					global::VoronoiTree.Node child3 = this.voronoiTree.GetChild(l);
					if (child3.type == global::VoronoiTree.Node.NodeType.Internal)
					{
						global::VoronoiTree.Tree tree3 = child3 as global::VoronoiTree.Tree;
						ProcGen.Node node2 = this.overworldGraph.FindNodeByID(tree3.site.id);
						Cell cell2 = this.overworldGraph.GetCell(node2.node);
						List<KeyValuePair<global::VoronoiTree.Node, LineSegment>> neighborsByEdge2 = tree3.GetNeighborsByEdge();
						for (int m = 0; m < neighborsByEdge2.Count; m++)
						{
							KeyValuePair<global::VoronoiTree.Node, LineSegment> keyValuePair2 = neighborsByEdge2[m];
							MapGraph mapGraph3 = this.overworldGraph;
							Vector2? p3 = keyValuePair2.Value.p0;
							Corner corner = mapGraph3.GetCorner(p3.Value, false);
							MapGraph mapGraph4 = this.overworldGraph;
							Vector2? p4 = keyValuePair2.Value.p1;
							Corner corner2 = mapGraph4.GetCorner(p4.Value, false);
							global::VoronoiTree.Node key = keyValuePair2.Key;
							Edge edge;
							if (key != null)
							{
								ProcGen.Node node3 = this.overworldGraph.FindNodeByID(key.site.id);
								Cell cell3 = this.overworldGraph.GetCell(node3.node);
								edge = this.overworldGraph.GetEdge(corner, corner2, cell2, cell3, true);
								SubWorld subWorld = WorldGen.Settings.GetSubWorld(node2.type);
								SubWorld subWorld2 = WorldGen.Settings.GetSubWorld(node3.type);
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
			ProcGen.Node node = this.overworldGraph.FindNode((ProcGen.Node n) => n.type == WorldGenTags.StartWorld.Name);
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
			global::VoronoiTree.Node.SplitCommand splitCommand = new global::VoronoiTree.Node.SplitCommand();
			splitCommand.dontCopyTags = tagSet;
			splitCommand.moveTags = tagSet2;
			splitCommand.SplitFunction = new Action<global::VoronoiTree.Tree, global::VoronoiTree.Node.SplitCommand>(this.SplitFunction);
			for (int i = 0; i < this.voronoiTree.ChildCount(); i++)
			{
				global::VoronoiTree.Node child = this.voronoiTree.GetChild(i);
				if (child.type == global::VoronoiTree.Node.NodeType.Internal)
				{
					global::VoronoiTree.Tree tree = child as global::VoronoiTree.Tree;
					ProcGen.Node node = this.overworldGraph.FindNodeByID(tree.site.id);
					SubWorld subWorld = WorldGen.Settings.GetSubWorld(node.type);
					tree.AddTag(new Tag(node.type));
					tree.AddTag(new Tag(subWorld.temperatureRange.ToString()));
					this.GenerateChildren(subWorld, tree, this.localGraph, (float)this.mapHeight, i + this.myRandom.seed);
					int num = tree.ChildCount();
					if (num < subWorld.minChildCount)
					{
						tree.AddTag(WorldGenTags.DEBUG_SplitForChildCount);
						splitCommand.dontCopyTags = tagSet;
						splitCommand.minChildCount = subWorld.minChildCount;
						tree.Split(splitCommand);
						if (subWorld.biomes != null && subWorld.biomes.Count > 0)
						{
							for (int j = num; j < tree.ChildCount(); j++)
							{
								WeightedBiome weightedBiome = WeightedRandom.Choose<WeightedBiome>(subWorld.biomes, this.myRandom);
								ProcGen.Node node2 = this.localGraph.FindNodeByID(tree.GetChild(j).site.id);
								node2.SetType(weightedBiome.name);
								tree.GetChild(j).AddTag(new Tag(node2.type));
							}
						}
						else
						{
							for (int k = num; k < tree.ChildCount(); k++)
							{
								ProcGen.Node node3 = this.localGraph.FindNodeByID(tree.GetChild(k).site.id);
								node3.SetType(WorldLayout.GetNodeTypeFromLayers(tree.site.position, (float)this.mapHeight, this.myRandom));
								tree.GetChild(k).AddTag(new Tag(node3.type));
							}
						}
					}
					tree.RelaxRecursive(0, 10, 1f, WorldGen.Settings.GetWorld().layoutMethod == World.LayoutMethod.PowerTree);
					List<global::VoronoiTree.Node> list = new List<global::VoronoiTree.Node>();
					tree.GetNodesWithTag(WorldGenTags.Feature, list);
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
								global::VoronoiTree.Tree tree2 = list[l].Split(splitCommand);
								if (tree2.ChildCount() <= 1)
								{
									global::Debug.LogError("split did not work.", null);
								}
								for (int m = 0; m < tree2.ChildCount(); m++)
								{
									global::VoronoiTree.Node child2 = tree2.GetChild(m);
									child2.Split(splitCommand);
								}
							}
						}
					}
				}
			}
			global::VoronoiTree.Node.maxDepth = this.voronoiTree.MaxDepth(0);
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

		public void GenerateChildren(SubWorld sw, global::VoronoiTree.Tree node, Graph graph, float worldHeight, int seed)
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
			ProcGen.Node node2 = sw.AddCenteralFeature(node, graph, tagSet3);
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
					ProcGen.Node node3 = graph.AddNode(feature.type);
					node3.biomeSpecificTags = new TagSet(tagSet6);
					global::VoronoiTree.Node node4 = node.AddSite(new Diagram.Site((uint)node3.node.Id, node3.position, 1f), global::VoronoiTree.Node.NodeType.Internal);
					node4.tags = new TagSet(tagSet5);
					node3.SetPosition(points[m++]);
					ProcGen.Node node5 = graph.AddNode(feature.type);
					node5.biomeSpecificTags = new TagSet(tagSet6);
					global::VoronoiTree.Node node6 = node.AddSite(new Diagram.Site((uint)node5.node.Id, node5.position, 1f), global::VoronoiTree.Node.NodeType.Internal);
					node6.tags = new TagSet(tagSet5);
					node5.SetPosition(points[m++]);
					graph.AddArc(node3, node5, feature.type);
				}
				else if (m < points.Count)
				{
					ProcGen.Node node7 = graph.AddNode(feature.type);
					node7.biomeSpecificTags = new TagSet(tagSet6);
					node7.SetPosition((!(feature.type == WorldGenTags.StartLocation.Name)) ? points[m++] : node.site.poly.Centroid());
					global::VoronoiTree.Node node8 = node.AddSite(new Diagram.Site((uint)node7.node.Id, node7.position, 1f), global::VoronoiTree.Node.NodeType.Internal);
					node8.tags = new TagSet(tagSet5);
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
				ProcGen.Node node9 = graph.AddNode(text2);
				node9.biomeSpecificTags = tagSet9;
				node9.SetPosition(points[m]);
				global::VoronoiTree.Node node10 = node.AddSite(new Diagram.Site((uint)node9.node.Id, node9.position, 1f), global::VoronoiTree.Node.NodeType.Internal);
				node10.tags = new TagSet(tagSet3);
				if (tagSet9 != null)
				{
					node10.tags.Union(tagSet9);
				}
				node10.AddTag(new Tag(text2));
				m++;
			}
			node.ComputeChildren(seededRandom.seed + 1, false, false);
			if (node.ChildCount() > 0)
			{
				for (int num2 = 0; num2 < tagSet2.Count; num2++)
				{
					global::Debug.Log(string.Format("Applying Moved Tag {0} to {1}", tagSet2[num2].Name, node.site.id), null);
					global::VoronoiTree.Node child = node.GetChild(seededRandom.RandomSource().Next(node.ChildCount()));
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
			List<global::VoronoiTree.Node> list = new List<global::VoronoiTree.Node>();
			this.voronoiTree.GetNodesWithTag(WorldGenTags.NearSurface, list);
			global::VoronoiTree.Node.SplitCommand splitCommand = new global::VoronoiTree.Node.SplitCommand();
			splitCommand.dontCopyTags = tagSet;
			splitCommand.moveTags = tagSet2;
			splitCommand.SplitFunction = new Action<global::VoronoiTree.Tree, global::VoronoiTree.Node.SplitCommand>(this.SplitFunction);
			for (int i = 0; i < list.Count; i++)
			{
				global::VoronoiTree.Node node = list[i];
				if (node.site.poly.Area() > defaultFloat)
				{
					node.Split(splitCommand);
				}
			}
			List<global::VoronoiTree.Node> list2 = new List<global::VoronoiTree.Node>();
			this.voronoiTree.GetNodesWithTag(WorldGenTags.NearDepths, list2);
			for (int j = 0; j < list2.Count; j++)
			{
				global::VoronoiTree.Node node2 = list2[j];
				if (node2.site.poly.Area() > defaultFloat)
				{
					node2.Split(splitCommand);
				}
			}
			global::VoronoiTree.Node.maxDepth = this.voronoiTree.MaxDepth(0);
			this.voronoiTree.ForceLowestToLeaf();
			list = new List<global::VoronoiTree.Node>();
			this.voronoiTree.GetNodesWithTag(WorldGenTags.AtSurface, list);
			for (int k = 0; k < list.Count; k++)
			{
				global::VoronoiTree.Node node3 = list[k];
				node3.tags.Remove(WorldGenTags.Geode);
				node3.tags.Remove(WorldGenTags.Feature);
			}
		}

		private void SplitFunction(global::VoronoiTree.Tree tree, global::VoronoiTree.Node.SplitCommand cmd)
		{
			ProcGen.Node node;
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
				ProcGen.Node node2 = WorldGen.WorldLayout.localGraph.AddNode(node.type);
				node2.SetPosition((!tagSet.Contains(WorldGenTags.StartLocation)) ? tree.site.position : tree.site.poly.Centroid());
				global::VoronoiTree.Node node3 = tree.AddSite(new Diagram.Site((uint)node2.node.Id, node2.position, 1f), global::VoronoiTree.Node.NodeType.Leaf);
				if (tagSet != null && tagSet.Count != 0)
				{
					node3.SetTags(tagSet);
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
				ProcGen.Node node4 = WorldGen.WorldLayout.localGraph.AddNode((cmd.typeOverride != null) ? cmd.typeOverride(points[j]) : node.type);
				node4.SetPosition(points[j]);
				global::VoronoiTree.Node node5 = tree.AddSite(new Diagram.Site((uint)node4.node.Id, node4.position, 1f), global::VoronoiTree.Node.NodeType.Leaf);
				if (tagSet != null && tagSet.Count != 0)
				{
					node5.SetTags(tagSet);
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
			List<global::VoronoiTree.Node> leafNodesWithTag = this.GetLeafNodesWithTag(WorldGenTags.StartFar);
			leafNodesWithTag.RemoveAll((global::VoronoiTree.Node vn) => vn.tags.Contains(WorldGenTags.AtDepths) || vn.tags.Contains(WorldGenTags.AtSurface));
			leafNodesWithTag.RemoveAll((global::VoronoiTree.Node vn) => vn.tags.Contains(WorldGenTags.AtEdge));
			leafNodesWithTag.RemoveAll((global::VoronoiTree.Node vn) => vn.tags.Contains(WorldGenTags.EdgeOfVoid));
			for (int i = 0; i < poi.Count; i++)
			{
				global::VoronoiTree.Node node = leafNodesWithTag.GetRandom<global::VoronoiTree.Node>(this.myRandom);
				node.AddTag(new Tag(poi[i].name));
				node.AddTag(WorldGenTags.POI);
				leafNodesWithTag.Remove(node);
				node = leafNodesWithTag.GetRandom<global::VoronoiTree.Node>(this.myRandom);
				node.AddTag(new Tag(poi[i].name));
				node.AddTag(WorldGenTags.POI);
				leafNodesWithTag.Remove(node);
				node = leafNodesWithTag.GetRandom<global::VoronoiTree.Node>(this.myRandom);
				node.AddTag(new Tag(poi[i].name));
				node.AddTag(WorldGenTags.POI);
				leafNodesWithTag.Remove(node);
			}
		}

		private void TagTopAndBottomSites(Tag topTag, Tag bottomTag)
		{
			List<Diagram.Site> list = new List<Diagram.Site>();
			List<Diagram.Site> list2 = new List<Diagram.Site>();
			this.voronoiTree.GetIntersectingLeafSites(this.topEdge, list);
			this.voronoiTree.GetIntersectingLeafSites(this.bottomEdge, list2);
			for (int i = 0; i < list.Count; i++)
			{
				global::VoronoiTree.Node nodeForSite = this.voronoiTree.GetNodeForSite(list[i]);
				nodeForSite.AddTag(topTag);
			}
			for (int j = 0; j < list2.Count; j++)
			{
				global::VoronoiTree.Node nodeForSite2 = this.voronoiTree.GetNodeForSite(list2[j]);
				nodeForSite2.AddTag(bottomTag);
			}
		}

		private void TagEdgeSites(Tag leftTag, Tag rightTag)
		{
			List<Diagram.Site> list = new List<Diagram.Site>();
			List<Diagram.Site> list2 = new List<Diagram.Site>();
			this.voronoiTree.GetIntersectingLeafSites(this.leftEdge, list);
			this.voronoiTree.GetIntersectingLeafSites(this.rightEdge, list2);
			for (int i = 0; i < list.Count; i++)
			{
				global::VoronoiTree.Node nodeForSite = this.voronoiTree.GetNodeForSite(list[i]);
				nodeForSite.AddTag(leftTag);
			}
			for (int j = 0; j < list2.Count; j++)
			{
				global::VoronoiTree.Node nodeForSite2 = this.voronoiTree.GetNodeForSite(list2[j]);
				nodeForSite2.AddTag(rightTag);
			}
		}

		private void SetTemperatureTags()
		{
			List<Leaf> list = new List<Leaf>();
			List<Leaf> list2 = new List<Leaf>();
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

		private bool StartAreaTooLarge(global::VoronoiTree.Node node)
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
			List<global::VoronoiTree.Node> list = new List<global::VoronoiTree.Node>();
			this.voronoiTree.GetLeafNodes(list, new global::VoronoiTree.Tree.LeafNodeTest(this.StartAreaTooLarge));
			global::VoronoiTree.Node.SplitCommand splitCommand = new global::VoronoiTree.Node.SplitCommand();
			splitCommand.dontCopyTags = tagSet;
			splitCommand.moveTags = tagSet2;
			splitCommand.SplitFunction = new Action<global::VoronoiTree.Tree, global::VoronoiTree.Node.SplitCommand>(this.SplitFunction);
			while (list.Count > 0)
			{
				foreach (global::VoronoiTree.Node node in list)
				{
					node.AddTag(WorldGenTags.DEBUG_SplitLargeStartingSites);
					node.Split(splitCommand);
				}
				list.Clear();
				this.voronoiTree.GetLeafNodes(list, new global::VoronoiTree.Tree.LeafNodeTest(this.StartAreaTooLarge));
			}
		}

		private void PropagateStartTag()
		{
			List<global::VoronoiTree.Node> startNodes = this.GetStartNodes();
			foreach (global::VoronoiTree.Node node in startNodes)
			{
				node.AddTagToNeighbors(WorldGenTags.NearStartLocation);
				node.AddTag(WorldGenTags.IgnoreCaveOverride);
			}
		}

		public List<global::VoronoiTree.Node> GetStartNodes()
		{
			return this.GetLeafNodesWithTag(WorldGenTags.StartLocation);
		}

		public List<global::VoronoiTree.Node> GetLeafNodesWithTag(Tag tag)
		{
			List<global::VoronoiTree.Node> list = new List<global::VoronoiTree.Node>();
			this.voronoiTree.GetLeafNodes(list, (global::VoronoiTree.Node node) => node.tags != null && node.tags.Contains(tag));
			return list;
		}

		public List<ProcGen.Node> GetTerrainNodesForTag(Tag tag)
		{
			List<ProcGen.Node> list = new List<ProcGen.Node>();
			List<global::VoronoiTree.Node> leafNodesWithTag = this.GetLeafNodesWithTag(tag);
			foreach (global::VoronoiTree.Node node in leafNodesWithTag)
			{
				ProcGen.Node node2 = this.localGraph.FindNodeByID(node.site.id);
				if (node2 != null)
				{
					list.Add(node2);
				}
			}
			return list;
		}

		private ProcGen.Node FindFirstNode(string nodeType)
		{
			return this.localGraph.FindNode((ProcGen.Node node) => node.type == nodeType);
		}

		private ProcGen.Node FindFirstNodeWithTag(Tag tag)
		{
			return this.localGraph.FindNode((ProcGen.Node node) => node.tags != null && node.tags.Contains(tag));
		}

		public Vector2I GetStartLocation()
		{
			ProcGen.Node node2 = this.FindFirstNodeWithTag(WorldGenTags.StartLocation);
			if (node2 == null)
			{
				List<global::VoronoiTree.Node> nodes = this.GetStartNodes();
				if (nodes == null || nodes.Count == 0)
				{
					global::Debug.LogWarning("Couldnt find start node", null);
					return new Vector2I(this.mapWidth / 2, this.mapHeight / 2);
				}
				node2 = this.localGraph.FindNode((ProcGen.Node node) => (uint)node.node.Id == nodes[0].site.id);
				node2.tags.Add(WorldGenTags.StartLocation);
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
			foreach (Arc arc in this.localGraph.baseGraph.Arcs(ArcFilter.All))
			{
				global::Satsuma.Node n0 = this.localGraph.baseGraph.U(arc);
				global::Satsuma.Node n1 = this.localGraph.baseGraph.V(arc);
				ProcGen.Node tn0 = this.localGraph.FindNode((ProcGen.Node n) => n.node == n0);
				ProcGen.Node tn1 = this.localGraph.FindNode((ProcGen.Node n) => n.node == n1);
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
							river = new River(tn0, tn1, SimHashes.Water.ToString(), "Granite", 373f, 2000f, 1000f, 100f, 1.5f, 1.5f);
						}
						river.widthCenter = this.myRandom.RandomRange(1f, river.widthCenter + 0.5f);
						river.widthBorder = this.myRandom.RandomRange(1f, river.widthBorder + 0.5f);
						river.Stagger(this.myRandom, (float)this.myRandom.RandomRange(8, 20), (float)this.myRandom.RandomRange(1, 3));
						list.Add(river);
					}
				}
			}
			return list;
		}

		private List<Diagram.Site> GetIntersectingSites(global::VoronoiTree.Node intersectingSiteSource, global::VoronoiTree.Tree sitesSource)
		{
			List<Diagram.Site> list = new List<Diagram.Site>();
			list = new List<Diagram.Site>();
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

		public void GetEdgeOfMapSites(global::VoronoiTree.Tree vt, List<Diagram.Site> topSites, List<Diagram.Site> bottomSites, List<Diagram.Site> leftSites, List<Diagram.Site> rightSites)
		{
			vt.GetIntersectingLeafSites(this.topEdge, topSites);
			vt.GetIntersectingLeafSites(this.bottomEdge, bottomSites);
			vt.GetIntersectingLeafSites(this.leftEdge, leftSites);
			vt.GetIntersectingLeafSites(this.rightEdge, rightSites);
		}

		public void ConvertEdgeCells(global::VoronoiTree.Tree vt)
		{
			List<Diagram.Site> list = new List<Diagram.Site>();
			List<Diagram.Site> list2 = new List<Diagram.Site>();
			List<Diagram.Site> list3 = new List<Diagram.Site>();
			List<Diagram.Site> list4 = new List<Diagram.Site>();
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
					List<global::VoronoiTree.Node> list = new List<global::VoronoiTree.Node>();
					this.voronoiTree.GetLeafNodes(list, null);
					Leaf ln;
					foreach (global::VoronoiTree.Node node in list)
					{
						ln = (Leaf)node;
						if (ln != null)
						{
							this.extra.leafInternalParent.Add(new KeyValuePair<int, int>(this.extra.leafs.Count, this.extra.internals.FindIndex(0, (global::VoronoiTree.Tree n) => n == ln.parent)));
							this.extra.leafs.Add(ln);
						}
					}
					for (int i = 0; i < this.extra.internals.Count; i++)
					{
						global::VoronoiTree.Tree vt = this.extra.internals[i];
						if (vt.parent != null)
						{
							int num = this.extra.internals.FindIndex(0, (global::VoronoiTree.Tree n) => n == vt.parent);
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
					global::VoronoiTree.Tree tree = this.extra.internals[keyValuePair.Key];
					global::VoronoiTree.Tree tree2 = this.extra.internals[keyValuePair.Value];
					tree2.AddChild(tree);
				}
				for (int j = 0; j < this.extra.leafInternalParent.Count; j++)
				{
					KeyValuePair<int, int> keyValuePair2 = this.extra.leafInternalParent[j];
					global::VoronoiTree.Node node = this.extra.leafs[keyValuePair2.Key];
					global::VoronoiTree.Tree tree3 = this.extra.internals[keyValuePair2.Value];
					tree3.AddChild(node);
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

		private global::VoronoiTree.Tree voronoiTree;

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
				this.leafs = new List<Leaf>();
				this.internals = new List<global::VoronoiTree.Tree>();
				this.leafInternalParent = new List<KeyValuePair<int, int>>();
				this.internalInternalParent = new List<KeyValuePair<int, int>>();
			}

			public List<Leaf> leafs = new List<Leaf>();

			public List<global::VoronoiTree.Tree> internals = new List<global::VoronoiTree.Tree>();

			public List<KeyValuePair<int, int>> leafInternalParent = new List<KeyValuePair<int, int>>();

			public List<KeyValuePair<int, int>> internalInternalParent = new List<KeyValuePair<int, int>>();
		}
	}
}
