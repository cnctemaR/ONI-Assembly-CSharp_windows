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

	public List<SpiceInstance> Spices
	{
		get
		{
			return this.spices;
		}
	}

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
		if (this.kpid.HasTag(GameTags.Rehydrated))
		{
			base.GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.RehydratedFood, null);
		}
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().MiscStatusItems.Edible, this);
	}

	public override HashedString[] GetWorkAnims(WorkerBase worker)
	{
		return null;
	}

	public override HashedString[] GetWorkPstAnims(WorkerBase worker, bool successfully_completed)
	{
		if (this.workerSnapshot.hasHat)
		{
			if (!this.workerSnapshot.useSalt)
			{
				return Edible.hatWorkPstAnim;
			}
			return Edible.saltHatWorkPstAnim;
		}
		else
		{
			if (!this.workerSnapshot.useSalt)
			{
				return Edible.normalWorkPstAnim;
			}
			return Edible.saltWorkPstAnim;
		}
	}

	private void OnCraft(object data)
	{
		WorldResourceAmountTracker<RationTracker>.Get().RegisterAmountProduced(this.Calories);
	}

	public float GetFeedingTime(WorkerBase worker)
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

	protected override void OnStartWork(WorkerBase worker)
	{
		this.totalFeedingTime = this.GetFeedingTime(worker);
		base.SetWorkTime(this.totalFeedingTime);
		this.caloriesConsumed = 0f;
		this.unitsConsumed = 0f;
		this.totalUnits = this.Units;
		this.kpid.AddTag(GameTags.AlwaysConverse, false);
		this.totalConsumableCalories = this.Units * this.foodInfo.CaloriesPerUnit;
		this.workerState = Edible.WorkerState.Irrelevant;
		EatChore.StatesInstance smi = worker.GetSMI<EatChore.StatesInstance>();
		this.workerSnapshot.convoAnims = Edible.convoAnims;
		this.workerSnapshot.useSalt = smi != null && smi.UseSalt();
		MinionResume minionResume;
		this.workerSnapshot.hasHat = worker.TryGetComponent<MinionResume>(out minionResume) && minionResume.CurrentHat != null;
		if (this.workerSnapshot.hasHat)
		{
			if (this.workerSnapshot.useSalt)
			{
				this.workerSnapshot.baseAnims = Edible.saltHatWorkAnims;
			}
			else
			{
				this.workerSnapshot.baseAnims = Edible.hatWorkAnims;
			}
		}
		else if (this.workerSnapshot.useSalt)
		{
			this.workerSnapshot.baseAnims = Edible.saltWorkAnims;
		}
		else
		{
			this.workerSnapshot.baseAnims = Edible.normalWorkAnims;
		}
		worker.GetComponent<KBatchedAnimController>().Stop();
		this.StartConsuming();
	}

	protected override bool OnWorkTick(WorkerBase worker, float dt)
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
		bool flag = this.OnTickConsume(worker, dt);
		if (!flag)
		{
			this.TickAnimation(worker);
		}
		return flag;
	}

	protected override void OnStopWork(WorkerBase worker)
	{
		if (this.currentModifier != null)
		{
			worker.GetAttributes().Remove(this.currentModifier);
			this.currentModifier = null;
		}
		worker.RemoveTag(GameTags.AlwaysConverse);
		this.workerSnapshot = default(Edible.WorkerSnapshot);
		this.workerState = Edible.WorkerState.Irrelevant;
		worker.RemoveTag(GameTags.DoNotInterruptMe);
		KBatchedAnimController component = worker.GetComponent<KBatchedAnimController>();
		component.SetSymbolVisiblity(Edible.SALT_SYMBOL, true);
		component.SetSymbolVisiblity(Edible.HAT_SYMBOL, true);
		this.StopConsuming(worker);
	}

	private bool OnTickConsume(WorkerBase worker, float dt)
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

	private void TickAnimation(WorkerBase worker)
	{
		KBatchedAnimController component = worker.GetComponent<KBatchedAnimController>();
		if (!component.IsStopped())
		{
			return;
		}
		switch (this.workerState)
		{
		case Edible.WorkerState.Irrelevant:
			this.workerState = Edible.WorkerState.EatPre;
			component.Queue(this.workerSnapshot.baseAnims[0], KAnim.PlayMode.Once, 1f, 0f);
			return;
		case Edible.WorkerState.EatPre:
			this.workerState = Edible.WorkerState.EatLoop;
			component.Queue(this.workerSnapshot.baseAnims[1], KAnim.PlayMode.Once, 1f, 0f);
			return;
		case Edible.WorkerState.EatLoop:
		{
			HashedString hashedString;
			if (worker.HasTag(GameTags.CommunalDining) && worker.HasTag(GameTags.WantsToTalk))
			{
				worker.RemoveTag(GameTags.WantsToTalk);
				hashedString = this.workerSnapshot.convoAnims[global::UnityEngine.Random.Range(0, this.workerSnapshot.convoAnims.Length)];
				component.SetSymbolVisiblity(Edible.SALT_SYMBOL, this.workerSnapshot.useSalt);
				component.SetSymbolVisiblity(Edible.HAT_SYMBOL, this.workerSnapshot.hasHat);
			}
			else
			{
				worker.RemoveTag(GameTags.DoNotInterruptMe);
				hashedString = this.workerSnapshot.baseAnims[1];
			}
			component.Queue(hashedString, KAnim.PlayMode.Once, 1f, 0f);
			return;
		}
		default:
			DebugUtil.DevLogError("Unexpected workerState " + this.workerState.ToString());
			return;
		}
	}

	public void SpiceEdible(SpiceInstance spice, StatusItem status)
	{
		this.spices.Add(spice);
		this.ApplySpiceEffects(spice, status);
	}

	protected virtual void ApplySpiceEffects(SpiceInstance spice, StatusItem status)
	{
		this.kpid.AddTag(spice.Id, true);
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
		if (isSpiced)
		{
			this.kpid.RemoveTag(GameTags.UnspicedFood);
			this.kpid.AddTag(GameTags.SpicedFood, true);
			return;
		}
		this.kpid.RemoveTag(GameTags.SpicedFood);
		this.kpid.AddTag(GameTags.UnspicedFood, false);
	}

	public bool CanAbsorb(Edible other)
	{
		bool flag = this.spices.Count == other.spices.Count;
		flag &= base.gameObject.HasTag(GameTags.Rehydrated) == other.gameObject.HasTag(GameTags.Rehydrated);
		flag &= !base.gameObject.HasTag(GameTags.Dehydrated) && !other.gameObject.HasTag(GameTags.Dehydrated);
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

	private void StopConsuming(WorkerBase worker)
	{
		DebugUtil.DevAssert(this.isBeingConsumed, "StopConsuming() called without StartConsuming()", null);
		this.isBeingConsumed = false;
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
		if (this.Units < 0.001f)
		{
			base.gameObject.DeleteObject();
		}
	}

	public static string GetEffectForFoodQuality(int qualityLevel)
	{
		qualityLevel = Mathf.Clamp(qualityLevel, -1, 5);
		return Edible.qualityEffects[qualityLevel];
	}

	private void AddOnConsumeEffects(WorkerBase worker)
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
		if (base.gameObject.HasTag(GameTags.Rehydrated))
		{
			component.Add(FoodRehydratorConfig.RehydrationEffect, true);
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

	public int GetMorale()
	{
		int num = 0;
		string effectForFoodQuality = Edible.GetEffectForFoodQuality(this.foodInfo.Quality);
		foreach (AttributeModifier attributeModifier in Db.Get().effects.Get(effectForFoodQuality).SelfModifiers)
		{
			if (attributeModifier.AttributeId == Db.Get().Attributes.QualityOfLife.Id)
			{
				num += Mathf.RoundToInt(attributeModifier.Value);
			}
		}
		return num;
	}

	public override List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		list.Add(new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.CALORIES, GameUtil.GetFormattedCalories(this.foodInfo.CaloriesPerUnit, GameUtil.TimeSlice.None, true)), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.CALORIES, GameUtil.GetFormattedCalories(this.foodInfo.CaloriesPerUnit, GameUtil.TimeSlice.None, true)), Descriptor.DescriptorType.Information, false));
		list.Add(new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.FOOD_QUALITY, GameUtil.GetFormattedFoodQuality(this.foodInfo.Quality)), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.FOOD_QUALITY, GameUtil.GetFormattedFoodQuality(this.foodInfo.Quality)), Descriptor.DescriptorType.Effect, false));
		int morale = this.GetMorale();
		list.Add(new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.FOOD_MORALE, GameUtil.AddPositiveSign(morale.ToString(), morale > 0)), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.FOOD_MORALE, GameUtil.AddPositiveSign(morale.ToString(), morale > 0)), Descriptor.DescriptorType.Effect, false));
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

	public void ApplySpicesToOtherEdible(Edible other)
	{
		if (this.spices != null && other != null)
		{
			for (int i = 0; i < this.spices.Count; i++)
			{
				other.SpiceEdible(this.spices[i], SpiceGrinderConfig.SpicedStatus);
			}
		}
	}

	public void OnSplitTick(Pickupable thePieceTaken)
	{
		Edible component = thePieceTaken.GetComponent<Edible>();
		this.ApplySpicesToOtherEdible(component);
		if (this.kpid.HasTag(GameTags.Rehydrated))
		{
			component.kpid.AddTag(GameTags.Rehydrated, false);
		}
	}

	[MyCmpReq]
	private readonly KPrefabID kpid;

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

	private AttributeModifier caloriesLitSpaceModifier = new AttributeModifier("CaloriesDelta", (1f + DUPLICANTSTATS.STANDARD.Light.LIGHT_WORK_EFFICIENCY_BONUS) / 2E-05f, DUPLICANTS.MODIFIERS.EATINGCALORIES.NAME, false, true, true);

	private AttributeModifier currentModifier;

	public static readonly HashedString SALT_SYMBOL = "saltshaker_fg";

	public static readonly HashedString HAT_SYMBOL = "hat";

	private Edible.WorkerState workerState;

	private Edible.WorkerSnapshot workerSnapshot;

	private static readonly EventSystem.IntraObjectHandler<Edible> OnCraftDelegate = new EventSystem.IntraObjectHandler<Edible>(delegate(Edible component, object data)
	{
		component.OnCraft(data);
	});

	private static readonly HashedString[] normalWorkAnims = new HashedString[] { "working_pre", "working_loop" };

	private static readonly HashedString[] hatWorkAnims = new HashedString[] { "hat_pre", "working_loop" };

	private static readonly HashedString[] saltWorkAnims = new HashedString[] { "salt_pre", "salt_loop" };

	private static readonly HashedString[] saltHatWorkAnims = new HashedString[] { "salt_hat_pre", "salt_hat_loop" };

	public static readonly HashedString[] convoAnims = new HashedString[] { "convo_loop_01", "convo_loop_02", "convo_loop_03", "convo_loop_04" };

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

	private enum WorkerState
	{
		Irrelevant,
		EatPre,
		EatLoop
	}

	private struct WorkerSnapshot
	{
		public bool useSalt;

		public bool hasHat;

		public HashedString[] baseAnims;

		public HashedString[] convoAnims;
	}

	public class EdibleStartWorkInfo : WorkerBase.StartWorkInfo
	{
		public float amount { get; private set; }

		public EdibleStartWorkInfo(Workable workable, float amount)
			: base(workable)
		{
			this.amount = amount;
		}
	}
}
