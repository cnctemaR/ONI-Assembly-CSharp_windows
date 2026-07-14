using System;
using System.Collections.Generic;
using TUNING;

public static class PufferFishTuning
{
	public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_BASE = new List<FertilityMonitor.BreedingChance>
	{
		new FertilityMonitor.BreedingChance
		{
			egg = "PufferFishEgg".ToTag(),
			weight = 1f
		}
	};

	public const float WATERWEED_PER_PUFFER = 1f;

	public const float PUFFERS_PER_DUPE = 4f;

	public const float POOPS_PER_CYCLE = 2f;

	public static float MASS = 200f;

	public static float EGG_MASS = 2f;

	public static int PEN_SIZE_PER_CREATURE = CREATURES.SPACE_REQUIREMENTS.TIER2;

	public static Tag POOP_ELEMENT = SimHashes.Oxygen.CreateTag();

	public static float OXYGEN_STORAGE_CAPACITY = 100f;

	public static float LIFESPAN = 25f;

	public static float HITPOINTS = 25f;

	public static readonly EffectorValues DECOR = global::TUNING.DECOR.BONUS.TIER0;

	public const float STANDARD_STARVE_CYCLES = 5f;

	public const float STANDARD_STOMACH_SIZE = 2000000f;

	private const float DUPE_OXYGEN_PER_CYCLE_KG = 60f;

	public const float OXYGEN_PRODUCED_PER_CYCLE = 15f;

	public const float STANDARD_CALORIES_PER_CYCLE = 400000f;

	public const float MIN_POOP_SIZE_IN_CALORIES = 200000f;

	public const float CALORIES_PER_PICKUPABLE_KG = 400000f;

	public const float PICKUPABLE_TO_PRODUCT_EFFICIENCY = 15f;

	public const float CALORIES_PER_GROWTH_EATEN = 400000f;

	public const float GROWTH_EATEN_TO_PRODUCT_EFFICIENCY = 15f;
}
