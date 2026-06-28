using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class SolidConduitOutbox : StateMachineComponent<SolidConduitOutbox.SMInstance>
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.meter = new MeterController(this, Meter.Offset.Infront, new string[0]);
		base.Subscribe(-1697596308, new Action<object>(this.OnStorageChanged));
		this.UpdateMeter();
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	private void OnStorageChanged(object data)
	{
		this.UpdateMeter();
	}

	private void UpdateMeter()
	{
		float num = Mathf.Clamp01(this.storage.MassStored() / this.storage.capacityKg);
		this.meter.SetPositionPercent(num);
	}

	private void UpdateConsuming()
	{
		base.smi.sm.consuming.Set(this.consumer.IsConsuming, base.smi);
	}

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private SolidConduitConsumer consumer;

	[MyCmpAdd]
	private Storage storage;

	private MeterController meter;

	public class SMInstance : GameStateMachine<SolidConduitOutbox.States, SolidConduitOutbox.SMInstance, SolidConduitOutbox, object>.GameInstance
	{
		public SMInstance(SolidConduitOutbox master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<SolidConduitOutbox.States, SolidConduitOutbox.SMInstance, SolidConduitOutbox>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idle;
			this.root.Update("RefreshConsuming", delegate(SolidConduitOutbox.SMInstance smi, float dt)
			{
				smi.master.UpdateConsuming();
			}, UpdateRate.SIM_1000ms, false);
			this.idle.PlayAnim("on").ParamTransition<bool>(this.consuming, this.working, (SolidConduitOutbox.SMInstance smi, bool consuming) => consuming);
			this.working.PlayAnim("working_pre").QueueAnim("working_loop", true, null).ParamTransition<bool>(this.consuming, this.post, (SolidConduitOutbox.SMInstance smi, bool consuming) => !consuming);
			this.post.PlayAnim("working_pst").OnAnimQueueComplete(this.idle);
		}

		public StateMachine<SolidConduitOutbox.States, SolidConduitOutbox.SMInstance, SolidConduitOutbox, object>.BoolParameter consuming;

		public GameStateMachine<SolidConduitOutbox.States, SolidConduitOutbox.SMInstance, SolidConduitOutbox, object>.State idle;

		public GameStateMachine<SolidConduitOutbox.States, SolidConduitOutbox.SMInstance, SolidConduitOutbox, object>.State working;

		public GameStateMachine<SolidConduitOutbox.States, SolidConduitOutbox.SMInstance, SolidConduitOutbox, object>.State post;
	}
}
