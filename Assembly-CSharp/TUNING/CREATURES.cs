using System;
using System.Collections.Generic;
using System.Linq;
using Klei.AI;
using STRINGS;

namespace TUNING
{
	public class CREATURES
	{
		public const int DEFAULT_PROBING_RADIUS = 32;

		public const float FERTILITY_TIME_BY_LIFESPAN = 0.6f;

		public const float INCUBATION_TIME_BY_LIFESPAN = 0.2f;

		public const float INCUBATOR_INCUBATION_MULTIPLIER = 4f;

		public const float WILD_CALORIE_BURN_RATIO = 0.25f;

		public const float VIABILITY_LOSS_RATE = -0.016666668f;

		public class HITPOINTS
		{
			public const float TIER0 = 5f;

			public const float TIER1 = 25f;

			public const float TIER2 = 50f;

			public const float TIER3 = 100f;

			public const float TIER4 = 150f;

			public const float TIER5 = 200f;

			public const float TIER6 = 400f;
		}

		public class MASS_KG
		{
			public const float TIER0 = 5f;

			public const float TIER1 = 25f;

			public const float TIER2 = 50f;

			public const float TIER3 = 100f;

			public const float TIER4 = 200f;

			public const float TIER5 = 400f;
		}

		public class TEMPERATURE
		{
			public static float FREEZING_3 = 243f;

			public static float FREEZING_2 = 253f;

			public static float FREEZING_1 = 263f;

			public static float FREEZING = 273f;

			public static float COOL = 283f;

			public static float MODERATE = 293f;

			public static float HOT = 303f;

			public static float HOT_1 = 313f;

			public static float HOT_2 = 323f;

			public static float HOT_3 = 333f;
		}

		public class LIFESPAN
		{
			public const float TIER0 = 5f;

			public const float TIER1 = 25f;

			public const float TIER2 = 75f;

			public const float TIER3 = 100f;

			public const float TIER4 = 150f;

			public const float TIER5 = 200f;

			public const float TIER6 = 400f;
		}

		public class CONVERSION_EFFICIENCY
		{
			public static float BAD_2 = 0.1f;

			public static float BAD_1 = 0.25f;

			public static float NORMAL = 0.5f;

			public static float GOOD_1 = 0.75f;

			public static float GOOD_2 = 0.95f;

			public static float GOOD_3 = 1f;
		}

		public class SPACE_REQUIREMENTS
		{
			public static int TIER2 = 8;

			public static int TIER3 = 12;

			public static int TIER4 = 16;
		}

		public class EGG_CHANCE_MODIFIERS
		{
			private static global::System.Action CreateDietaryModifier(string id, Tag eggTag, TagBits foodTags, float modifierPerCal)
			{
				FertilityModifier.FertilityModFn <>9__2;
				return delegate
				{
					string text = CREATURES.FERTILITY_MODIFIERS.DIET.NAME;
					string text2 = CREATURES.FERTILITY_MODIFIERS.DIET.DESC;
					List<Tag> foodTagsActual = foodTags.GetTagsVerySlow();
					ModifierSet modifierSet = Db.Get();
					string id2 = id;
					Tag eggTag2 = eggTag;
					string text3 = text;
					string text4 = text2;
					Func<string, string> func = delegate(string descStr)
					{
						string text5 = string.Join(", ", foodTagsActual.Select<Tag, string>((Tag t) => t.ProperName()).ToArray<string>());
						descStr = string.Format(descStr, text5);
						return descStr;
					};
					FertilityModifier.FertilityModFn fertilityModFn;
					if ((fertilityModFn = <>9__2) == null)
					{
						fertilityModFn = (<>9__2 = delegate(FertilityMonitor.Instance inst, Tag eggType)
						{
							inst.gameObject.Subscribe(-2038961714, delegate(object data)
							{
								CreatureCalorieMonitor.CaloriesConsumedEvent caloriesConsumedEvent = (CreatureCalorieMonitor.CaloriesConsumedEvent)data;
								TagBits tagBits = new TagBits(caloriesConsumedEvent.tag);
								if (foodTags.HasAny(ref tagBits))
								{
									inst.AddBreedingChance(eggType, caloriesConsumedEvent.calories * modifierPerCal);
								}
							});
						});
					}
					modifierSet.CreateFertilityModifier(id2, eggTag2, text3, text4, func, fertilityModFn);
				};
			}

			private static global::System.Action CreateDietaryModifier(string id, Tag eggTag, Tag foodTag, float modifierPerCal)
			{
				return CREATURES.EGG_CHANCE_MODIFIERS.CreateDietaryModifier(id, eggTag, new TagBits(foodTag), modifierPerCal);
			}

			private static global::System.Action CreateNearbyCreatureModifier(string id, Tag eggTag, Tag nearbyCreature, float modifierPerSecond, bool alsoInvert)
			{
				Func<string, string> <>9__1;
				FertilityModifier.FertilityModFn <>9__2;
				return delegate
				{
					string text = ((modifierPerSecond < 0f) ? CREATURES.FERTILITY_MODIFIERS.NEARBY_CREATURE_NEG.NAME : CREATURES.FERTILITY_MODIFIERS.NEARBY_CREATURE.NAME);
					string text2 = ((modifierPerSecond < 0f) ? CREATURES.FERTILITY_MODIFIERS.NEARBY_CREATURE_NEG.DESC : CREATURES.FERTILITY_MODIFIERS.NEARBY_CREATURE.DESC);
					ModifierSet modifierSet = Db.Get();
					string id2 = id;
					Tag eggTag2 = eggTag;
					string text3 = text;
					string text4 = text2;
					Func<string, string> func;
					if ((func = <>9__1) == null)
					{
						func = (<>9__1 = (string descStr) => string.Format(descStr, nearbyCreature.ProperName()));
					}
					FertilityModifier.FertilityModFn fertilityModFn;
					if ((fertilityModFn = <>9__2) == null)
					{
						fertilityModFn = (<>9__2 = delegate(FertilityMonitor.Instance inst, Tag eggType)
						{
							NearbyCreatureMonitor.Instance instance = inst.gameObject.GetSMI<NearbyCreatureMonitor.Instance>();
							if (instance == null)
							{
								instance = new NearbyCreatureMonitor.Instance(inst.master);
								instance.StartSM();
							}
							instance.OnUpdateNearbyCreatures += delegate(float dt, List<KPrefabID> creatures)
							{
								bool flag = false;
								using (List<KPrefabID>.Enumerator enumerator = creatures.GetEnumerator())
								{
									while (enumerator.MoveNext())
									{
										if (enumerator.Current.PrefabTag == nearbyCreature)
										{
											flag = true;
											break;
										}
									}
								}
								if (flag)
								{
									inst.AddBreedingChance(eggType, dt * modifierPerSecond);
									return;
								}
								if (alsoInvert)
								{
									inst.AddBreedingChance(eggType, dt * -modifierPerSecond);
								}
							};
						});
					}
					modifierSet.CreateFertilityModifier(id2, eggTag2, text3, text4, func, fertilityModFn);
				};
			}

			private static global::System.Action CreateTemperatureModifier(string id, Tag eggTag, float minTemp, float maxTemp, float modifierPerSecond, bool alsoInvert)
			{
				Func<string, string> <>9__1;
				FertilityModifier.FertilityModFn <>9__2;
				return delegate
				{
					string text = CREATURES.FERTILITY_MODIFIERS.TEMPERATURE.NAME;
					ModifierSet modifierSet = Db.Get();
					string id2 = id;
					Tag eggTag2 = eggTag;
					string text2 = text;
					string text3 = null;
					Func<string, string> func;
					if ((func = <>9__1) == null)
					{
						func = (<>9__1 = (string src) => string.Format(CREATURES.FERTILITY_MODIFIERS.TEMPERATURE.DESC, GameUtil.GetFormattedTemperature(minTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false), GameUtil.GetFormattedTemperature(maxTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)));
					}
					FertilityModifier.FertilityModFn fertilityModFn;
					if ((fertilityModFn = <>9__2) == null)
					{
						fertilityModFn = (<>9__2 = delegate(FertilityMonitor.Instance inst, Tag eggType)
						{
							TemperatureVulnerable component = inst.master.GetComponent<TemperatureVulnerable>();
							if (component != null)
							{
								component.OnTemperature += delegate(float dt, float newTemp)
								{
									if (newTemp > minTemp && newTemp < maxTemp)
									{
										inst.AddBreedingChance(eggType, dt * modifierPerSecond);
										return;
									}
									if (alsoInvert)
									{
										inst.AddBreedingChance(eggType, dt * -modifierPerSecond);
									}
								};
								return;
							}
							DebugUtil.LogErrorArgs(new object[]
							{
								"Ack! Trying to add temperature modifier",
								id,
								"to",
								inst.master.name,
								"but it's not temperature vulnerable!"
							});
						});
					}
					modifierSet.CreateFertilityModifier(id2, eggTag2, text2, text3, func, fertilityModFn);
				};
			}

			public static List<global::System.Action> MODIFIER_CREATORS = new List<global::System.Action>
			{
				CREATURES.EGG_CHANCE_MODIFIERS.CreateDietaryModifier("HatchHard", "HatchHardEgg".ToTag(), SimHashes.SedimentaryRock.CreateTag(), 0.05f / HatchTuning.STANDARD_CALORIES_PER_CYCLE),
				CREATURES.EGG_CHANCE_MODIFIERS.CreateDietaryModifier("HatchVeggie", "HatchVeggieEgg".ToTag(), SimHashes.Dirt.CreateTag(), 0.05f / HatchTuning.STANDARD_CALORIES_PER_CYCLE),
				CREATURES.EGG_CHANCE_MODIFIERS.CreateDietaryModifier("HatchMetal", "HatchMetalEgg".ToTag(), HatchMetalConfig.METAL_ORE_TAGS, 0.05f / HatchTuning.STANDARD_CALORIES_PER_CYCLE),
				CREATURES.EGG_CHANCE_MODIFIERS.CreateNearbyCreatureModifier("PuftAlphaBalance", "PuftAlphaEgg".ToTag(), "PuftAlpha".ToTag(), -0.00025f, true),
				CREATURES.EGG_CHANCE_MODIFIERS.CreateNearbyCreatureModifier("PuftAlphaNearbyOxylite", "PuftOxyliteEgg".ToTag(), "PuftAlpha".ToTag(), 8.333333E-05f, false),
				CREATURES.EGG_CHANCE_MODIFIERS.CreateNearbyCreatureModifier("PuftAlphaNearbyBleachstone", "PuftBleachstoneEgg".ToTag(), "PuftAlpha".ToTag(), 8.333333E-05f, false),
				CREATURES.EGG_CHANCE_MODIFIERS.CreateTemperatureModifier("OilFloaterHighTemp", "OilfloaterHighTempEgg".ToTag(), 373.15f, 523.15f, 8.333333E-05f, false),
				CREATURES.EGG_CHANCE_MODIFIERS.CreateTemperatureModifier("OilFloaterDecor", "OilfloaterDecorEgg".ToTag(), 293.15f, 333.15f, 8.333333E-05f, false),
				CREATURES.EGG_CHANCE_MODIFIERS.CreateDietaryModifier("LightBugOrange", "LightBugOrangeEgg".ToTag(), "GrilledPrickleFruit".ToTag(), 0.00125f),
				CREATURES.EGG_CHANCE_MODIFIERS.CreateDietaryModifier("LightBugPurple", "LightBugPurpleEgg".ToTag(), "FriedMushroom".ToTag(), 0.00125f),
				CREATURES.EGG_CHANCE_MODIFIERS.CreateDietaryModifier("LightBugPink", "LightBugPinkEgg".ToTag(), "SpiceBread".ToTag(), 0.00125f),
				CREATURES.EGG_CHANCE_MODIFIERS.CreateDietaryModifier("LightBugBlue", "LightBugBlueEgg".ToTag(), "Salsa".ToTag(), 0.00125f),
				CREATURES.EGG_CHANCE_MODIFIERS.CreateDietaryModifier("LightBugBlack", "LightBugBlackEgg".ToTag(), SimHashes.Phosphorus.CreateTag(), 0.00125f),
				CREATURES.EGG_CHANCE_MODIFIERS.CreateDietaryModifier("LightBugCrystal", "LightBugCrystalEgg".ToTag(), "CookedMeat".ToTag(), 0.00125f),
				CREATURES.EGG_CHANCE_MODIFIERS.CreateTemperatureModifier("PacuTropical", "PacuTropicalEgg".ToTag(), 308.15f, 353.15f, 8.333333E-05f, false),
				CREATURES.EGG_CHANCE_MODIFIERS.CreateTemperatureModifier("PacuCleaner", "PacuCleanerEgg".ToTag(), 243.15f, 278.15f, 8.333333E-05f, false),
				CREATURES.EGG_CHANCE_MODIFIERS.CreateDietaryModifier("DreckoPlastic", "DreckoPlasticEgg".ToTag(), "BasicSingleHarvestPlant".ToTag(), 0.025f / DreckoTuning.STANDARD_CALORIES_PER_CYCLE)
			};
		}
	}
}
