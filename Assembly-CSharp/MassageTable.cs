using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

public class MassageTable : RelaxationPoint, IEffectDescriptor, IActivationRangeTarget
{
	public string ActivateTooltip
	{
		get
		{
			return BUILDINGS.PREFABS.MASSAGETABLE.ACTIVATE_TOOLTIP;
		}
	}

	public string DeactivateTooltip
	{
		get
		{
			return BUILDINGS.PREFABS.MASSAGETABLE.DEACTIVATE_TOOLTIP;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<MassageTable>(-905833192, MassageTable.OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		MassageTable component = gameObject.GetComponent<MassageTable>();
		if (component != null)
		{
			this.ActivateValue = component.ActivateValue;
			this.DeactivateValue = component.DeactivateValue;
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		Effects component = worker.GetComponent<Effects>();
		for (int i = 0; i < MassageTable.EffectsRemoved.Length; i++)
		{
			string text = MassageTable.EffectsRemoved[i];
			component.Remove(text);
		}
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
	}

	public new List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.STRESSREDUCEDPERMINUTE, GameUtil.GetFormattedPercent(this.stressModificationValue / 600f * 60f, GameUtil.TimeSlice.None)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.STRESSREDUCEDPERMINUTE, GameUtil.GetFormattedPercent(this.stressModificationValue / 600f * 60f, GameUtil.TimeSlice.None)), Descriptor.DescriptorType.Effect);
		list.Add(descriptor);
		if (MassageTable.EffectsRemoved.Length > 0)
		{
			Descriptor descriptor2 = default(Descriptor);
			descriptor2.SetupDescriptor(UI.BUILDINGEFFECTS.REMOVESEFFECTSUBTITLE, UI.BUILDINGEFFECTS.TOOLTIPS.REMOVESEFFECTSUBTITLE, Descriptor.DescriptorType.Effect);
			list.Add(descriptor2);
			for (int i = 0; i < MassageTable.EffectsRemoved.Length; i++)
			{
				string text = MassageTable.EffectsRemoved[i];
				string text2 = Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + text.ToUpper() + ".NAME");
				string text3 = Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + text.ToUpper() + ".CAUSE");
				Descriptor descriptor3 = default(Descriptor);
				descriptor3.IncreaseIndent();
				descriptor3.SetupDescriptor("• " + string.Format(UI.BUILDINGEFFECTS.REMOVEDEFFECT, text2), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.REMOVEDEFFECT, text3), Descriptor.DescriptorType.Effect);
				list.Add(descriptor3);
			}
		}
		return list;
	}

	protected override WorkChore<RelaxationPoint> CreateWorkChore()
	{
		WorkChore<RelaxationPoint> workChore = new WorkChore<RelaxationPoint>(Db.Get().ChoreTypes.StressHeal, this, null, null, true, null, null, null, false, null, true, true, null, false, true, false, PriorityScreen.PriorityClass.high, 5, false);
		workChore.AddPrecondition(MassageTable.IsStressAboveActivationRange, this);
		return workChore;
	}

	public float ActivateValue
	{
		get
		{
			return this.activateValue;
		}
		set
		{
			this.activateValue = value;
		}
	}

	public float DeactivateValue
	{
		get
		{
			return this.stopStressingValue;
		}
		set
		{
			this.stopStressingValue = value;
		}
	}

	public bool UseWholeNumbers
	{
		get
		{
			return true;
		}
	}

	public float MinValue
	{
		get
		{
			return 0f;
		}
	}

	public float MaxValue
	{
		get
		{
			return 100f;
		}
	}

	public string ActivationRangeTitleText
	{
		get
		{
			return UI.UISIDESCREENS.ACTIVATION_RANGE_SIDE_SCREEN.NAME;
		}
	}

	public string ActivateSliderLabelText
	{
		get
		{
			return UI.UISIDESCREENS.ACTIVATION_RANGE_SIDE_SCREEN.ACTIVATE;
		}
	}

	public string DeactivateSliderLabelText
	{
		get
		{
			return UI.UISIDESCREENS.ACTIVATION_RANGE_SIDE_SCREEN.DEACTIVATE;
		}
	}

	[Serialize]
	private float activateValue = 50f;

	private static readonly string[] EffectsRemoved = new string[] { "SoreBack" };

	private static readonly EventSystem.IntraObjectHandler<MassageTable> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<MassageTable>(delegate(MassageTable component, object data)
	{
		component.OnCopySettings(data);
	});

	private static readonly Chore.Precondition IsStressAboveActivationRange = new Chore.Precondition
	{
		id = "IsStressAboveActivationRange",
		description = DUPLICANTS.CHORES.PRECONDITIONS.IS_STRESS_ABOVE_ACTIVATION_RANGE,
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			IActivationRangeTarget activationRangeTarget = (IActivationRangeTarget)data;
			AmountInstance amountInstance = Db.Get().Amounts.Stress.Lookup(context.consumerState.gameObject);
			float value = amountInstance.value;
			return value >= activationRangeTarget.ActivateValue;
		}
	};
}
