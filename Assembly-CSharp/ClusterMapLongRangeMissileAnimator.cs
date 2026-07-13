using System;
using UnityEngine;

public class ClusterMapLongRangeMissileAnimator : GameStateMachine<ClusterMapLongRangeMissileAnimator, ClusterMapLongRangeMissileAnimator.StatesInstance, ClusterMapVisualizer>
{
	public override void InitializeStates(out StateMachine.BaseState defaultState)
	{
		defaultState = this.moving;
		this.root.OnTargetLost(this.entityTarget, null).Target(this.entityTarget).TagTransition(GameTags.LongRangeMissileMoving, this.moving, false)
			.TagTransition(GameTags.LongRangeMissileIdle, this.idle, false)
			.TagTransition(GameTags.LongRangeMissileExploding, this.exploding, false);
		this.moving.Enter(delegate(ClusterMapLongRangeMissileAnimator.StatesInstance smi)
		{
			smi.PlayVisAnim("inflight_loop", KAnim.PlayMode.Loop);
		});
		this.idle.Enter(delegate(ClusterMapLongRangeMissileAnimator.StatesInstance smi)
		{
			smi.PlayVisAnim("idle_loop", KAnim.PlayMode.Loop);
		});
		this.exploding.DefaultState(this.exploding.pre);
		this.exploding.pre.ScheduleGoTo(10f, this.exploding.animating).EventTransition(GameHashes.ClusterMapTravelAnimatorMoveComplete, this.exploding.animating, null);
		this.exploding.animating.Enter(delegate(ClusterMapLongRangeMissileAnimator.StatesInstance smi)
		{
			smi.PlayVisAnim("explode", KAnim.PlayMode.Once);
			smi.SubscribeOnVisAnimComplete(delegate(object _)
			{
				smi.GoTo(this.exploding.post);
			});
		});
		this.exploding.post.Enter(delegate(ClusterMapLongRangeMissileAnimator.StatesInstance smi)
		{
			if (smi.entity != null)
			{
				smi.entity.Trigger(-1311384361, null);
			}
			smi.GoTo(null);
		});
	}

	public StateMachine<ClusterMapLongRangeMissileAnimator, ClusterMapLongRangeMissileAnimator.StatesInstance, ClusterMapVisualizer, object>.TargetParameter entityTarget;

	public GameStateMachine<ClusterMapLongRangeMissileAnimator, ClusterMapLongRangeMissileAnimator.StatesInstance, ClusterMapVisualizer, object>.State moving;

	public GameStateMachine<ClusterMapLongRangeMissileAnimator, ClusterMapLongRangeMissileAnimator.StatesInstance, ClusterMapVisualizer, object>.State idle;

	public ClusterMapLongRangeMissileAnimator.ExplodingStates exploding;

	public class ExplodingStates : GameStateMachine<ClusterMapLongRangeMissileAnimator, ClusterMapLongRangeMissileAnimator.StatesInstance, ClusterMapVisualizer, object>.State
	{
		public GameStateMachine<ClusterMapLongRangeMissileAnimator, ClusterMapLongRangeMissileAnimator.StatesInstance, ClusterMapVisualizer, object>.State pre;

		public GameStateMachine<ClusterMapLongRangeMissileAnimator, ClusterMapLongRangeMissileAnimator.StatesInstance, ClusterMapVisualizer, object>.State animating;

		public GameStateMachine<ClusterMapLongRangeMissileAnimator, ClusterMapLongRangeMissileAnimator.StatesInstance, ClusterMapVisualizer, object>.State post;
	}

	public class StatesInstance : GameStateMachine<ClusterMapLongRangeMissileAnimator, ClusterMapLongRangeMissileAnimator.StatesInstance, ClusterMapVisualizer, object>.GameInstance
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
