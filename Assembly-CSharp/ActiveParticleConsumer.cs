using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class ActiveParticleConsumer : GameStateMachine<ActiveParticleConsumer, ActiveParticleConsumer.Instance, IStateMachineTarget, ActiveParticleConsumer.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.inoperational;
		this.root.Enter(delegate(ActiveParticleConsumer.Instance smi)
		{
			smi.GetComponent<Operational>().SetFlag(ActiveParticleConsumer.canConsumeParticlesFlag, false);
		});
		this.inoperational.EventTransition(GameHashes.OnParticleStorageChanged, this.operational, new StateMachine<ActiveParticleConsumer, ActiveParticleConsumer.Instance, IStateMachineTarget, ActiveParticleConsumer.Def>.Transition.ConditionCallback(this.IsReady)).ToggleStatusItem(Db.Get().BuildingStatusItems.WaitingForHighEnergyParticles, null);
		this.operational.DefaultState(this.operational.waiting).EventTransition(GameHashes.OnParticleStorageChanged, this.inoperational, GameStateMachine<ActiveParticleConsumer, ActiveParticleConsumer.Instance, IStateMachineTarget, ActiveParticleConsumer.Def>.Not(new StateMachine<ActiveParticleConsumer, ActiveParticleConsumer.Instance, IStateMachineTarget, ActiveParticleConsumer.Def>.Transition.ConditionCallback(this.IsReady))).ToggleOperationalFlag(ActiveParticleConsumer.canConsumeParticlesFlag);
		this.operational.waiting.EventTransition(GameHashes.ActiveChanged, this.operational.consuming, (ActiveParticleConsumer.Instance smi) => smi.GetComponent<Operational>().IsActive);
		this.operational.consuming.EventTransition(GameHashes.ActiveChanged, this.operational.waiting, (ActiveParticleConsumer.Instance smi) => !smi.GetComponent<Operational>().IsActive).Update(delegate(ActiveParticleConsumer.Instance smi, float dt)
		{
			smi.Update(dt);
		}, UpdateRate.SIM_1000ms, false);
	}

	public bool IsReady(ActiveParticleConsumer.Instance smi)
	{
		return smi.storage.Particles >= smi.def.minParticlesForOperational;
	}

	public static readonly Operational.Flag canConsumeParticlesFlag = new Operational.Flag("canConsumeParticles", Operational.Flag.Type.Requirement);

	public GameStateMachine<ActiveParticleConsumer, ActiveParticleConsumer.Instance, IStateMachineTarget, ActiveParticleConsumer.Def>.State inoperational;

	public ActiveParticleConsumer.OperationalStates operational;

	public class Def : StateMachine.BaseDef, IGameObjectEffectDescriptor
	{
		public List<Descriptor> GetDescriptors(GameObject go)
		{
			return new List<Descriptor>
			{
				new Descriptor(UI.BUILDINGEFFECTS.ACTIVE_PARTICLE_CONSUMPTION.Replace("{Rate}", GameUtil.GetFormattedHighEnergyParticles(this.activeConsumptionRate, GameUtil.TimeSlice.PerSecond, true)), UI.BUILDINGEFFECTS.TOOLTIPS.ACTIVE_PARTICLE_CONSUMPTION.Replace("{Rate}", GameUtil.GetFormattedHighEnergyParticles(this.activeConsumptionRate, GameUtil.TimeSlice.PerSecond, true)), Descriptor.DescriptorType.Requirement, false)
			};
		}

		public float activeConsumptionRate = 1f;

		public float minParticlesForOperational = 1f;

		public string meterSymbolName;
	}

	public class OperationalStates : GameStateMachine<ActiveParticleConsumer, ActiveParticleConsumer.Instance, IStateMachineTarget, ActiveParticleConsumer.Def>.State
	{
		public GameStateMachine<ActiveParticleConsumer, ActiveParticleConsumer.Instance, IStateMachineTarget, ActiveParticleConsumer.Def>.State waiting;

		public GameStateMachine<ActiveParticleConsumer, ActiveParticleConsumer.Instance, IStateMachineTarget, ActiveParticleConsumer.Def>.State consuming;
	}

	public new class Instance : GameStateMachine<ActiveParticleConsumer, ActiveParticleConsumer.Instance, IStateMachineTarget, ActiveParticleConsumer.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, ActiveParticleConsumer.Def def)
			: base(master, def)
		{
			this.storage = master.GetComponent<HighEnergyParticleStorage>();
		}

		public void Update(float dt)
		{
			this.storage.ConsumeAndGet(dt * base.def.activeConsumptionRate);
		}

		public bool ShowWorkingStatus;

		public HighEnergyParticleStorage storage;
	}
}
