using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Klei;

namespace ProcGen
{
	[DebuggerDisplay("{name} - worldSize = {worldsize}")]
	[Serializable]
	public class World : IHasDlcRestrictions
	{
		public string GetProperName()
		{
			StringEntry stringEntry;
			if (!Strings.TryGet(this.name, out stringEntry))
			{
				return this.name;
			}
			return stringEntry.String;
		}

		public string GetProperDescription()
		{
			StringEntry stringEntry;
			if (!Strings.TryGet(this.description, out stringEntry))
			{
				return this.description;
			}
			return stringEntry.String;
		}

		public string name { get; private set; }

		public string description { get; private set; }

		public string[] nameTables { get; private set; }

		public string[] overrideName { get; private set; }

		public string asteroidIcon { get; private set; }

		public float iconScale { get; private set; }

		public List<string> worldTags { get; set; }

		public string dlcIdFrom { get; set; }

		public string[] requiredDlcIds { get; set; }

		public string[] forbiddenDlcIds { get; set; }

		public bool disableWorldTraits { get; private set; }

		public List<World.TraitRule> worldTraitRules { get; private set; }

		public float worldTraitScale { get; private set; }

		public World.Skip skip { get; private set; }

		public bool moduleInterior { get; private set; }

		public World.WorldCategory category { get; private set; }

		public Vector2I worldsize { get; private set; }

		public DefaultSettings defaultsOverrides { get; private set; }

		public World.LayoutMethod layoutMethod { get; private set; }

		public List<WeightedSubworldName> subworldFiles { get; private set; }

		public List<World.AllowedCellsFilter> unknownCellsAllowedSubworlds { get; private set; }

		public List<World.SubworldMixingRule> subworldMixingRules { get; private set; }

		public string startSubworldName { get; private set; }

		public string startingBaseTemplate { get; set; }

		public MinMax startingBasePositionHorizontal { get; private set; }

		public MinMax startingBasePositionVertical { get; private set; }

		public Dictionary<string, int> globalFeatures { get; private set; }

		public List<World.TemplateSpawnRules> worldTemplateRules { get; private set; }

		public List<string> seasons { get; private set; }

		public List<string> fixedTraits { get; private set; }

		public bool adjacentTemporalTear { get; private set; }

		public World()
		{
			this.subworldFiles = new List<WeightedSubworldName>();
			this.unknownCellsAllowedSubworlds = new List<World.AllowedCellsFilter>();
			this.subworldMixingRules = new List<World.SubworldMixingRule>();
			this.startingBasePositionHorizontal = new MinMax(0.5f, 0.5f);
			this.startingBasePositionVertical = new MinMax(0.5f, 0.5f);
			this.globalFeatures = new Dictionary<string, int>();
			this.seasons = new List<string>();
			this.fixedTraits = new List<string>();
			this.category = World.WorldCategory.Asteroid;
			this.worldTraitScale = 1f;
			this.iconScale = 1f;
			this.worldTags = new List<string>();
			this.worldTraitRules = new List<World.TraitRule>
			{
				new World.TraitRule(2, 4)
			};
			this.worldTemplateRules = new List<World.TemplateSpawnRules>();
		}

		public void ReplaceSeasons(List<string> seasons)
		{
			this.seasons = new List<string>(seasons);
		}

		public void ModStartLocation(MinMax hMod, MinMax vMod)
		{
			MinMax startingBasePositionHorizontal = this.startingBasePositionHorizontal;
			MinMax startingBasePositionVertical = this.startingBasePositionVertical;
			startingBasePositionHorizontal.Mod(hMod);
			startingBasePositionVertical.Mod(vMod);
			this.startingBasePositionHorizontal = startingBasePositionHorizontal;
			this.startingBasePositionVertical = startingBasePositionVertical;
		}

		public void AddUnknownCellsAllowedSubworlds(List<World.AllowedCellsFilter> additions)
		{
			this.unknownCellsAllowedSubworlds.AddRange(additions);
			this.unknownCellsAllowedSubworlds = this.unknownCellsAllowedSubworlds.StableSort<World.AllowedCellsFilter>().ToList<World.AllowedCellsFilter>();
		}

		public void AddSeasons(List<string> additions)
		{
			this.seasons.AddRange(additions);
		}

		public void Validate()
		{
			if (this.unknownCellsAllowedSubworlds != null)
			{
				List<string> usedSubworldFiles = new List<string>();
				this.subworldFiles.ForEach(delegate(WeightedSubworldName x)
				{
					usedSubworldFiles.Add(x.name);
				});
				foreach (World.AllowedCellsFilter allowedCellsFilter in this.unknownCellsAllowedSubworlds)
				{
					allowedCellsFilter.Validate(this.name, this.subworldFiles);
					if (allowedCellsFilter.subworldNames != null)
					{
						foreach (string text in allowedCellsFilter.subworldNames)
						{
							usedSubworldFiles.Remove(text);
						}
					}
				}
				usedSubworldFiles.Remove(this.startSubworldName);
				if (usedSubworldFiles.Count > 0)
				{
					DebugUtil.LogWarningArgs(new object[] { "World " + this.filePath + ": defines subworldNames that are not used in unknownCellsAllowedSubworlds: \n" + string.Join(", ", usedSubworldFiles) });
				}
			}
			if (this.subworldMixingRules != null)
			{
				foreach (World.SubworldMixingRule subworldMixingRule in this.subworldMixingRules)
				{
					bool flag = false;
					using (List<World.AllowedCellsFilter>.Enumerator enumerator = this.unknownCellsAllowedSubworlds.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current.subworldNames.Contains(subworldMixingRule.name))
							{
								flag = true;
							}
						}
					}
					if (!flag)
					{
						DebugUtil.LogErrorArgs(new object[] { string.Concat(new string[] { "World ", this.filePath, ": defines a subworldMixingRule '", subworldMixingRule.name, "' but it's not referenced in unknownCellsAllowedSubworlds" }) });
					}
				}
			}
			if (this.worldTraitRules != null)
			{
				foreach (World.TraitRule traitRule in this.worldTraitRules)
				{
					traitRule.Validate();
				}
			}
		}

		public bool IsValidTrait(WorldTrait trait)
		{
			if (this.disableWorldTraits)
			{
				return false;
			}
			foreach (World.TraitRule traitRule in this.worldTraitRules)
			{
				if (traitRule.specificTraits == null)
				{
					TagSet tagSet = ((traitRule.requiredTags != null) ? new TagSet(traitRule.requiredTags) : null);
					TagSet tagSet2 = ((traitRule.forbiddenTags != null) ? new TagSet(traitRule.forbiddenTags) : null);
					if ((tagSet == null || trait.traitTagsSet.ContainsAll(tagSet)) && (tagSet2 == null || !trait.traitTagsSet.ContainsOne(tagSet2)) && (traitRule.forbiddenTraits == null || !traitRule.forbiddenTraits.Contains(trait.filePath)) && trait.IsValid(this, false))
					{
						return true;
					}
				}
			}
			return false;
		}

		public string[] GetRequiredDlcIds()
		{
			return this.requiredDlcIds;
		}

		public string[] GetForbiddenDlcIds()
		{
			return this.forbiddenDlcIds;
		}

		public string filePath;

		[NonSerialized]
		public bool isModded;

		[NonSerialized]
		public List<string> generatedSubworlds;

		public enum WorldCategory
		{
			Asteroid,
			Moon
		}

		public enum Skip
		{
			Never,
			False = 0,
			Always = 99,
			True = 99,
			EditorOnly
		}

		public enum LayoutMethod
		{
			Default,
			VoronoiTree = 0,
			PowerTree
		}

		[Serializable]
		public class TraitRule
		{
			public int min { get; private set; }

			public int max { get; private set; }

			public List<string> requiredTags { get; private set; }

			public List<string> specificTraits { get; private set; }

			public List<string> forbiddenTags { get; private set; }

			public List<string> forbiddenTraits { get; private set; }

			public TraitRule()
			{
			}

			public TraitRule(int min, int max)
			{
				this.min = min;
				this.max = max;
			}

			public void Validate()
			{
				if (this.specificTraits != null)
				{
					DebugUtil.DevAssert(this.requiredTags == null, "TraitRule using specificTraits does not support requiredTags", null);
					DebugUtil.DevAssert(this.forbiddenTags == null, "TraitRule using specificTraits does not support forbiddenTags", null);
					DebugUtil.DevAssert(this.forbiddenTraits == null, "TraitRule using specificTraits does not support forbiddenTraits", null);
				}
			}
		}

		[DebuggerDisplay("{listRule}, someCount = {someCount}, moreCount = {moreCount}, priority = {priority}")]
		[Serializable]
		public class TemplateSpawnRules
		{
			public TemplateSpawnRules()
			{
				this.times = 1;
				this.allowedCellsFilter = new List<World.AllowedCellsFilter>();
				this.allowDuplicates = false;
				this.useRelaxedFiltering = false;
				this.overrideOffset = Vector2I.zero;
				this.overridePlacement = Vector2I.minusone;
			}

			public string ruleId { get; private set; }

			public List<string> names { get; private set; }

			public World.TemplateSpawnRules.ListRule listRule { get; private set; }

			public int someCount { get; private set; }

			public int moreCount { get; private set; }

			public Vector2I range { get; private set; }

			public int times { get; private set; }

			public float priority { get; private set; }

			public bool allowDuplicates { get; private set; }

			public bool allowExtremeTemperatureOverlap { get; private set; }

			public bool allowNearStart { get; private set; }

			public bool useRelaxedFiltering { get; private set; }

			public Vector2I overrideOffset { get; set; }

			public Vector2I overridePlacement { get; set; }

			public List<World.AllowedCellsFilter> allowedCellsFilter { get; private set; }

			public bool IsGuaranteeRule()
			{
				switch (this.listRule)
				{
				case World.TemplateSpawnRules.ListRule.GuaranteeOne:
					return true;
				case World.TemplateSpawnRules.ListRule.GuaranteeSome:
					return true;
				case World.TemplateSpawnRules.ListRule.GuaranteeSomeTryMore:
					return true;
				case World.TemplateSpawnRules.ListRule.GuaranteeAll:
					return true;
				case World.TemplateSpawnRules.ListRule.GuaranteeRange:
					return true;
				default:
					return false;
				}
			}

			public enum ListRule
			{
				GuaranteeOne,
				GuaranteeSome,
				GuaranteeSomeTryMore,
				GuaranteeAll,
				GuaranteeRange,
				TryOne,
				TrySome,
				TryRange,
				TryAll
			}
		}

		[DebuggerDisplay("command = {command}, tag = {tag}, minDistance = {minDistance}, maxDistance = {maxDistance}")]
		[Serializable]
		public class AllowedCellsFilter : IComparable<World.AllowedCellsFilter>
		{
			public AllowedCellsFilter()
			{
				this.temperatureRanges = new List<Temperature.Range>();
				this.zoneTypes = new List<SubWorld.ZoneType>();
				this.subworldNames = new List<string>();
				this.command = World.AllowedCellsFilter.Command.Replace;
				this.ignoreIfMissingTag = false;
				this.sortOrder = 0;
			}

			public World.AllowedCellsFilter.TagCommand tagcommand { get; private set; }

			public string tag { get; private set; }

			public int minDistance { get; private set; }

			public int maxDistance { get; private set; }

			public World.AllowedCellsFilter.Command command { get; private set; }

			public List<Temperature.Range> temperatureRanges { get; private set; }

			public List<SubWorld.ZoneType> zoneTypes { get; private set; }

			public List<string> subworldNames { get; private set; }

			public int sortOrder { get; private set; }

			public bool ignoreIfMissingTag { get; set; }

			public void Validate(string parentFile, List<WeightedSubworldName> parentCachedFiles)
			{
				if (this.subworldNames != null)
				{
					using (List<string>.Enumerator enumerator = this.subworldNames.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							string subworld = enumerator.Current;
							if (!subworld.StartsWith("("))
							{
								DebugUtil.DevAssert(parentCachedFiles.Any<WeightedSubworldName>((WeightedSubworldName val) => val.name == subworld), string.Concat(new string[] { "World ", parentFile, ": should include ", subworld, " in its subworldFiles since it's used in a command" }), null);
								DebugUtil.DevAssert(FileSystem.FileExists(SettingsCache.RewriteWorldgenPathYaml(subworld)), "World " + parentFile + ": Incorrect subworldFile " + subworld, null);
							}
						}
					}
				}
			}

			public int CompareTo(World.AllowedCellsFilter other)
			{
				return this.sortOrder.CompareTo(other.sortOrder);
			}

			public enum TagCommand
			{
				Default,
				AtTag,
				NotAtTag,
				DistanceFromTag
			}

			public enum Command
			{
				Clear,
				Replace,
				UnionWith,
				IntersectWith,
				ExceptWith,
				SymmetricExceptWith,
				All
			}
		}

		[DebuggerDisplay("name = {name}, minCount = {minCount}, maxCount = {maxCount}")]
		[Serializable]
		public class SubworldMixingRule
		{
			public string name { get; private set; }

			public int minCount { get; private set; }

			public int maxCount { get; private set; }

			public List<string> forbiddenTags { get; private set; }

			public List<string> requiredTags { get; private set; }

			public SubworldMixingRule()
			{
				this.forbiddenTags = new List<string>();
				this.requiredTags = new List<string>();
				this.maxCount = int.MaxValue;
				this.minCount = 0;
			}
		}
	}
}
