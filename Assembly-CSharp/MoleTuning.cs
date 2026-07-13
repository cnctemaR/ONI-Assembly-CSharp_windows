using System;
using System.Collections.Generic;

public static class MoleTuning
{
	public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_BASE = new List<FertilityMonitor.BreedingChance>
	{
		new FertilityMonitor.BreedingChance
		{
			egg = "MoleEgg".ToTag(),
			weight = 0.98f
		},
		new FertilityMonitor.BreedingChance
		{
			egg = "MoleDelicacyEgg".ToTag(),
			weight = 0.02f
		}
	};

	public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_DELICACY = new List<FertilityMonitor.BreedingChance>
	{
		new FertilityMonitor.BreedingChance
		{
			egg = "MoleEgg".ToTag(),
			weight = 0.32f
		},
		new FertilityMonitor.BreedingChance
		{
			egg = "MoleDelicacyEgg".ToTag(),
			weight = 0.65f
		}
	};

	public static float STANDARD_CALORIES_PER_CYCLE = 4800000f;

	public static float STANDARD_STARVE_CYCLES = 10f;

	public static float STANDARD_STOMACH_SIZE = MoleTuning.STANDARD_CALORIES_PER_CYCLE * MoleTuning.STANDARD_STARVE_CYCLES;

	public static float DELICACY_STOMACH_SIZE = MoleTuning.STANDARD_STOMACH_SIZE / 2f;

	public static int PEN_SIZE_PER_CREATURE = 0;

	public static float EGG_MASS = 2f;

	public static int DEPTH_TO_HIDE = 2;

	public static HashedString[] GINGER_SYMBOL_NAMES = new HashedString[] { "del_ginger", "del_ginger1", "del_ginger2", "del_ginger3", "del_ginger4", "del_ginger5" };
}
