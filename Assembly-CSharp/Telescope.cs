using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class Telescope : Workable, OxygenBreather.IGasProvider, IEffectDescriptor, ISim200ms
{
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
		if (Telescope.reducedVisibilityStatusItem == null)
		{
			Telescope.reducedVisibilityStatusItem = new StatusItem("SPACE_VISIBILITY_REDUCED", "BUILDING", "status_item_no_sky", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			Telescope.reducedVisibilityStatusItem.resolveStringCallback = new Func<string, object, string>(Telescope.GetStatusItemString);
			Telescope.noVisibilityStatusItem = new StatusItem("SPACE_VISIBILITY_NONE", "BUILDING", "status_item_no_sky", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			Telescope.noVisibilityStatusItem.resolveStringCallback = new Func<string, object, string>(Telescope.GetStatusItemString);
		}
		this.OnWorkableEventCB = (Action<Workable.WorkableEvent>)Delegate.Combine(this.OnWorkableEventCB, new Action<Workable.WorkableEvent>(this.OnWorkableEvent));
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
		Extents extents = base.GetComponent<Building>().GetExtents();
		int num = Mathf.Max(0, extents.x - this.clearScanCellRadius);
		int num2 = Mathf.Min(new int[] { extents.x + this.clearScanCellRadius });
		int num3 = extents.y + extents.height - 3;
		int num4 = num2 - num + 1;
		int num5 = Grid.XYToCell(num, num3);
		int num6 = Grid.XYToCell(num2, num3);
		int num7 = 0;
		for (int i = num5; i <= num6; i++)
		{
			if (Grid.ExposedToSunlight[i] >= 253)
			{
				num7++;
			}
		}
		Operational component = base.GetComponent<Operational>();
		component.SetFlag(Telescope.visibleSkyFlag, num7 > 0);
		bool flag = num7 < num4;
		KSelectable component2 = base.GetComponent<KSelectable>();
		if (num7 > 0)
		{
			component2.ToggleStatusItem(Telescope.noVisibilityStatusItem, false, null);
			component2.ToggleStatusItem(Telescope.reducedVisibilityStatusItem, flag, this);
		}
		else
		{
			component2.ToggleStatusItem(Telescope.noVisibilityStatusItem, true, this);
			component2.ToggleStatusItem(Telescope.reducedVisibilityStatusItem, false, null);
		}
		this.percentClear = (float)num7 / (float)num4;
		if (!component.IsActive && component.IsOperational && this.chore == null)
		{
			this.chore = this.CreateChore();
			base.SetWorkTime(float.PositiveInfinity);
		}
	}

	private static string GetStatusItemString(string src_str, object data)
	{
		Telescope telescope = (Telescope)data;
		return src_str.Replace("{VISIBILITY}", GameUtil.GetFormattedPercent(telescope.percentClear * 100f, GameUtil.TimeSlice.None)).Replace("{RADIUS}", telescope.clearScanCellRadius.ToString());
	}

	private void OnWorkableEvent(Workable.WorkableEvent ev)
	{
		Worker worker = base.worker;
		if (worker == null)
		{
			return;
		}
		OxygenBreather component = worker.GetComponent<OxygenBreather>();
		KPrefabID component2 = worker.GetComponent<KPrefabID>();
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
			this.workerGasProvider = component.GetGasProvider();
			component.SetGasProvider(this);
			component.GetComponent<CreatureSimTemperatureTransfer>().enabled = false;
			component2.AddTag(GameTags.Shaded, false);
			return;
		}
		if (ev != Workable.WorkableEvent.WorkStopped)
		{
			return;
		}
		component.SetGasProvider(this.workerGasProvider);
		component.GetComponent<CreatureSimTemperatureTransfer>().enabled = true;
		base.ShowProgressBar(false);
		component2.RemoveTag(GameTags.Shaded);
	}

	protected override bool OnWorkTick(Worker worker, float dt)
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

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Element element = ElementLoader.FindElementByHash(SimHashes.Oxygen);
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(element.tag.ProperName(), string.Format(global::STRINGS.BUILDINGS.PREFABS.TELESCOPE.REQUIREMENT_TOOLTIP, element.tag.ProperName()), Descriptor.DescriptorType.Requirement);
		list.Add(descriptor);
		return list;
	}

	protected Chore CreateChore()
	{
		WorkChore<Telescope> workChore = new WorkChore<Telescope>(Db.Get().ChoreTypes.Research, this, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
		workChore.AddPrecondition(Telescope.ContainsOxygen, null);
		return workChore;
	}

	protected void UpdateWorkingState(object data)
	{
		bool flag = false;
		if (SpacecraftManager.instance.HasAnalysisTarget() && SpacecraftManager.instance.GetDestinationAnalysisState(SpacecraftManager.instance.GetDestination(SpacecraftManager.instance.GetStarmapAnalysisDestinationID())) != SpacecraftManager.DestinationAnalysisState.Complete)
		{
			flag = true;
		}
		KSelectable component = base.GetComponent<KSelectable>();
		bool flag2 = !flag && !SpacecraftManager.instance.AreAllDestinationsAnalyzed();
		component.ToggleStatusItem(Db.Get().BuildingStatusItems.NoApplicableAnalysisSelected, flag2, null);
		this.operational.SetFlag(this.flag, flag);
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
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		bool flag = component.Mass >= amount;
		component.Mass = Mathf.Max(0f, component.Mass - amount);
		return flag;
	}

	public int clearScanCellRadius = 15;

	private OxygenBreather.IGasProvider workerGasProvider;

	private Operational operational;

	private float percentClear;

	private static readonly Operational.Flag visibleSkyFlag = new Operational.Flag("VisibleSky", Operational.Flag.Type.Requirement);

	private static StatusItem reducedVisibilityStatusItem;

	private static StatusItem noVisibilityStatusItem;

	private Storage storage;

	public static readonly Chore.Precondition ContainsOxygen = new Chore.Precondition
	{
		id = "ContainsOxygen",
		sortOrder = 1,
		description = DUPLICANTS.CHORES.PRECONDITIONS.CONTAINS_OXYGEN,
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return context.chore.target.GetComponent<Storage>().FindFirstWithMass(GameTags.Oxygen) != null;
		}
	};

	private Chore chore;

	private Operational.Flag flag = new Operational.Flag("ValidTarget", Operational.Flag.Type.Requirement);
}
