using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Delaunay.Geo;
using KSerialization;
using ObjectCloner;
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
		[Serialize]
		public int mapWidth { get; private set; }

		[Serialize]
		public int mapHeight { get; private set; }

		public bool layoutOK { get; private set; }

		public static LevelLayer levelLayerGradient { get; private set; }

		public WorldLayout(WorldGen worldGen, int seed)
		{
			this.worldGen = worldGen;
			this.localGraph = new MapGraph(seed);
			this.overworldGraph = new MapGraph(seed);
			this.SetSeed(seed);
		}

		public WorldLayout(WorldGen worldGen, int width, int height, int seed)
			: this(worldGen, seed)
		{
			this.mapWidth = width;
			this.mapHeight = height;
		}

		public void SetSeed(int seed)
		{
			this.myRandom = new SeededRandom(seed);
			this.localGraph.SetSeed(seed);
			this.overworldGraph.SetSeed(seed);
		}

		public Tree GetVoronoiTree()
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

		public Tree GenerateOverworld(bool usePD)
		{
			global::Debug.Assert(this.mapWidth != 0 && this.mapHeight != 0, "Map size has not been set");
			global::Debug.Assert(this.worldGen.Settings.world != null, "You need to set a world");
			Diagram.Site site = new Diagram.Site(0U, new Vector2((float)(this.mapWidth / 2), (float)(this.mapHeight / 2)), 1f);
			this.topEdge = new LineSegment(new Vector2?(new Vector2(0f, (float)(this.mapHeight - 5))), new Vector2?(new Vector2((float)this.mapWidth, (float)(this.mapHeight - 5))));
			this.bottomEdge = new LineSegment(new Vector2?(new Vector2(0f, 5f)), new Vector2?(new Vector2((float)this.mapWidth, 5f)));
			this.leftEdge = new LineSegment(new Vector2?(new Vector2(5f, 0f)), new Vector2?(new Vector2(5f, (float)this.mapHeight)));
			this.rightEdge = new LineSegment(new Vector2?(new Vector2((float)(this.mapWidth - 5), 0f)), new Vector2?(new Vector2((float)(this.mapWidth - 5), (float)this.mapHeight)));
			site.poly = new Polygon(new Rect(0f, 0f, (float)this.mapWidth, (float)this.mapHeight));
			this.voronoiTree = new Tree(site, null, this.myRandom.seed);
			global::VoronoiTree.Node.maxIndex = 0U;
			float floatSetting = this.worldGen.Settings.GetFloatSetting("OverworldDensityMin");
			float floatSetting2 = this.worldGen.Settings.GetFloatSetting("OverworldDensityMax");
			float num = this.myRandom.RandomRange(floatSetting, floatSetting2);
			float floatSetting3 = this.worldGen.Settings.GetFloatSetting("OverworldAvoidRadius");
			PointGenerator.SampleBehaviour enumSetting = this.worldGen.Settings.GetEnumSetting<PointGenerator.SampleBehaviour>("OverworldSampleBehaviour");
			global::Debug.Log(string.Format("Generating overworld points using {0}, density {1}", enumSetting.ToString(), num));
			ProcGen.Node node = null;
			if (!this.worldGen.Settings.world.noStart)
			{
				string startSubworldName = this.worldGen.Settings.world.startSubworldName;
				SubWorld subWorld = this.worldGen.Settings.GetSubWorld(startSubworldName);
				Vector2 vector = new Vector2((float)this.mapWidth * this.worldGen.Settings.world.startingBasePositionHorizontal.GetRandomValueWithinRange(this.myRandom), (float)this.mapHeight * this.worldGen.Settings.world.startingBasePositionVertical.GetRandomValueWithinRange(this.myRandom));
				global::Debug.Log("Start node position is " + vector);
				node = this.overworldGraph.AddNode(startSubworldName);
				node.SetPosition(vector);
				global::VoronoiTree.Node node2 = this.voronoiTree.AddSite(new Diagram.Site((uint)node.node.Id, node.position, subWorld.pdWeight), global::VoronoiTree.Node.NodeType.Internal);
				node2.AddTag(WorldGenTags.AtStart);
				this.ApplySubworldToNode(node2, subWorld);
			}
			List<Vector2> list = new List<Vector2>();
			if (node != null)
			{
				list.Add(node.position);
			}
			List<Vector2> randomPoints = PointGenerator.GetRandomPoints(site.poly, num, floatSetting3, list, enumSetting, false, this.myRandom, false, true);
			global::Debug.Log(string.Format(" -> Generated {0} points", randomPoints.Count));
			int intSetting = this.worldGen.Settings.GetIntSetting("OverworldMaxNodes");
			if (randomPoints.Count > intSetting)
			{
				randomPoints.ShuffleSeeded<Vector2>(this.myRandom.RandomSource());
				randomPoints.RemoveRange(intSetting, randomPoints.Count - intSetting);
			}
			for (int i = 0; i < randomPoints.Count; i++)
			{
				ProcGen.Node node3 = this.overworldGraph.AddNode(WorldGenTags.UnassignedNode.Name);
				node3.SetPosition(randomPoints[i]);
				this.voronoiTree.AddSite(new Diagram.Site((uint)node3.node.Id, node3.position, 1f), global::VoronoiTree.Node.NodeType.Internal).tags.Add(WorldGenTags.UnassignedNode);
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
			this.TagTopAndBottomSites(WorldGenTags.AtSurface, WorldGenTags.AtDepths);
			this.TagEdgeSites(WorldGenTags.AtEdge, WorldGenTags.AtEdge);
			for (int k = 0; k < this.voronoiTree.ChildCount(); k++)
			{
				global::VoronoiTree.Node child = this.voronoiTree.GetChild(k);
				ProcGen.Node node4 = this.overworldGraph.FindNodeByID(child.site.id);
				node4.tags.Union(child.tags);
				node4.SetPosition(child.site.position);
				List<global::VoronoiTree.Node> neighbors = child.GetNeighbors();
				for (int l = 0; l < neighbors.Count; l++)
				{
					ProcGen.Node node5 = this.overworldGraph.FindNodeByID(neighbors[l].site.id);
					this.overworldGraph.AddArc(node4, node5, "Neighbor");
				}
			}
			this.PropagateDistanceTags(this.voronoiTree, WorldGenTags.DistanceTags);
			this.ConvertUnknownCells();
			int intSetting2 = this.worldGen.Settings.GetIntSetting("OverworldRelaxIterations");
			float floatSetting4 = this.worldGen.Settings.GetFloatSetting("OverworldRelaxEnergyMin");
			this.voronoiTree.RelaxRecursive(0, intSetting2, floatSetting4, usePD);
			if (this.worldGen.Settings.GetOverworldAddTags() != null)
			{
				foreach (string text in this.worldGen.Settings.GetOverworldAddTags())
				{
					int num2 = this.myRandom.RandomSource().Next(this.voronoiTree.ChildCount());
					this.voronoiTree.GetChild(num2).AddTag(new Tag(text));
				}
			}
			this.FlattenOverworld();
			return this.voronoiTree;
		}

		public void PopulateSubworlds()
		{
			this.AddSubworldChildren();
			this.GetStartLocation();
			this.PropagateStartTag();
		}

		private void PropagateDistanceTags(Tree tree, TagSet tags)
		{
			foreach (Tag tag in tags)
			{
				Dictionary<uint, int> distanceToTag = this.overworldGraph.GetDistanceToTag(tag);
				if (distanceToTag != null)
				{
					int num = 0;
					for (int i = 0; i < tree.ChildCount(); i++)
					{
						global::VoronoiTree.Node child = tree.GetChild(i);
						uint id = child.site.id;
						if (distanceToTag.ContainsKey(id))
						{
							child.minDistanceToTag.Add(tag, distanceToTag[id]);
							num++;
							if (distanceToTag[id] > 0)
							{
								child.AddTag(new Tag(tag.Name + "_Distance" + distanceToTag[id]));
							}
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

		private HashSet<WeightedSubWorld> GetNameFilterSet(global::VoronoiTree.Node vn, World.AllowedCellsFilter filter, List<WeightedSubWorld> subworlds)
		{
			HashSet<WeightedSubWorld> hashSet = new HashSet<WeightedSubWorld>();
			switch (filter.tagcommand)
			{
			case World.AllowedCellsFilter.TagCommand.Default:
			{
				int num;
				int j;
				for (j = 0; j < filter.subworldNames.Count; j = num + 1)
				{
					hashSet.UnionWith(subworlds.FindAll((WeightedSubWorld f) => f.subWorld.name == filter.subworldNames[j]));
					num = j;
				}
				break;
			}
			case World.AllowedCellsFilter.TagCommand.AtTag:
				if (vn.tags.Contains(filter.tag))
				{
					int num;
					int k;
					for (k = 0; k < filter.subworldNames.Count; k = num + 1)
					{
						hashSet.UnionWith(subworlds.FindAll((WeightedSubWorld f) => f.subWorld.name == filter.subworldNames[k]));
						num = k;
					}
				}
				break;
			case World.AllowedCellsFilter.TagCommand.DistanceFromTag:
				global::Debug.Assert(vn.minDistanceToTag.ContainsKey(filter.tag.ToTag()), filter.tag);
				if (vn.minDistanceToTag[filter.tag.ToTag()] >= filter.minDistance && vn.minDistanceToTag[filter.tag.ToTag()] <= filter.maxDistance)
				{
					int num;
					int i;
					for (i = 0; i < filter.subworldNames.Count; i = num + 1)
					{
						hashSet.UnionWith(subworlds.FindAll((WeightedSubWorld f) => f.subWorld.name == filter.subworldNames[i]));
						num = i;
					}
				}
				break;
			}
			return hashSet;
		}

		private HashSet<WeightedSubWorld> GetZoneTypeFilterSet(global::VoronoiTree.Node vn, World.AllowedCellsFilter filter, Dictionary<string, List<WeightedSubWorld>> subworldsByZoneType)
		{
			HashSet<WeightedSubWorld> hashSet = new HashSet<WeightedSubWorld>();
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
			case World.AllowedCellsFilter.TagCommand.AtTag:
				if (vn.tags.Contains(filter.tag))
				{
					for (int j = 0; j < filter.zoneTypes.Count; j++)
					{
						hashSet.UnionWith(subworldsByZoneType[filter.zoneTypes[j].ToString()]);
					}
				}
				break;
			case World.AllowedCellsFilter.TagCommand.DistanceFromTag:
				global::Debug.Assert(vn.minDistanceToTag.ContainsKey(filter.tag.ToTag()), filter.tag);
				if (vn.minDistanceToTag[filter.tag.ToTag()] >= filter.minDistance && vn.minDistanceToTag[filter.tag.ToTag()] <= filter.maxDistance)
				{
					for (int k = 0; k < filter.zoneTypes.Count; k++)
					{
						hashSet.UnionWith(subworldsByZoneType[filter.zoneTypes[k].ToString()]);
					}
				}
				break;
			}
			return hashSet;
		}

		private HashSet<WeightedSubWorld> GetTemperatureFilterSet(global::VoronoiTree.Node vn, World.AllowedCellsFilter filter, Dictionary<string, List<WeightedSubWorld>> subworldsByTemperature)
		{
			HashSet<WeightedSubWorld> hashSet = new HashSet<WeightedSubWorld>();
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
			case World.AllowedCellsFilter.TagCommand.AtTag:
				if (vn.tags.Contains(filter.tag))
				{
					for (int j = 0; j < filter.temperatureRanges.Count; j++)
					{
						hashSet.UnionWith(subworldsByTemperature[filter.temperatureRanges[j].ToString()]);
					}
				}
				break;
			case World.AllowedCellsFilter.TagCommand.DistanceFromTag:
				global::Debug.Assert(vn.minDistanceToTag.ContainsKey(filter.tag.ToTag()), filter.tag);
				if (vn.minDistanceToTag[filter.tag.ToTag()] >= filter.minDistance && vn.minDistanceToTag[filter.tag.ToTag()] <= filter.maxDistance)
				{
					for (int k = 0; k < filter.temperatureRanges.Count; k++)
					{
						hashSet.UnionWith(subworldsByTemperature[filter.temperatureRanges[k].ToString()]);
					}
				}
				break;
			}
			return hashSet;
		}

		private void RunFilterClearCommand(global::VoronoiTree.Node vn, World.AllowedCellsFilter filter, HashSet<WeightedSubWorld> allowedSubworldsSet)
		{
			switch (filter.tagcommand)
			{
			case World.AllowedCellsFilter.TagCommand.Default:
				allowedSubworldsSet.Clear();
				return;
			case World.AllowedCellsFilter.TagCommand.AtTag:
				if (vn.tags.Contains(filter.tag))
				{
					allowedSubworldsSet.Clear();
					return;
				}
				break;
			case World.AllowedCellsFilter.TagCommand.DistanceFromTag:
				global::Debug.Assert(vn.minDistanceToTag.ContainsKey(filter.tag.ToTag()), filter.tag);
				if (vn.minDistanceToTag[filter.tag.ToTag()] >= filter.minDistance && vn.minDistanceToTag[filter.tag.ToTag()] <= filter.maxDistance)
				{
					allowedSubworldsSet.Clear();
				}
				break;
			default:
				return;
			}
		}

		private HashSet<WeightedSubWorld> Filter(global::VoronoiTree.Node vn, List<WeightedSubWorld> allSubWorlds, Dictionary<string, List<WeightedSubWorld>> subworldsByTemperature, Dictionary<string, List<WeightedSubWorld>> subworldsByZoneType)
		{
			HashSet<WeightedSubWorld> hashSet = new HashSet<WeightedSubWorld>();
			World world = this.worldGen.Settings.world;
			string text = "";
			foreach (KeyValuePair<Tag, int> keyValuePair in vn.minDistanceToTag)
			{
				text = string.Concat(new string[]
				{
					text,
					keyValuePair.Key.Name,
					":",
					keyValuePair.Value.ToString(),
					", "
				});
			}
			foreach (World.AllowedCellsFilter allowedCellsFilter in world.unknownCellsAllowedSubworlds)
			{
				HashSet<WeightedSubWorld> hashSet2 = new HashSet<WeightedSubWorld>();
				if (allowedCellsFilter.subworldNames != null && allowedCellsFilter.subworldNames.Count > 0)
				{
					hashSet2.UnionWith(this.GetNameFilterSet(vn, allowedCellsFilter, allSubWorlds));
				}
				if (allowedCellsFilter.temperatureRanges != null && allowedCellsFilter.temperatureRanges.Count > 0)
				{
					hashSet2.UnionWith(this.GetTemperatureFilterSet(vn, allowedCellsFilter, subworldsByTemperature));
				}
				if (allowedCellsFilter.zoneTypes != null && allowedCellsFilter.zoneTypes.Count > 0)
				{
					hashSet2.UnionWith(this.GetZoneTypeFilterSet(vn, allowedCellsFilter, subworldsByZoneType));
				}
				switch (allowedCellsFilter.command)
				{
				case World.AllowedCellsFilter.Command.Clear:
					this.RunFilterClearCommand(vn, allowedCellsFilter, hashSet);
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
			List<WeightedName> list2 = new List<WeightedName>(this.worldGen.Settings.world.subworldFiles);
			list2.RemoveAll((WeightedName s) => s.name == this.worldGen.Settings.world.startSubworldName);
			List<WeightedSubWorld> subworldsForWorld = this.worldGen.Settings.GetSubworldsForWorld(list2);
			Dictionary<string, List<WeightedSubWorld>> dictionary = new Dictionary<string, List<WeightedSubWorld>>();
			using (IEnumerator enumerator = Enum.GetValues(typeof(Temperature.Range)).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Temperature.Range range = (Temperature.Range)enumerator.Current;
					dictionary.Add(range.ToString(), subworldsForWorld.FindAll((WeightedSubWorld sw) => sw.subWorld.temperatureRange == range));
				}
			}
			Dictionary<string, List<WeightedSubWorld>> dictionary2 = new Dictionary<string, List<WeightedSubWorld>>();
			using (IEnumerator enumerator = Enum.GetValues(typeof(SubWorld.ZoneType)).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SubWorld.ZoneType zt = (SubWorld.ZoneType)enumerator.Current;
					dictionary2.Add(zt.ToString(), subworldsForWorld.FindAll((WeightedSubWorld sw) => sw.subWorld.zoneType == zt));
				}
			}
			foreach (global::VoronoiTree.Node node in list)
			{
				ProcGen.Node node2 = this.overworldGraph.FindNodeByID(node.site.id);
				node.tags.Remove(WorldGenTags.UnassignedNode);
				node2.tags.Remove(WorldGenTags.UnassignedNode);
				WeightedSubWorld weightedSubWorld = WeightedRandom.Choose<WeightedSubWorld>(new List<WeightedSubWorld>(this.Filter(node, subworldsForWorld, dictionary, dictionary2)), this.myRandom);
				if (weightedSubWorld != null)
				{
					SubWorld subWorld = weightedSubWorld.subWorld;
					this.ApplySubworldToNode(node, subWorld);
				}
				else
				{
					string text = "";
					foreach (KeyValuePair<Tag, int> keyValuePair in node.minDistanceToTag)
					{
						text = string.Concat(new string[]
						{
							text,
							keyValuePair.Key.Name,
							":",
							keyValuePair.Value.ToString(),
							", "
						});
					}
					DebugUtil.LogWarningArgs(new object[]
					{
						"No allowed Subworld types. Using default. ",
						node2.tags.ToString(),
						"Distances:",
						text
					});
					node2.SetType("Default");
				}
			}
		}

		private ProcGen.Node ApplySubworldToNode(global::VoronoiTree.Node vn, SubWorld subWorld)
		{
			ProcGen.Node node = this.overworldGraph.FindNodeByID(vn.site.id);
			node.SetType(subWorld.name);
			vn.site.weight = subWorld.pdWeight;
			foreach (string text in subWorld.tags)
			{
				vn.AddTag(new Tag(text));
			}
			return node;
		}

		private void FlattenOverworld()
		{
			try
			{
				for (int i = 0; i < this.voronoiTree.ChildCount(); i++)
				{
					global::VoronoiTree.Node child = this.voronoiTree.GetChild(i);
					if (child.type == global::VoronoiTree.Node.NodeType.Internal)
					{
						Tree tree = child as Tree;
						ProcGen.Node node = this.overworldGraph.FindNodeByID(tree.site.id);
						node.tags.Union(tree.tags);
						bool flag;
						ProcGen.Node cell = this.overworldGraph.GetCell(node.position, node.node, true, out flag);
						global::Debug.Assert(flag, "Tried creating a new cell but one already exists. Huh? " + child.site.id);
						cell.tags.Union(tree.tags);
					}
				}
				for (int j = 0; j < this.voronoiTree.ChildCount(); j++)
				{
					global::VoronoiTree.Node child2 = this.voronoiTree.GetChild(j);
					if (child2.type == global::VoronoiTree.Node.NodeType.Internal)
					{
						List<KeyValuePair<global::VoronoiTree.Node, LineSegment>> neighborsByEdge = (child2 as Tree).GetNeighborsByEdge();
						for (int k = 0; k < neighborsByEdge.Count; k++)
						{
							KeyValuePair<global::VoronoiTree.Node, LineSegment> keyValuePair = neighborsByEdge[k];
							this.overworldGraph.GetCorner(keyValuePair.Value.p0.Value, true);
							this.overworldGraph.GetCorner(keyValuePair.Value.p1.Value, true);
						}
					}
				}
				TagSet tagSet = new TagSet();
				tagSet.Add(WorldGenTags.NearDepths);
				for (int l = 0; l < this.voronoiTree.ChildCount(); l++)
				{
					global::VoronoiTree.Node child3 = this.voronoiTree.GetChild(l);
					if (child3.type == global::VoronoiTree.Node.NodeType.Internal)
					{
						Tree tree2 = child3 as Tree;
						ProcGen.Node node2 = this.overworldGraph.FindNodeByID(tree2.site.id);
						Cell cell2 = this.overworldGraph.GetCell(node2.node);
						global::Debug.Assert(cell2 != null, "cell is null: " + node2.node);
						List<KeyValuePair<global::VoronoiTree.Node, LineSegment>> neighborsByEdge2 = tree2.GetNeighborsByEdge();
						for (int m = 0; m < neighborsByEdge2.Count; m++)
						{
							KeyValuePair<global::VoronoiTree.Node, LineSegment> keyValuePair2 = neighborsByEdge2[m];
							Corner corner = this.overworldGraph.GetCorner(keyValuePair2.Value.p0.Value, false);
							global::Debug.Assert(corner != null, "corner0 is null: " + keyValuePair2.Value.p0);
							Corner corner2 = this.overworldGraph.GetCorner(keyValuePair2.Value.p1.Value, false);
							global::Debug.Assert(corner2 != null, "corner1 is null: " + keyValuePair2.Value.p1);
							global::VoronoiTree.Node key = keyValuePair2.Key;
							Edge edge;
							if (key != null)
							{
								ProcGen.Node node3 = this.overworldGraph.FindNodeByID(key.site.id);
								Cell cell3 = this.overworldGraph.GetCell(node3.node);
								global::Debug.Assert(cell3 != null, "otherCell is null: " + node3.node);
								bool flag2;
								edge = this.overworldGraph.GetEdge(corner, corner2, cell2, cell3, true, out flag2);
								SubWorld subWorld = this.worldGen.Settings.GetSubWorld(node2.type);
								global::Debug.Assert(subWorld != null, "SubWorld is null: " + node2.type);
								SubWorld subWorld2 = this.worldGen.Settings.GetSubWorld(node3.type);
								global::Debug.Assert(subWorld2 != null, "other SubWorld is null: " + node3.type);
								if (node2.type == node3.type || subWorld.zoneType == subWorld2.zoneType || (subWorld.zoneType == SubWorld.ZoneType.Space && subWorld2.zoneType == SubWorld.ZoneType.Space) || (cell2.tags.ContainsOne(tagSet) && cell3.tags.ContainsOne(tagSet)))
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
								bool flag3;
								edge = this.overworldGraph.GetEdge(corner, corner2, cell2, cell2, true, out flag3);
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
				global::Debug.LogError("ex: " + message + " " + stackTrace);
			}
		}

		public static bool TestEdgeConsistency(Cell cell, out Edge problemEdge)
		{
			for (int i = 0; i < cell.edges.Count; i++)
			{
				Edge edge = cell.edges[i];
				if (!WorldLayout.IsEdgeConsistent(cell, edge))
				{
					problemEdge = edge;
					return false;
				}
			}
			problemEdge = null;
			return true;
		}

		public static bool IsEdgeConsistent(Cell cell, Edge edge1)
		{
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < cell.edges.Count; i++)
			{
				Edge edge2 = cell.edges[i];
				if (edge1 != edge2)
				{
					if (edge1.corner0 == edge2.corner0 || edge1.corner0 == edge2.corner1)
					{
						flag = true;
					}
					if (edge1.corner1 == edge2.corner0 || edge1.corner1 == edge2.corner1)
					{
						flag2 = true;
					}
				}
			}
			return flag && flag2;
		}

		public bool IsNodeBorderOpen(global::VoronoiTree.Node n1, global::VoronoiTree.Node n2, TagSet edgeOpenTags)
		{
			global::Debug.Assert(n1 != null, "Border test: n1 was null");
			global::Debug.Assert(n2 != null, "Border test: n2 was null");
			ProcGen.Node node = this.overworldGraph.FindNodeByID(n1.site.id);
			ProcGen.Node node2 = this.overworldGraph.FindNodeByID(n2.site.id);
			global::Debug.Assert(node != null, "Border test: tn1 was null");
			global::Debug.Assert(node2 != null, "Border test: tn2 was null");
			Cell cell = this.overworldGraph.GetCell(node.node);
			Cell cell2 = this.overworldGraph.GetCell(node2.node);
			global::Debug.Assert(cell != null, "Border test: cell1 was null");
			global::Debug.Assert(cell2 != null, "Border test: cell2 was null");
			SubWorld subWorld = this.worldGen.Settings.GetSubWorld(node.type);
			SubWorld subWorld2 = this.worldGen.Settings.GetSubWorld(node2.type);
			global::Debug.Assert(subWorld != null, "Border test: sw1 was null");
			global::Debug.Assert(subWorld2 != null, "Border test: sw2 was null");
			return node.type == node2.type || subWorld.zoneType == subWorld2.zoneType || (subWorld.zoneType == SubWorld.ZoneType.Space && subWorld2.zoneType == SubWorld.ZoneType.Space) || (cell.tags.ContainsOne(edgeOpenTags) && cell2.tags.ContainsOne(edgeOpenTags));
		}

		private void AddSubworldChildren()
		{
			TagSet tagSet = new TagSet();
			tagSet.Add(WorldGenTags.Overworld);
			List<string> defaultMoveTags = this.worldGen.Settings.GetDefaultMoveTags();
			TagSet tagSet2 = ((defaultMoveTags != null) ? new TagSet(defaultMoveTags) : null);
			global::VoronoiTree.Node.SplitCommand splitCommand = new global::VoronoiTree.Node.SplitCommand();
			splitCommand.dontCopyTags = tagSet;
			splitCommand.moveTags = tagSet2;
			splitCommand.SplitFunction = new Action<Tree, global::VoronoiTree.Node.SplitCommand>(this.SplitFunction);
			List<Feature> list = new List<Feature>();
			foreach (KeyValuePair<string, int> keyValuePair in this.worldGen.Settings.world.globalFeatures)
			{
				for (int i = 0; i < keyValuePair.Value; i++)
				{
					list.Add(new Feature
					{
						type = keyValuePair.Key
					});
				}
			}
			Dictionary<uint, List<Feature>> dictionary = new Dictionary<uint, List<Feature>>();
			List<global::VoronoiTree.Node> list2 = new List<global::VoronoiTree.Node>();
			this.voronoiTree.GetNodesWithoutTag(WorldGenTags.NoGlobalFeatureSpawning, list2);
			list2.ShuffleSeeded<global::VoronoiTree.Node>(this.myRandom.RandomSource());
			foreach (Feature feature in list)
			{
				global::VoronoiTree.Node node = list2[0];
				list2.RemoveAt(0);
				if (!dictionary.ContainsKey(node.site.id))
				{
					dictionary[node.site.id] = new List<Feature>();
				}
				dictionary[node.site.id].Add(feature);
			}
			for (int j = 0; j < this.voronoiTree.ChildCount(); j++)
			{
				global::VoronoiTree.Node child = this.voronoiTree.GetChild(j);
				if (child.type == global::VoronoiTree.Node.NodeType.Internal)
				{
					Tree tree = child as Tree;
					ProcGen.Node node2 = this.overworldGraph.FindNodeByID(tree.site.id);
					SubWorld subWorld = SerializingCloner.Copy<SubWorld>(this.worldGen.Settings.GetSubWorld(node2.type));
					tree.AddTag(new Tag(node2.type));
					tree.AddTag(new Tag(subWorld.temperatureRange.ToString()));
					if (dictionary.ContainsKey(child.site.id))
					{
						subWorld.features.AddRange(dictionary[child.site.id]);
					}
					this.GenerateChildren(subWorld, tree, this.localGraph, (float)this.mapHeight, j + this.myRandom.seed);
					int num = tree.ChildCount();
					if (num < subWorld.minChildCount)
					{
						tree.AddTag(WorldGenTags.DEBUG_SplitForChildCount);
						splitCommand.dontCopyTags = tagSet;
						splitCommand.minChildCount = subWorld.minChildCount - num;
						tree.Split(splitCommand);
						if (subWorld.biomes != null && subWorld.biomes.Count > 0)
						{
							for (int k = num; k < tree.ChildCount(); k++)
							{
								WeightedBiome weightedBiome = WeightedRandom.Choose<WeightedBiome>(subWorld.biomes, this.myRandom);
								ProcGen.Node node3 = this.localGraph.FindNodeByID(tree.GetChild(k).site.id);
								node3.SetType(weightedBiome.name);
								tree.GetChild(k).AddTag(new Tag(node3.type));
							}
						}
						else
						{
							for (int l = num; l < tree.ChildCount(); l++)
							{
								ProcGen.Node node4 = this.localGraph.FindNodeByID(tree.GetChild(l).site.id);
								node4.SetType(WorldLayout.GetNodeTypeFromLayers(tree.site.position, (float)this.mapHeight, this.myRandom));
								tree.GetChild(l).AddTag(new Tag(node4.type));
							}
						}
					}
					tree.RelaxRecursive(0, 10, 1f, this.worldGen.Settings.world.layoutMethod == World.LayoutMethod.PowerTree);
					List<global::VoronoiTree.Node> list3 = new List<global::VoronoiTree.Node>();
					tree.GetNodesWithTag(WorldGenTags.Feature, list3);
					splitCommand.dontCopyTags = new TagSet
					{
						WorldGenTags.Feature,
						WorldGenTags.SplitOnParentDensity
					};
					for (int m = 0; m < list3.Count; m++)
					{
						if (!list3[m].tags.Contains(WorldGenTags.CenteralFeature))
						{
							if (list3[m].tags.Contains(WorldGenTags.SplitOnParentDensity))
							{
								list3[m].Split(splitCommand);
							}
							if (list3[m].tags.Contains(WorldGenTags.SplitTwice))
							{
								Tree tree2 = list3[m].Split(splitCommand);
								if (tree2.ChildCount() <= 1)
								{
									global::Debug.LogError("split did not work.");
								}
								for (int n = 0; n < tree2.ChildCount(); n++)
								{
									tree2.GetChild(n).Split(splitCommand);
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
					bool isRunningDebugGen = this.worldGen.isRunningDebugGen;
				}
				num++;
			}
			while (randomPoints.Count < minPointCount && num < 10);
			return randomPoints;
		}

		public void GenerateChildren(SubWorld sw, Tree node, Graph graph, float worldHeight, int seed)
		{
			SeededRandom seededRandom = new SeededRandom(seed);
			List<string> defaultMoveTags = this.worldGen.Settings.GetDefaultMoveTags();
			TagSet tagSet = ((defaultMoveTags != null) ? new TagSet(defaultMoveTags) : null);
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
			List<Vector2> list = new List<Vector2>();
			if (sw.centralFeature != null)
			{
				list.Add(node.site.poly.Centroid());
				this.CreateTreeNodeWithFeatureAndBiome(this.worldGen.Settings, sw, node, graph, sw.centralFeature, node.site.poly.Centroid(), tagSet3, -1).AddTag(WorldGenTags.CenteralFeature);
			}
			node.dontRelaxChildren = sw.dontRelaxChildren;
			int num = ((sw.features.Count > 0) ? sw.features.Count : 2);
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
				string text = "";
				for (int l = 0; l < node.site.poly.Vertices.Count; l++)
				{
					text = text + node.site.poly.Vertices[l] + ", ";
				}
				if (this.worldGen.isRunningDebugGen)
				{
					global::Debug.Assert(points.Count >= num, string.Concat(new object[]
					{
						"Error not enough points ",
						sw.name,
						" in node ",
						node.site.id
					}));
				}
				return;
			}
			int count = sw.features.Count;
			int count2 = points.Count;
			for (int m = 0; m < points.Count; m++)
			{
				Feature feature = null;
				if (m < sw.features.Count)
				{
					feature = sw.features[m];
				}
				this.CreateTreeNodeWithFeatureAndBiome(this.worldGen.Settings, sw, node, graph, feature, points[m], tagSet3, m);
			}
			node.ComputeChildren(seededRandom.seed + 1, false, false);
			if (node.ChildCount() > 0)
			{
				for (int n = 0; n < tagSet2.Count; n++)
				{
					global::Debug.Log(string.Format("Applying Moved Tag {0} to {1}", tagSet2[n].Name, node.site.id));
					node.GetChild(seededRandom.RandomSource().Next(node.ChildCount())).AddTag(tagSet2[n]);
				}
			}
		}

		private global::VoronoiTree.Node CreateTreeNodeWithFeatureAndBiome(WorldGenSettings settings, SubWorld sw, Tree node, Graph graph, Feature feature, Vector2 pos, TagSet newTags, int i)
		{
			bool flag = false;
			TagSet tagSet = new TagSet();
			TagSet tagSet2 = new TagSet();
			string text;
			if (feature != null)
			{
				FeatureSettings feature2 = settings.GetFeature(feature.type);
				text = feature.type;
				tagSet2.Union(new TagSet(feature2.tags));
				if (feature.tags != null && feature.tags.Count > 0)
				{
					tagSet2.Union(new TagSet(feature.tags));
				}
				if (feature.excludesTags != null && feature.excludesTags.Count > 0)
				{
					tagSet2.Remove(new TagSet(feature.excludesTags));
				}
				tagSet2.Add(new Tag(feature.type));
				tagSet2.Add(WorldGenTags.Feature);
				if (feature2.forceBiome != null)
				{
					tagSet.Add(feature2.forceBiome);
					flag = true;
				}
				if (feature2.biomeTags != null)
				{
					tagSet.Union(new TagSet(feature2.biomeTags));
				}
			}
			if (!flag && sw.biomes.Count > 0)
			{
				WeightedBiome weightedBiome = WeightedRandom.Choose<WeightedBiome>(sw.biomes, this.myRandom);
				text = weightedBiome.name;
				tagSet.Add(weightedBiome.name);
				if (weightedBiome.tags != null && weightedBiome.tags.Count > 0)
				{
					tagSet.Union(new TagSet(weightedBiome.tags));
				}
			}
			else
			{
				text = "UNKNOWN";
			}
			ProcGen.Node node2 = graph.AddNode(text);
			node2.biomeSpecificTags = new TagSet(tagSet);
			node2.featureSpecificTags = new TagSet(tagSet2);
			node2.SetPosition(pos);
			global::VoronoiTree.Node node3 = node.AddSite(new Diagram.Site((uint)node2.node.Id, node2.position, 1f), global::VoronoiTree.Node.NodeType.Internal);
			node3.tags = new TagSet(newTags);
			node3.tags.Add(text);
			node3.tags.Union(tagSet);
			node3.tags.Union(tagSet2);
			return node3;
		}

		private void SplitTopAndBottomSites()
		{
			float floatSetting = this.worldGen.Settings.GetFloatSetting("SplitTopAndBottomSitesMaxArea");
			TagSet tagSet = new TagSet();
			tagSet.Add(WorldGenTags.Overworld);
			TagSet tagSet2 = new TagSet(this.worldGen.Settings.GetDefaultMoveTags());
			List<global::VoronoiTree.Node> list = new List<global::VoronoiTree.Node>();
			this.voronoiTree.GetNodesWithTag(WorldGenTags.NearSurface, list);
			global::VoronoiTree.Node.SplitCommand splitCommand = new global::VoronoiTree.Node.SplitCommand();
			splitCommand.dontCopyTags = tagSet;
			splitCommand.moveTags = tagSet2;
			splitCommand.SplitFunction = new Action<Tree, global::VoronoiTree.Node.SplitCommand>(this.SplitFunction);
			for (int i = 0; i < list.Count; i++)
			{
				global::VoronoiTree.Node node = list[i];
				if (node.site.poly.Area() > floatSetting)
				{
					node.Split(splitCommand);
				}
			}
			List<global::VoronoiTree.Node> list2 = new List<global::VoronoiTree.Node>();
			this.voronoiTree.GetNodesWithTag(WorldGenTags.NearDepths, list2);
			for (int j = 0; j < list2.Count; j++)
			{
				global::VoronoiTree.Node node2 = list2[j];
				if (node2.site.poly.Area() > floatSetting)
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

		private void SplitFunction(Tree tree, global::VoronoiTree.Node.SplitCommand cmd)
		{
			ProcGen.Node node;
			if (tree.tags.Contains(WorldGenTags.Overworld))
			{
				node = this.worldGen.WorldLayout.overworldGraph.FindNodeByID(tree.site.id);
			}
			else
			{
				node = this.worldGen.WorldLayout.localGraph.FindNodeByID(tree.site.id);
			}
			global::Debug.Assert(node != null, "Null terrain node WTF");
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
				ProcGen.Node node2 = this.worldGen.WorldLayout.localGraph.AddNode(node.type);
				node2.SetPosition(tagSet.Contains(WorldGenTags.CenteralFeature) ? tree.site.poly.Centroid() : tree.site.position);
				global::VoronoiTree.Node node3 = tree.AddSite(new Diagram.Site((uint)node2.node.Id, node2.position, 1f), global::VoronoiTree.Node.NodeType.Leaf);
				if (tagSet != null && tagSet.Count != 0)
				{
					node3.SetTags(tagSet);
				}
				tagSet.Remove(WorldGenTags.Feature);
				tagSet.Remove(new Tag(node.type));
				list.Add(node2.position);
			}
			float num = this.worldGen.Settings.GetFloatSetting("SplitDensityMin");
			float num2 = this.worldGen.Settings.GetFloatSetting("SplitDensityMax");
			if (tree.tags.Contains(WorldGenTags.UltraHighDensitySplit))
			{
				num = this.worldGen.Settings.GetFloatSetting("UltraHighSplitDensityMin");
				num2 = this.worldGen.Settings.GetFloatSetting("UltraHighSplitDensityMax");
			}
			else if (tree.tags.Contains(WorldGenTags.VeryHighDensitySplit))
			{
				num = this.worldGen.Settings.GetFloatSetting("VeryHighSplitDensityMin");
				num2 = this.worldGen.Settings.GetFloatSetting("VeryHighSplitDensityMax");
			}
			else if (tree.tags.Contains(WorldGenTags.HighDensitySplit))
			{
				num = this.worldGen.Settings.GetFloatSetting("HighSplitDensityMin");
				num2 = this.worldGen.Settings.GetFloatSetting("HighSplitDensityMax");
			}
			else if (tree.tags.Contains(WorldGenTags.MediumDensitySplit))
			{
				num = this.worldGen.Settings.GetFloatSetting("MediumSplitDensityMin");
				num2 = this.worldGen.Settings.GetFloatSetting("MediumSplitDensityMax");
			}
			float num3 = tree.myRandom.RandomRange(num, num2);
			List<Vector2> points = this.GetPoints(tree.site.id.ToString(), tree.log, cmd.minChildCount, tree.site.poly, num3, 1f, list, PointGenerator.SampleBehaviour.PoissonDisk, true, tree.myRandom, true, true);
			if (points.Count < cmd.minChildCount)
			{
				if (this.worldGen.isRunningDebugGen)
				{
					global::Debug.Assert(points.Count >= cmd.minChildCount, string.Concat(new object[]
					{
						"Error not enough points [",
						cmd.minChildCount,
						"] for tree split ",
						tree.site.id.ToString()
					}));
				}
				if (points.Count == 0)
				{
					return;
				}
			}
			for (int j = 0; j < points.Count; j++)
			{
				ProcGen.Node node4 = this.worldGen.WorldLayout.localGraph.AddNode((cmd.typeOverride == null) ? node.type : cmd.typeOverride(points[j]));
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
				this.voronoiTree.GetNodeForSite(list[i]).AddTag(topTag);
			}
			for (int j = 0; j < list2.Count; j++)
			{
				this.voronoiTree.GetNodeForSite(list2[j]).AddTag(bottomTag);
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
				this.voronoiTree.GetNodeForSite(list[i]).AddTag(leftTag);
			}
			for (int j = 0; j < list2.Count; j++)
			{
				this.voronoiTree.GetNodeForSite(list2[j]).AddTag(rightTag);
			}
		}

		private bool StartAreaTooLarge(global::VoronoiTree.Node node)
		{
			return node.tags.Contains(WorldGenTags.AtStart) && node.site.poly.Area() > 2000f;
		}

		private void SplitLargeStartingSites()
		{
			TagSet tagSet = new TagSet();
			tagSet.Add(WorldGenTags.Overworld);
			List<string> defaultMoveTags = this.worldGen.Settings.GetDefaultMoveTags();
			TagSet tagSet2 = ((defaultMoveTags != null) ? new TagSet(defaultMoveTags) : null);
			List<global::VoronoiTree.Node> list = new List<global::VoronoiTree.Node>();
			this.voronoiTree.GetLeafNodes(list, new Tree.LeafNodeTest(this.StartAreaTooLarge));
			global::VoronoiTree.Node.SplitCommand splitCommand = new global::VoronoiTree.Node.SplitCommand();
			splitCommand.dontCopyTags = tagSet;
			splitCommand.moveTags = tagSet2;
			splitCommand.SplitFunction = new Action<Tree, global::VoronoiTree.Node.SplitCommand>(this.SplitFunction);
			while (list.Count > 0)
			{
				foreach (global::VoronoiTree.Node node in list)
				{
					node.AddTag(WorldGenTags.DEBUG_SplitLargeStartingSites);
					node.Split(splitCommand);
				}
				list.Clear();
				this.voronoiTree.GetLeafNodes(list, new Tree.LeafNodeTest(this.StartAreaTooLarge));
			}
		}

		private void PropagateStartTag()
		{
			foreach (global::VoronoiTree.Node node in this.GetStartNodes())
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
			foreach (global::VoronoiTree.Node node in this.GetLeafNodesWithTag(tag))
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
			if (this.worldGen.Settings.world.noStart)
			{
				global::Debug.Log("World is configured 'noStart'");
				return new Vector2I(this.mapWidth / 2, this.mapHeight / 2);
			}
			ProcGen.Node node2 = this.FindFirstNodeWithTag(WorldGenTags.StartLocation);
			if (node2 == null)
			{
				List<global::VoronoiTree.Node> nodes = this.GetStartNodes();
				if (nodes == null || nodes.Count == 0)
				{
					global::Debug.LogWarning("Couldnt find start node");
					return new Vector2I(this.mapWidth / 2, this.mapHeight / 2);
				}
				node2 = this.localGraph.FindNode((ProcGen.Node node) => (uint)node.node.Id == nodes[0].site.id);
				node2.tags.Add(WorldGenTags.StartLocation);
			}
			if (node2 == null)
			{
				global::Debug.LogWarning("Couldnt find start node");
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
				if (tn0 != null && tn1 != null && !(tn0.type != tn1.type) && tn0.type.Contains(WorldGenTags.River.Name) && list.Find((River r) => r.SinkPosition() == tn0.position && r.SourcePosition() == tn1.position) == null)
				{
					River river;
					if (SettingsCache.rivers.ContainsKey(tn0.type))
					{
						river = new River(SettingsCache.rivers[tn0.type], false);
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
			return list;
		}

		private List<Diagram.Site> GetIntersectingSites(global::VoronoiTree.Node intersectingSiteSource, Tree sitesSource)
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

		public void GetEdgeOfMapSites(Tree vt, List<Diagram.Site> topSites, List<Diagram.Site> bottomSites, List<Diagram.Site> leftSites, List<Diagram.Site> rightSites)
		{
			vt.GetIntersectingLeafSites(this.topEdge, topSites);
			vt.GetIntersectingLeafSites(this.bottomEdge, bottomSites);
			vt.GetIntersectingLeafSites(this.leftEdge, leftSites);
			vt.GetIntersectingLeafSites(this.rightEdge, rightSites);
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
					using (List<global::VoronoiTree.Node>.Enumerator enumerator = list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Leaf ln = (Leaf)enumerator.Current;
							if (ln != null)
							{
								this.extra.leafInternalParent.Add(new KeyValuePair<int, int>(this.extra.leafs.Count, this.extra.internals.FindIndex(0, (Tree n) => n == ln.parent)));
								this.extra.leafs.Add(ln);
							}
						}
					}
					for (int i = 0; i < this.extra.internals.Count; i++)
					{
						Tree vt = this.extra.internals[i];
						if (vt.parent != null)
						{
							int num = this.extra.internals.FindIndex(0, (Tree n) => n == vt.parent);
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
				global::Debug.Log("Error deserialising " + ex.Message);
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
					Tree tree = this.extra.internals[keyValuePair.Key];
					this.extra.internals[keyValuePair.Value].AddChild(tree);
				}
				for (int j = 0; j < this.extra.leafInternalParent.Count; j++)
				{
					KeyValuePair<int, int> keyValuePair2 = this.extra.leafInternalParent[j];
					global::VoronoiTree.Node node = this.extra.leafs[keyValuePair2.Key];
					this.extra.internals[keyValuePair2.Value].AddChild(node);
				}
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				string stackTrace = ex.StackTrace;
				WorldGenLogger.LogException(message, stackTrace);
				global::Debug.Log("Error deserialising " + ex.Message);
			}
			this.extra = null;
		}

		private Tree voronoiTree;

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

		private WorldGen worldGen;

		[Serialize]
		private WorldLayout.ExtraIO extra;

		[Flags]
		public enum DebugFlags
		{
			LocalGraph = 1,
			OverworldGraph = 2,
			VoronoiTree = 4,
			PowerDiagram = 8
		}

		[SerializationConfig(MemberSerialization.OptOut)]
		private class ExtraIO
		{
			[OnDeserializing]
			internal void OnDeserializingMethod()
			{
				this.leafs = new List<Leaf>();
				this.internals = new List<Tree>();
				this.leafInternalParent = new List<KeyValuePair<int, int>>();
				this.internalInternalParent = new List<KeyValuePair<int, int>>();
			}

			public List<Leaf> leafs = new List<Leaf>();

			public List<Tree> internals = new List<Tree>();

			public List<KeyValuePair<int, int>> leafInternalParent = new List<KeyValuePair<int, int>>();

			public List<KeyValuePair<int, int>> internalInternalParent = new List<KeyValuePair<int, int>>();
		}
	}
}
