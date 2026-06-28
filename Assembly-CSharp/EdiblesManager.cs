using System;
using System.Collections.Generic;
using TUNING;

public class EdiblesManager : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		EdiblesManager.instance = this;
	}

	public EdiblesManager.FoodInfo GetFoodInfo(string FoodID)
	{
		string text = FoodID.Replace("Compost", string.Empty);
		foreach (EdiblesManager.FoodInfo foodInfo in FOOD.FOOD_TYPES_LIST)
		{
			if (foodInfo.Id == text)
			{
				return foodInfo;
			}
		}
		Debug.LogWarning("No food with ID: " + FoodID, null);
		return null;
	}

	public static EdiblesManager instance;

	public class FoodInfo
	{
		public FoodInfo(string id, float caloriesPerUnit, int quality, float preserveTemperatue, float rotTemperature, float spoilTime)
		{
			this.Id = id;
			this.CaloriesPerUnit = caloriesPerUnit;
			this.Quality = quality;
			this.PreserveTemperature = preserveTemperatue;
			this.RotTemperature = rotTemperature;
			this.StaleTime = spoilTime / 2f;
			this.SpoilTime = spoilTime;
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

		public string Id;

		public string Name;

		public string Description;

		public float CaloriesPerUnit;

		public float PreserveTemperature;

		public float RotTemperature;

		public float StaleTime;

		public float SpoilTime;

		public int Quality;

		public List<string> Effects;
	}
}
