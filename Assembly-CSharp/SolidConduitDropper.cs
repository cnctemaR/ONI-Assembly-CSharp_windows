using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class SolidConduitDropper : StateMachineComponent<SolidConduitDropper.SMInstance>
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	private void Update()
	{
		base.smi.sm.consuming.Set(this.consumer.IsConsuming, base.smi);
		base.smi.sm.isclosed.Set(!this.operational.IsOperational, base.smi);
		this.storage.DropAll(false, false, default(Vector3), true, null);
	}

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private SolidConduitConsumer consumer;

	[MyCmpAdd]
	private Storage storage;

	public class SMInstance : GameStateMachine<SolidConduitDropper.States, SolidConduitDropper.SMInstance, SolidConduitDropper, object>.GameInstance
	{
		public SMInstance(SolidConduitDropper master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<SolidConduitDropper.States, SolidConduitDropper.SMInstance, SolidConduitDropper>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idle;
			this.root.Update("Update", delegate(SolidConduitDropper.SMInstance smi, float dt)
			{
				smi.master.Update();
			}, UpdateRate.SIM_1000ms, false);
			this.idle.PlayAnim("on").ParamTransition<bool>(this.consuming, this.working, GameStateMachine<SolidConduitDropper.States, SolidConduitDropper.SMInstance, SolidConduitDropper, object>.IsTrue).ParamTransition<bool>(this.isclosed, this.closed, GameStateMachine<SolidConduitDropper.States, SolidConduitDropper.SMInstance, SolidConduitDropper, object>.IsTrue);
			this.working.PlayAnim("working_pre").QueueAnim("working_loop", true, null).ParamTransition<bool>(this.consuming, this.post, GameStateMachine<SolidConduitDropper.States, SolidConduitDropper.SMInstance, SolidConduitDropper, object>.IsFalse);
			this.post.PlayAnim("working_pst").OnAnimQueueComplete(this.idle);
			this.closed.PlayAnim("closed").ParamTransition<bool>(this.consuming, this.working, GameStateMachine<SolidConduitDropper.States, SolidConduitDropper.SMInstance, SolidConduitDropper, object>.IsTrue).ParamTransition<bool>(this.isclosed, this.idle, GameStateMachine<SolidConduitDropper.States, SolidConduitDropper.SMInstance, SolidConduitDropper, object>.IsFalse);
		}

		public StateMachine<SolidConduitDropper.States, SolidConduitDropper.SMInstance, SolidConduitDropper, object>.BoolParameter consuming;

		public StateMachine<SolidConduitDropper.States, SolidConduitDropper.SMInstance, SolidConduitDropper, object>.BoolParameter isclosed;

		public GameStateMachine<SolidConduitDropper.States, SolidConduitDropper.SMInstance, SolidConduitDropper, object>.State idle;

		public GameStateMachine<SolidConduitDropper.States, SolidConduitDropper.SMInstance, SolidConduitDropper, object>.State working;

		public GameStateMachine<SolidConduitDropper.States, SolidConduitDropper.SMInstance, SolidConduitDropper, object>.State post;

		public GameStateMachine<SolidConduitDropper.States, SolidConduitDropper.SMInstance, SolidConduitDropper, object>.State closed;
	}
}
