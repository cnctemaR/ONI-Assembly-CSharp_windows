using System;
using System.Collections.Generic;
using ObjectCloner;

namespace ProcGen
{
	public class MutatedWorldData
	{
		public MutatedWorldData(World world, List<WorldTrait> traits)
		{
			this.world = SerializingCloner.Copy<World>(world);
			if (traits == null)
			{
				this.traits = new List<WorldTrait>();
			}
			else
			{
				this.traits = new List<WorldTrait>(traits);
			}
			SettingsCache.CloneInToNewWorld(this);
			this.ApplyTraits();
			foreach (ElementBandConfiguration elementBandConfiguration in this.biomes.BiomeBackgroundElementBandConfigurations.Values)
			{
				elementBandConfiguration.ConvertBandSizeToMaxSize();
			}
		}

		private void ApplyTraits()
		{
			foreach (WorldTrait worldTrait in this.traits)
			{
				this.ApplyTrait(worldTrait);
			}
		}

		private void ApplyTrait(WorldTrait trait)
		{
			this.world.ModStartLocation(trait.startingBasePositionHorizontalMod, trait.startingBasePositionVerticalMod);
			foreach (WeightedName weightedName in trait.additionalSubworldFiles)
			{
				this.world.subworldFiles.Add(weightedName);
			}
			foreach (World.AllowedCellsFilter allowedCellsFilter in trait.additionalUnknownCellFilters)
			{
				this.world.unknownCellsAllowedSubworlds.Add(allowedCellsFilter);
			}
			foreach (KeyValuePair<string, int> keyValuePair in trait.globalFeatureTemplateMods)
			{
				if (!this.world.globalFeatureTemplates.ContainsKey(keyValuePair.Key))
				{
					this.world.globalFeatureTemplates[keyValuePair.Key] = 0;
				}
				Dictionary<string, int> dictionary;
				string key;
				(dictionary = this.world.globalFeatureTemplates)[key = keyValuePair.Key] = dictionary[key] + keyValuePair.Value;
			}
			foreach (KeyValuePair<string, int> keyValuePair2 in trait.globalFeatureMods)
			{
				if (!this.world.globalFeatures.ContainsKey(keyValuePair2.Key))
				{
					this.world.globalFeatures[keyValuePair2.Key] = 0;
				}
				Dictionary<string, int> dictionary;
				string key2;
				(dictionary = this.world.globalFeatures)[key2 = keyValuePair2.Key] = dictionary[key2] + keyValuePair2.Value;
			}
			foreach (KeyValuePair<string, ElementBandConfiguration> keyValuePair3 in this.biomes.BiomeBackgroundElementBandConfigurations)
			{
				foreach (ElementGradient elementGradient in keyValuePair3.Value)
				{
					foreach (WorldTrait.ElementBandModifier elementBandModifier in trait.elementBandModifiers)
					{
						if (elementBandModifier.element == elementGradient.content)
						{
							elementGradient.Mod(elementBandModifier);
						}
					}
				}
			}
		}

		public World world;

		public List<WorldTrait> traits;

		public Dictionary<string, SubWorld> subworlds;

		public Dictionary<string, FeatureSettings> features;

		public TerrainElementBandSettings biomes;

		public MobSettings mobs;
	}
}
