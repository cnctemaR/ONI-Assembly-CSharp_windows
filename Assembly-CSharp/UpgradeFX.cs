using System;
using UnityEngine;

public class UpgradeFX : GameStateMachine<UpgradeFX, UpgradeFX.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		base.Target(this.fx);
		this.root.PlayAnim("upgrade", KAnim.PlayMode.Once, null).OnAnimQueueComplete(null).Exit("DestroyFX", delegate(UpgradeFX.Instance smi)
		{
			smi.DestroyFX();
		});
	}

	public StateMachine<UpgradeFX, UpgradeFX.Instance, IStateMachineTarget>.TargetParameter fx;

	public new class Instance : GameStateMachine<UpgradeFX, UpgradeFX.Instance, IStateMachineTarget>.GameInstance
	{
		public Instance(IStateMachineTarget master, Vector3 offset)
			: base(master)
		{
			KBatchedAnimController kbatchedAnimController = FXHelpers.CreateEffect("upgrade_fx", null, false, Grid.SceneLayer.Front);
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
