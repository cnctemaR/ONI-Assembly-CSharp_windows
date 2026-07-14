using System;
using System.Collections.Generic;
using TUNING;

public static class SquidTuning
{
	public const float LIFESPAWN = 100f;

	public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_BASE = new List<FertilityMonitor.BreedingChance>
	{
		new FertilityMonitor.BreedingChance
		{
			egg = "SquidEgg".ToTag(),
			weight = 1f
		}
	};

	public static float STANDARD_STARVE_CYCLES = 5f;

	public static float STANDARD_CALORIES_PER_CYCLE = 100000f;

	public static float STANDARD_STOMACH_SIZE = SquidTuning.STANDARD_CALORIES_PER_CYCLE * SquidTuning.STANDARD_STARVE_CYCLES;

	public static readonly float WELLFED_CALORIES_PER_CYCLE = SquidTuning.STANDARD_CALORIES_PER_CYCLE * 0.9f;

	public static int PEN_SIZE_PER_CREATURE = CREATURES.SPACE_REQUIREMENTS.TIER3;

	public const float POOP_MASS_KG = 80f;

	public static Tag POOP_ELEMENT = SimHashes.Katairite.CreateTag();

	public static float EGG_MASS = 2f;

	public static float TUBE_WORM_PLANTS_PER_SQUID = 2f;

	public static float TUBE_WORM_GROWTH_EATEN_PER_CYCLE = 0.125f * SquidTuning.TUBE_WORM_PLANTS_PER_SQUID;

	public static float CALORIES_PER_GROWTH_EATEN = SquidTuning.STANDARD_CALORIES_PER_CYCLE / (SquidTuning.TUBE_WORM_GROWTH_EATEN_PER_CYCLE * 8f);

	public static float GROWTH_TO_PRODUCT_EFFICIENCY = 10f;

	public const float INK_PER_PUNCH = 5f;

	public const float INK_PUNCH_EFFECTIVENESS = 0.5f;

	public const float INK_MASS_REQUIRED_TO_ATTACK = 10f;

	public const float INK_PER_CYCLE = 50f;

	public const float CYCLES_UNTIL_MILKING = 4f;

	public static readonly float INK_CAPACITY = 200f;

	public static readonly float INK_AMOUNT_AT_MILKING = 200f;

	public static readonly float INK_PRODUCTION_PERCENTAGE_PER_SECOND = 0.041666668f;
}
