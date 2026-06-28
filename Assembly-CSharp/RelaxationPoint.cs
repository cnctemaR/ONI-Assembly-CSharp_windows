using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class RelaxationPoint : BuildingWorkable, IAssignable, IEffectDescriptor
{
	public RelaxationPoint()
	{
		this.showProgressBar = false;
	}

	public Assignable Assignable
	{
		get
		{
			return this.assignable;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.GetComponent<KPrefabID>().AddTag(TagManager.Create("RelaxationPoint", TAGS.RELAXATION_POINT));
		this.stressReductionEffect = new Effect("StressReduction", DUPLICANTS.RELAXATION.RELAXATION_EFFECT.NAME, DUPLICANTS.RELAXATION.RELAXATION_EFFECT.DESCRIPTION, 0f, true, false, false);
		AttributeModifier attributeModifier = new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, this.stressModificationValue / 600f, DUPLICANTS.RELAXATION.RELAXATION_EFFECT.NAME, false);
		this.stressReductionEffect.Add(attributeModifier);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.smi = new RelaxationPoint.RelaxationPointSM.Instance(this);
		this.smi.StartSM();
		base.SetWorkTime(float.PositiveInfinity);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
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
		this.assignable.Unassign();
		base.OnCompleteWork(worker);
	}

	private void OnRegionChanged(Region new_region)
	{
		GameUtil.UpdateRegion(new_region, this, Db.Get().OwnableSlots.RelaxationPoint, global::TUNING.REGIONS.RecreationRegionTag);
	}

	public int DescriptionOrder { get; set; }

	public List<Descriptor> GetRequirementDescriptions(BuildingDef def)
	{
		return null;
	}

	public List<Descriptor> GetEffectDescriptions(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(string.Format(string.Format(UI.LISTENTRYSTRINGNOLINEBREAK, UI.BUILDINGEFFECTS.STRESSREDUCEDPERMINUTE), GameUtil.GetFormattedPercent(this.stressModificationValue / 600f * 60f, GameUtil.TimeSlice.None)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.STRESSREDUCEDPERMINUTE, GameUtil.GetFormattedPercent(this.stressModificationValue / 600f * 60f, GameUtil.TimeSlice.None)));
		list.Add(descriptor);
		return list;
	}

	virtual GameObject IAssignable.get_gameObject()
	{
		return base.gameObject;
	}

	[MyCmpGet]
	private Assignable assignable;

	public float stopStressingValue;

	public float stressModificationValue;

	private RelaxationPoint.RelaxationPointSM.Instance smi;

	private Effect stressReductionEffect;

	public class RelaxationPointSM : GameStateMachine<RelaxationPoint.RelaxationPointSM, RelaxationPoint.RelaxationPointSM.Instance>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.unoperational;
			this.unoperational.EventTransition(GameHashes.OperationalChanged, this.operational, (RelaxationPoint.RelaxationPointSM.Instance smi) => smi.GetComponent<Operational>().IsOperational);
			this.operational.DefaultState(this.operational.idle).ToggleChore((RelaxationPoint.RelaxationPointSM.Instance smi) => new WorkChore<RelaxationPoint>(Db.Get().ChoreTypes.Relax, smi.master, null, false, null, null, null, false, null, true, global::TUNING.REGIONS.RecreationRegionTag, null, false, true), this.unoperational, false);
			this.operational.idle.EventTransition(GameHashes.WorkStarted, this.operational.healing, null);
			this.operational.healing.EventTransition(GameHashes.AssigneeChanged, this.operational.exiting, null).EventTransition(GameHashes.WorkStopped, this.operational.exiting, null).EventTransition(GameHashes.OperationalChanged, this.operational.exiting, (RelaxationPoint.RelaxationPointSM.Instance smi) => !smi.GetComponent<Operational>().IsOperational)
				.Enter(delegate(RelaxationPoint.RelaxationPointSM.Instance smi)
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

		public GameStateMachine<RelaxationPoint.RelaxationPointSM, RelaxationPoint.RelaxationPointSM.Instance, IStateMachineTarget>.State unoperational;

		public RelaxationPoint.RelaxationPointSM.OperationalStates operational;

		public class OperationalStates : GameStateMachine<RelaxationPoint.RelaxationPointSM, RelaxationPoint.RelaxationPointSM.Instance, IStateMachineTarget>.State
		{
			public GameStateMachine<RelaxationPoint.RelaxationPointSM, RelaxationPoint.RelaxationPointSM.Instance, IStateMachineTarget>.State idle;

			public GameStateMachine<RelaxationPoint.RelaxationPointSM, RelaxationPoint.RelaxationPointSM.Instance, IStateMachineTarget>.State healing;

			public GameStateMachine<RelaxationPoint.RelaxationPointSM, RelaxationPoint.RelaxationPointSM.Instance, IStateMachineTarget>.State exiting;
		}

		public new class Instance : GameStateMachine<RelaxationPoint.RelaxationPointSM, RelaxationPoint.RelaxationPointSM.Instance, IStateMachineTarget>.GameInstance
		{
			public Instance(IStateMachineTarget master)
				: base(master)
			{
			}
		}
	}
}
