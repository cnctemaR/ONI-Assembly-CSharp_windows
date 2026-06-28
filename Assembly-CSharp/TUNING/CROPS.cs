using System;
using System.Collections.Generic;

namespace TUNING
{
	public class CROPS
	{
		public const float GROWTH_RATE = 0.0016666667f;

		public const float WILD_GROWTH_RATE = 0.00041666668f;

		public const float PLANTERPLOT_GROWTH_PENTALY = -0.5f;

		public const float SELF_HARVEST_TIME = 2400f;

		public const float SELF_PLANT_TIME = 2400f;

		public const float FERTILIZATION_GAIN_RATE = 0.16666667f;

		public const float FERTILIZATION_LOSS_RATE = -0.16666667f;

		public static List<CROPS.CropVal> CROP_TYPES = new List<CROPS.CropVal>
		{
			new CROPS.CropVal("BasicPlantFood", 3000f, 1500f, 15, false, 3),
			new CROPS.CropVal("SwampLilyFlower", 1200f, 1200f, 1, false, 1),
			new CROPS.CropVal(PrickleFruitConfig.ID, 6000f, 1200f, 1, false, 5)
		};

		public struct CropVal
		{
			public CropVal(string crop_id, float crop_duration, float regrow_duration, int crop_num = 1, bool renewable = true, int harvests = 1)
			{
				this.crop_id = crop_id;
				this.crop_duration = crop_duration;
				this.regrow_duration = regrow_duration;
				this.renewable = renewable;
				this.harvests = harvests;
				this.crop_num = crop_num;
			}

			public string crop_id;

			public float crop_duration;

			public float regrow_duration;

			public bool renewable;

			public int harvests;

			public int crop_num;
		}
	}
}
