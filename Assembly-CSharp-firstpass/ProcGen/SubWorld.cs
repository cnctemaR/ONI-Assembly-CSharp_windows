using System;
using System.Collections.Generic;
using KSerialization.Converters;

namespace ProcGen
{
	[Serializable]
	public class SubWorld : SampleDescriber
	{
		public string nameKey { get; protected set; }

		public string descriptionKey { get; protected set; }

		public string utilityKey { get; protected set; }

		public string biomeNoise { get; protected set; }

		public string overrideNoise { get; protected set; }

		public string densityNoise { get; protected set; }

		public string borderOverride { get; protected set; }

		public MinMax borderSizeOverride { get; protected set; }

		[StringEnumConverter]
		public Temperature.Range temperatureRange { get; protected set; }

		public Feature centralFeature { get; protected set; }

		public List<Feature> features { get; protected set; }

		public SampleDescriber.Override overrides { get; protected set; }

		public List<string> tags { get; protected set; }

		public int minChildCount { get; protected set; }

		public int extraBiomeChildren { get; protected set; }

		public List<WeightedBiome> biomes { get; protected set; }

		public Dictionary<string, int> featureTemplates { get; protected set; }

		public List<World.TemplateSpawnRules> subworldTemplateRules { get; protected set; }

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
			this.featureTemplates = new Dictionary<string, int>();
			this.pdWeight = 1f;
			this.borderSizeOverride = new MinMax(1f, 2.5f);
		}

		public void EnforceTemplateSpawnRuleSelfConsistency()
		{
			if (this.subworldTemplateRules == null)
			{
				return;
			}
			foreach (World.TemplateSpawnRules templateSpawnRules in this.subworldTemplateRules)
			{
				bool flag = true;
				foreach (World.AllowedCellsFilter allowedCellsFilter in templateSpawnRules.allowedCellsFilter)
				{
					DebugUtil.DevAssert(allowedCellsFilter.command != World.AllowedCellsFilter.Command.Replace, "subworldTemplateRules in " + base.name + " contains an AllowedCellsFilter with Command.Replace, which replaces the implicit subworld filter.", null);
					DebugUtil.Assert(allowedCellsFilter.zoneTypes == null || allowedCellsFilter.zoneTypes.Count == 0, "subworldTemplateRules in " + base.name + " contains zoneTypes, which is unsupported since there is an implicit subworld filter. Use worldTemplateRules instead.");
					DebugUtil.Assert(allowedCellsFilter.command != World.AllowedCellsFilter.Command.All || flag, "subworldTemplateRules in " + base.name + " contains an All command that's not the first filter in the list.");
					flag = false;
				}
				DebugUtil.Assert(!templateSpawnRules.IsGuaranteeRule(), "subworldTemplateRules in " + base.name + " contains a guaranteed rule, which is not allowed. Include such rules in worldTemplateRules.");
				World.AllowedCellsFilter allowedCellsFilter2 = new World.AllowedCellsFilter();
				allowedCellsFilter2.subworldNames.Add(base.name);
				templateSpawnRules.allowedCellsFilter.Insert(0, allowedCellsFilter2);
			}
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
			Forest,
			Radioactive,
			Swamp,
			Wasteland,
			RocketInterior,
			Metallic,
			Barren,
			Moo
		}
	}
}
