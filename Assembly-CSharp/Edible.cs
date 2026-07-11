using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class Edible : Workable, IGameObjectEffectDescriptor, IHasSortOrder
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

	public int sortOrder { get; set; }

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (this.foodInfo == null)
		{
			if (this.FoodID == null)
			{
				Output.LogError(new object[] { "No food FoodID" });
			}
			this.foodInfo = Game.Instance.ediblesManager.GetFoodInfo(this.FoodID);
		}
		base.GetComponent<KPrefabID>().AddTag(GameTags.Edible);
		base.Subscribe<Edible>(748399584, Edible.OnCraftDelegate);
		base.Subscribe<Edible>(1272413801, Edible.OnCraftDelegate);
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Eating;
		this.synchronizeAnims = false;
		Components.Edibles.Add(this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().MiscStatusItems.Edible, this);
	}

	public override HashedString[] GetWorkAnims(Worker worker)
	{
		MinionResume component = worker.GetComponent<MinionResume>();
		if (component != null && component.CurrentRole != "NoRole")
		{
			return Edible.hatWorkAnims;
		}
		return Edible.normalWorkAnims;
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
		worker.GetAttributes().Add(this.caloriesModifier);
		KPrefabID component = worker.GetComponent<KPrefabID>();
		component.AddTag(GameTags.AlwaysConverse);
		this.StartConsuming();
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
	}

	protected override void OnStopWork(Worker worker)
	{
		worker.GetAttributes().Remove(this.caloriesModifier);
		KPrefabID component = worker.GetComponent<KPrefabID>();
		component.RemoveTag(GameTags.AlwaysConverse);
		this.StopConsuming(worker);
	}

	private void StartConsuming()
	{
		this.consumptionStartTime = Time.time;
		base.worker.Trigger(1406130139, this);
	}

	private void StopConsuming(Worker worker)
	{
		if (float.IsNaN(this.consumptionStartTime))
		{
			KCrashReporter.Assert(false, "How did stop consuming get called twice?");
			return;
		}
		PrimaryElement component = base.gameObject.GetComponent<PrimaryElement>();
		if (component != null && component.DiseaseCount > 0)
		{
			new EmoteChore(worker.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, "anim_react_contaminated_food_kanim", new HashedString[] { "react" }, null);
		}
		float num = Time.time - this.consumptionStartTime;
		float num2 = Mathf.Clamp01(num / this.GetFeedingTime(worker));
		this.unitsConsumed = this.Units * num2;
		if (float.IsNaN(this.unitsConsumed))
		{
			KCrashReporter.Assert(false, "Why is unitsConsumed NaN?");
			this.unitsConsumed = this.Units;
		}
		this.caloriesConsumed = this.unitsConsumed * this.foodInfo.CaloriesPerUnit;
		this.Units -= this.unitsConsumed;
		for (int i = 0; i < this.foodInfo.Effects.Count; i++)
		{
			worker.GetComponent<Effects>().Add(this.foodInfo.Effects[i], true);
		}
		ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, -this.caloriesConsumed, StringFormatter.Replace(UI.ENDOFDAYREPORT.NOTES.EATEN, "{0}", this.GetProperName()), worker.GetProperName());
		this.AddQualityEffects(worker);
		worker.Trigger(1121894420, this);
		base.Trigger(-10536414, worker.gameObject);
		this.unitsConsumed = float.NaN;
		this.caloriesConsumed = float.NaN;
		this.consumptionStartTime = float.NaN;
		if (this.Units <= 0f)
		{
			base.gameObject.DeleteObject();
		}
	}

	public static string GetEffectForFoodQuality(int qualityLevel)
	{
		qualityLevel = Mathf.Clamp(qualityLevel, -1, 5);
		return Edible.qualityEffects[qualityLevel];
	}

	private void AddQualityEffects(Worker worker)
	{
		Attributes attributes = worker.GetAttributes();
		AttributeInstance attributeInstance = attributes.Add(Db.Get().Attributes.FoodExpectation);
		float totalValue = attributeInstance.GetTotalValue();
		int num = Mathf.RoundToInt(totalValue);
		int num2 = this.FoodInfo.Quality + num;
		Effects component = worker.GetComponent<Effects>();
		component.Add(Edible.GetEffectForFoodQuality(num2), true);
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

	private AttributeModifier caloriesModifier = new AttributeModifier("CaloriesDelta", 50000f, DUPLICANTS.MODIFIERS.EATINGCALORIES.NAME, false, false, true);

	private static readonly EventSystem.IntraObjectHandler<Edible> OnCraftDelegate = new EventSystem.IntraObjectHandler<Edible>(delegate(Edible component, object data)
	{
		component.OnCraft(data);
	});

	private static readonly HashedString[] normalWorkAnims = new HashedString[] { "working_pre", "working_loop" };

	private static readonly HashedString[] hatWorkAnims = new HashedString[] { "hat_pre", "working_loop" };

	private static readonly HashedString normalWorkPstAnim = "working_pst";

	private static readonly HashedString hatWorkPstAnim = "hat_pst";

	private static Dictionary<int, string> qualityEffects = new Dictionary<int, string>
	{
		{ -1, "EdibleMinus3" },
		{ 0, "EdibleMinus2" },
		{ 1, "EdibleMinus1" },
		{ 2, "Edible0" },
		{ 3, "Edible1" },
		{ 4, "Edible2" },
		{ 5, "Edible3" }
	};

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
