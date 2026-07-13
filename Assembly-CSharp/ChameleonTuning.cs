using System;
using System.Collections.Generic;
using TUNING;

public static class ChameleonTuning
{
	public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_BASE = new List<FertilityMonitor.BreedingChance>
	{
		new FertilityMonitor.BreedingChance
		{
			egg = "ChameleonEgg".ToTag(),
			weight = 0.98f
		}
	};

	public static float STANDARD_CALORIES_PER_CYCLE = 2000000f;

	public static float STANDARD_STARVE_CYCLES = 5f;

	public static float STANDARD_STOMACH_SIZE = ChameleonTuning.STANDARD_CALORIES_PER_CYCLE * ChameleonTuning.STANDARD_STARVE_CYCLES;

	public static int PEN_SIZE_PER_CREATURE = CREATURES.SPACE_REQUIREMENTS.TIER1;

	public static float EGG_MASS = 2f;

	public const float HARVEST_COOLDOWN = 150f;
}
