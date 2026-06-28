using System;
using System.Collections.Generic;
using UnityEngine;

namespace TUNING
{
	public class DUPLICANTSTATS
	{
		public const float PEE_PER_FLOOR_PEE = 0.05f;

		public const float PEE_PER_TOILET_PEE = 100f;

		public const float DUPLICANT_COOLING_WATTS = 48f;

		public const float DUPLICANT_WARMING_WATTS = 48f;

		public static float FOUNDATION_MOVEMENT_BOOST = 1.5f;

		public static string[] ATTRIBUTES = new string[] { "Strength", "Construction", "Digging", "Machinery", "Athletics", "Learning", "Cooking", "Medical", "Art" };

		public static float PROBABILITY_MINISCULE = 2f;

		public static float PROBABILITY_LOW = 1.5f;

		public static float PROBABILITY_MED = 1f;

		public static int SMALL_STATPOINT_BONUS = 4;

		public static int MEDIUM_STATPOINT_BONUS = 7;

		public static int MIN_STAT_POINTS = 7;

		public static int MAX_STAT_POINTS = 10;

		public static int MAX_TRAITS = 4;

		public static List<string> CONTRACTEDTRAITS_HEALING = new List<string> { "IrritableBowel", "Aggressive", "SlowLearner", "WeakImmuneSystem", "Snorer", "CantDig" };

		public static List<DUPLICANTSTATS.TraitVal> BADTRAITS = new List<DUPLICANTSTATS.TraitVal>
		{
			new DUPLICANTSTATS.TraitVal
			{
				id = "CantResearch",
				statBonus = DUPLICANTSTATS.MEDIUM_STATPOINT_BONUS,
				probability = DUPLICANTSTATS.PROBABILITY_LOW
			},
			new DUPLICANTSTATS.TraitVal
			{
				id = "CantDig",
				statBonus = DUPLICANTSTATS.MEDIUM_STATPOINT_BONUS,
				probability = DUPLICANTSTATS.PROBABILITY_LOW
			},
			new DUPLICANTSTATS.TraitVal
			{
				id = "CantCook",
				statBonus = DUPLICANTSTATS.MEDIUM_STATPOINT_BONUS,
				probability = DUPLICANTSTATS.PROBABILITY_LOW
			},
			new DUPLICANTSTATS.TraitVal
			{
				id = "CantBuild",
				statBonus = DUPLICANTSTATS.MEDIUM_STATPOINT_BONUS,
				probability = DUPLICANTSTATS.PROBABILITY_LOW
			},
			new DUPLICANTSTATS.TraitVal
			{
				id = "Narcolepsy",
				statBonus = DUPLICANTSTATS.MEDIUM_STATPOINT_BONUS,
				probability = DUPLICANTSTATS.PROBABILITY_LOW
			},
			new DUPLICANTSTATS.TraitVal
			{
				id = "Flatulence",
				statBonus = DUPLICANTSTATS.SMALL_STATPOINT_BONUS,
				probability = DUPLICANTSTATS.PROBABILITY_MED
			},
			new DUPLICANTSTATS.TraitVal
			{
				id = "IrritableBowel",
				statBonus = DUPLICANTSTATS.SMALL_STATPOINT_BONUS,
				probability = DUPLICANTSTATS.PROBABILITY_MED
			},
			new DUPLICANTSTATS.TraitVal
			{
				id = "Snorer",
				statBonus = DUPLICANTSTATS.SMALL_STATPOINT_BONUS,
				probability = DUPLICANTSTATS.PROBABILITY_MED
			},
			new DUPLICANTSTATS.TraitVal
			{
				id = "MouthBreather",
				statBonus = DUPLICANTSTATS.SMALL_STATPOINT_BONUS,
				probability = DUPLICANTSTATS.PROBABILITY_MED
			},
			new DUPLICANTSTATS.TraitVal
			{
				id = "SmallBladder",
				statBonus = DUPLICANTSTATS.SMALL_STATPOINT_BONUS,
				probability = DUPLICANTSTATS.PROBABILITY_MED
			},
			new DUPLICANTSTATS.TraitVal
			{
				id = "CalorieBurner",
				statBonus = DUPLICANTSTATS.SMALL_STATPOINT_BONUS,
				probability = DUPLICANTSTATS.PROBABILITY_MED
			},
			new DUPLICANTSTATS.TraitVal
			{
				id = "Anemic",
				statBonus = DUPLICANTSTATS.SMALL_STATPOINT_BONUS,
				probability = DUPLICANTSTATS.PROBABILITY_MED
			},
			new DUPLICANTSTATS.TraitVal
			{
				id = "WeakImmuneSystem",
				statBonus = DUPLICANTSTATS.SMALL_STATPOINT_BONUS,
				probability = DUPLICANTSTATS.PROBABILITY_MED
			},
			new DUPLICANTSTATS.TraitVal
			{
				id = "SlowLearner",
				statBonus = DUPLICANTSTATS.SMALL_STATPOINT_BONUS,
				probability = DUPLICANTSTATS.PROBABILITY_MED
			},
			new DUPLICANTSTATS.TraitVal
			{
				id = "ScaredyCat",
				statBonus = DUPLICANTSTATS.SMALL_STATPOINT_BONUS,
				probability = DUPLICANTSTATS.PROBABILITY_MED
			}
		};

		public static List<DUPLICANTSTATS.TraitVal> STRESSTRAITS = new List<DUPLICANTSTATS.TraitVal>
		{
			new DUPLICANTSTATS.TraitVal
			{
				id = "Aggressive"
			},
			new DUPLICANTSTATS.TraitVal
			{
				id = "StressVomiter"
			}
		};

		public static List<DUPLICANTSTATS.TraitVal> GOODTRAITS = new List<DUPLICANTSTATS.TraitVal>
		{
			new DUPLICANTSTATS.TraitVal
			{
				id = "Twinkletoes",
				statBonus = -DUPLICANTSTATS.SMALL_STATPOINT_BONUS,
				probability = DUPLICANTSTATS.PROBABILITY_MED,
				mutuallyExclusiveTraits = new List<string> { "Anemic" }
			},
			new DUPLICANTSTATS.TraitVal
			{
				id = "Greasemonkey",
				statBonus = -DUPLICANTSTATS.SMALL_STATPOINT_BONUS,
				probability = DUPLICANTSTATS.PROBABILITY_MED
			},
			new DUPLICANTSTATS.TraitVal
			{
				id = "DiversLung",
				statBonus = -DUPLICANTSTATS.SMALL_STATPOINT_BONUS,
				probability = DUPLICANTSTATS.PROBABILITY_MED,
				mutuallyExclusiveTraits = new List<string> { "MouthBreather" }
			},
			new DUPLICANTSTATS.TraitVal
			{
				id = "IronGut",
				statBonus = -DUPLICANTSTATS.SMALL_STATPOINT_BONUS,
				probability = DUPLICANTSTATS.PROBABILITY_MED
			},
			new DUPLICANTSTATS.TraitVal
			{
				id = "StrongImmuneSystem",
				statBonus = -DUPLICANTSTATS.SMALL_STATPOINT_BONUS,
				probability = DUPLICANTSTATS.PROBABILITY_MED,
				mutuallyExclusiveTraits = new List<string> { "WeakImmuneSystem" }
			},
			new DUPLICANTSTATS.TraitVal
			{
				id = "EarlyBird",
				statBonus = -DUPLICANTSTATS.SMALL_STATPOINT_BONUS,
				probability = DUPLICANTSTATS.PROBABILITY_MED,
				mutuallyExclusiveTraits = new List<string> { "NightOwl" }
			},
			new DUPLICANTSTATS.TraitVal
			{
				id = "NightOwl",
				statBonus = -DUPLICANTSTATS.SMALL_STATPOINT_BONUS,
				probability = DUPLICANTSTATS.PROBABILITY_MED,
				mutuallyExclusiveTraits = new List<string> { "EarlyBird" }
			},
			new DUPLICANTSTATS.TraitVal
			{
				id = "MoleHands",
				statBonus = -DUPLICANTSTATS.SMALL_STATPOINT_BONUS,
				probability = DUPLICANTSTATS.PROBABILITY_MED,
				mutuallyExclusiveTraits = new List<string> { "CantDig" }
			},
			new DUPLICANTSTATS.TraitVal
			{
				id = "FastLearner",
				statBonus = -DUPLICANTSTATS.SMALL_STATPOINT_BONUS,
				probability = DUPLICANTSTATS.PROBABILITY_MED,
				mutuallyExclusiveTraits = new List<string> { "SlowLearner", "CantResearch" }
			},
			new DUPLICANTSTATS.TraitVal
			{
				id = "Amphibious",
				statBonus = -DUPLICANTSTATS.SMALL_STATPOINT_BONUS,
				probability = DUPLICANTSTATS.PROBABILITY_MED
			},
			new DUPLICANTSTATS.TraitVal
			{
				id = "InteriorDecorator",
				statBonus = -DUPLICANTSTATS.SMALL_STATPOINT_BONUS,
				probability = DUPLICANTSTATS.PROBABILITY_MED,
				mutuallyExclusiveTraits = new List<string> { "Uncultured" }
			},
			new DUPLICANTSTATS.TraitVal
			{
				id = "Uncultured",
				statBonus = -DUPLICANTSTATS.SMALL_STATPOINT_BONUS,
				probability = DUPLICANTSTATS.PROBABILITY_MED,
				mutuallyExclusiveTraits = new List<string> { "InteriorDecorator" }
			}
		};

		public class BASESTATS
		{
			public const float STAMINA_USED_PER_DAY = -100f;

			public const float CALORIES_BURNED_PER_DAY = -1000000f;

			public const float CALORIES_BURNED_PER_SECOND = -1666.6666f;

			public const float OXYGEN_USED_PER_SECOND = 0.1f;

			public const float BLADDER_INCREASE_PER_DAY = 100f;

			public const float DECOR_EXPECTATION = -25f;

			public const float MAX_PROFESSION_EXPECTATION = 75f;

			public const int MAX_UNDERWATER_TRAVEL_COST = 8;

			public const float TOILET_EFFICIENCY = 1f;
		}

		public class BREATH
		{
			public const float BREATH_BAR_TOTAL_SECONDS = 110f;

			public const float RETREAT_AT_SECONDS = 80f;

			public const float SUFFOCATION_WARN_AT_SECONDS = 50f;
		}

		public class COMBAT
		{
			public const float HIT_POINTS = 100f;

			public const Health.HealthState FLEE_THRESHOLD = Health.HealthState.Critical;

			public class BASICWEAPON
			{
				public const float ATTACKS_PER_SECOND = 2f;

				public const float MIN_DAMAGE_PER_HIT = 1f;

				public const float MAX_DAMAGE_PER_HIT = 1f;

				public const AttackProperties.TargetType TARGET_TYPE = AttackProperties.TargetType.Single;

				public const AttackProperties.DamageType DAMAGE_TYPE = AttackProperties.DamageType.Standard;

				public const int MAX_HITS = 1;

				public const float AREA_OF_EFFECT_RADIUS = 0f;
			}
		}

		public class DISTRIBUTIONS
		{
			public static int[] GetRandomDistribution()
			{
				return DUPLICANTSTATS.DISTRIBUTIONS.TYPES[global::UnityEngine.Random.Range(0, DUPLICANTSTATS.DISTRIBUTIONS.TYPES.Count)];
			}

			public static List<int[]> TYPES = new List<int[]>
			{
				new int[] { 7, 6, 5, 4, 3, 2, 1 },
				new int[] { 7, 4, 2, 1 },
				new int[] { 7, 2, 2, 1 },
				new int[] { 8, 1 },
				new int[] { 8, 4, 1 },
				new int[] { 4, 4, 4, 4, 1 },
				new int[] { 6 },
				new int[] { 4 },
				new int[] { 2 },
				new int[] { 1 }
			};
		}

		public struct TraitVal
		{
			public string id;

			public int statBonus;

			public float probability;

			public List<string> mutuallyExclusiveTraits;
		}
	}
}
