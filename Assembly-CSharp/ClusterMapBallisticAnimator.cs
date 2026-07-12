using System;
using UnityEngine;

public class ClusterMapBallisticAnimator : GameStateMachine<ClusterMapBallisticAnimator, ClusterMapBallisticAnimator.StatesInstance, ClusterMapVisualizer>
{
	public override void InitializeStates(out StateMachine.BaseState defaultState)
	{
		defaultState = this.moving;
		this.root.Target(this.entityTarget).TagTransition(GameTags.BallisticEntityLaunching, this.launching, false).TagTransition(GameTags.BallisticEntityLanding, this.landing, false)
			.TagTransition(GameTags.BallisticEntityMoving, this.moving, false);
		this.moving.Enter(delegate(ClusterMapBallisticAnimator.StatesInstance smi)
		{
			smi.PlayVisAnim("inflight_loop", KAnim.PlayMode.Loop);
		});
		this.landing.Enter(delegate(ClusterMapBallisticAnimator.StatesInstance smi)
		{
			smi.PlayVisAnim("landing", KAnim.PlayMode.Loop);
		});
		this.launching.Enter(delegate(ClusterMapBallisticAnimator.StatesInstance smi)
		{
			smi.PlayVisAnim("launching", KAnim.PlayMode.Loop);
		});
	}

	public StateMachine<ClusterMapBallisticAnimator, ClusterMapBallisticAnimator.StatesInstance, ClusterMapVisualizer, object>.TargetParameter entityTarget;

	public GameStateMachine<ClusterMapBallisticAnimator, ClusterMapBallisticAnimator.StatesInstance, ClusterMapVisualizer, object>.State launching;

	public GameStateMachine<ClusterMapBallisticAnimator, ClusterMapBallisticAnimator.StatesInstance, ClusterMapVisualizer, object>.State moving;

	public GameStateMachine<ClusterMapBallisticAnimator, ClusterMapBallisticAnimator.StatesInstance, ClusterMapVisualizer, object>.State landing;

	public class StatesInstance : GameStateMachine<ClusterMapBallisticAnimator, ClusterMapBallisticAnimator.StatesInstance, ClusterMapVisualizer, object>.GameInstance
	{
		public StatesInstance(ClusterMapVisualizer master, ClusterGridEntity entity)
			: base(master)
		{
			this.entity = entity;
			base.sm.entityTarget.Set(entity, this);
		}

		public void PlayVisAnim(string animName, KAnim.PlayMode playMode)
		{
			base.GetComponent<ClusterMapVisualizer>().PlayAnim(animName, playMode);
		}

		public void ToggleVisAnim(bool on)
		{
			ClusterMapVisualizer component = base.GetComponent<ClusterMapVisualizer>();
			if (!on)
			{
				component.GetFirstAnimController().Play("grounded", KAnim.PlayMode.Once, 1f, 0f);
			}
		}

		public void SubscribeOnVisAnimComplete(Action<object> action)
		{
			ClusterMapVisualizer component = base.GetComponent<ClusterMapVisualizer>();
			this.UnsubscribeOnVisAnimComplete();
			this.animCompleteSubscriber = component.GetFirstAnimController().gameObject;
			this.animCompleteHandle = this.animCompleteSubscriber.Subscribe(-1061186183, action);
		}

		public void UnsubscribeOnVisAnimComplete()
		{
			if (this.animCompleteHandle != -1)
			{
				DebugUtil.DevAssert(this.animCompleteSubscriber != null, "ClustermapBallisticAnimator animCompleteSubscriber GameObject is null. Whatever the previous gameObject in this variable was, it may not have unsubscribed from an event properly", null);
				this.animCompleteSubscriber.Unsubscribe(this.animCompleteHandle);
				this.animCompleteHandle = -1;
			}
		}

		protected override void OnCleanUp()
		{
			base.OnCleanUp();
			this.UnsubscribeOnVisAnimComplete();
		}

		public ClusterGridEntity entity;

		private int animCompleteHandle = -1;

		private GameObject animCompleteSubscriber;
	}
}
