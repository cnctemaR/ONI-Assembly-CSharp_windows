using System;
using UnityEngine;

public class HotTubWorkerStateMachine : GameStateMachine<HotTubWorkerStateMachine, HotTubWorkerStateMachine.StatesInstance, Worker>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.pre_front;
		base.Target(this.worker);
		this.root.ToggleAnims("anim_interacts_hottub_kanim", 0f);
		this.pre_front.PlayAnim("working_pre_front").OnAnimQueueComplete(this.pre_back);
		this.pre_back.PlayAnim("working_pre_back").Enter(delegate(HotTubWorkerStateMachine.StatesInstance smi)
		{
			Vector3 position = smi.transform.GetPosition();
			position.z = Grid.GetLayerZ(Grid.SceneLayer.BuildingUse);
			smi.transform.SetPosition(position);
		}).OnAnimQueueComplete(this.loop);
		this.loop.PlayAnim((HotTubWorkerStateMachine.StatesInstance smi) => HotTubWorkerStateMachine.workAnimLoopVariants[global::UnityEngine.Random.Range(0, HotTubWorkerStateMachine.workAnimLoopVariants.Length)], KAnim.PlayMode.Once).OnAnimQueueComplete(this.loop_reenter).EventTransition(GameHashes.WorkerPlayPostAnim, this.pst_back, (HotTubWorkerStateMachine.StatesInstance smi) => smi.GetComponent<Worker>().state == Worker.State.PendingCompletion);
		this.loop_reenter.GoTo(this.loop).EventTransition(GameHashes.WorkerPlayPostAnim, this.pst_back, (HotTubWorkerStateMachine.StatesInstance smi) => smi.GetComponent<Worker>().state == Worker.State.PendingCompletion);
		this.pst_back.PlayAnim("working_pst_back").OnAnimQueueComplete(this.pst_front);
		this.pst_front.PlayAnim("working_pst_front").Enter(delegate(HotTubWorkerStateMachine.StatesInstance smi)
		{
			Vector3 position2 = smi.transform.GetPosition();
			position2.z = Grid.GetLayerZ(Grid.SceneLayer.Move);
			smi.transform.SetPosition(position2);
		}).OnAnimQueueComplete(this.complete);
	}

	private GameStateMachine<HotTubWorkerStateMachine, HotTubWorkerStateMachine.StatesInstance, Worker, object>.State pre_front;

	private GameStateMachine<HotTubWorkerStateMachine, HotTubWorkerStateMachine.StatesInstance, Worker, object>.State pre_back;

	private GameStateMachine<HotTubWorkerStateMachine, HotTubWorkerStateMachine.StatesInstance, Worker, object>.State loop;

	private GameStateMachine<HotTubWorkerStateMachine, HotTubWorkerStateMachine.StatesInstance, Worker, object>.State loop_reenter;

	private GameStateMachine<HotTubWorkerStateMachine, HotTubWorkerStateMachine.StatesInstance, Worker, object>.State pst_back;

	private GameStateMachine<HotTubWorkerStateMachine, HotTubWorkerStateMachine.StatesInstance, Worker, object>.State pst_front;

	private GameStateMachine<HotTubWorkerStateMachine, HotTubWorkerStateMachine.StatesInstance, Worker, object>.State complete;

	public StateMachine<HotTubWorkerStateMachine, HotTubWorkerStateMachine.StatesInstance, Worker, object>.TargetParameter worker;

	public static string[] workAnimLoopVariants = new string[] { "working_loop1", "working_loop2", "working_loop3" };

	public class StatesInstance : GameStateMachine<HotTubWorkerStateMachine, HotTubWorkerStateMachine.StatesInstance, Worker, object>.GameInstance
	{
		public StatesInstance(Worker master)
			: base(master)
		{
			base.sm.worker.Set(master, base.smi);
		}
	}
}
