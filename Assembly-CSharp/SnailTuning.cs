using System;
using System.Collections.Generic;
using TUNING;

public class SnailTuning
{
	public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_BASE = new List<FertilityMonitor.BreedingChance>
	{
		new FertilityMonitor.BreedingChance
		{
			egg = "SnailEgg".ToTag(),
			weight = 0.99f
		},
		new FertilityMonitor.BreedingChance
		{
			egg = "SnailIronEgg".ToTag(),
			weight = 0.01f
		}
	};

	public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_IRON = new List<FertilityMonitor.BreedingChance>
	{
		new FertilityMonitor.BreedingChance
		{
			egg = "SnailEgg".ToTag(),
			weight = 0.35f
		},
		new FertilityMonitor.BreedingChance
		{
			egg = "SnailIronEgg".ToTag(),
			weight = 0.65f
		}
	};

	public static int PEN_SIZE_PER_CREATURE = CREATURES.SPACE_REQUIREMENTS.TIER3;

	public static float MASS = 75f;

	public static float STANDARD_STARVE_CYCLES = 3f;

	public static float STANDARD_CALORIES_PER_CYCLE = 100000f;

	public static float STANDARD_STOMACH_SIZE = SnailTuning.STANDARD_CALORIES_PER_CYCLE * SnailTuning.STANDARD_STARVE_CYCLES;

	public static float EGG_MASS = 0.5f;

	public static float KG_ORE_EATEN_PER_CYCLE = 100f;

	public static float CALORIES_PER_KG_OF_ORE = SnailTuning.STANDARD_CALORIES_PER_CYCLE / SnailTuning.KG_ORE_EATEN_PER_CYCLE;

	public static float MIN_POOP_SIZE_IN_KG = 5f;

	public static float MUCUS_PER_CYCLE = 15f;

	public static float MUCUS_PER_CYCLE_DRY_LAND_BONUS = 15f;

	public static float DEFAULT_DRYING_RATE = -0.16666667f;
}
