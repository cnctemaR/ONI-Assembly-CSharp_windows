using System;
using UnityEngine;

public class BalloonFX : GameStateMachine<BalloonFX, BalloonFX.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		base.Target(this.fx);
		this.root.Exit("DestroyFX", delegate(BalloonFX.Instance smi)
		{
			smi.DestroyFX();
		});
	}

	public StateMachine<BalloonFX, BalloonFX.Instance, IStateMachineTarget, object>.TargetParameter fx;

	public new class Instance : GameStateMachine<BalloonFX, BalloonFX.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			KBatchedAnimController kbatchedAnimController = FXHelpers.CreateEffect("balloon_anim_kanim", master.gameObject.transform.GetPosition() + new Vector3(0f, 0.3f, 1f), master.transform, true, Grid.SceneLayer.Creatures, false);
			base.sm.fx.Set(kbatchedAnimController.gameObject, base.smi);
			master.GetComponent<KBatchedAnimController>().GetSynchronizer().Add(kbatchedAnimController.GetComponent<KBatchedAnimController>());
		}

		public void DestroyFX()
		{
			Util.KDestroyGameObject(base.sm.fx.Get(base.smi));
		}
	}
}
