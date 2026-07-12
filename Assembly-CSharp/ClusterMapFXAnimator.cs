using System;

public class ClusterMapFXAnimator : GameStateMachine<ClusterMapFXAnimator, ClusterMapFXAnimator.StatesInstance, ClusterMapVisualizer>
{
	public override void InitializeStates(out StateMachine.BaseState defaultState)
	{
		defaultState = this.play;
		this.play.OnSignal(this.onAnimComplete, this.finished);
		this.finished.Enter(delegate(ClusterMapFXAnimator.StatesInstance smi)
		{
			smi.DestroyEntity();
		});
	}

	private KBatchedAnimController animController;

	public StateMachine<ClusterMapFXAnimator, ClusterMapFXAnimator.StatesInstance, ClusterMapVisualizer, object>.TargetParameter entityTarget;

	public GameStateMachine<ClusterMapFXAnimator, ClusterMapFXAnimator.StatesInstance, ClusterMapVisualizer, object>.State play;

	public GameStateMachine<ClusterMapFXAnimator, ClusterMapFXAnimator.StatesInstance, ClusterMapVisualizer, object>.State finished;

	public StateMachine<ClusterMapFXAnimator, ClusterMapFXAnimator.StatesInstance, ClusterMapVisualizer, object>.Signal onAnimComplete;

	public class StatesInstance : GameStateMachine<ClusterMapFXAnimator, ClusterMapFXAnimator.StatesInstance, ClusterMapVisualizer, object>.GameInstance
	{
		public StatesInstance(ClusterMapVisualizer visualizer, ClusterGridEntity entity)
			: base(visualizer)
		{
			base.sm.entityTarget.Set(entity, this);
			visualizer.GetFirstAnimController().gameObject.Subscribe(-1061186183, new Action<object>(this.OnAnimQueueComplete));
		}

		private void OnAnimQueueComplete(object data)
		{
			base.sm.onAnimComplete.Trigger(this);
		}

		public void DestroyEntity()
		{
			base.sm.entityTarget.Get(this).DeleteObject();
		}
	}
}
