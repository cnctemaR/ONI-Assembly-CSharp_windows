using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class Telescope : KMonoBehaviour, ISim1000ms, OxygenBreather.IGasProvider, IEffectDescriptor
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (Telescope.reducedVisibilityStatusItem == null)
		{
			Telescope.reducedVisibilityStatusItem = new StatusItem("SPACE_VISIBILITY_REDUCED", "BUILDING", "status_item_no_sky", StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, true, 63486);
			Telescope.reducedVisibilityStatusItem.resolveStringCallback = new Func<string, object, string>(Telescope.GetStatusItemString);
			Telescope.noVisibilityStatusItem = new StatusItem("SPACE_VISIBILITY_NONE", "BUILDING", "status_item_no_sky", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, true, 63486);
			Telescope.noVisibilityStatusItem.resolveStringCallback = new Func<string, object, string>(Telescope.GetStatusItemString);
		}
		ResearchCenter component = base.GetComponent<ResearchCenter>();
		ResearchCenter researchCenter = component;
		researchCenter.OnWorkableEventCB = (Action<Workable.WorkableEvent>)Delegate.Combine(researchCenter.OnWorkableEventCB, new Action<Workable.WorkableEvent>(this.OnWorkableEvent));
		ResearchCenter researchCenter2 = component;
		researchCenter2.onCreateChore = (Action<Chore>)Delegate.Combine(researchCenter2.onCreateChore, new Action<Chore>(Telescope.OnCreateChore));
	}

	public void Sim1000ms(float dt)
	{
		Building component = base.GetComponent<Building>();
		Extents extents = component.GetExtents();
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
		Operational component2 = base.GetComponent<Operational>();
		component2.SetFlag(Telescope.visibleSkyFlag, num7 > 0);
		bool flag = num7 < num4;
		KSelectable component3 = base.GetComponent<KSelectable>();
		if (num7 > 0)
		{
			component3.ToggleStatusItem(Telescope.noVisibilityStatusItem, false, null);
			component3.ToggleStatusItem(Telescope.reducedVisibilityStatusItem, flag, this);
		}
		else
		{
			component3.ToggleStatusItem(Telescope.noVisibilityStatusItem, true, this);
			component3.ToggleStatusItem(Telescope.reducedVisibilityStatusItem, false, null);
		}
		this.percentClear = (float)num7 / (float)num4;
		ResearchCenter component4 = base.GetComponent<ResearchCenter>();
		component4.Effectiveness = this.percentClear;
	}

	private static string GetStatusItemString(string src_str, object data)
	{
		Telescope telescope = (Telescope)data;
		string text = src_str.Replace("{VISIBILITY}", GameUtil.GetFormattedPercent(telescope.percentClear * 100f, GameUtil.TimeSlice.None));
		return text.Replace("{RADIUS}", telescope.clearScanCellRadius.ToString());
	}

	private void OnWorkableEvent(Workable.WorkableEvent ev)
	{
		ResearchCenter component = base.GetComponent<ResearchCenter>();
		if (component == null || component.worker == null)
		{
			return;
		}
		OxygenBreather component2 = component.worker.GetComponent<OxygenBreather>();
		if (ev != Workable.WorkableEvent.WorkStarted)
		{
			if (ev == Workable.WorkableEvent.WorkStopped)
			{
				component2.SetGasProvider(this.workerGasProvider);
				component2.GetComponent<CreatureSimTemperatureTransfer>().enabled = true;
			}
		}
		else
		{
			this.workerGasProvider = component2.GetGasProvider();
			component2.SetGasProvider(this);
			component2.GetComponent<CreatureSimTemperatureTransfer>().enabled = false;
		}
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor descriptor = default(Descriptor);
		Element element = ElementLoader.FindElementByHash(SimHashes.Oxygen);
		descriptor.SetupDescriptor(element.tag.ProperName(), string.Format(BUILDINGS.PREFABS.TELESCOPE.REQUIREMENT_TOOLTIP, element.tag.ProperName()), Descriptor.DescriptorType.Requirement);
		list.Add(descriptor);
		return list;
	}

	public static void OnCreateChore(Chore chore)
	{
		chore.AddPrecondition(Telescope.ContainsOxygen, null);
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

	private float percentClear;

	private static readonly Operational.Flag visibleSkyFlag = new Operational.Flag("VisibleSky", Operational.Flag.Type.Requirement);

	private static StatusItem reducedVisibilityStatusItem;

	private static StatusItem noVisibilityStatusItem;

	[MyCmpGet]
	private Storage storage;

	public static readonly Chore.Precondition ContainsOxygen = new Chore.Precondition
	{
		id = "ContainsOxygen",
		sortOrder = 1,
		description = DUPLICANTS.CHORES.PRECONDITIONS.CONTAINS_OXYGEN,
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			Storage component = context.chore.target.GetComponent<Storage>();
			PrimaryElement primaryElement = component.FindFirstWithMass(GameTags.Oxygen);
			return primaryElement != null;
		}
	};
}
