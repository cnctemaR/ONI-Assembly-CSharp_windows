using System;
using System.Collections.Generic;
using TUNING;

public static class DeerTuning
{
	public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_BASE = new List<FertilityMonitor.BreedingChance>
	{
		new FertilityMonitor.BreedingChance
		{
			egg = "WoodDeerEgg".ToTag(),
			weight = 0.98f
		},
		new FertilityMonitor.BreedingChance
		{
			egg = "GlassDeerEgg".ToTag(),
			weight = 0.02f
		}
	};

	public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_GLASS = new List<FertilityMonitor.BreedingChance>
	{
		new FertilityMonitor.BreedingChance
		{
			egg = "WoodDeerEgg".ToTag(),
			weight = 0.35f
		},
		new FertilityMonitor.BreedingChance
		{
			egg = "GlassDeerEgg".ToTag(),
			weight = 0.65f
		}
	};

	public const float STANDARD_CALORIES_PER_CYCLE = 100000f;

	public const float STANDARD_STARVE_CYCLES = 10f;

	public const float STANDARD_STOMACH_SIZE = 1000000f;

	public static int PEN_SIZE_PER_CREATURE = CREATURES.SPACE_REQUIREMENTS.TIER3;

	public static int PEN_SIZE_PER_CREATURE_HUG = CREATURES.SPACE_REQUIREMENTS.TIER1;

	public const float MORPH_DECOR_TRESHOLD = 100f;

	public static float EGG_MASS = 2f;

	public static float DROP_ANTLER_DURATION = 1200f;
}
