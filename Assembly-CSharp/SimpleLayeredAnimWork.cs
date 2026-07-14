using System;
using UnityEngine;

public class SimpleLayeredAnimWork : GameStateMachine<SimpleLayeredAnimWork, SimpleLayeredAnimWork.Instance, WorkerBase>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.work;
		base.Target(this.worker);
		this.work.Exit(new StateMachine<SimpleLayeredAnimWork, SimpleLayeredAnimWork.Instance, WorkerBase, object>.State.Callback(SimpleLayeredAnimWork.ClearForegroundLayer)).ToggleAnims(new Func<SimpleLayeredAnimWork.Instance, KAnimFile[]>(SimpleLayeredAnimWork.GetAnimOverrides)).Enter(new StateMachine<SimpleLayeredAnimWork, SimpleLayeredAnimWork.Instance, WorkerBase, object>.State.Callback(SimpleLayeredAnimWork.SetForegroundLayer))
			.DefaultState(this.work.pre);
		this.work.pre.EventTransition(GameHashes.WorkerPlayPostAnim, this.work.pst, null).PlayAnim("working_pre", KAnim.PlayMode.Once).Enter(delegate(SimpleLayeredAnimWork.Instance smi)
		{
			SimpleLayeredAnimWork.PlayAnimsOnWorkProvider(smi, "working_pre", KAnim.PlayMode.Once);
		})
			.OnAnimQueueComplete(this.work.loop);
		this.work.loop.EventTransition(GameHashes.WorkerPlayPostAnim, this.work.pst, null).PlayAnim("working_loop", KAnim.PlayMode.Loop).Enter(delegate(SimpleLayeredAnimWork.Instance smi)
		{
			SimpleLayeredAnimWork.PlayAnimsOnWorkProvider(smi, "working_loop", KAnim.PlayMode.Loop);
		});
		this.work.pst.PlayAnim("working_pst", KAnim.PlayMode.Once).Enter(delegate(SimpleLayeredAnimWork.Instance smi)
		{
			SimpleLayeredAnimWork.PlayAnimsOnWorkProvider(smi, "working_pst", KAnim.PlayMode.Once);
		}).OnAnimQueueComplete(this.complete);
		this.complete.GoTo(null);
	}

	private static void SetForegroundLayer(SimpleLayeredAnimWork.Instance smi)
	{
		smi.SetForegroundLayer();
	}

	private static void ClearForegroundLayer(SimpleLayeredAnimWork.Instance smi)
	{
		smi.ClearForegroundLayer();
	}

	private static void PlayAnimsOnWorkProvider(SimpleLayeredAnimWork.Instance smi, string animName, KAnim.PlayMode playMode)
	{
		smi.PlayAnimOnWorkProvider(animName, playMode);
	}

	private static KAnimFile[] GetAnimOverrides(SimpleLayeredAnimWork.Instance smi)
	{
		return smi.anims;
	}

	private const string ANIM_NAME_PRE = "working_pre";

	private const string ANIM_NAME_LOOP = "working_loop";

	private const string ANIM_NAME_PST = "working_pst";

	public GameStateMachine<SimpleLayeredAnimWork, SimpleLayeredAnimWork.Instance, WorkerBase, object>.PreLoopPostState work;

	public GameStateMachine<SimpleLayeredAnimWork, SimpleLayeredAnimWork.Instance, WorkerBase, object>.State complete;

	public StateMachine<SimpleLayeredAnimWork, SimpleLayeredAnimWork.Instance, WorkerBase, object>.TargetParameter workProvider;

	public StateMachine<SimpleLayeredAnimWork, SimpleLayeredAnimWork.Instance, WorkerBase, object>.TargetParameter worker;

	public new class Instance : GameStateMachine<SimpleLayeredAnimWork, SimpleLayeredAnimWork.Instance, WorkerBase, object>.GameInstance
	{
		public GameObject WorkProvider
		{
			get
			{
				return base.sm.workProvider.Get(this);
			}
		}

		public Instance(GameObject workProvider, WorkerBase master, Grid.SceneLayer sceneLayer, bool synchAnims, KAnimFile[] overrideAnims)
			: base(master)
		{
			base.sm.workProvider.Set(workProvider, base.smi, false);
			base.sm.worker.Set(master, base.smi);
			this.sceneLayer = sceneLayer;
			this.anims = overrideAnims;
			this.SynchAnims = synchAnims;
			this.animController = base.GetComponent<KBatchedAnimController>();
		}

		public void SetForegroundLayer()
		{
			this.animController.SetFGLayer(this.sceneLayer);
			this.animController.GetLayering().HideSymbols();
		}

		public void ClearForegroundLayer()
		{
			this.animController.SetFGLayer(Grid.SceneLayer.NoLayer);
			this.animController.GetLayering().HideSymbols();
		}

		public void PlayAnimOnWorkProvider(string animName, KAnim.PlayMode playmode)
		{
			if (this.SynchAnims && this.WorkProvider != null)
			{
				this.WorkProvider.GetComponent<KBatchedAnimController>().Play(animName, playmode, 1f, 0f);
			}
		}

		public readonly bool SynchAnims;

		public KAnimFile[] anims;

		public Grid.SceneLayer sceneLayer;

		private KBatchedAnimController animController;
	}
}
