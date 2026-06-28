using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class Edible : Workable, IGameObjectEffectDescriptor
{
	private Edible()
	{
		this.showProgressBar = false;
		base.SetOffsetTable(OffsetGroups.InvertedStandardTable);
	}

	public float Units
	{
		get
		{
			return base.GetComponent<PrimaryElement>().Units;
		}
		set
		{
			base.GetComponent<PrimaryElement>().Units = value;
		}
	}

	public float Calories
	{
		get
		{
			return this.Units * this.foodInfo.CaloriesPerUnit;
		}
		set
		{
			this.Units = value / this.foodInfo.CaloriesPerUnit;
		}
	}

	public EdiblesManager.FoodInfo FoodInfo
	{
		get
		{
			return this.foodInfo;
		}
		set
		{
			this.foodInfo = value;
			this.FoodID = this.foodInfo.Id;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (this.foodInfo == null)
		{
			if (this.FoodID == null)
			{
				Output.LogError(new object[] { "No food FoodID" });
			}
			this.foodInfo = EdiblesManager.instance.GetFoodInfo(this.FoodID);
		}
		base.GetComponent<KSelectable>().SetName(this.foodInfo.Name);
		base.GetComponent<KPrefabID>().AddTag(GameTags.Edible);
		this.Subscribe(748399584, new Action<object>(this.OnCraft));
		this.Subscribe(1272413801, new Action<object>(this.OnCraft));
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Eating;
		Components.Edibles.Add(this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().MiscStatusItems.Edible, this);
	}

	private void OnCraft(object data)
	{
		RationTracker.Get().RegisterCaloriesProduced(this.Calories);
	}

	public float GetFeedingTime(Worker worker)
	{
		float num = this.Calories * 2E-05f;
		if (worker != null)
		{
			BingeEatChore.StatesInstance smi = worker.GetSMI<BingeEatChore.StatesInstance>();
			if (smi != null && smi.IsBingeEating())
			{
				num /= 2f;
			}
		}
		return num;
	}

	protected override void OnStartWork(Worker worker)
	{
		base.SetWorkTime(this.GetFeedingTime(worker));
		worker.GetAttributes().Add("Eating", this.caloriesModifier);
		this.StartConsuming();
		if (this.FoodID == "CookedMeat")
		{
			worker.GetComponent<Effects>().Add("GoodEats", true);
		}
	}

	protected override void OnStopWork(Worker worker)
	{
		worker.GetAttributes().Remove(this.caloriesModifier);
		this.StopConsuming(worker);
	}

	private void StartConsuming()
	{
		this.consumptionStartTime = Time.time;
		base.worker.Trigger(1406130139, this);
	}

	private void StopConsuming(Worker worker)
	{
		float num = Time.time - this.consumptionStartTime;
		float num2 = Mathf.Clamp01(num / this.GetFeedingTime(worker));
		this.unitsConsumed = this.Units * num2;
		this.caloriesConsumed = this.unitsConsumed * this.foodInfo.CaloriesPerUnit;
		this.Units -= this.unitsConsumed;
		worker.Trigger(1121894420, this);
		this.Trigger(-10536414, worker.gameObject);
		this.unitsConsumed = float.NaN;
		this.caloriesConsumed = float.NaN;
		this.consumptionStartTime = float.NaN;
		if (this.Units <= 0f)
		{
			base.gameObject.DeleteObject();
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.Edibles.Remove(this);
	}

	public int GetQuality()
	{
		return this.foodInfo.Quality;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		list.Add(new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.CALORIES, GameUtil.GetFormattedCalories(this.foodInfo.CaloriesPerUnit, GameUtil.TimeSlice.None, true)), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.CALORIES, GameUtil.GetFormattedCalories(this.foodInfo.CaloriesPerUnit, GameUtil.TimeSlice.None, true)), Descriptor.DescriptorType.Effect, false));
		list.Add(new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.FOOD_QUALITY, GameUtil.GetFormattedFoodQuality(this.foodInfo.Quality, false)), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.FOOD_QUALITY, GameUtil.GetFormattedFoodQuality(this.foodInfo.Quality, true)), Descriptor.DescriptorType.Effect, false));
		if (this.consumptionEffects.Count > 0)
		{
			for (int i = 0; i < this.consumptionEffects.Count; i++)
			{
				list.Add(this.consumptionEffects[i]);
			}
		}
		return list;
	}

	public string FoodID;

	private EdiblesManager.FoodInfo foodInfo;

	private float consumptionStartTime = float.NaN;

	public float unitsConsumed = float.NaN;

	public float caloriesConsumed = float.NaN;

	private AttributeModifier caloriesModifier = new AttributeModifier("CaloriesDelta", 50000f, DUPLICANTS.MODIFIERS.EATINGCALORIES.NAME, false, false);

	public List<Descriptor> consumptionEffects = new List<Descriptor>();

	public enum Quality
	{
		Awful = -3,
		Terrible,
		Poor,
		Average,
		Good,
		Great,
		Amazing
	}
}
