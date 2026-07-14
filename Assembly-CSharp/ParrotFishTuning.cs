using System;
using System.Collections.Generic;
using TUNING;

public static class ParrotFishTuning
{
	public static float MASS = 200f;

	public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_BASE = new List<FertilityMonitor.BreedingChance>
	{
		new FertilityMonitor.BreedingChance
		{
			egg = "ParrotFishEgg".ToTag(),
			weight = 1f
		}
	};

	public static float STANDARD_CALORIES_PER_CYCLE = 100000f;

	public static float STANDARD_STARVE_CYCLES = 5f;

	public static float STANDARD_STOMACH_SIZE = ParrotFishTuning.STANDARD_CALORIES_PER_CYCLE * ParrotFishTuning.STANDARD_STARVE_CYCLES;

	public static int PEN_SIZE_PER_CREATURE = CREATURES.SPACE_REQUIREMENTS.TIER2;

	public static float EGG_MASS = 0.75f;

	public static float EGG_SHELL_RATIO = 0f;

	public const string SURFACE_AIR_CONSUMED_EFFECT_ID = "SurfaceAirConsumed";

	public const float SURFACE_AIR_CONSUME_RATE = 3f;

	public const float SURFACE_AIR_CONSUME_DURATION = 6f;

	public const float SURFACE_AIR_MIN_MASS = 2f;

	public const float SURFACE_AIR_COOLDOWN = 600f;

	public const float SURFACE_AIR_HAPPINESS_BONUS = 2f;
}
