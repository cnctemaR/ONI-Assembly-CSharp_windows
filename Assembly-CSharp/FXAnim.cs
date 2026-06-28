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

	public StateMachine<FXAnim, FXAnim.Instance, IStateMachineTarget>.TargetParameter fx;

	public GameStateMachine<FXAnim, FXAnim.Instance, IStateMachineTarget>.State loop;

	public new class Instance : GameStateMachine<FXAnim, FXAnim.Instance, IStateMachineTarget>.GameInstance
	{
		public Instance(IStateMachineTarget master, string kanim_file, string pre, string loop, string post, Vector3 offset)
			: base(master)
		{
			this.controller = FXHelpers.CreateEffect(kanim_file, null, false, Grid.SceneLayer.Front);
			this.controller.transform.parent = base.smi.master.transform;
			this.controller.transform.localPosition = offset;
			this.controller.gameObject.Subscribe(-1061186183, new EventSystem.EventHandler(this.OnAnimComplete));
			base.sm.fx.Set(this.controller.gameObject, base.smi);
			this.pre = pre;
			this.loop = loop;
			this.post = post;
		}

		public void Enter()
		{
			this.controller.Play(this.pre, KAnim.PlayMode.Once, 1f, 0f);
			this.controller.Queue(this.loop, KAnim.PlayMode.Loop, 1f, 0f);
		}

		public void Exit()
		{
			if (this.destroyQueued)
			{
				return;
			}
			if (this.post != null)
			{
				this.destroyQueued = true;
				this.controller.Play(this.post, KAnim.PlayMode.Once, 1f, 0f);
			}
			else
			{
				this.DestroyFX();
			}
		}

		private void OnAnimComplete(object data)
		{
			if (this.destroyQueued)
			{
				this.DestroyFX();
			}
		}

		private void DestroyFX()
		{
			global::UnityEngine.Object.Destroy(base.sm.fx.Get(base.smi));
		}

		private KAnimControllerBase controller;

		private bool destroyQueued;

		private string pre;

		private string loop;

		private string post;
	}
}
