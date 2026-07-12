using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/EdiblesManager")]
public class EdiblesManager : KMonoBehaviour
{
	public static List<EdiblesManager.FoodInfo> GetAllFoodTypes()
	{
		return EdiblesManager.s_allFoodTypes.Where<EdiblesManager.FoodInfo>((EdiblesManager.FoodInfo x) => DlcManager.IsContentActive(x.DlcId)).ToList<EdiblesManager.FoodInfo>();
	}

	public static EdiblesManager.FoodInfo GetFoodInfo(string foodID)
	{
		string text = foodID.Replace("Compost", "");
		EdiblesManager.FoodInfo foodInfo = null;
		EdiblesManager.s_allFoodMap.TryGetValue(text, out foodInfo);
		return foodInfo;
	}

	private static List<EdiblesManager.FoodInfo> s_allFoodTypes = new List<EdiblesManager.FoodInfo>();

	private static Dictionary<string, EdiblesManager.FoodInfo> s_allFoodMap = new Dictionary<string, EdiblesManager.FoodInfo>();

	public class FoodInfo : IConsumableUIItem
	{
		public FoodInfo(string id, string dlcId, float caloriesPerUnit, int quality, float preserveTemperatue, float rotTemperature, float spoilTime, bool can_rot)
		{
			this.Id = id;
			this.DlcId = dlcId;
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
			EdiblesManager.s_allFoodTypes.Add(this);
			EdiblesManager.s_allFoodMap[this.Id] = this;
		}

		public EdiblesManager.FoodInfo AddEffects(List<string> effects, string[] dlcIds)
		{
			if (DlcManager.IsDlcListValidForCurrentContent(dlcIds))
			{
				this.Effects.AddRange(effects);
			}
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

		public string DlcId;

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
