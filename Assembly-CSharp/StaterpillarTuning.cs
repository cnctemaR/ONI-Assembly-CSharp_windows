using System;
using System.Collections.Generic;

public static class StaterpillarTuning
{
	public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_BASE = new List<FertilityMonitor.BreedingChance>
	{
		new FertilityMonitor.BreedingChance
		{
			egg = "StaterpillarEgg".ToTag(),
			weight = 0.98f
		},
		new FertilityMonitor.BreedingChance
		{
			egg = "StaterpillarGasEgg".ToTag(),
			weight = 0.02f
		},
		new FertilityMonitor.BreedingChance
		{
			egg = "StaterpillarLiquidEgg".ToTag(),
			weight = 0.02f
		}
	};

	public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_GAS = new List<FertilityMonitor.BreedingChance>
	{
		new FertilityMonitor.BreedingChance
		{
			egg = "StaterpillarEgg".ToTag(),
			weight = 0.32f
		},
		new FertilityMonitor.BreedingChance
		{
			egg = "StaterpillarGasEgg".ToTag(),
			weight = 0.66f
		},
		new FertilityMonitor.BreedingChance
		{
			egg = "StaterpillarLiquidEgg".ToTag(),
			weight = 0.02f
		}
	};

	public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_LIQUID = new List<FertilityMonitor.BreedingChance>
	{
		new FertilityMonitor.BreedingChance
		{
			egg = "StaterpillarEgg".ToTag(),
			weight = 0.32f
		},
		new FertilityMonitor.BreedingChance
		{
			egg = "StaterpillarGasEgg".ToTag(),
			weight = 0.02f
		},
		new FertilityMonitor.BreedingChance
		{
			egg = "StaterpillarLiquidEgg".ToTag(),
			weight = 0.66f
		}
	};

	public static float STANDARD_CALORIES_PER_CYCLE = 2000000f;

	public static float STANDARD_STARVE_CYCLES = 5f;

	public static float STANDARD_STOMACH_SIZE = StaterpillarTuning.STANDARD_CALORIES_PER_CYCLE * StaterpillarTuning.STANDARD_STARVE_CYCLES;

	public static float POOP_CONVERSTION_RATE = 0.05f;

	public static float EGG_MASS = 2f;
}
