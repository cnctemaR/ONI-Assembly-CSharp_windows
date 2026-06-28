using System;
using System.Collections.Generic;
using KSerialization.Converters;
using VoronoiTree;

namespace ProcGen
{
	public class SubWorld : SampleDescriber
	{
		public SubWorld()
		{
			this.minChildCount = 2;
			this.features = new List<Feature>();
			this.tags = new List<string>();
			this.biomes = new List<WeightedBiome>();
			this.samplers = new List<SampleDescriber>();
			this.featureTemplates = new Dictionary<string, int>();
		}

		public string biomeNoise { get; private set; }

		public string overrideNoise { get; private set; }

		public string densityNoise { get; private set; }

		[StringEnumConverter]
		public Temperature.Range temperatureRange { get; private set; }

		public Feature centralFeature { get; private set; }

		public List<Feature> features { get; private set; }

		public SampleDescriber.Override overrides { get; private set; }

		public List<string> tags { get; private set; }

		public int minChildCount { get; private set; }

		public List<WeightedBiome> biomes { get; private set; }

		public Dictionary<string, string[]> pointsOfInterest { get; private set; }

		public Dictionary<string, int> featureTemplates { get; private set; }

		public int iterations { get; private set; }

		public float minEnergy { get; private set; }

		public SubWorld.ZoneType zoneType { get; private set; }

		public List<SampleDescriber> samplers { get; private set; }

		public Node AddCenteralFeature(Tree node, Graph graph, TagSet newTags)
		{
			if (this.centralFeature == null)
			{
				return null;
			}
			Node node2 = graph.AddNode(this.centralFeature.type);
			node2.SetPosition(node.site.poly.Centroid());
			Node node3 = node.AddSite(new Diagram.Site((uint)node2.node.Id, node2.position, 1f), Node.NodeType.Internal);
			node3.tags = new TagSet(newTags);
			node3.AddTag(new Tag(this.centralFeature.type));
			node3.AddTag(WorldGenTags.Feature);
			node3.AddTag(WorldGenTags.CenteralFeature);
			for (int i = 0; i < this.centralFeature.tags.Count; i++)
			{
				node3.AddTag(new Tag(this.centralFeature.tags[i]));
			}
			return node2;
		}

		public void GenerateStartArea(Tree node, Graph graph)
		{
		}

		public float pdWeight;

		public enum ZoneType
		{
			FrozenWastes,
			CrystalCaverns,
			BoggyMarsh,
			Sandstone,
			ToxicJungle,
			MagmaCore,
			OilField
		}
	}
}
