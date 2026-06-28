using System;
using System.Collections.Generic;

namespace TUNING
{
	public class FOOD
	{
		public const float EATING_SECONDS_PER_RATION = 3f;

		public const float CALORIES_PER_RATION = 100000f;

		public const float RATIONS_PER_CYCLE = 10f;

		public const int MAX_RATIONS_BEFORE_INFINITE = 15;

		public const int FOOD_AMOUNT_TINY = 1;

		public const int FOOD_AMOUNT_SMALL = 10;

		public const float DEFAULT_ROT_TEMPERATURE = 277.15f;

		public const float DEFAULT_STALE_TIME = 2400f;

		public const float DEFAULT_SPOIL_TIME = 4800f;

		public const float DEFAULT_MASS = 1f;

		public static List<EdiblesManager.FoodInfo> FOOD_TYPES_LIST = new List<EdiblesManager.FoodInfo>();

		public class FOOD_TYPES
		{
			public static EdiblesManager.FoodInfo FIELDRATION = new EdiblesManager.FoodInfo("FieldRation", 10, Edible.Quality.Average, 277.15f, 2400f, 4800f, string.Empty);

			public static EdiblesManager.FoodInfo MUSHBAR = new EdiblesManager.FoodInfo("MushBar", 10, Edible.Quality.Poor, 277.15f, 2400f, 4800f, string.Empty);

			public static EdiblesManager.FoodInfo FRUIT = new EdiblesManager.FoodInfo("Fruit", 10, Edible.Quality.Average, 277.15f, 2400f, 4800f, string.Empty);

			public static EdiblesManager.FoodInfo MEAT = new EdiblesManager.FoodInfo("Meat", 10, Edible.Quality.Average, 277.15f, 2400f, 4800f, string.Empty);

			public static EdiblesManager.FoodInfo FRIEDMUSHBAR = new EdiblesManager.FoodInfo("FriedMushBar", 10, Edible.Quality.Average, 277.15f, 2400f, 4800f, string.Empty);

			public static EdiblesManager.FoodInfo PRICKLEFRUIT = new EdiblesManager.FoodInfo(PrickleFruitConfig.ID, 10, Edible.Quality.Good, 277.15f, 2400f, 4800f, string.Empty);

			public static EdiblesManager.FoodInfo BASICPLANTFOOD = new EdiblesManager.FoodInfo("BasicPlantFood", 1, Edible.Quality.Poor, 277.15f, 2400f, 4800f, string.Empty);

			public static EdiblesManager.FoodInfo BASICPLANTBAR = new EdiblesManager.FoodInfo("BasicPlantBar", 10, Edible.Quality.Average, 277.15f, 2400f, 4800f, string.Empty);

			public static EdiblesManager.FoodInfo BASICFORAGEPLANT = new EdiblesManager.FoodInfo("BasicForagePlant", 10, Edible.Quality.Average, 277.15f, 2400f, 4800f, string.Empty);
		}
	}
}
