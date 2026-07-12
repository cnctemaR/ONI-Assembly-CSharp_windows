using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace ProcGen
{
	[DebuggerDisplay("{name}")]
	[Serializable]
	public class WorldTrait
	{
		public string name { get; private set; }

		public string description { get; private set; }

		public string colorHex { get; private set; }

		public string icon { get; private set; }

		public List<string> forbiddenDLCIds { get; private set; }

		public List<string> exclusiveWith { get; private set; }

		public List<string> exclusiveWithTags { get; private set; }

		public List<string> traitTags { get; private set; }

		public MinMax startingBasePositionHorizontalMod { get; private set; }

		public MinMax startingBasePositionVerticalMod { get; private set; }

		public List<WeightedSubworldName> additionalSubworldFiles { get; private set; }

		public List<World.AllowedCellsFilter> additionalUnknownCellFilters { get; private set; }

		public List<World.TemplateSpawnRules> additionalWorldTemplateRules { get; private set; }

		public Dictionary<string, int> globalFeatureMods { get; private set; }

		public List<string> removeWorldTemplateRulesById { get; private set; }

		public List<WorldTrait.ElementBandModifier> elementBandModifiers { get; private set; }

		public TagSet traitTagsSet
		{
			get
			{
				if (this.m_traitTagSet == null)
				{
					this.m_traitTagSet = new TagSet(this.traitTags);
				}
				return this.m_traitTagSet;
			}
		}

		public WorldTrait()
		{
			this.additionalSubworldFiles = new List<WeightedSubworldName>();
			this.additionalUnknownCellFilters = new List<World.AllowedCellsFilter>();
			this.additionalWorldTemplateRules = new List<World.TemplateSpawnRules>();
			this.removeWorldTemplateRulesById = new List<string>();
			this.globalFeatureMods = new Dictionary<string, int>();
			this.elementBandModifiers = new List<WorldTrait.ElementBandModifier>();
			this.exclusiveWith = new List<string>();
			this.exclusiveWithTags = new List<string>();
			this.forbiddenDLCIds = new List<string>();
			this.traitTags = new List<string>();
			this.name = "";
			this.description = "";
			this.icon = "";
		}

		public bool IsValid(World world, bool logErrors)
		{
			int num = 0;
			int num2 = 0;
			foreach (KeyValuePair<string, int> keyValuePair in this.globalFeatureMods)
			{
				num += keyValuePair.Value;
				num2 += Mathf.FloorToInt(world.worldTraitScale * (float)keyValuePair.Value);
			}
			if (this.globalFeatureMods.Count > 0 && num2 == 0)
			{
				if (logErrors)
				{
					DebugUtil.LogWarningArgs(new object[] { string.Concat(new string[] { "Trait '", this.filePath, "' cannot be applied to world '", world.name, "' due to globalFeatureMods and worldTraitScale resulting in no features being generated." }) });
				}
				return false;
			}
			return true;
		}

		public string filePath;

		private TagSet m_traitTagSet;

		[DebuggerDisplay("{element} massMultiplier = {massMultiplier}, bandMultiplier = {bandMultiplier}")]
		[Serializable]
		public class ElementBandModifier
		{
			public string element { get; private set; }

			public float massMultiplier { get; private set; }

			public float bandMultiplier { get; private set; }

			public ElementBandModifier()
			{
				this.massMultiplier = 1f;
				this.bandMultiplier = 1f;
			}
		}
	}
}
