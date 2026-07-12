using System;
using UnityEngine;

public class GameplayEventFX : GameStateMachine<GameplayEventFX, GameplayEventFX.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		base.Target(this.fx);
		this.root.PlayAnim("event_pre").OnAnimQueueComplete(this.single).Exit("DestroyFX", delegate(GameplayEventFX.Instance smi)
		{
			smi.DestroyFX();
		});
		this.single.PlayAnim("event_loop", KAnim.PlayMode.Loop).ParamTransition<int>(this.notificationCount, this.multiple, (GameplayEventFX.Instance smi, int p) => p > 1);
		this.multiple.PlayAnim("event_loop_multiple", KAnim.PlayMode.Loop).ParamTransition<int>(this.notificationCount, this.single, (GameplayEventFX.Instance smi, int p) => p == 1);
	}

	public StateMachine<GameplayEventFX, GameplayEventFX.Instance, IStateMachineTarget, object>.TargetParameter fx;

	public StateMachine<GameplayEventFX, GameplayEventFX.Instance, IStateMachineTarget, object>.IntParameter notificationCount;

	public GameStateMachine<GameplayEventFX, GameplayEventFX.Instance, IStateMachineTarget, object>.State single;

	public GameStateMachine<GameplayEventFX, GameplayEventFX.Instance, IStateMachineTarget, object>.State multiple;

	public new class Instance : GameStateMachine<GameplayEventFX, GameplayEventFX.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master, Vector3 offset)
			: base(master)
		{
			KBatchedAnimController kbatchedAnimController = FXHelpers.CreateEffect("event_alert_fx_kanim", base.smi.master.transform.GetPosition() + offset, base.smi.master.transform, false, Grid.SceneLayer.Front, false);
			base.sm.fx.Set(kbatchedAnimController.gameObject, base.smi, false);
		}

		public void DestroyFX()
		{
			Util.KDestroyGameObject(base.sm.fx.Get(base.smi));
		}

		public int previousCount;
	}
}
