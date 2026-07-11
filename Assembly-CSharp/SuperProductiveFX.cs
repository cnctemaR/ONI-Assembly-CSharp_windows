using System;
using UnityEngine;

public class SuperProductiveFX : GameStateMachine<SuperProductiveFX, SuperProductiveFX.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.pre;
		base.Target(this.fx);
		this.root.OnSignal(this.wasProductive, this.productive, (SuperProductiveFX.Instance smi) => smi.GetCurrentState() != smi.sm.pst).OnSignal(this.destroyFX, this.pst);
		this.pre.PlayAnim("productive_pre", KAnim.PlayMode.Once).OnAnimQueueComplete(this.idle);
		this.idle.PlayAnim("productive_loop", KAnim.PlayMode.Loop);
		this.productive.QueueAnim("productive_achievement", false, null).OnAnimQueueComplete(this.idle);
		this.pst.PlayAnim("productive_pst").EventHandler(GameHashes.AnimQueueComplete, delegate(SuperProductiveFX.Instance smi)
		{
			smi.DestroyFX();
		});
	}

	public StateMachine<SuperProductiveFX, SuperProductiveFX.Instance, IStateMachineTarget, object>.Signal wasProductive;

	public StateMachine<SuperProductiveFX, SuperProductiveFX.Instance, IStateMachineTarget, object>.Signal destroyFX;

	public StateMachine<SuperProductiveFX, SuperProductiveFX.Instance, IStateMachineTarget, object>.TargetParameter fx;

	public GameStateMachine<SuperProductiveFX, SuperProductiveFX.Instance, IStateMachineTarget, object>.State pre;

	public GameStateMachine<SuperProductiveFX, SuperProductiveFX.Instance, IStateMachineTarget, object>.State idle;

	public GameStateMachine<SuperProductiveFX, SuperProductiveFX.Instance, IStateMachineTarget, object>.State productive;

	public GameStateMachine<SuperProductiveFX, SuperProductiveFX.Instance, IStateMachineTarget, object>.State pst;

	public new class Instance : GameStateMachine<SuperProductiveFX, SuperProductiveFX.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master, Vector3 offset)
			: base(master)
		{
			KBatchedAnimController kbatchedAnimController = FXHelpers.CreateEffect("productive_fx_kanim", master.gameObject.transform.GetPosition() + offset, master.gameObject.transform, true, Grid.SceneLayer.Front, false);
			base.sm.fx.Set(kbatchedAnimController.gameObject, base.smi);
		}

		public void DestroyFX()
		{
			Util.KDestroyGameObject(base.sm.fx.Get(base.smi));
			base.smi.StopSM("destroyed");
		}
	}
}
