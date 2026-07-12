using System;
using System.Collections.Generic;
using TUNING;

public static class BellyTuning
{
	public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_BASE = new List<FertilityMonitor.BreedingChance>
	{
		new FertilityMonitor.BreedingChance
		{
			egg = "IceBellyEgg".ToTag(),
			weight = 0.98f
		}
	};

	public const float KW_GENERATED_TO_WARM_UP = 1.3f;

	public static float STANDARD_CALORIES_PER_CYCLE = 4f * FOOD.FOOD_TYPES.CARROT.CaloriesPerUnit / (CROPS.CROP_TYPES.Find((Crop.CropVal m) => m.cropId == CarrotConfig.ID).cropDuration / 600f);

	public const float STANDARD_STARVE_CYCLES = 10f;

	public static float STANDARD_STOMACH_SIZE = BellyTuning.STANDARD_CALORIES_PER_CYCLE * 10f;

	public static int PEN_SIZE_PER_CREATURE = CREATURES.SPACE_REQUIREMENTS.TIER3;

	public const float EGG_MASS = 8f;
}
