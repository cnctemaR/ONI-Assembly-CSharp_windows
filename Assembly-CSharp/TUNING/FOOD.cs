using System;
using System.Collections.Generic;

namespace TUNING
{
	public class FOOD
	{
		public const float EATING_SECONDS_PER_CALORIE = 2E-05f;

		public const float FOOD_CALORIES_PER_CYCLE = 1000000f;

		public const int FOOD_AMOUNT_INGREDIENT_ONLY = 0;

		public const float FOOD_AMOUNT_TINY = 100000f;

		public const float FOOD_AMOUNT_SMALL = 1000000f;

		public const float FOOD_AMOUNT_NORMAL = 1600000f;

		public const float DEFAULT_PRESERVE_TEMPERATURE = 255.15f;

		public const float DEFAULT_ROT_TEMPERATURE = 277.15f;

		public const float HIGH_PRESERVE_TEMPERATURE = 283.15f;

		public const float HIGH_ROT_TEMPERATURE = 308.15f;

		public const float DEFAULT_MASS = 1f;

		public const float DEFAULT_SPICE_MASS = 1f;

		public const float ROT_TO_ELEMENT_TIME = 600f;

		public const float MAX_RATIONS_STACK = 10000000f;

		public const float MAX_UNITS_STACK = 10f;

		public static List<EdiblesManager.FoodInfo> FOOD_TYPES_LIST = new List<EdiblesManager.FoodInfo>();

		public class SPOIL_TIME
		{
			public const float DEFAULT = 2400f;

			public const float QUICK = 1200f;

			public const float SLOW = 4800f;

			public const float VERYSLOW = 9600f;
		}

		public class FOOD_TYPES
		{
			public static EdiblesManager.FoodInfo FIELDRATION = new EdiblesManager.FoodInfo("FieldRation", 1000000f, -3, 255.15f, 277.15f, 9600f);

			public static EdiblesManager.FoodInfo MUSHBAR = new EdiblesManager.FoodInfo("MushBar", 1000000f, -3, 255.15f, 277.15f, 2400f).AddEffects(new List<string> { "Diarrhea" });

			public static EdiblesManager.FoodInfo MUSHROOM = new EdiblesManager.FoodInfo(MushroomConfig.ID, 1600000f, 0, 255.15f, 277.15f, 2400f);

			public static EdiblesManager.FoodInfo MEAT = new EdiblesManager.FoodInfo("Meat", 1000000f, -2, 255.15f, 277.15f, 2400f);

			public static EdiblesManager.FoodInfo PRICKLEFRUIT = new EdiblesManager.FoodInfo(PrickleFruitConfig.ID, 1600000f, -1, 255.15f, 277.15f, 2400f);

			public static EdiblesManager.FoodInfo BASICPLANTFOOD = new EdiblesManager.FoodInfo("BasicPlantFood", 100000f, -3, 255.15f, 277.15f, 2400f);

			public static EdiblesManager.FoodInfo BASICFORAGEPLANT = new EdiblesManager.FoodInfo("BasicForagePlant", 1000000f, -3, 255.15f, 277.15f, 2400f);

			public static EdiblesManager.FoodInfo FRUITCAKE = new EdiblesManager.FoodInfo("FruitCake", 1600000f, -1, 255.15f, 277.15f, 9600f);

			public static EdiblesManager.FoodInfo BASICPLANTBAR = new EdiblesManager.FoodInfo("BasicPlantBar", 1000000f, -2, 255.15f, 277.15f, 2400f);

			public static EdiblesManager.FoodInfo FRIEDMUSHBAR = new EdiblesManager.FoodInfo("FriedMushBar", 1000000f, -2, 255.15f, 277.15f, 2400f);

			public static EdiblesManager.FoodInfo PICKLEDMEAL = new EdiblesManager.FoodInfo("PickledMeal", 100000f, -3, 255.15f, 277.15f, 9600f);

			public static EdiblesManager.FoodInfo GRILLED_PRICKLEFRUIT = new EdiblesManager.FoodInfo("GrilledPrickleFruit", 1000000f, 0, 255.15f, 277.15f, 2400f);

			public static EdiblesManager.FoodInfo FRIED_MUSHROOM = new EdiblesManager.FoodInfo("FriedMushroom", 1000000f, 1, 255.15f, 277.15f, 2400f);

			public static EdiblesManager.FoodInfo COLD_WHEAT_BREAD = new EdiblesManager.FoodInfo("ColdWheatBread", 1000000f, -1, 255.15f, 277.15f, 2400f);

			public static EdiblesManager.FoodInfo SALSA = new EdiblesManager.FoodInfo("Salsa", 1000000f, 2, 255.15f, 277.15f, 1200f);

			public static EdiblesManager.FoodInfo SPICEBREAD = new EdiblesManager.FoodInfo("SpiceBread", 1000000f, 1, 255.15f, 277.15f, 2400f);

			public static EdiblesManager.FoodInfo COOKEDMEAT = new EdiblesManager.FoodInfo("CookedMeat", 1600000f, 1, 255.15f, 277.15f, 1200f).AddEffects(new List<string> { "GoodEats" });

			public static EdiblesManager.FoodInfo SPICENUT = new EdiblesManager.FoodInfo(SpiceNutConfig.ID, 0f, 0, 255.15f, 277.15f, 1200f);

			public static EdiblesManager.FoodInfo COLD_WHEAT_SEED = new EdiblesManager.FoodInfo("ColdWheatSeed", 0f, 0, 283.15f, 308.15f, 4800f);
		}

		public class RECIPES
		{
			public static float SMALL_COOK_TIME = 20f;

			public static float STANDARD_COOK_TIME = 40f;
		}
	}
}
