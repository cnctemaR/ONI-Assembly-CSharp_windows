using System;
using System.Collections.Generic;
using System.Linq;
using STRINGS;

namespace TUNING
{
	public class CREATURES
	{
		public const int DEFAULT_PROBING_RADIUS = 32;

		public const float BASE_INCUBATION_RATE = 0.008333334f;

		public const float INCUBATOR_INCUBATION_MULTIPLIER = 4f;

		public const float WILD_CALORIE_BURN_RATIO = 0.25f;

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
				return delegate
				{
					string text = CREATURES.FERTILITY_MODIFIERS.DIET.NAME;
					string text2 = CREATURES.FERTILITY_MODIFIERS.DIET.DESC;
					List<Tag> foodTagsActual = foodTags.GetTagsVerySlow();
					Db.Get().CreateFertilityModifier(id, eggTag, text, text2, delegate(string descStr)
					{
						string text3 = string.Join(", ", foodTagsActual.Select<Tag, string>(new Func<Tag, string>(GameTagExtensions.ProperName)).ToArray<string>());
						descStr = string.Format(descStr, text3);
						return descStr;
					}, delegate(FertilityMonitor.Instance inst, Tag eggType)
					{
						inst.gameObject.Subscribe(-2038961714, delegate(object data)
						{
							CreatureCalorieMonitor.CaloriesConsumedEvent caloriesConsumedEvent = (CreatureCalorieMonitor.CaloriesConsumedEvent)data;
							if (foodTags.HasAny(caloriesConsumedEvent.tag))
							{
								inst.AddBreedingChance(eggType, caloriesConsumedEvent.calories * modifierPerCal);
							}
						});
					});
				};
			}

			private static global::System.Action CreateNearbyCreatureModifier(string id, Tag eggTag, Tag nearbyCreature, float modifierPerSecond, bool alsoInvert)
			{
				return delegate
				{
					string text = ((modifierPerSecond >= 0f) ? CREATURES.FERTILITY_MODIFIERS.NEARBY_CREATURE.NAME : CREATURES.FERTILITY_MODIFIERS.NEARBY_CREATURE_NEG.NAME);
					string text2 = ((modifierPerSecond >= 0f) ? CREATURES.FERTILITY_MODIFIERS.NEARBY_CREATURE.DESC : CREATURES.FERTILITY_MODIFIERS.NEARBY_CREATURE_NEG.DESC);
					Db.Get().CreateFertilityModifier(id, eggTag, text, text2, (string descStr) => string.Format(descStr, nearbyCreature.ProperName()), delegate(FertilityMonitor.Instance inst, Tag eggType)
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
							foreach (KPrefabID kprefabID in creatures)
							{
								if (kprefabID.PrefabTag == nearbyCreature)
								{
									flag = true;
									break;
								}
							}
							if (flag)
							{
								inst.AddBreedingChance(eggType, dt * modifierPerSecond);
							}
							else if (alsoInvert)
							{
								inst.AddBreedingChance(eggType, dt * -modifierPerSecond);
							}
						};
					});
				};
			}

			private static global::System.Action CreateTemperatureModifier(string id, Tag eggTag, float minTemp, float maxTemp, float modifierPerSecond, bool alsoInvert)
			{
				return delegate
				{
					string text = CREATURES.FERTILITY_MODIFIERS.TEMPERATURE.NAME;
					string text2 = string.Format(CREATURES.FERTILITY_MODIFIERS.TEMPERATURE.DESC, GameUtil.GetFormattedTemperature(minTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true), GameUtil.GetFormattedTemperature(maxTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
					Db.Get().CreateFertilityModifier(id, eggTag, text, text2, null, delegate(FertilityMonitor.Instance inst, Tag eggType)
					{
						TemperatureVulnerable component = inst.master.GetComponent<TemperatureVulnerable>();
						if (component != null)
						{
							component.OnTemperature += delegate(float dt, float newTemp)
							{
								if (newTemp > minTemp && newTemp < maxTemp)
								{
									inst.AddBreedingChance(eggType, dt * modifierPerSecond);
								}
								else if (alsoInvert)
								{
									inst.AddBreedingChance(eggType, dt * -modifierPerSecond);
								}
							};
						}
						else
						{
							Output.LogError(new object[]
							{
								"Ack! Trying to add temperature modifier",
								id,
								"to",
								inst.master.name,
								"but it's not temperature vulnerable!"
							});
						}
					});
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
