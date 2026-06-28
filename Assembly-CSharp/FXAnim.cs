using System;
using UnityEngine;

public class FXAnim : GameStateMachine<FXAnim, FXAnim.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.loop;
		base.Target(this.fx);
		this.loop.Enter(delegate(FXAnim.Instance smi)
		{
			smi.Enter();
		}).EventTransition(GameHashes.AnimQueueComplete, this.loop, null).Exit("Post", delegate(FXAnim.Instance smi)
		{
			smi.Exit();
		});
	}

	public StateMachine<FXAnim, FXAnim.Instance, IStateMachineTarget, object>.TargetParameter fx;

	public GameStateMachine<FXAnim, FXAnim.Instance, IStateMachineTarget, object>.State loop;

	public new class Instance : GameStateMachine<FXAnim, FXAnim.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master, string kanim_file, string anim, KAnim.PlayMode mode, Vector3 offset, Color32 tint_colour)
			: base(master)
		{
			this.controller = FXHelpers.CreateEffect(kanim_file, base.smi.master.transform.position + offset, base.smi.master.transform, false, Grid.SceneLayer.Front);
			this.controller.gameObject.Subscribe(-1061186183, new Action<object>(this.OnAnimQueueComplete));
			this.controller.TintColour = tint_colour;
			base.sm.fx.Set(this.controller.gameObject, base.smi);
			this.anim = anim;
			this.mode = mode;
		}

		public void Enter()
		{
			this.controller.Play(this.anim, this.mode, 1f, 0f);
		}

		public void Exit()
		{
			this.DestroyFX();
		}

		private void OnAnimQueueComplete(object data)
		{
			this.DestroyFX();
		}

		private void DestroyFX()
		{
			global::UnityEngine.Object.Destroy(base.sm.fx.Get(base.smi));
		}

		private KAnimControllerBase controller;

		private string anim;

		private KAnim.PlayMode mode;
	}
}
