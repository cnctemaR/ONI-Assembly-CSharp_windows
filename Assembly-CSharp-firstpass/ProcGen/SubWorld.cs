using System;
using System.Collections.Generic;
using KSerialization.Converters;

namespace ProcGen
{
	[Serializable]
	public class SubWorld : SampleDescriber
	{
		public string biomeNoise { get; protected set; }

		public string overrideNoise { get; protected set; }

		public string densityNoise { get; protected set; }

		public string borderOverride { get; protected set; }

		[StringEnumConverter]
		public Temperature.Range temperatureRange { get; protected set; }

		public Feature centralFeature { get; protected set; }

		public List<Feature> features { get; protected set; }

		public SampleDescriber.Override overrides { get; protected set; }

		public List<string> tags { get; protected set; }

		public int minChildCount { get; protected set; }

		public List<WeightedBiome> biomes { get; protected set; }

		public Dictionary<string, string[]> pointsOfInterest { get; protected set; }

		public Dictionary<string, int> featureTemplates { get; protected set; }

		public int iterations { get; protected set; }

		public float minEnergy { get; protected set; }

		public SubWorld.ZoneType zoneType { get; private set; }

		public List<SampleDescriber> samplers { get; private set; }

		public float pdWeight { get; private set; }

		public SubWorld()
		{
			this.minChildCount = 2;
			this.features = new List<Feature>();
			this.tags = new List<string>();
			this.biomes = new List<WeightedBiome>();
			this.samplers = new List<SampleDescriber>();
			this.pointsOfInterest = new Dictionary<string, string[]>();
			this.featureTemplates = new Dictionary<string, int>();
			this.pdWeight = 1f;
		}

		public enum ZoneType
		{
			FrozenWastes,
			CrystalCaverns,
			BoggyMarsh,
			Sandstone,
			ToxicJungle,
			MagmaCore,
			OilField,
			Space,
			Ocean,
			Rust,
			Forest
		}
	}
}
