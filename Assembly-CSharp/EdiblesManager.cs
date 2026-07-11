using System;
using System.Collections.Generic;
using TUNING;

public class EdiblesManager : KMonoBehaviour
{
	public static EdiblesManager.FoodInfo GetFoodInfo(string foodID)
	{
		string text = foodID.Replace("Compost", "");
		foreach (EdiblesManager.FoodInfo foodInfo in FOOD.FOOD_TYPES_LIST)
		{
			if (foodInfo.Id == text)
			{
				return foodInfo;
			}
		}
		return null;
	}

	public class FoodInfo : IConsumableUIItem
	{
		public FoodInfo(string id, float caloriesPerUnit, int quality, float preserveTemperatue, float rotTemperature, float spoilTime, bool can_rot)
		{
			this.Id = id;
			this.CaloriesPerUnit = caloriesPerUnit;
			this.Quality = quality;
			this.PreserveTemperature = preserveTemperatue;
			this.RotTemperature = rotTemperature;
			this.StaleTime = spoilTime / 2f;
			this.SpoilTime = spoilTime;
			this.CanRot = can_rot;
			this.Name = Strings.Get("STRINGS.ITEMS.FOOD." + id.ToUpper() + ".NAME");
			this.Description = Strings.Get("STRINGS.ITEMS.FOOD." + id.ToUpper() + ".DESC");
			this.Effects = new List<string>();
			FOOD.FOOD_TYPES_LIST.Add(this);
		}

		public EdiblesManager.FoodInfo AddEffects(List<string> effects)
		{
			this.Effects.AddRange(effects);
			return this;
		}

		public string ConsumableId
		{
			get
			{
				return this.Id;
			}
		}

		public string ConsumableName
		{
			get
			{
				return this.Name;
			}
		}

		public int MajorOrder
		{
			get
			{
				return this.Quality;
			}
		}

		public int MinorOrder
		{
			get
			{
				return (int)this.CaloriesPerUnit;
			}
		}

		public bool Display
		{
			get
			{
				return this.CaloriesPerUnit != 0f;
			}
		}

		public string Id;

		public string Name;

		public string Description;

		public float CaloriesPerUnit;

		public float PreserveTemperature;

		public float RotTemperature;

		public float StaleTime;

		public float SpoilTime;

		public bool CanRot;

		public int Quality;

		public List<string> Effects;
	}
}
