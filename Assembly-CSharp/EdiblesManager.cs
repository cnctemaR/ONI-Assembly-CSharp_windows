using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/EdiblesManager")]
public class EdiblesManager : KMonoBehaviour
{
	public static List<EdiblesManager.FoodInfo> GetAllLoadedFoodTypes()
	{
		return EdiblesManager.s_allFoodTypes.Where<EdiblesManager.FoodInfo>(new Func<EdiblesManager.FoodInfo, bool>(DlcManager.IsCorrectDlcSubscribed)).ToList<EdiblesManager.FoodInfo>();
	}

	public static void ClearSaveFoodCache()
	{
		EdiblesManager.s_loadedFoodTypes = null;
	}

	public static List<EdiblesManager.FoodInfo> GetAllFoodTypes()
	{
		if (EdiblesManager.s_loadedFoodTypes == null)
		{
			EdiblesManager.s_loadedFoodTypes = EdiblesManager.s_allFoodTypes.Where<EdiblesManager.FoodInfo>(new Func<EdiblesManager.FoodInfo, bool>(Game.IsCorrectDlcActiveForCurrentSave)).ToList<EdiblesManager.FoodInfo>();
		}
		global::Debug.Assert(SaveLoader.Instance != null, "Call GetAllLoadedFoodTypes from the frontend");
		return EdiblesManager.s_loadedFoodTypes;
	}

	public static EdiblesManager.FoodInfo GetFoodInfo(string foodID)
	{
		string text = foodID.Replace("Compost", "");
		EdiblesManager.FoodInfo foodInfo = null;
		EdiblesManager.s_allFoodMap.TryGetValue(text, out foodInfo);
		return foodInfo;
	}

	public static bool TryGetFoodInfo(string foodID, out EdiblesManager.FoodInfo info)
	{
		info = null;
		if (string.IsNullOrEmpty(foodID))
		{
			return false;
		}
		info = EdiblesManager.GetFoodInfo(foodID);
		return info != null;
	}

	private static List<EdiblesManager.FoodInfo> s_allFoodTypes = new List<EdiblesManager.FoodInfo>();

	private static Dictionary<string, EdiblesManager.FoodInfo> s_allFoodMap = new Dictionary<string, EdiblesManager.FoodInfo>();

	private static List<EdiblesManager.FoodInfo> s_loadedFoodTypes;

	public class FoodInfo : IConsumableUIItem, IHasDlcRestrictions
	{
		public string[] GetRequiredDlcIds()
		{
			return this.requiredDlcIds;
		}

		public string[] GetForbiddenDlcIds()
		{
			return this.forbiddenDlcIds;
		}

		[Obsolete("Use constructor with required/forbidden instead")]
		public FoodInfo(string id, string dlcId, float caloriesPerUnit, int quality, float preserveTemperatue, float rotTemperature, float spoilTime, bool can_rot)
			: this(id, caloriesPerUnit, quality, preserveTemperatue, rotTemperature, spoilTime, can_rot, null, null)
		{
			if (dlcId != "")
			{
				this.requiredDlcIds = new string[] { dlcId };
			}
		}

		public FoodInfo(string id, float caloriesPerUnit, int quality, float preserveTemperatue, float rotTemperature, float spoilTime, bool can_rot, string[] requiredDlcIds = null, string[] forbiddenDlcIds = null)
		{
			this.Id = id;
			this.requiredDlcIds = requiredDlcIds;
			this.forbiddenDlcIds = forbiddenDlcIds;
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

		public EdiblesManager.FoodInfo AddEffects(List<string> effects, string[] requiredDlcIds = null, string[] forbiddenDlcIds = null)
		{
			if (DlcManager.IsCorrectDlcSubscribed(requiredDlcIds, forbiddenDlcIds))
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

		private string[] requiredDlcIds;

		private string[] forbiddenDlcIds;
	}
}
