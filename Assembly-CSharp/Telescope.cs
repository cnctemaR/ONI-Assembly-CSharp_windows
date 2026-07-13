using System;
using System.Collections.Generic;
using Database;
using Klei;
using STRINGS;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Telescope")]
public class Telescope : Workable, OxygenBreather.IGasProvider, ISim200ms, BuildingStatusItems.ISkyVisInfo
{
	float BuildingStatusItems.ISkyVisInfo.GetPercentVisible01()
	{
		return this.percentClear;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.attributeConverter = Db.Get().AttributeConverters.ResearchSpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.ALL_DAY_EXPERIENCE;
		this.skillExperienceSkillGroup = Db.Get().SkillGroups.Research.Id;
		this.skillExperienceMultiplier = SKILLS.ALL_DAY_EXPERIENCE;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		SpacecraftManager.instance.Subscribe(532901469, new Action<object>(this.UpdateWorkingState));
		Components.Telescopes.Add(this);
		this.OnWorkableEventCB = (Action<Workable, Workable.WorkableEvent>)Delegate.Combine(this.OnWorkableEventCB, new Action<Workable, Workable.WorkableEvent>(this.OnWorkableEvent));
		this.operational = base.GetComponent<Operational>();
		this.storage = base.GetComponent<Storage>();
		this.UpdateWorkingState(null);
	}

	protected override void OnCleanUp()
	{
		Components.Telescopes.Remove(this);
		SpacecraftManager.instance.Unsubscribe(532901469, new Action<object>(this.UpdateWorkingState));
		base.OnCleanUp();
	}

	public void Sim200ms(float dt)
	{
		base.GetComponent<Building>().GetExtents();
		ValueTuple<bool, float> visibilityOf = TelescopeConfig.SKY_VISIBILITY_INFO.GetVisibilityOf(base.gameObject);
		bool item = visibilityOf.Item1;
		float item2 = visibilityOf.Item2;
		this.percentClear = item2;
		KSelectable component = base.GetComponent<KSelectable>();
		component.ToggleStatusItem(Db.Get().BuildingStatusItems.SkyVisNone, !item, this);
		component.ToggleStatusItem(Db.Get().BuildingStatusItems.SkyVisLimited, item && item2 < 1f, this);
		Operational component2 = base.GetComponent<Operational>();
		component2.SetFlag(Telescope.visibleSkyFlag, item);
		if (!component2.IsActive && component2.IsOperational && this.chore == null)
		{
			this.chore = this.CreateChore();
			base.SetWorkTime(float.PositiveInfinity);
		}
	}

	private void OnWorkableEvent(Workable workable, Workable.WorkableEvent ev)
	{
		WorkerBase worker = base.worker;
		if (worker == null)
		{
			return;
		}
		OxygenBreather component = worker.GetComponent<OxygenBreather>();
		KPrefabID component2 = worker.GetComponent<KPrefabID>();
		KSelectable component3 = base.GetComponent<KSelectable>();
		if (ev == Workable.WorkableEvent.WorkStarted)
		{
			base.ShowProgressBar(true);
			this.progressBar.SetUpdateFunc(delegate
			{
				if (SpacecraftManager.instance.HasAnalysisTarget())
				{
					return SpacecraftManager.instance.GetDestinationAnalysisScore(SpacecraftManager.instance.GetStarmapAnalysisDestinationID()) / (float)ROCKETRY.DESTINATION_ANALYSIS.COMPLETE;
				}
				return 0f;
			});
			if (component != null)
			{
				component.AddGasProvider(this);
			}
			worker.GetComponent<CreatureSimTemperatureTransfer>().enabled = false;
			component2.AddTag(GameTags.Shaded, false);
			component3.AddStatusItem(Db.Get().BuildingStatusItems.TelescopeWorking, this);
			return;
		}
		if (ev != Workable.WorkableEvent.WorkStopped)
		{
			return;
		}
		if (component != null)
		{
			component.RemoveGasProvider(this);
		}
		worker.GetComponent<CreatureSimTemperatureTransfer>().enabled = true;
		base.ShowProgressBar(false);
		component2.RemoveTag(GameTags.Shaded);
		component3.AddStatusItem(Db.Get().BuildingStatusItems.TelescopeWorking, this);
	}

	public override float GetEfficiencyMultiplier(WorkerBase worker)
	{
		return base.GetEfficiencyMultiplier(worker) * Mathf.Clamp01(this.percentClear);
	}

	protected override bool OnWorkTick(WorkerBase worker, float dt)
	{
		if (SpacecraftManager.instance.HasAnalysisTarget())
		{
			int starmapAnalysisDestinationID = SpacecraftManager.instance.GetStarmapAnalysisDestinationID();
			SpaceDestination destination = SpacecraftManager.instance.GetDestination(starmapAnalysisDestinationID);
			float num = 1f / (float)destination.OneBasedDistance;
			float num2 = (float)ROCKETRY.DESTINATION_ANALYSIS.DISCOVERED;
			float default_CYCLES_PER_DISCOVERY = ROCKETRY.DESTINATION_ANALYSIS.DEFAULT_CYCLES_PER_DISCOVERY;
			float num3 = num2 / default_CYCLES_PER_DISCOVERY / 600f;
			float num4 = dt * num * num3;
			SpacecraftManager.instance.EarnDestinationAnalysisPoints(starmapAnalysisDestinationID, num4);
		}
		return base.OnWorkTick(worker, dt);
	}

	public override List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> descriptors = base.GetDescriptors(go);
		Element element = ElementLoader.FindElementByHash(SimHashes.Oxygen);
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(element.tag.ProperName(), string.Format(global::STRINGS.BUILDINGS.PREFABS.TELESCOPE.REQUIREMENT_TOOLTIP, element.tag.ProperName()), Descriptor.DescriptorType.Requirement);
		descriptors.Add(descriptor);
		return descriptors;
	}

	protected Chore CreateChore()
	{
		WorkChore<Telescope> workChore = new WorkChore<Telescope>(Db.Get().ChoreTypes.Research, this, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
		workChore.AddPrecondition(Telescope.ContainsOxygen, null);
		return workChore;
	}

	protected void UpdateWorkingState(object _)
	{
		bool flag = false;
		if (SpacecraftManager.instance.HasAnalysisTarget() && SpacecraftManager.instance.GetDestinationAnalysisState(SpacecraftManager.instance.GetDestination(SpacecraftManager.instance.GetStarmapAnalysisDestinationID())) != SpacecraftManager.DestinationAnalysisState.Complete)
		{
			flag = true;
		}
		KSelectable component = base.GetComponent<KSelectable>();
		bool flag2 = !flag && !SpacecraftManager.instance.AreAllDestinationsAnalyzed();
		component.ToggleStatusItem(Db.Get().BuildingStatusItems.NoApplicableAnalysisSelected, flag2, null);
		this.operational.SetFlag(Telescope.flag, flag);
		if (!flag && base.worker)
		{
			base.StopWork(base.worker, true);
		}
	}

	public void OnSetOxygenBreather(OxygenBreather oxygen_breather)
	{
	}

	public void OnClearOxygenBreather(OxygenBreather oxygen_breather)
	{
	}

	public bool ShouldEmitCO2()
	{
		return false;
	}

	public bool ShouldStoreCO2()
	{
		return false;
	}

	public bool ConsumeGas(OxygenBreather oxygen_breather, float amount)
	{
		if (this.storage.items.Count <= 0)
		{
			return false;
		}
		GameObject gameObject = this.storage.items[0];
		if (gameObject == null)
		{
			return false;
		}
		float mass = gameObject.GetComponent<PrimaryElement>().Mass;
		float num = 0f;
		float num2 = 0f;
		SimHashes simHashes = SimHashes.Vacuum;
		SimUtil.DiseaseInfo diseaseInfo;
		this.storage.ConsumeAndGetDisease(GameTags.Breathable, amount, out num, out diseaseInfo, out num2, out simHashes);
		bool flag = num >= amount;
		OxygenBreather.BreathableGasConsumed(oxygen_breather, simHashes, num, num2, diseaseInfo.idx, diseaseInfo.count);
		return flag;
	}

	public bool IsLowOxygen()
	{
		if (this.storage.items.Count <= 0)
		{
			return true;
		}
		PrimaryElement primaryElement = this.storage.FindFirstWithMass(GameTags.Breathable, 0f);
		return primaryElement == null || primaryElement.Mass == 0f;
	}

	public bool HasOxygen()
	{
		if (this.storage.items.Count <= 0)
		{
			return true;
		}
		PrimaryElement primaryElement = this.storage.FindFirstWithMass(GameTags.Breathable, 0f);
		return primaryElement != null && primaryElement.Mass > 0f;
	}

	public bool IsBlocked()
	{
		return false;
	}

	private Operational operational;

	private float percentClear;

	private static readonly Operational.Flag visibleSkyFlag = new Operational.Flag("VisibleSky", Operational.Flag.Type.Requirement);

	private Storage storage;

	public static readonly Chore.Precondition ContainsOxygen = new Chore.Precondition
	{
		id = "ContainsOxygen",
		sortOrder = 1,
		description = DUPLICANTS.CHORES.PRECONDITIONS.CONTAINS_OXYGEN,
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return context.chore.target.GetComponent<Storage>().FindFirstWithMass(GameTags.Oxygen, 0f) != null;
		}
	};

	private Chore chore;

	private static readonly Operational.Flag flag = new Operational.Flag("ValidTarget", Operational.Flag.Type.Requirement);
}
