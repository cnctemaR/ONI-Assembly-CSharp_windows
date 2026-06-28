using System;
using System.Collections.Generic;

namespace TUNING
{
	public class CROPS
	{
		public const float GROWTH_RATE = 0.0016666667f;

		public const float WILD_GROWTH_RATE = 0.00041666668f;

		public const float PLANTERPLOT_GROWTH_PENTALY = -0.5f;

		public const float BONUS_SEED_PROBABILITY = 0.33f;

		public const float YIELD_DISPLAY_TOTAL = 100f;

		public const float MED_YIELD_THRESHOLD = 0.4f;

		public const float HIGH_YIELD_THRESHOLD = 0.8f;

		public const float SELF_HARVEST_TIME = 2400f;

		public const float SELF_PLANT_TIME = 2400f;

		public const float FERTILIZATION_GAIN_RATE = 1.6666666f;

		public const float FERTILIZATION_LOSS_RATE = -0.16666667f;

		public static List<Crop.CropVal> CROP_TYPES = new List<Crop.CropVal>
		{
			new Crop.CropVal("BasicPlantFood", 1800f, 8, true),
			new Crop.CropVal("SwampLilyFlower", 1200f, 1, true),
			new Crop.CropVal(PrickleFruitConfig.ID, 2400f, 1, true),
			new Crop.CropVal("ColdWheatSeed", 12000f, 25, true),
			new Crop.CropVal(SpiceNutConfig.ID, 13200f, 4, true),
			new Crop.CropVal(BasicFabricConfig.ID, 1200f, 1, true),
			new Crop.CropVal(MushroomConfig.ID, 6000f, 2, true)
		};
	}
}
