using System;
using UnityEngine;

public class FliesFX : GameStateMachine<FliesFX, FliesFX.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		base.Target(this.fx);
		this.root.PlayAnim("swarm_pre", KAnim.PlayMode.Once, null).QueueAnim("swarm_loop", true, null).Exit("DestroyFX", delegate(FliesFX.Instance smi)
		{
			smi.DestroyFX();
		});
	}

	public StateMachine<FliesFX, FliesFX.Instance, IStateMachineTarget>.TargetParameter fx;

	public new class Instance : GameStateMachine<FliesFX, FliesFX.Instance, IStateMachineTarget>.GameInstance
	{
		public Instance(IStateMachineTarget master, Vector3 offset)
			: base(master)
		{
			KBatchedAnimController kbatchedAnimController = FXHelpers.CreateEffect("fly_swarm", null, false, Grid.SceneLayer.Front);
			kbatchedAnimController.transform.parent = base.smi.master.transform;
			kbatchedAnimController.transform.localPosition = offset;
			base.sm.fx.Set(kbatchedAnimController.gameObject, base.smi);
		}

		public void DestroyFX()
		{
			global::UnityEngine.Object.Destroy(base.sm.fx.Get(base.smi));
		}
	}
}
