using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class EdiblesManager : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		EdiblesManager.instance = this;
		this.foodInfos = new List<EdiblesManager.FoodInfo>(FOOD.FOOD_TYPES_LIST);
	}

	public EdiblesManager.FoodInfo GetFoodInfo(string FoodID)
	{
		foreach (EdiblesManager.FoodInfo foodInfo in this.foodInfos)
		{
			if (foodInfo.Id == FoodID)
			{
				return foodInfo;
			}
		}
		Debug.LogWarning("No food with ID: " + FoodID);
		return null;
	}

	public static EdiblesManager instance;

	[HideInInspector]
	public List<EdiblesManager.FoodInfo> foodInfos;

	public class FoodInfo
	{
		public FoodInfo(string id, int rations, Edible.Quality quality, float rotTemperature, float staleTime, float spoilTime, string tags = "")
		{
			this.Id = id;
			this.Rations = rations;
			this.Quality = quality;
			this.Tags = tags;
			this.RotTemperature = rotTemperature;
			this.StaleTime = staleTime;
			this.SpoilTime = spoilTime;
			this.Name = Strings.Get("STRINGS.ITEMS.FOOD." + id.ToUpper() + ".NAME");
			this.Description = Strings.Get("STRINGS.ITEMS.FOOD." + id.ToUpper() + ".DESC");
			FOOD.FOOD_TYPES_LIST.Add(this);
		}

		public string Id;

		public string Name;

		public string Description;

		public int Rations;

		public float RotTemperature;

		public float StaleTime;

		public float SpoilTime;

		public string Tags;

		public Edible.Quality Quality;
	}
}
