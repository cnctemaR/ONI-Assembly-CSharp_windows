using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class AirFilter : StateMachineComponent<AirFilter.StatesInstance>, IGameObjectEffectDescriptor
{
	public bool HasFilter()
	{
		return this.elementConverter.HasEnoughMass(this.filterTag, false);
	}

	public bool IsConvertable()
	{
		return this.elementConverter.HasEnoughMassToStartConverting(false);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	public List<Descriptor> GetDescriptors(GameObject go)
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
			this.waiting.EventTransition(GameHashes.OnStorageChange, this.hasFilter, (AirFilter.StatesInstance smi) => smi.master.HasFilter() && smi.master.operational.IsOperational).EventTransition(GameHashes.OperationalChanged, this.hasFilter, (AirFilter.StatesInstance smi) => smi.master.HasFilter() && smi.master.operational.IsOperational);
			this.hasFilter.EventTransition(GameHashes.OperationalChanged, this.waiting, (AirFilter.StatesInstance smi) => !smi.master.operational.IsOperational).Enter("EnableConsumption", delegate(AirFilter.StatesInstance smi)
			{
				smi.master.elementConsumer.EnableConsumption(true);
			}).Exit("DisableConsumption", delegate(AirFilter.StatesInstance smi)
			{
				smi.master.elementConsumer.EnableConsumption(false);
			})
				.DefaultState(this.hasFilter.idle);
			this.hasFilter.idle.EventTransition(GameHashes.OnStorageChange, this.hasFilter.converting, (AirFilter.StatesInstance smi) => smi.master.IsConvertable());
			this.hasFilter.converting.Enter("SetActive(true)", delegate(AirFilter.StatesInstance smi)
			{
				smi.master.operational.SetActive(true, false);
			}).Exit("SetActive(false)", delegate(AirFilter.StatesInstance smi)
			{
				smi.master.operational.SetActive(false, false);
			}).EventTransition(GameHashes.OnStorageChange, this.hasFilter.idle, (AirFilter.StatesInstance smi) => !smi.master.IsConvertable());
		}

		public AirFilter.States.ReadyStates hasFilter;

		public GameStateMachine<AirFilter.States, AirFilter.StatesInstance, AirFilter, object>.State waiting;

		public class ReadyStates : GameStateMachine<AirFilter.States, AirFilter.StatesInstance, AirFilter, object>.State
		{
			public GameStateMachine<AirFilter.States, AirFilter.StatesInstance, AirFilter, object>.State idle;

			public GameStateMachine<AirFilter.States, AirFilter.StatesInstance, AirFilter, object>.State converting;
		}
	}
}
