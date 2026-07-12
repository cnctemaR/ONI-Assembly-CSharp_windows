using System;
using System.Collections.Generic;
using ObjectCloner;
using UnityEngine;

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
			foreach (WeightedSubworldName weightedSubworldName in trait.additionalSubworldFiles)
			{
				this.world.subworldFiles.Add(weightedSubworldName);
			}
			foreach (World.AllowedCellsFilter allowedCellsFilter in trait.additionalUnknownCellFilters)
			{
				this.world.unknownCellsAllowedSubworlds.Add(allowedCellsFilter);
			}
			foreach (KeyValuePair<string, int> keyValuePair in trait.globalFeatureMods)
			{
				if (!this.world.globalFeatures.ContainsKey(keyValuePair.Key))
				{
					this.world.globalFeatures[keyValuePair.Key] = 0;
				}
				int num = Mathf.FloorToInt(this.world.worldTraitScale * (float)keyValuePair.Value);
				Dictionary<string, int> globalFeatures = this.world.globalFeatures;
				string key = keyValuePair.Key;
				globalFeatures[key] += num;
			}
			using (List<string>.Enumerator enumerator4 = trait.removeWorldTemplateRulesById.GetEnumerator())
			{
				while (enumerator4.MoveNext())
				{
					string rule = enumerator4.Current;
					this.world.worldTemplateRules.RemoveAll((World.TemplateSpawnRules x) => x.ruleId == rule);
				}
			}
			foreach (World.TemplateSpawnRules templateSpawnRules in trait.additionalWorldTemplateRules)
			{
				this.world.worldTemplateRules.Add(templateSpawnRules);
			}
			foreach (KeyValuePair<string, ElementBandConfiguration> keyValuePair2 in this.biomes.BiomeBackgroundElementBandConfigurations)
			{
				foreach (ElementGradient elementGradient in keyValuePair2.Value)
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
