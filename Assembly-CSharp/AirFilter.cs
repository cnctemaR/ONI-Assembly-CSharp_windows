using System;
using System.Collections.Generic;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class AirFilter : StateMachineComponent<AirFilter.StatesInstance>, IEffectDescriptor
{
	public bool HasFilter()
	{
		return this.elementConverter.HasEnoughMass(this.filterTag);
	}

	public bool IsConvertable()
	{
		return this.elementConverter.HasEnoughMassToStartConverting();
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		component.randomiseLoopedOffset = true;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		return null;
	}

	[MyCmpGet]
	private Operational operational;

	[MyCmpGet]
	private Storage storage;

	[MyCmpGet]
	private ElementConverter elementConverter;

	[MyCmpGet]
	private ElementConsumer elementConsumer;

	public Tag filterTag;

	public class StatesInstance : GameStateMachine<AirFilter.States, AirFilter.StatesInstance, AirFilter, object>.GameInstance
	{
		public StatesInstance(AirFilter smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<AirFilter.States, AirFilter.StatesInstance, AirFilter>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.waiting;
			this.waiting.EventTransition(GameHashes.OnStorageChange, this.hasfilter, (AirFilter.StatesInstance smi) => smi.master.HasFilter() && smi.master.operational.IsOperational).EventTransition(GameHashes.OperationalChanged, this.hasfilter, (AirFilter.StatesInstance smi) => smi.master.HasFilter() && smi.master.operational.IsOperational);
			this.hasfilter.EventTransition(GameHashes.OnStorageChange, this.converting, (AirFilter.StatesInstance smi) => smi.master.IsConvertable()).EventTransition(GameHashes.OperationalChanged, this.waiting, (AirFilter.StatesInstance smi) => !smi.master.operational.IsOperational).Enter("EnableConsumption", delegate(AirFilter.StatesInstance smi)
			{
				smi.master.elementConsumer.EnableConsumption(true);
			})
				.Exit("DisableConsumption", delegate(AirFilter.StatesInstance smi)
				{
					smi.master.elementConsumer.EnableConsumption(false);
				});
			this.converting.Enter("SetActive(true)", delegate(AirFilter.StatesInstance smi)
			{
				smi.master.operational.SetActive(true, false);
			}).Exit("SetActive(false)", delegate(AirFilter.StatesInstance smi)
			{
				smi.master.operational.SetActive(false, false);
			}).Enter("EnableConsumption", delegate(AirFilter.StatesInstance smi)
			{
				smi.master.elementConsumer.EnableConsumption(true);
			})
				.Exit("DisableConsumption", delegate(AirFilter.StatesInstance smi)
				{
					smi.master.elementConsumer.EnableConsumption(false);
				})
				.EventTransition(GameHashes.OnStorageChange, this.waiting, (AirFilter.StatesInstance smi) => !smi.master.IsConvertable())
				.EventTransition(GameHashes.OperationalChanged, this.waiting, (AirFilter.StatesInstance smi) => !smi.master.operational.IsOperational);
		}

		public GameStateMachine<AirFilter.States, AirFilter.StatesInstance, AirFilter, object>.State waiting;

		public GameStateMachine<AirFilter.States, AirFilter.StatesInstance, AirFilter, object>.State hasfilter;

		public GameStateMachine<AirFilter.States, AirFilter.StatesInstance, AirFilter, object>.State converting;
	}
}
