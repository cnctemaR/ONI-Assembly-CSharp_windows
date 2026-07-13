using System;
using System.Collections.Generic;

namespace TUNING
{
	public class FOOD
	{
		public const float EATING_SECONDS_PER_CALORIE = 2E-05f;

		public static float FOOD_CALORIES_PER_CYCLE = -DUPLICANTSTATS.STANDARD.BaseStats.CALORIES_BURNED_PER_CYCLE;

		public const int FOOD_AMOUNT_INGREDIENT_ONLY = 0;

		public const float KCAL_SMALL_PORTION = 600000f;

		public const float KCAL_BONUS_COOKING_LOW = 250000f;

		public const float KCAL_BASIC_PORTION = 800000f;

		public const float KCAL_PREPARED_FOOD = 4000000f;

		public const float KCAL_BONUS_COOKING_BASIC = 400000f;

		public const float KCAL_BONUS_COOKING_DEEPFRIED = 1200000f;

		public const float DEFAULT_PRESERVE_TEMPERATURE = 255.15f;

		public const float DEFAULT_ROT_TEMPERATURE = 277.15f;

		public const float HIGH_PRESERVE_TEMPERATURE = 283.15f;

		public const float HIGH_ROT_TEMPERATURE = 308.15f;

		public const float EGG_COOK_TEMPERATURE = 344.15f;

		public const float DEFAULT_MASS = 1f;

		public const float DEFAULT_SPICE_MASS = 1f;

		public const float ROT_TO_ELEMENT_TIME = 600f;

		public const int MUSH_BAR_SPAWN_GERMS = 1000;

		public const float IDEAL_TEMPERATURE_TOLERANCE = 10f;

		public const int FOOD_QUALITY_AWFUL = -1;

		public const int FOOD_QUALITY_TERRIBLE = 0;

		public const int FOOD_QUALITY_MEDIOCRE = 1;

		public const int FOOD_QUALITY_GOOD = 2;

		public const int FOOD_QUALITY_GREAT = 3;

		public const int FOOD_QUALITY_AMAZING = 4;

		public const int FOOD_QUALITY_WONDERFUL = 5;

		public const int FOOD_QUALITY_MORE_WONDERFUL = 6;

		public class SPOIL_TIME
		{
			public const float DEFAULT = 4800f;

			public const float QUICK = 2400f;

			public const float SLOW = 9600f;

			public const float VERYSLOW = 19200f;
		}

		public class FOOD_TYPES
		{
			public static readonly EdiblesManager.FoodInfo FIELDRATION = new EdiblesManager.FoodInfo("FieldRation", 800000f, -1, 255.15f, 277.15f, 19200f, false, null, null);

			public static readonly EdiblesManager.FoodInfo MUSHBAR = new EdiblesManager.FoodInfo("MushBar", 800000f, -1, 255.15f, 277.15f, 4800f, true, null, null);

			public static readonly EdiblesManager.FoodInfo BASICPLANTFOOD = new EdiblesManager.FoodInfo("BasicPlantFood", 600000f, -1, 255.15f, 277.15f, 4800f, true, null, null);

			public static readonly EdiblesManager.FoodInfo BASICFORAGEPLANT = new EdiblesManager.FoodInfo("BasicForagePlant", 800000f, -1, 255.15f, 277.15f, 4800f, false, null, null);

			public static readonly EdiblesManager.FoodInfo FORESTFORAGEPLANT = new EdiblesManager.FoodInfo("ForestForagePlant", 6400000f, -1, 255.15f, 277.15f, 4800f, false, null, null);

			public static readonly EdiblesManager.FoodInfo SWAMPFORAGEPLANT = new EdiblesManager.FoodInfo("SwampForagePlant", 2400000f, -1, 255.15f, 277.15f, 4800f, false, DlcManager.EXPANSION1, null);

			public static readonly EdiblesManager.FoodInfo ICECAVESFORAGEPLANT = new EdiblesManager.FoodInfo("IceCavesForagePlant", 800000f, -1, 255.15f, 277.15f, 4800f, false, DlcManager.DLC2, null);

			public static readonly EdiblesManager.FoodInfo MUSHROOM = new EdiblesManager.FoodInfo(MushroomConfig.ID, 2400000f, 0, 255.15f, 277.15f, 4800f, true, null, null);

			public static readonly EdiblesManager.FoodInfo LETTUCE = new EdiblesManager.FoodInfo("Lettuce", 400000f, 0, 255.15f, 277.15f, 2400f, true, null, null).AddEffects(new List<string> { "SeafoodRadiationResistance" }, DlcManager.EXPANSION1, null);

			public static readonly EdiblesManager.FoodInfo RAWEGG = new EdiblesManager.FoodInfo("RawEgg", 1600000f, -1, 255.15f, 277.15f, 4800f, true, null, null);

			public static readonly EdiblesManager.FoodInfo MEAT = new EdiblesManager.FoodInfo("Meat", 1600000f, -1, 255.15f, 277.15f, 4800f, true, null, null);

			public static readonly EdiblesManager.FoodInfo PLANTMEAT = new EdiblesManager.FoodInfo("PlantMeat", 1200000f, 1, 255.15f, 277.15f, 2400f, true, DlcManager.EXPANSION1, null);

			public static readonly EdiblesManager.FoodInfo PRICKLEFRUIT = new EdiblesManager.FoodInfo(PrickleFruitConfig.ID, 1600000f, 0, 255.15f, 277.15f, 4800f, true, null, null);

			public static readonly EdiblesManager.FoodInfo SWAMPFRUIT = new EdiblesManager.FoodInfo(SwampFruitConfig.ID, 1840000f, 0, 255.15f, 277.15f, 2400f, true, DlcManager.EXPANSION1, null);

			public static readonly EdiblesManager.FoodInfo FISH_MEAT = new EdiblesManager.FoodInfo("FishMeat", 1000000f, 2, 255.15f, 277.15f, 2400f, true, null, null).AddEffects(new List<string> { "SeafoodRadiationResistance" }, DlcManager.EXPANSION1, null);

			public static readonly EdiblesManager.FoodInfo SHELLFISH_MEAT = new EdiblesManager.FoodInfo("ShellfishMeat", 1000000f, 2, 255.15f, 277.15f, 2400f, true, null, null).AddEffects(new List<string> { "SeafoodRadiationResistance" }, DlcManager.EXPANSION1, null);

			public static readonly EdiblesManager.FoodInfo WORMBASICFRUIT = new EdiblesManager.FoodInfo("WormBasicFruit", 800000f, 0, 255.15f, 277.15f, 4800f, true, DlcManager.EXPANSION1, null);

			public static readonly EdiblesManager.FoodInfo WORMSUPERFRUIT = new EdiblesManager.FoodInfo("WormSuperFruit", 250000f, 1, 255.15f, 277.15f, 2400f, true, DlcManager.EXPANSION1, null);

			public static readonly EdiblesManager.FoodInfo HARDSKINBERRY = new EdiblesManager.FoodInfo("HardSkinBerry", 800000f, -1, 255.15f, 277.15f, 9600f, true, DlcManager.DLC2, null);

			public static readonly EdiblesManager.FoodInfo CARROT = new EdiblesManager.FoodInfo(CarrotConfig.ID, 4000000f, 0, 255.15f, 277.15f, 9600f, true, DlcManager.DLC2, null);

			public static readonly EdiblesManager.FoodInfo PEMMICAN = new EdiblesManager.FoodInfo("Pemmican", FOOD.FOOD_TYPES.HARDSKINBERRY.CaloriesPerUnit * 2f + 1000000f, 2, 255.15f, 277.15f, 19200f, false, DlcManager.DLC2, null);

			public static readonly EdiblesManager.FoodInfo FRIES_CARROT = new EdiblesManager.FoodInfo("FriesCarrot", 5400000f, 3, 255.15f, 277.15f, 2400f, true, DlcManager.DLC2, null);

			public static readonly EdiblesManager.FoodInfo DEEP_FRIED_MEAT = new EdiblesManager.FoodInfo("DeepFriedMeat", 4000000f, 3, 255.15f, 277.15f, 2400f, true, DlcManager.DLC2, null);

			public static readonly EdiblesManager.FoodInfo DEEP_FRIED_NOSH = new EdiblesManager.FoodInfo("DeepFriedNosh", 5000000f, 3, 255.15f, 277.15f, 4800f, true, DlcManager.DLC2, null);

			public static readonly EdiblesManager.FoodInfo DEEP_FRIED_FISH = new EdiblesManager.FoodInfo("DeepFriedFish", 4200000f, 4, 255.15f, 277.15f, 2400f, true, DlcManager.DLC2, null).AddEffects(new List<string> { "SeafoodRadiationResistance" }, DlcManager.EXPANSION1, null);

			public static readonly EdiblesManager.FoodInfo DEEP_FRIED_SHELLFISH = new EdiblesManager.FoodInfo("DeepFriedShellfish", 4200000f, 4, 255.15f, 277.15f, 2400f, true, DlcManager.DLC2, null).AddEffects(new List<string> { "SeafoodRadiationResistance" }, DlcManager.EXPANSION1, null);

			public static readonly EdiblesManager.FoodInfo PICKLEDMEAL = new EdiblesManager.FoodInfo("PickledMeal", 1800000f, -1, 255.15f, 277.15f, 19200f, true, null, null);

			public static readonly EdiblesManager.FoodInfo BASICPLANTBAR = new EdiblesManager.FoodInfo("BasicPlantBar", 1700000f, 0, 255.15f, 277.15f, 4800f, true, null, null);

			public static readonly EdiblesManager.FoodInfo FRIEDMUSHBAR = new EdiblesManager.FoodInfo("FriedMushBar", 1050000f, 0, 255.15f, 277.15f, 4800f, true, null, null);

			public static readonly EdiblesManager.FoodInfo GAMMAMUSH = new EdiblesManager.FoodInfo("GammaMush", 1050000f, 1, 255.15f, 277.15f, 2400f, true, null, null);

			public static readonly EdiblesManager.FoodInfo GRILLED_PRICKLEFRUIT = new EdiblesManager.FoodInfo("GrilledPrickleFruit", 2000000f, 1, 255.15f, 277.15f, 4800f, true, null, null);

			public static readonly EdiblesManager.FoodInfo SWAMP_DELIGHTS = new EdiblesManager.FoodInfo("SwampDelights", 2240000f, 1, 255.15f, 277.15f, 4800f, true, DlcManager.EXPANSION1, null);

			public static readonly EdiblesManager.FoodInfo FRIED_MUSHROOM = new EdiblesManager.FoodInfo("FriedMushroom", 2800000f, 1, 255.15f, 277.15f, 4800f, true, null, null);

			public static readonly EdiblesManager.FoodInfo COOKED_PIKEAPPLE = new EdiblesManager.FoodInfo("CookedPikeapple", 1200000f, 1, 255.15f, 277.15f, 4800f, true, DlcManager.DLC2, null);

			public static readonly EdiblesManager.FoodInfo COLD_WHEAT_BREAD = new EdiblesManager.FoodInfo("ColdWheatBread", 1200000f, 2, 255.15f, 277.15f, 4800f, true, null, null);

			public static readonly EdiblesManager.FoodInfo COOKED_EGG = new EdiblesManager.FoodInfo("CookedEgg", 2800000f, 2, 255.15f, 277.15f, 2400f, true, null, null);

			public static readonly EdiblesManager.FoodInfo COOKED_FISH = new EdiblesManager.FoodInfo("CookedFish", 1600000f, 3, 255.15f, 277.15f, 2400f, true, null, null).AddEffects(new List<string> { "SeafoodRadiationResistance" }, DlcManager.EXPANSION1, null);

			public static readonly EdiblesManager.FoodInfo COOKED_MEAT = new EdiblesManager.FoodInfo("CookedMeat", 4000000f, 3, 255.15f, 277.15f, 2400f, true, null, null);

			public static readonly EdiblesManager.FoodInfo PANCAKES = new EdiblesManager.FoodInfo("Pancakes", 3600000f, 3, 255.15f, 277.15f, 4800f, true, null, null);

			public static readonly EdiblesManager.FoodInfo WORMBASICFOOD = new EdiblesManager.FoodInfo("WormBasicFood", 1200000f, 1, 255.15f, 277.15f, 4800f, true, DlcManager.EXPANSION1, null);

			public static readonly EdiblesManager.FoodInfo WORMSUPERFOOD = new EdiblesManager.FoodInfo("WormSuperFood", 2400000f, 3, 255.15f, 277.15f, 19200f, true, DlcManager.EXPANSION1, null);

			public static readonly EdiblesManager.FoodInfo FRUITCAKE = new EdiblesManager.FoodInfo("FruitCake", 4000000f, 3, 255.15f, 277.15f, 19200f, false, null, null);

			public static readonly EdiblesManager.FoodInfo SALSA = new EdiblesManager.FoodInfo("Salsa", 4400000f, 4, 255.15f, 277.15f, 2400f, true, null, null);

			public static readonly EdiblesManager.FoodInfo SURF_AND_TURF = new EdiblesManager.FoodInfo("SurfAndTurf", 6000000f, 4, 255.15f, 277.15f, 2400f, true, null, null).AddEffects(new List<string> { "SeafoodRadiationResistance" }, DlcManager.EXPANSION1, null);

			public static readonly EdiblesManager.FoodInfo MUSHROOM_WRAP = new EdiblesManager.FoodInfo("MushroomWrap", 4800000f, 4, 255.15f, 277.15f, 2400f, true, null, null).AddEffects(new List<string> { "SeafoodRadiationResistance" }, DlcManager.EXPANSION1, null);

			public static readonly EdiblesManager.FoodInfo TOFU = new EdiblesManager.FoodInfo("Tofu", 3600000f, 2, 255.15f, 277.15f, 2400f, true, null, null);

			public static readonly EdiblesManager.FoodInfo CURRY = new EdiblesManager.FoodInfo("Curry", 5000000f, 4, 255.15f, 277.15f, 9600f, true, null, null).AddEffects(new List<string> { "HotStuff", "WarmTouchFood" }, null, null);

			public static readonly EdiblesManager.FoodInfo SPICEBREAD = new EdiblesManager.FoodInfo("SpiceBread", 4000000f, 5, 255.15f, 277.15f, 4800f, true, null, null);

			public static readonly EdiblesManager.FoodInfo SPICY_TOFU = new EdiblesManager.FoodInfo("SpicyTofu", 4000000f, 5, 255.15f, 277.15f, 2400f, true, null, null).AddEffects(new List<string> { "WarmTouchFood" }, null, null);

			public static readonly EdiblesManager.FoodInfo QUICHE = new EdiblesManager.FoodInfo("Quiche", 6400000f, 5, 255.15f, 277.15f, 2400f, true, null, null).AddEffects(new List<string> { "SeafoodRadiationResistance" }, DlcManager.EXPANSION1, null);

			public static readonly EdiblesManager.FoodInfo BERRY_PIE = new EdiblesManager.FoodInfo("BerryPie", 4200000f, 5, 255.15f, 277.15f, 2400f, true, DlcManager.EXPANSION1, null);

			public static readonly EdiblesManager.FoodInfo BURGER = new EdiblesManager.FoodInfo("Burger", 6000000f, 6, 255.15f, 277.15f, 2400f, true, null, null).AddEffects(new List<string> { "GoodEats" }, null, null).AddEffects(new List<string> { "SeafoodRadiationResistance" }, DlcManager.EXPANSION1, null);

			public static readonly EdiblesManager.FoodInfo BEAN = new EdiblesManager.FoodInfo("BeanPlantSeed", 0f, 3, 255.15f, 277.15f, 4800f, true, null, null);

			public static readonly EdiblesManager.FoodInfo SPICENUT = new EdiblesManager.FoodInfo(SpiceNutConfig.ID, 0f, 0, 255.15f, 277.15f, 2400f, true, null, null);

			public static readonly EdiblesManager.FoodInfo COLD_WHEAT_SEED = new EdiblesManager.FoodInfo("ColdWheatSeed", 0f, 0, 283.15f, 308.15f, 9600f, true, null, null);
		}

		public class RECIPES
		{
			public static float SMALL_COOK_TIME = 30f;

			public static float STANDARD_COOK_TIME = 50f;
		}
	}
}
