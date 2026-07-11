using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/RelaxationPoint")]
public class RelaxationPoint : Workable, IEffectDescriptor
{
	public RelaxationPoint()
	{
		base.SetReportType(ReportManager.ReportType.PersonalTime);
		this.showProgressBar = false;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.lightEfficiencyBonus = false;
		base.GetComponent<KPrefabID>().AddTag(TagManager.Create("RelaxationPoint", MISC.TAGS.RELAXATION_POINT), false);
		if (RelaxationPoint.stressReductionEffect == null)
		{
			RelaxationPoint.stressReductionEffect = this.CreateEffect();
			RelaxationPoint.roomStressReductionEffect = this.CreateRoomEffect();
		}
	}

	public Effect CreateEffect()
	{
		Effect effect = new Effect("StressReduction", DUPLICANTS.MODIFIERS.STRESSREDUCTION.NAME, DUPLICANTS.MODIFIERS.STRESSREDUCTION.TOOLTIP, 0f, true, false, false, null, 0f, null);
		AttributeModifier attributeModifier = new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, this.stressModificationValue / 600f, DUPLICANTS.MODIFIERS.STRESSREDUCTION.NAME, false, false, true);
		effect.Add(attributeModifier);
		return effect;
	}

	public Effect CreateRoomEffect()
	{
		Effect effect = new Effect("RoomRelaxationEffect", DUPLICANTS.MODIFIERS.STRESSREDUCTION_CLINIC.NAME, DUPLICANTS.MODIFIERS.STRESSREDUCTION_CLINIC.TOOLTIP, 0f, true, false, false, null, 0f, null);
		AttributeModifier attributeModifier = new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, this.roomStressModificationValue / 600f, DUPLICANTS.MODIFIERS.STRESSREDUCTION_CLINIC.NAME, false, false, true);
		effect.Add(attributeModifier);
		return effect;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.smi = new RelaxationPoint.RelaxationPointSM.Instance(this);
		this.smi.StartSM();
		base.SetWorkTime(float.PositiveInfinity);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		if (this.roomTracker != null && this.roomTracker.room != null && this.roomTracker.room.roomType == Db.Get().RoomTypes.MassageClinic)
		{
			worker.GetComponent<Effects>().Add(RelaxationPoint.roomStressReductionEffect, false);
		}
		else
		{
			worker.GetComponent<Effects>().Add(RelaxationPoint.stressReductionEffect, false);
		}
		base.GetComponent<Operational>().SetActive(true, false);
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		if (Db.Get().Amounts.Stress.Lookup(worker.gameObject).value <= this.stopStressingValue)
		{
			return true;
		}
		base.OnWorkTick(worker, dt);
		return false;
	}

	protected override void OnStopWork(Worker worker)
	{
		worker.GetComponent<Effects>().Remove(RelaxationPoint.stressReductionEffect);
		worker.GetComponent<Effects>().Remove(RelaxationPoint.roomStressReductionEffect);
		base.GetComponent<Operational>().SetActive(false, false);
		base.OnStopWork(worker);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
	}

	public override bool InstantlyFinish(Worker worker)
	{
		return false;
	}

	protected virtual WorkChore<RelaxationPoint> CreateWorkChore()
	{
		return new WorkChore<RelaxationPoint>(Db.Get().ChoreTypes.Relax, this, null, false, null, null, null, false, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.STRESSREDUCEDPERMINUTE, GameUtil.GetFormattedPercent(this.stressModificationValue / 600f * 60f, GameUtil.TimeSlice.None)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.STRESSREDUCEDPERMINUTE, GameUtil.GetFormattedPercent(this.stressModificationValue / 600f * 60f, GameUtil.TimeSlice.None)), Descriptor.DescriptorType.Effect);
		list.Add(descriptor);
		return list;
	}

	[MyCmpGet]
	private RoomTracker roomTracker;

	[Serialize]
	protected float stopStressingValue;

	public float stressModificationValue;

	public float roomStressModificationValue;

	private RelaxationPoint.RelaxationPointSM.Instance smi;

	private static Effect stressReductionEffect;

	private static Effect roomStressReductionEffect;

	public class RelaxationPointSM : GameStateMachine<RelaxationPoint.RelaxationPointSM, RelaxationPoint.RelaxationPointSM.Instance, RelaxationPoint>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.unoperational;
			this.unoperational.EventTransition(GameHashes.OperationalChanged, this.operational, (RelaxationPoint.RelaxationPointSM.Instance smi) => smi.GetComponent<Operational>().IsOperational).PlayAnim("off");
			this.operational.ToggleChore((RelaxationPoint.RelaxationPointSM.Instance smi) => smi.master.CreateWorkChore(), this.unoperational);
		}

		public GameStateMachine<RelaxationPoint.RelaxationPointSM, RelaxationPoint.RelaxationPointSM.Instance, RelaxationPoint, object>.State unoperational;

		public GameStateMachine<RelaxationPoint.RelaxationPointSM, RelaxationPoint.RelaxationPointSM.Instance, RelaxationPoint, object>.State operational;

		public new class Instance : GameStateMachine<RelaxationPoint.RelaxationPointSM, RelaxationPoint.RelaxationPointSM.Instance, RelaxationPoint, object>.GameInstance
		{
			public Instance(RelaxationPoint master)
				: base(master)
			{
			}
		}
	}
}
