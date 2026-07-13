using System;
using System.Collections.Generic;
using TUNING;

public static class RaptorTuning
{
	public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_BASE = new List<FertilityMonitor.BreedingChance>
	{
		new FertilityMonitor.BreedingChance
		{
			egg = "RaptorEgg".ToTag(),
			weight = 1f
		}
	};

	public static float STANDARD_CALORIES_PER_CYCLE = 0.5f * FOOD.FOOD_TYPES.MEAT.CaloriesPerUnit;

	public const float STANDARD_STARVE_CYCLES = 10f;

	public static float STANDARD_STOMACH_SIZE = RaptorTuning.STANDARD_CALORIES_PER_CYCLE * 10f;

	public const float EGG_MASS = 8f;

	public static float CALORIES_PER_UNIT_EATEN = FOOD.FOOD_TYPES.MEAT.CaloriesPerUnit;

	public const float MIN_POOP_SIZE_IN_KG = 0.1f;

	public static float BASE_PRODUCTION_RATE = 128f;

	public static float PREY_PRODUCTION_RATE = 256f;

	public static Tag POOP_ELEMENT = SimHashes.BrineIce.CreateTag();
}
