using System;
using System.Collections.Generic;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class AirFilter : StateMachineComponent<AirFilter.StatesInstance>, IEffectDescriptor
{
	public bool IsConvertable()
	{
		Storage component = base.GetComponent<Storage>();
		ManualDeliveryKG[] components = base.GetComponents<ManualDeliveryKG>();
		foreach (ManualDeliveryKG manualDeliveryKG in components)
		{
			List<PrimaryElement> list = component.FindPrimaryElements(manualDeliveryKG.requestedItemTag);
			if (list.Count == 0)
			{
				return false;
			}
		}
		return true;
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
			this.waiting.Enter("Waiting", delegate(AirFilter.StatesInstance smi)
			{
				smi.master.operational.SetActive(false, false);
			}).EventTransition(GameHashes.OnStorageChange, this.ready, (AirFilter.StatesInstance smi) => smi.master.IsConvertable() && smi.master.operational.IsOperational).EventTransition(GameHashes.OperationalChanged, this.ready, (AirFilter.StatesInstance smi) => smi.master.IsConvertable() && smi.master.operational.IsOperational);
			this.ready.Enter("Ready", delegate(AirFilter.StatesInstance smi)
			{
				smi.master.operational.SetActive(true, false);
			}).EventTransition(GameHashes.OnStorageChange, this.waiting, (AirFilter.StatesInstance smi) => !smi.master.IsConvertable()).EventTransition(GameHashes.OperationalChanged, this.waiting, (AirFilter.StatesInstance smi) => !smi.master.operational.IsOperational);
		}

		public GameStateMachine<AirFilter.States, AirFilter.StatesInstance, AirFilter, object>.State waiting;

		public GameStateMachine<AirFilter.States, AirFilter.StatesInstance, AirFilter, object>.State ready;
	}
}
