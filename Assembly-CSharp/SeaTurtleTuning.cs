using System;
using System.Collections.Generic;
using TUNING;

public static class SeaTurtleTuning
{
	public const float LIFESPAWN = 100f;

	public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_BASE = new List<FertilityMonitor.BreedingChance>
	{
		new FertilityMonitor.BreedingChance
		{
			egg = "SeaTurtleEgg".ToTag(),
			weight = 1f
		}
	};

	public const int FAIRIES_EATEN_PER_CYCLE = 2;

	public static float STANDARD_CALORIES_PER_CYCLE = 800000f;

	public static float STANDARD_STARVE_CYCLES = 5f;

	public static float STANDARD_STOMACH_SIZE = SeaTurtleTuning.STANDARD_CALORIES_PER_CYCLE * SeaTurtleTuning.STANDARD_STARVE_CYCLES;

	public static int PEN_SIZE_PER_CREATURE = CREATURES.SPACE_REQUIREMENTS.TIER3;

	public const float POOP_MASS_KG = 50f;

	public static Tag POOP_ELEMENT = SimHashes.ToxicMud.CreateTag();

	public static float POOP_KG_PER_CYCLE = 50f;

	public static float EGG_MASS = 3f;

	public static float EGG_SHELL_RATIO = 0.33333334f;

	public static float SCALE_GROWTH_TIME_IN_CYCLES = 10f;

	public static float ORE_PER_CYCLE = 25f;

	public static float SCALE_INITIAL_GROWTH_PCT = 0.5f;
}
