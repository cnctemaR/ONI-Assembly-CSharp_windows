using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;

public class RelaxationPoint : Workable, IEffectDescriptor
{
	public RelaxationPoint()
	{
		this.showProgressBar = false;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.GetComponent<KPrefabID>().AddTag(TagManager.Create("RelaxationPoint", MISC.TAGS.RELAXATION_POINT));
		this.stressReductionEffect = this.CreateEffect();
	}

	public Effect CreateEffect()
	{
		Effect effect = new Effect("StressReduction", DUPLICANTS.RELAXATION.RELAXATION_EFFECT.NAME, DUPLICANTS.RELAXATION.RELAXATION_EFFECT.DESCRIPTION, 0f, true, false, false);
		AttributeModifier attributeModifier = new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, this.stressModificationValue / 600f, DUPLICANTS.RELAXATION.RELAXATION_EFFECT.NAME, false, false);
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
		worker.GetComponent<Effects>().Add(this.stressReductionEffect, false);
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		AmountInstance amountInstance = Db.Get().Amounts.Stress.Lookup(worker.gameObject);
		if (amountInstance.value <= this.stopStressingValue)
		{
			return true;
		}
		base.OnWorkTick(worker, dt);
		return false;
	}

	protected override void OnStopWork(Worker worker)
	{
		worker.GetComponent<Effects>().Remove(this.stressReductionEffect);
		base.OnStopWork(worker);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
	}

	protected virtual WorkChore<RelaxationPoint> CreateWorkChore()
	{
		return new WorkChore<RelaxationPoint>(Db.Get().ChoreTypes.Relax, this, null, false, null, null, null, false, null, true, default(Tag), null, false, true, true, int.MaxValue);
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.STRESSREDUCEDPERMINUTE, GameUtil.GetFormattedPercent(this.stressModificationValue / 600f * 60f, GameUtil.TimeSlice.None)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.STRESSREDUCEDPERMINUTE, GameUtil.GetFormattedPercent(this.stressModificationValue / 600f * 60f, GameUtil.TimeSlice.None)), Descriptor.DescriptorType.Effect);
		list.Add(descriptor);
		return list;
	}

	[Serialize]
	protected float stopStressingValue;

	public float stressModificationValue;

	private RelaxationPoint.RelaxationPointSM.Instance smi;

	private Effect stressReductionEffect;

	public class RelaxationPointSM : GameStateMachine<RelaxationPoint.RelaxationPointSM, RelaxationPoint.RelaxationPointSM.Instance, RelaxationPoint>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.unoperational;
			this.unoperational.EventTransition(GameHashes.OperationalChanged, this.operational, (RelaxationPoint.RelaxationPointSM.Instance smi) => smi.GetComponent<Operational>().IsOperational).PlayAnim("off", KAnim.PlayMode.Once, null);
			this.operational.DefaultState(this.operational.idle).ToggleChore((RelaxationPoint.RelaxationPointSM.Instance smi) => smi.master.CreateWorkChore(), this.unoperational);
			this.operational.idle.WorkableStartTransition((RelaxationPoint.RelaxationPointSM.Instance smi) => smi.master, this.operational.healing);
			this.operational.healing.WorkableStopTransition((RelaxationPoint.RelaxationPointSM.Instance smi) => smi.master, this.operational.exiting).EventTransition(GameHashes.OperationalChanged, this.operational.exiting, (RelaxationPoint.RelaxationPointSM.Instance smi) => !smi.GetComponent<Operational>().IsOperational).Enter(delegate(RelaxationPoint.RelaxationPointSM.Instance smi)
			{
				if (!smi.master.GetComponent<Operational>().IsOperational)
				{
					smi.GoTo(this.operational.exiting);
				}
				else
				{
					smi.master.gameObject.GetComponent<Operational>().SetActive(true, false);
					smi.Queue("working_pre", KAnim.PlayMode.Once);
					smi.Queue("working_loop", KAnim.PlayMode.Loop);
					smi.master.GetComponent<Operational>().SetActive(true, false);
				}
			});
			this.operational.exiting.PlayAnim("working_pst", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.unoperational).Enter(delegate(RelaxationPoint.RelaxationPointSM.Instance smi)
			{
				smi.master.gameObject.GetComponent<Operational>().SetActive(false, false);
			});
		}

		public GameStateMachine<RelaxationPoint.RelaxationPointSM, RelaxationPoint.RelaxationPointSM.Instance, RelaxationPoint, object>.State unoperational;

		public RelaxationPoint.RelaxationPointSM.OperationalStates operational;

		public class OperationalStates : GameStateMachine<RelaxationPoint.RelaxationPointSM, RelaxationPoint.RelaxationPointSM.Instance, RelaxationPoint, object>.State
		{
			public GameStateMachine<RelaxationPoint.RelaxationPointSM, RelaxationPoint.RelaxationPointSM.Instance, RelaxationPoint, object>.State idle;

			public GameStateMachine<RelaxationPoint.RelaxationPointSM, RelaxationPoint.RelaxationPointSM.Instance, RelaxationPoint, object>.State healing;

			public GameStateMachine<RelaxationPoint.RelaxationPointSM, RelaxationPoint.RelaxationPointSM.Instance, RelaxationPoint, object>.State exiting;
		}

		public new class Instance : GameStateMachine<RelaxationPoint.RelaxationPointSM, RelaxationPoint.RelaxationPointSM.Instance, RelaxationPoint, object>.GameInstance
		{
			public Instance(RelaxationPoint master)
				: base(master)
			{
			}
		}
	}
}
