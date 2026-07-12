using System;
using System.Collections.Generic;
using System.Linq;
using Klei;
using Klei.CustomSettings;
using ObjectCloner;
using ProcGen;
using STRINGS;

namespace ProcGenGame
{
	public class WorldgenMixing
	{
		public static void RefreshWorldMixing(MutatedClusterLayout mutatedLayout, int seed, bool isRunningWorldgenDebug, bool muteErrors)
		{
			foreach (WorldPlacement worldPlacement in mutatedLayout.layout.worldPlacements)
			{
				worldPlacement.UndoWorldMixing();
			}
			WorldgenMixing.DoWorldMixingInternal(mutatedLayout, seed, isRunningWorldgenDebug, muteErrors);
		}

		public static MutatedClusterLayout DoWorldMixing(ClusterLayout layout, int seed, bool isRunningWorldgenDebug, bool muteErrors)
		{
			return WorldgenMixing.DoWorldMixingInternal(new MutatedClusterLayout(layout), seed, isRunningWorldgenDebug, muteErrors);
		}

		private static MutatedClusterLayout DoWorldMixingInternal(MutatedClusterLayout mutatedClusterLayout, int seed, bool isRunningWorldgenDebug, bool muteErrors)
		{
			List<WorldgenMixing.WorldMixingOption> list = new List<WorldgenMixing.WorldMixingOption>();
			if (CustomGameSettings.Instance != null && !GenericGameSettings.instance.devAutoWorldGen)
			{
				using (List<WorldMixingSettingConfig>.Enumerator enumerator = CustomGameSettings.Instance.GetActiveWorldMixingSettings().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						WorldMixingSettingConfig worldMixingSettingConfig = enumerator.Current;
						WorldMixingSettings worldMixingSettings = SettingsCache.TryGetCachedWorldMixingSetting(worldMixingSettingConfig.worldgenPath);
						if (!mutatedClusterLayout.layout.HasAnyTags(worldMixingSettings.forbiddenClusterTags))
						{
							int num = ((CustomGameSettings.Instance.GetCurrentMixingSettingLevel(worldMixingSettingConfig.id).id == "GuranteeMixing") ? 1 : 0);
							global::ProcGen.World worldData = SettingsCache.worlds.GetWorldData(worldMixingSettings.world);
							list.Add(new WorldgenMixing.WorldMixingOption
							{
								worldgenPath = worldMixingSettings.world,
								mixingSettings = worldMixingSettings,
								minCount = num,
								maxCount = 1,
								cachedWorld = worldData
							});
						}
					}
					goto IL_0167;
				}
			}
			string[] devWorldMixing = GenericGameSettings.instance.devWorldMixing;
			for (int i = 0; i < devWorldMixing.Length; i++)
			{
				WorldMixingSettings worldMixingSettings2 = SettingsCache.TryGetCachedWorldMixingSetting(devWorldMixing[i]);
				global::ProcGen.World worldData2 = SettingsCache.worlds.GetWorldData(worldMixingSettings2.world);
				list.Add(new WorldgenMixing.WorldMixingOption
				{
					worldgenPath = worldMixingSettings2.world,
					mixingSettings = worldMixingSettings2,
					minCount = 1,
					maxCount = 1,
					cachedWorld = worldData2
				});
			}
			IL_0167:
			KRandom krandom = new KRandom(seed);
			foreach (WorldPlacement worldPlacement in mutatedClusterLayout.layout.worldPlacements)
			{
				worldPlacement.UndoWorldMixing();
			}
			List<WorldPlacement> list2 = new List<WorldPlacement>(mutatedClusterLayout.layout.worldPlacements);
			list2.ShuffleSeeded<WorldPlacement>(krandom);
			foreach (WorldPlacement worldPlacement2 in list2)
			{
				if (worldPlacement2.IsMixingPlacement())
				{
					list.ShuffleSeeded<WorldgenMixing.WorldMixingOption>(krandom);
					WorldgenMixing.WorldMixingOption worldMixingOption = WorldgenMixing.FindWorldMixingOption(worldPlacement2, list);
					if (worldMixingOption != null)
					{
						Debug.Log("Mixing: Applied world substitution " + worldPlacement2.world + " -> " + worldMixingOption.worldgenPath);
						worldPlacement2.worldMixing.previousWorld = worldPlacement2.world;
						worldPlacement2.worldMixing.mixingWasApplied = true;
						worldPlacement2.world = worldMixingOption.worldgenPath;
						worldMixingOption.Consume();
						if (worldMixingOption.IsExhausted)
						{
							list.Remove(worldMixingOption);
						}
					}
				}
			}
			if (!WorldgenMixing.ValidateWorldMixingOptions(list, isRunningWorldgenDebug, muteErrors))
			{
				return null;
			}
			return mutatedClusterLayout;
		}

		private static WorldgenMixing.WorldMixingOption FindWorldMixingOption(WorldPlacement worldPlacement, List<WorldgenMixing.WorldMixingOption> options)
		{
			options = options.StableSort<WorldgenMixing.WorldMixingOption>().ToList<WorldgenMixing.WorldMixingOption>();
			foreach (WorldgenMixing.WorldMixingOption worldMixingOption in options)
			{
				if (!worldMixingOption.IsExhausted)
				{
					bool flag = true;
					foreach (string text in worldPlacement.worldMixing.requiredTags)
					{
						if (!worldMixingOption.cachedWorld.worldTags.Contains(text))
						{
							flag = false;
							break;
						}
					}
					foreach (string text2 in worldPlacement.worldMixing.forbiddenTags)
					{
						if (worldMixingOption.cachedWorld.worldTags.Contains(text2))
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						return worldMixingOption;
					}
				}
			}
			return null;
		}

		private static bool ValidateWorldMixingOptions(List<WorldgenMixing.WorldMixingOption> options, bool isRunningWorldgenDebug, bool muteErrors)
		{
			List<string> list = new List<string>();
			foreach (WorldgenMixing.WorldMixingOption worldMixingOption in options)
			{
				if (!worldMixingOption.IsSatisfied)
				{
					list.Add(string.Format("{0} ({1})", worldMixingOption.worldgenPath, worldMixingOption.minCount));
				}
			}
			if (list.Count <= 0)
			{
				return true;
			}
			if (muteErrors)
			{
				return false;
			}
			string text = "WorldgenMixing: Could not guarantee these world mixings: " + string.Join("\n - ", list);
			if (!isRunningWorldgenDebug)
			{
				DebugUtil.LogWarningArgs(new object[] { text });
				throw new WorldgenException(text, UI.FRONTEND.SUPPORTWARNINGS.WORLD_GEN_FAILURE_MIXING);
			}
			DebugUtil.LogErrorArgs(new object[] { text });
			return false;
		}

		public static void DoSubworldMixing(Cluster cluster, int seed, Func<int, WorldGen, bool> ShouldSkipWorldCallback, bool isRunningWorldgenDebug)
		{
			List<WorldgenMixing.MixingOption<SubworldMixingSettings>> list = new List<WorldgenMixing.MixingOption<SubworldMixingSettings>>();
			if (CustomGameSettings.Instance != null && !GenericGameSettings.instance.devAutoWorldGen)
			{
				using (List<SubworldMixingSettingConfig>.Enumerator enumerator = CustomGameSettings.Instance.GetActiveSubworldMixingSettings().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						SubworldMixingSettingConfig subworldMixingSettingConfig = enumerator.Current;
						SubworldMixingSettings subworldMixingSettings = SettingsCache.TryGetCachedSubworldMixingSetting(subworldMixingSettingConfig.worldgenPath);
						if (!cluster.clusterLayout.HasAnyTags(subworldMixingSettingConfig.forbiddenClusterTags))
						{
							int num = ((CustomGameSettings.Instance.GetCurrentMixingSettingLevel(subworldMixingSettingConfig.id).id == "GuranteeMixing") ? 1 : 0);
							list.Add(new WorldgenMixing.MixingOption<SubworldMixingSettings>
							{
								worldgenPath = subworldMixingSettingConfig.worldgenPath,
								mixingSettings = subworldMixingSettings,
								minCount = num,
								maxCount = 3
							});
						}
					}
					goto IL_0130;
				}
			}
			foreach (string text in GenericGameSettings.instance.devSubworldMixing)
			{
				SubworldMixingSettings subworldMixingSettings2 = SettingsCache.TryGetCachedSubworldMixingSetting(text);
				list.Add(new WorldgenMixing.MixingOption<SubworldMixingSettings>
				{
					worldgenPath = text,
					mixingSettings = subworldMixingSettings2,
					minCount = 1,
					maxCount = 3
				});
			}
			IL_0130:
			KRandom krandom = new KRandom(seed);
			List<WorldGen> list2 = new List<WorldGen>(cluster.worlds);
			list2.ShuffleSeeded<WorldGen>(krandom);
			list2.Sort((WorldGen a, WorldGen b) => WorldPlacement.GetSortOrder(a.Settings.worldType).CompareTo(WorldPlacement.GetSortOrder(b.Settings.worldType)));
			for (int j = 0; j < cluster.worlds.Count; j++)
			{
				WorldGen worldGen = list2[j];
				list.ShuffleSeeded<WorldgenMixing.MixingOption<SubworldMixingSettings>>(krandom);
				WorldgenMixing.ApplySubworldMixingToWorld(worldGen.Settings.world, list);
			}
			WorldgenMixing.ValidateSubworldMixingOptions(list, isRunningWorldgenDebug);
		}

		private static void ValidateSubworldMixingOptions(List<WorldgenMixing.MixingOption<SubworldMixingSettings>> options, bool isRunningWorldgenDebug)
		{
			List<string> list = new List<string>();
			foreach (WorldgenMixing.MixingOption<SubworldMixingSettings> mixingOption in options)
			{
				if (!mixingOption.IsSatisfied)
				{
					list.Add(string.Format("{0} ({1})", mixingOption.worldgenPath, mixingOption.minCount));
				}
			}
			if (list.Count > 0)
			{
				string text = "WorldgenMixing: Could not guarantee these subworld mixings: " + string.Join("\n - ", list);
				if (!isRunningWorldgenDebug)
				{
					DebugUtil.LogWarningArgs(new object[] { text });
					throw new WorldgenException(text, UI.FRONTEND.SUPPORTWARNINGS.WORLD_GEN_FAILURE_MIXING);
				}
				DebugUtil.LogErrorArgs(new object[] { text });
			}
		}

		private static void ApplySubworldMixingToWorld(global::ProcGen.World world, List<WorldgenMixing.MixingOption<SubworldMixingSettings>> availableSubworldsForMixing)
		{
			List<global::ProcGen.World.SubworldMixingRule> list = new List<global::ProcGen.World.SubworldMixingRule>();
			foreach (global::ProcGen.World.SubworldMixingRule subworldMixingRule in world.subworldMixingRules)
			{
				if (availableSubworldsForMixing.Count == 0)
				{
					WorldgenMixing.CleanupUnusedMixing(world);
					return;
				}
				WorldgenMixing.MixingOption<SubworldMixingSettings> mixingOption = WorldgenMixing.FindSubworldMixing(subworldMixingRule, availableSubworldsForMixing);
				if (mixingOption == null)
				{
					string[] array = new string[6];
					array[0] = "WorldgenMixing: No valid mixing for '";
					array[1] = subworldMixingRule.name;
					array[2] = "' on World '";
					array[3] = world.name;
					array[4] = "' from options: ";
					array[5] = string.Join(", ", from x in availableSubworldsForMixing
						where !x.IsExhausted
						select x.mixingSettings.name);
					Debug.Log(string.Concat(array));
				}
				else
				{
					WeightedSubworldName weightedSubworldName = SerializingCloner.Copy<WeightedSubworldName>(mixingOption.mixingSettings.subworld);
					weightedSubworldName.minCount = Math.Max(subworldMixingRule.minCount, weightedSubworldName.minCount);
					weightedSubworldName.maxCount = Math.Min(subworldMixingRule.maxCount, weightedSubworldName.maxCount);
					world.subworldFiles.Add(weightedSubworldName);
					foreach (global::ProcGen.World.AllowedCellsFilter allowedCellsFilter in world.unknownCellsAllowedSubworlds)
					{
						for (int i = 0; i < allowedCellsFilter.subworldNames.Count; i++)
						{
							if (allowedCellsFilter.subworldNames[i] == subworldMixingRule.name)
							{
								allowedCellsFilter.subworldNames[i] = weightedSubworldName.name;
							}
						}
					}
					if (!list.Contains(subworldMixingRule))
					{
						world.worldTemplateRules.AddRange(mixingOption.mixingSettings.additionalWorldTemplateRules);
						list.Add(subworldMixingRule);
					}
					mixingOption.Consume();
					if (mixingOption.IsExhausted)
					{
						availableSubworldsForMixing.Remove(mixingOption);
					}
				}
			}
			WorldgenMixing.CleanupUnusedMixing(world);
		}

		private static WorldgenMixing.MixingOption<SubworldMixingSettings> FindSubworldMixing(global::ProcGen.World.SubworldMixingRule rule, List<WorldgenMixing.MixingOption<SubworldMixingSettings>> options)
		{
			options = options.StableSort<WorldgenMixing.MixingOption<SubworldMixingSettings>>().ToList<WorldgenMixing.MixingOption<SubworldMixingSettings>>();
			foreach (WorldgenMixing.MixingOption<SubworldMixingSettings> mixingOption in options)
			{
				if (!mixingOption.IsExhausted)
				{
					bool flag = true;
					foreach (string text in rule.forbiddenTags)
					{
						if (mixingOption.mixingSettings.mixingTags.Contains(text))
						{
							flag = false;
						}
					}
					foreach (string text2 in rule.requiredTags)
					{
						if (!mixingOption.mixingSettings.mixingTags.Contains(text2))
						{
							flag = false;
						}
					}
					int num = Math.Max(rule.minCount, mixingOption.mixingSettings.subworld.minCount);
					int num2 = Math.Min(rule.maxCount, mixingOption.mixingSettings.subworld.maxCount);
					if (num > num2)
					{
						flag = false;
					}
					if (flag)
					{
						return mixingOption;
					}
				}
			}
			return null;
		}

		private static void CleanupUnusedMixing(global::ProcGen.World world)
		{
			foreach (global::ProcGen.World.AllowedCellsFilter allowedCellsFilter in world.unknownCellsAllowedSubworlds)
			{
				allowedCellsFilter.subworldNames.RemoveAll(new Predicate<string>(WorldgenMixing.IsMixingProxyName));
			}
		}

		private static bool IsMixingProxyName(string name)
		{
			return name.StartsWith("(");
		}

		private const int NUM_WORLD_TO_TRY_SUBWORLDMIXING = 3;

		public class MixingOption<T> : IComparable<WorldgenMixing.MixingOption<T>>
		{
			public bool IsExhausted
			{
				get
				{
					return this.maxCount <= 0;
				}
			}

			public bool IsSatisfied
			{
				get
				{
					return this.minCount <= 0;
				}
			}

			public void Consume()
			{
				this.minCount--;
				this.maxCount--;
			}

			public int CompareTo(WorldgenMixing.MixingOption<T> other)
			{
				int num = other.minCount.CompareTo(this.minCount);
				if (num != 0)
				{
					return num;
				}
				return other.maxCount.CompareTo(this.maxCount);
			}

			public string worldgenPath;

			public T mixingSettings;

			public int minCount;

			public int maxCount;
		}

		public class WorldMixingOption : WorldgenMixing.MixingOption<WorldMixingSettings>
		{
			public global::ProcGen.World cachedWorld;
		}
	}
}
