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
		this.shouldTransferDiseaseWithWorker = false;
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
		for (int i = 0; i < this.foodInfo.Effects.Count; i++)
		{
			worker.GetComponent<Effects>().Add(this.foodInfo.Effects[i], true);
		}
		ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, -this.caloriesConsumed, string.Format(UI.ENDOFDAYREPORT.NOTES.EATEN, this.GetProperName()), worker.GetProperName());
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

	public override List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		list.Add(new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.CALORIES, GameUtil.GetFormattedCalories(this.foodInfo.CaloriesPerUnit, GameUtil.TimeSlice.None, true)), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.CALORIES, GameUtil.GetFormattedCalories(this.foodInfo.CaloriesPerUnit, GameUtil.TimeSlice.None, true)), Descriptor.DescriptorType.Effect, false));
		list.Add(new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.FOOD_QUALITY, GameUtil.GetFormattedFoodQuality(this.foodInfo.Quality)), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.FOOD_QUALITY, GameUtil.GetFormattedFoodQuality(this.foodInfo.Quality)), Descriptor.DescriptorType.Effect, false));
		foreach (string text in this.foodInfo.Effects)
		{
			list.Add(new Descriptor(Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + text.ToUpper() + ".NAME"), Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + text.ToUpper() + ".DESCRIPTION"), Descriptor.DescriptorType.Effect, false));
		}
		return list;
	}

	public string FoodID;

	private EdiblesManager.FoodInfo foodInfo;

	private float consumptionStartTime = float.NaN;

	public float unitsConsumed = float.NaN;

	public float caloriesConsumed = float.NaN;

	private AttributeModifier caloriesModifier = new AttributeModifier("CaloriesDelta", 50000f, DUPLICANTS.MODIFIERS.EATINGCALORIES.NAME, false, false);

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

	public class EdibleStartWorkInfo : Worker.StartWorkInfo
	{
		public EdibleStartWorkInfo(Workable workable, float amount)
			: base(workable)
		{
			this.amount = amount;
		}

		public float amount { get; private set; }
	}
}
