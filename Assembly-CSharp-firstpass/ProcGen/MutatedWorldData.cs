using System;
using System.Collections.Generic;
using System.Diagnostics;
using ObjectCloner;
using UnityEngine;

namespace ProcGen
{
	[DebuggerDisplay("{world.name}")]
	public class MutatedWorldData
	{
		public MutatedWorldData(World world, List<WorldTrait> worldTraits, List<WorldTrait> storyTraits)
		{
			this.world = SerializingCloner.Copy<World>(world);
			this.worldTraits = new List<WorldTrait>();
			if (worldTraits != null)
			{
				this.worldTraits.AddRange(worldTraits);
			}
			this.storyTraits = new List<WorldTrait>();
			if (storyTraits != null)
			{
				this.storyTraits.AddRange(storyTraits);
			}
			this.storyTraitCandidates = new List<WorldTrait>();
			SettingsCache.CloneInToNewWorld(this);
			this.ApplyWorldTraits();
			foreach (ElementBandConfiguration elementBandConfiguration in this.biomes.BiomeBackgroundElementBandConfigurations.Values)
			{
				elementBandConfiguration.ConvertBandSizeToMaxSize(false);
			}
		}

		public void AddWorldTemplateRules(List<World.TemplateSpawnRules> rules)
		{
			foreach (World.TemplateSpawnRules templateSpawnRules in rules)
			{
				this.world.worldTemplateRules.Add(templateSpawnRules);
			}
		}

		private void ApplyWorldTraits()
		{
			foreach (WorldTrait worldTrait in this.worldTraits)
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
			this.world.AddUnknownCellsAllowedSubworlds(trait.additionalUnknownCellFilters);
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
			using (List<string>.Enumerator enumerator3 = trait.removeWorldTemplateRulesById.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					string rule = enumerator3.Current;
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

		public List<WorldTrait> worldTraits;

		public List<WorldTrait> storyTraits;

		public Dictionary<string, SubWorld> subworlds;

		public Dictionary<string, FeatureSettings> features;

		public TerrainElementBandSettings biomes;

		public MobSettings mobs;

		public List<WorldTrait> storyTraitCandidates;
	}
}
