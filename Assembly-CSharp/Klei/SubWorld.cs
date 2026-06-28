using System;
using System.Collections.Generic;
using Generated;
using KSerialization.Converters;
using UnityEngine;

namespace Klei
{
	public class SubWorld : SampleDescriber
	{
		public SubWorld()
		{
			this.minChildCount = 2;
			this.features = new List<Feature>();
			this.overrides = new List<SampleDescriber.Override>();
			this.tags = new List<string>();
			this.biomes = new List<WeightedBiome>();
			this.samplers = new List<SampleDescriber>();
		}

		public string biomeNoise { get; private set; }

		public string overrideNoise { get; private set; }

		public string densityNoise { get; private set; }

		[StringEnumConverter]
		public Temperature.Range temperatureRange { get; private set; }

		public Feature centralFeature { get; private set; }

		public List<Feature> features { get; private set; }

		public List<SampleDescriber.Override> overrides { get; private set; }

		public List<string> tags { get; private set; }

		public int minChildCount { get; private set; }

		public List<WeightedBiome> biomes { get; private set; }

		public int iterations { get; private set; }

		public float minEnergy { get; private set; }

		public SubWorld.ZoneType zoneType { get; private set; }

		public List<SampleDescriber> samplers { get; private set; }

		private Node AddCenteralFeature(VoronoiTree node, Graph graph, TagSet newTags)
		{
			if (this.centralFeature == null)
			{
				return null;
			}
			Node node2 = graph.AddNode(this.centralFeature.type);
			node2.position = node.site.poly.Centroid();
			VoronoiNode voronoiNode = node.AddSite(new VoronoiDiagram.Site((uint)node2.node.Id, node2.position, 1f), VoronoiNode.NodeType.Internal);
			voronoiNode.tags = new TagSet(newTags);
			voronoiNode.AddTag(new Tag(this.centralFeature.type));
			voronoiNode.AddTag(WorldGenTags.Feature);
			voronoiNode.AddTag(WorldGenTags.CenteralFeature);
			for (int i = 0; i < this.centralFeature.tags.Count; i++)
			{
				voronoiNode.AddTag(new Tag(this.centralFeature.tags[i]));
			}
			return node2;
		}

		public void GenerateStartArea(VoronoiTree node, Graph graph)
		{
		}

		public void GenerateChildren(VoronoiTree node, Graph graph, float worldHeight)
		{
			TagSet tagSet = new TagSet();
			tagSet.Add(WorldGenTags.Geode);
			TagSet tagSet2 = new TagSet(node.tags);
			tagSet2.Remove(WorldGenTags.Overworld);
			for (int i = 0; i < this.tags.Count; i++)
			{
				tagSet2.Add(new Tag(this.tags[i]));
			}
			TagSet tagSet3 = new TagSet();
			if (tagSet != null)
			{
				for (int j = 0; j < tagSet.Count; j++)
				{
					Tag tag = tagSet[j];
					if (node.tags.Contains(tag))
					{
						node.tags.Remove(tag);
						tagSet3.Add(tag);
					}
					else if (tagSet2.Contains(tag))
					{
						tagSet2.Remove(tag);
						tagSet3.Add(tag);
					}
				}
			}
			float value = base.density.GetValue();
			Node node2 = this.AddCenteralFeature(node, graph, tagSet2);
			List<Vector2> list = null;
			if (node2 != null)
			{
				list = new List<Vector2>();
				list.Add(node2.position);
				foreach (WeightedBiome weightedBiome in this.biomes)
				{
					if (weightedBiome.name == node2.type)
					{
						TagSet tagSet4 = new TagSet(weightedBiome.tags);
						node2.tags.Union(tagSet4);
						break;
					}
				}
			}
			node.dontRelaxChildren = base.dontRelaxChildren;
			List<Vector2> randomPoints = PointGenerator.GetRandomPoints(node.site.poly, value, base.avoidRadius, list, base.sampleBehaviour, true, true, base.doAvoidPoints);
			for (int k = 0; k < this.samplers.Count; k++)
			{
				list.AddRange(randomPoints);
				float value2 = this.samplers[k].density.GetValue();
				randomPoints.AddRange(PointGenerator.GetRandomPoints(node.site.poly, value2, this.samplers[k].avoidRadius, list, this.samplers[k].sampleBehaviour, true, true, this.samplers[k].doAvoidPoints));
			}
			if (randomPoints.Count > 200)
			{
				randomPoints.RemoveRange(200, randomPoints.Count - 200);
			}
			int l = 0;
			for (int m = 0; m < this.features.Count; m++)
			{
				Feature feature = this.features[m];
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
					foreach (WeightedBiome weightedBiome2 in this.biomes)
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
				if (feature.type.Contains(WorldGenTags.River.Name) && l + 2 < randomPoints.Count)
				{
					Node node3 = graph.AddNode(feature.type);
					node3.biomeSpecificTags = new TagSet(tagSet6);
					VoronoiNode voronoiNode = node.AddSite(new VoronoiDiagram.Site((uint)node3.node.Id, node3.position, 1f), VoronoiNode.NodeType.Internal);
					voronoiNode.tags = new TagSet(tagSet5);
					node3.position = randomPoints[l++];
					Node node4 = graph.AddNode(feature.type);
					node4.biomeSpecificTags = new TagSet(tagSet6);
					VoronoiNode voronoiNode2 = node.AddSite(new VoronoiDiagram.Site((uint)node4.node.Id, node4.position, 1f), VoronoiNode.NodeType.Internal);
					voronoiNode2.tags = new TagSet(tagSet5);
					node4.position = randomPoints[l++];
					graph.AddArc(node3, node4, feature.type);
				}
				else if (l + 1 < randomPoints.Count)
				{
					Node node5 = graph.AddNode(feature.type);
					node5.biomeSpecificTags = new TagSet(tagSet6);
					node5.position = ((!(feature.type == WorldGenTags.StartLocation.Name)) ? randomPoints[l++] : node.site.poly.Centroid());
					VoronoiNode voronoiNode3 = node.AddSite(new VoronoiDiagram.Site((uint)node5.node.Id, node5.position, 1f), VoronoiNode.NodeType.Internal);
					voronoiNode3.tags = new TagSet(tagSet5);
				}
			}
			while (l < randomPoints.Count)
			{
				TagSet tagSet9 = null;
				string text;
				if (this.biomes.Count > 0)
				{
					WeightedBiome weightedBiome3 = global::Generated.Util.WeightedRandom.Choose<WeightedBiome>(this.biomes);
					text = weightedBiome3.name;
					if (weightedBiome3.tags != null && weightedBiome3.tags.Count > 0)
					{
						tagSet9 = new TagSet(weightedBiome3.tags);
					}
				}
				else
				{
					text = WorldLayout.GetNodeTypeFromLayers(randomPoints[l], worldHeight);
				}
				Node node6 = graph.AddNode(text);
				node6.biomeSpecificTags = tagSet9;
				node6.position = randomPoints[l];
				VoronoiNode voronoiNode4 = node.AddSite(new VoronoiDiagram.Site((uint)node6.node.Id, node6.position, 1f), VoronoiNode.NodeType.Internal);
				voronoiNode4.tags = new TagSet(tagSet2);
				if (tagSet9 != null)
				{
					voronoiNode4.tags.Union(tagSet9);
				}
				voronoiNode4.AddTag(new Tag(text));
				voronoiNode4.AddTag(new Tag(string.Concat(new object[] { "ExtraPoint:", text, "(", l, ")" })));
				l++;
			}
			node.ComputeChildren();
			for (int n = 0; n < tagSet3.Count; n++)
			{
				node.GetChild((int)WorldGen.RandomRange(0f, (float)node.ChildCount())).AddTag(tagSet3[n]);
			}
		}

		public enum ZoneType
		{
			FrozenWastes,
			CrystalCaverns,
			BoggyMarsh,
			Sandstone,
			ToxicJungle,
			MagmaCore
		}
	}
}
