using System;
using System.Collections.Generic;
using TUNING;

public static class SeaHorseTuning
{
	public static float MASS = 200f;

	public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_BASE = new List<FertilityMonitor.BreedingChance>
	{
		new FertilityMonitor.BreedingChance
		{
			egg = "SeaHorseEgg".ToTag(),
			weight = 1f
		}
	};

	public static float STANDARD_CALORIES_PER_CYCLE = 100000f;

	public static float STANDARD_STARVE_CYCLES = 5f;

	public static float STANDARD_STOMACH_SIZE = SeaHorseTuning.STANDARD_CALORIES_PER_CYCLE * SeaHorseTuning.STANDARD_STARVE_CYCLES;

	public static int PEN_SIZE_PER_CREATURE = CREATURES.SPACE_REQUIREMENTS.TIER2;

	public static float EGG_MASS = 2f;
}
