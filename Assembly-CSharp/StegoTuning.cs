using System;
using System.Collections.Generic;
using TUNING;

public static class StegoTuning
{
	public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_BASE = new List<FertilityMonitor.BreedingChance>
	{
		new FertilityMonitor.BreedingChance
		{
			egg = "StegoEgg".ToTag(),
			weight = 1f
		}
	};

	public static float VINE_FOOD_PER_CYCLE = 4f;

	public static readonly float PEAT_PRODUCED_PER_CYCLE = 200f;

	public static readonly float STANDARD_CALORIES_PER_CYCLE = StegoTuning.VINE_FOOD_PER_CYCLE * 325000f;

	public static readonly float STANDARD_STARVE_CYCLES = 10f;

	public static readonly float STANDARD_STOMACH_SIZE = StegoTuning.STANDARD_CALORIES_PER_CYCLE * StegoTuning.STANDARD_STARVE_CYCLES;

	public static readonly float CALORIES_PER_KG_OF_ORE = StegoTuning.STANDARD_CALORIES_PER_CYCLE / StegoTuning.VINE_FOOD_PER_CYCLE;

	public static float CALORIES_PER_UNIT_EATEN = FOOD.FOOD_TYPES.VINEFRUIT.CaloriesPerUnit;

	public static float MIN_POOP_SIZE_IN_KG = StegoTuning.VINE_FOOD_PER_CYCLE;

	public static Tag POOP_ELEMENT = SimHashes.Peat.CreateTag();

	public const float EGG_MASS = 8f;

	public const float STOMP_COOLDOWN = 60f;

	public static readonly int PEN_SIZE_PER_CREATURE = CREATURES.SPACE_REQUIREMENTS.TIER4;

	public const int SEARCH_RADIUS = 10;
}
