using System;
using System.Collections.Generic;
using UnityEngine;

namespace TUNING
{
	public class CROPS
	{
		public const float WILD_GROWTH_RATE_MODIFIER = 0.25f;

		public const float GROWTH_RATE = 0.0016666667f;

		public const float WILD_GROWTH_RATE = 0.00041666668f;

		public const float PLANTERPLOT_GROWTH_PENTALY = -0.5f;

		public const float BASE_BONUS_SEED_PROBABILITY = 0.1f;

		public const float SELF_HARVEST_TIME = 2400f;

		public const float SELF_PLANT_TIME = 2400f;

		public const float TREE_BRANCH_SELF_HARVEST_TIME = 12000f;

		public const float FERTILIZATION_GAIN_RATE = 1.6666666f;

		public const float FERTILIZATION_LOSS_RATE = -0.16666667f;

		public static List<Crop.CropVal> CROP_TYPES = new List<Crop.CropVal>
		{
			new Crop.CropVal("BasicPlantFood", 1800f, 1, true),
			new Crop.CropVal(PrickleFruitConfig.ID, 3600f, 1, true),
			new Crop.CropVal(SwampFruitConfig.ID, 3960f, 1, true),
			new Crop.CropVal(MushroomConfig.ID, 4500f, 1, true),
			new Crop.CropVal("ColdWheatSeed", 10800f, 18, true),
			new Crop.CropVal(SpiceNutConfig.ID, 4800f, 4, true),
			new Crop.CropVal(BasicFabricConfig.ID, 1200f, 1, true),
			new Crop.CropVal(SwampLilyFlowerConfig.ID, 7200f, 2, true),
			new Crop.CropVal("GasGrassHarvested", 2400f, 1, true),
			new Crop.CropVal("WoodLog", 2700f, 300, true),
			new Crop.CropVal(SimHashes.WoodLog.ToString(), 2700f, 300, true),
			new Crop.CropVal(SimHashes.SugarWater.ToString(), 150f, 20, true),
			new Crop.CropVal("SpaceTreeBranch", 2700f, 1, true),
			new Crop.CropVal("HardSkinBerry", 1800f, 1, true),
			new Crop.CropVal(CarrotConfig.ID, 5400f, 1, true),
			new Crop.CropVal(VineFruitConfig.ID, 1800f, 1, true),
			new Crop.CropVal(SimHashes.OxyRock.ToString(), 1200f, 2 * Mathf.RoundToInt(17.76f), true),
			new Crop.CropVal("Lettuce", 7200f, 12, true),
			new Crop.CropVal(KelpConfig.ID, 3000f, 50, true),
			new Crop.CropVal("BeanPlantSeed", 12600f, 12, true),
			new Crop.CropVal("OxyfernSeed", 7200f, 1, true),
			new Crop.CropVal("PlantMeat", 18000f, 10, true),
			new Crop.CropVal("WormBasicFruit", 2400f, 1, true),
			new Crop.CropVal("WormSuperFruit", 4800f, 8, true),
			new Crop.CropVal(DewDripConfig.ID, 1200f, 1, true),
			new Crop.CropVal(FernFoodConfig.ID, 5400f, 36, true),
			new Crop.CropVal(SimHashes.Salt.ToString(), 3600f, 65, true),
			new Crop.CropVal(SimHashes.Water.ToString(), 6000f, 350, true),
			new Crop.CropVal(SimHashes.Amber.ToString(), 7200f, 264, true),
			new Crop.CropVal("GardenFoodPlantFood", 1800f, 1, true),
			new Crop.CropVal("Butterfly", 3000f, 1, true)
		};
	}
}
