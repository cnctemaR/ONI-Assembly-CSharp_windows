using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Edible")]
public class Edible : Workable, IGameObjectEffectDescriptor, ISaveLoadable, IExtendSplitting
{
	public float Units
	{
		get
		{
			return this.primaryElement.Units;
		}
		set
		{
			this.primaryElement.Units = value;
		}
	}

	public float MassPerUnit
	{
		get
		{
			return this.primaryElement.MassPerUnit;
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

	public bool isBeingConsumed { get; private set; }

	protected override void OnPrefabInit()
	{
		this.primaryElement = base.GetComponent<PrimaryElement>();
		base.SetReportType(ReportManager.ReportType.PersonalTime);
		this.showProgressBar = false;
		base.SetOffsetTable(OffsetGroups.InvertedStandardTable);
		this.shouldTransferDiseaseWithWorker = false;
		base.OnPrefabInit();
		if (this.foodInfo == null)
		{
			if (this.FoodID == null)
			{
				global::Debug.LogError("No food FoodID");
			}
			this.foodInfo = EdiblesManager.GetFoodInfo(this.FoodID);
		}
		base.GetComponent<KPrefabID>().AddTag(GameTags.Edible, false);
		base.Subscribe<Edible>(748399584, Edible.OnCraftDelegate);
		base.Subscribe<Edible>(1272413801, Edible.OnCraftDelegate);
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Eating;
		this.synchronizeAnims = false;
		Components.Edibles.Add(this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.ToggleGenericSpicedTag(base.gameObject.HasTag(GameTags.SpicedFood));
		if (this.spices != null)
		{
			for (int i = 0; i < this.spices.Count; i++)
			{
				this.ApplySpiceEffects(this.spices[i], SpiceGrinderConfig.SpicedStatus);
			}
		}
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().MiscStatusItems.Edible, this);
	}

	public override HashedString[] GetWorkAnims(Worker worker)
	{
		EatChore.StatesInstance smi = worker.GetSMI<EatChore.StatesInstance>();
		bool flag = smi != null && smi.UseSalt();
		MinionResume component = worker.GetComponent<MinionResume>();
		if (component != null && component.CurrentHat != null)
		{
			if (!flag)
			{
				return Edible.hatWorkAnims;
			}
			return Edible.saltHatWorkAnims;
		}
		else
		{
			if (!flag)
			{
				return Edible.normalWorkAnims;
			}
			return Edible.saltWorkAnims;
		}
	}

	public override HashedString[] GetWorkPstAnims(Worker worker, bool successfully_completed)
	{
		EatChore.StatesInstance smi = worker.GetSMI<EatChore.StatesInstance>();
		bool flag = smi != null && smi.UseSalt();
		MinionResume component = worker.GetComponent<MinionResume>();
		if (component != null && component.CurrentHat != null)
		{
			if (!flag)
			{
				return Edible.hatWorkPstAnim;
			}
			return Edible.saltHatWorkPstAnim;
		}
		else
		{
			if (!flag)
			{
				return Edible.normalWorkPstAnim;
			}
			return Edible.saltWorkPstAnim;
		}
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
		this.totalFeedingTime = this.GetFeedingTime(worker);
		base.SetWorkTime(this.totalFeedingTime);
		this.caloriesConsumed = 0f;
		this.unitsConsumed = 0f;
		this.totalUnits = this.Units;
		worker.GetComponent<KPrefabID>().AddTag(GameTags.AlwaysConverse, false);
		this.totalConsumableCalories = this.Units * this.foodInfo.CaloriesPerUnit;
		this.StartConsuming();
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		if (this.currentlyLit)
		{
			if (this.currentModifier != this.caloriesLitSpaceModifier)
			{
				worker.GetAttributes().Remove(this.currentModifier);
				worker.GetAttributes().Add(this.caloriesLitSpaceModifier);
				this.currentModifier = this.caloriesLitSpaceModifier;
			}
		}
		else if (this.currentModifier != this.caloriesModifier)
		{
			worker.GetAttributes().Remove(this.currentModifier);
			worker.GetAttributes().Add(this.caloriesModifier);
			this.currentModifier = this.caloriesModifier;
		}
		return this.OnTickConsume(worker, dt);
	}

	protected override void OnStopWork(Worker worker)
	{
		if (this.currentModifier != null)
		{
			worker.GetAttributes().Remove(this.currentModifier);
			this.currentModifier = null;
		}
		worker.GetComponent<KPrefabID>().RemoveTag(GameTags.AlwaysConverse);
		this.StopConsuming(worker);
	}

	private bool OnTickConsume(Worker worker, float dt)
	{
		if (!this.isBeingConsumed)
		{
			DebugUtil.DevLogError("OnTickConsume while we're not eating, this would set a NaN mass on this Edible");
			return true;
		}
		bool flag = false;
		float num = dt / this.totalFeedingTime;
		float num2 = num * this.totalConsumableCalories;
		if (this.caloriesConsumed + num2 > this.totalConsumableCalories)
		{
			num2 = this.totalConsumableCalories - this.caloriesConsumed;
		}
		this.caloriesConsumed += num2;
		worker.GetAmounts().Get("Calories").value += num2;
		float num3 = this.totalUnits * num;
		if (this.Units - num3 < 0f)
		{
			num3 = this.Units;
		}
		this.Units -= num3;
		this.unitsConsumed += num3;
		if (this.Units <= 0f)
		{
			flag = true;
		}
		return flag;
	}

	public void SpiceEdible(SpiceInstance spice, StatusItem status)
	{
		this.spices.Add(spice);
		this.ApplySpiceEffects(spice, status);
	}

	protected virtual void ApplySpiceEffects(SpiceInstance spice, StatusItem status)
	{
		base.GetComponent<KPrefabID>().AddTag(spice.Id, true);
		this.ToggleGenericSpicedTag(true);
		base.GetComponent<KSelectable>().AddStatusItem(status, this.spices);
		if (spice.FoodModifier != null)
		{
			base.gameObject.GetAttributes().Add(spice.FoodModifier);
		}
		if (spice.CalorieModifier != null)
		{
			this.Calories += spice.CalorieModifier.Value;
		}
	}

	private void ToggleGenericSpicedTag(bool isSpiced)
	{
		KPrefabID component = base.GetComponent<KPrefabID>();
		if (isSpiced)
		{
			component.RemoveTag(GameTags.UnspicedFood);
			component.AddTag(GameTags.SpicedFood, true);
			return;
		}
		component.RemoveTag(GameTags.SpicedFood);
		component.AddTag(GameTags.UnspicedFood, false);
	}

	public bool CanAbsorb(Edible other)
	{
		bool flag = this.spices.Count == other.spices.Count;
		int num = 0;
		while (flag && num < this.spices.Count)
		{
			int num2 = 0;
			while (flag && num2 < other.spices.Count)
			{
				flag = this.spices[num].Id == other.spices[num2].Id;
				num2++;
			}
			num++;
		}
		return flag;
	}

	private void StartConsuming()
	{
		DebugUtil.DevAssert(!this.isBeingConsumed, "Can't StartConsuming()...we've already started", null);
		this.isBeingConsumed = true;
		base.worker.Trigger(1406130139, this);
	}

	private void StopConsuming(Worker worker)
	{
		DebugUtil.DevAssert(this.isBeingConsumed, "StopConsuming() called without StartConsuming()", null);
		this.isBeingConsumed = false;
		if (this.primaryElement != null && this.primaryElement.DiseaseCount > 0)
		{
			new EmoteChore(worker.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, Db.Get().Emotes.Minion.FoodPoisoning, 1, null);
		}
		for (int i = 0; i < this.foodInfo.Effects.Count; i++)
		{
			worker.GetComponent<Effects>().Add(this.foodInfo.Effects[i], true);
		}
		ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, -this.caloriesConsumed, StringFormatter.Replace(UI.ENDOFDAYREPORT.NOTES.EATEN, "{0}", this.GetProperName()), worker.GetProperName());
		this.AddOnConsumeEffects(worker);
		worker.Trigger(1121894420, this);
		base.Trigger(-10536414, worker.gameObject);
		this.unitsConsumed = float.NaN;
		this.caloriesConsumed = float.NaN;
		this.totalUnits = float.NaN;
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

	private void AddOnConsumeEffects(Worker worker)
	{
		int num = Mathf.RoundToInt(worker.GetAttributes().Add(Db.Get().Attributes.FoodExpectation).GetTotalValue());
		int num2 = this.FoodInfo.Quality + num;
		Effects component = worker.GetComponent<Effects>();
		component.Add(Edible.GetEffectForFoodQuality(num2), true);
		for (int i = 0; i < this.spices.Count; i++)
		{
			Effect statBonus = this.spices[i].StatBonus;
			if (statBonus != null)
			{
				float duration = statBonus.duration;
				statBonus.duration = this.caloriesConsumed * 0.001f / 1000f * 600f;
				component.Add(statBonus, true);
				statBonus.duration = duration;
			}
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
		list.Add(new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.CALORIES, GameUtil.GetFormattedCalories(this.foodInfo.CaloriesPerUnit, GameUtil.TimeSlice.None, true)), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.CALORIES, GameUtil.GetFormattedCalories(this.foodInfo.CaloriesPerUnit, GameUtil.TimeSlice.None, true)), Descriptor.DescriptorType.Information, false));
		list.Add(new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.FOOD_QUALITY, GameUtil.GetFormattedFoodQuality(this.foodInfo.Quality)), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.FOOD_QUALITY, GameUtil.GetFormattedFoodQuality(this.foodInfo.Quality)), Descriptor.DescriptorType.Effect, false));
		foreach (string text in this.foodInfo.Effects)
		{
			string text2 = "";
			foreach (AttributeModifier attributeModifier in Db.Get().effects.Get(text).SelfModifiers)
			{
				text2 = string.Concat(new string[]
				{
					text2,
					"\n    • ",
					Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + attributeModifier.AttributeId.ToUpper() + ".NAME"),
					": ",
					attributeModifier.GetFormattedString()
				});
			}
			list.Add(new Descriptor(Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + text.ToUpper() + ".NAME"), Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + text.ToUpper() + ".DESCRIPTION") + text2, Descriptor.DescriptorType.Effect, false));
		}
		return list;
	}

	public void OnSplitTick(Pickupable thePieceTaken)
	{
		Edible component = thePieceTaken.GetComponent<Edible>();
		if (this.spices != null)
		{
			for (int i = 0; i < this.spices.Count; i++)
			{
				SpiceInstance spiceInstance = this.spices[i];
				component.SpiceEdible(spiceInstance, SpiceGrinderConfig.SpicedStatus);
			}
		}
	}

	private PrimaryElement primaryElement;

	public string FoodID;

	private EdiblesManager.FoodInfo foodInfo;

	public float unitsConsumed = float.NaN;

	public float caloriesConsumed = float.NaN;

	private float totalFeedingTime = float.NaN;

	private float totalUnits = float.NaN;

	private float totalConsumableCalories = float.NaN;

	[Serialize]
	private List<SpiceInstance> spices = new List<SpiceInstance>();

	private AttributeModifier caloriesModifier = new AttributeModifier("CaloriesDelta", 50000f, DUPLICANTS.MODIFIERS.EATINGCALORIES.NAME, false, true, true);

	private AttributeModifier caloriesLitSpaceModifier = new AttributeModifier("CaloriesDelta", (1f + DUPLICANTSTATS.LIGHT.LIGHT_WORK_EFFICIENCY_BONUS) / 2E-05f, DUPLICANTS.MODIFIERS.EATINGCALORIES.NAME, false, true, true);

	private AttributeModifier currentModifier;

	private static readonly EventSystem.IntraObjectHandler<Edible> OnCraftDelegate = new EventSystem.IntraObjectHandler<Edible>(delegate(Edible component, object data)
	{
		component.OnCraft(data);
	});

	private static readonly HashedString[] normalWorkAnims = new HashedString[] { "working_pre", "working_loop" };

	private static readonly HashedString[] hatWorkAnims = new HashedString[] { "hat_pre", "working_loop" };

	private static readonly HashedString[] saltWorkAnims = new HashedString[] { "salt_pre", "salt_loop" };

	private static readonly HashedString[] saltHatWorkAnims = new HashedString[] { "salt_hat_pre", "salt_hat_loop" };

	private static readonly HashedString[] normalWorkPstAnim = new HashedString[] { "working_pst" };

	private static readonly HashedString[] hatWorkPstAnim = new HashedString[] { "hat_pst" };

	private static readonly HashedString[] saltWorkPstAnim = new HashedString[] { "salt_pst" };

	private static readonly HashedString[] saltHatWorkPstAnim = new HashedString[] { "salt_hat_pst" };

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
		public float amount { get; private set; }

		public EdibleStartWorkInfo(Workable workable, float amount)
			: base(workable)
		{
			this.amount = amount;
		}
	}
}
