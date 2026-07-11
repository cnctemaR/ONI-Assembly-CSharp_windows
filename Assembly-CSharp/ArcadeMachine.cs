using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ArcadeMachine : StateMachineComponent<ArcadeMachine.StatesInstance>, ISharedWorkable
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.workables = new ArcadeMachineWorkable[this.choreOffsets.Length];
		this.chores = new Chore[this.choreOffsets.Length];
		for (int i = 0; i < this.workables.Length; i++)
		{
			int num = Grid.OffsetCell(Grid.PosToCell(this), this.choreOffsets[i]);
			Vector3 vector = Grid.CellToPosCBC(num, Grid.SceneLayer.Move);
			GameObject gameObject = ChoreHelpers.CreateLocator("ArcadeMachineWorkable", vector);
			ArcadeMachineWorkable arcadeMachineWorkable = gameObject.AddOrGet<ArcadeMachineWorkable>();
			arcadeMachineWorkable.SetWorkerStatusItem(Db.Get().DuplicantStatusItems.Gaming);
			arcadeMachineWorkable.owner = this;
			arcadeMachineWorkable.overrideAnims = this.overrideAnims[i];
			arcadeMachineWorkable.workAnims = this.workAnims[i];
			this.workables[i] = arcadeMachineWorkable;
		}
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		this.UpdateChores(false);
		for (int i = 0; i < this.workables.Length; i++)
		{
			if (this.workables[i])
			{
				Util.KDestroyGameObject(this.workables[i]);
				this.workables[i] = null;
			}
		}
		base.OnCleanUp();
	}

	private Chore CreateChore(int i)
	{
		Workable workable = this.workables[i];
		ChoreType relax = Db.Get().ChoreTypes.Relax;
		Workable workable2 = workable;
		ScheduleBlockType recreation = Db.Get().ScheduleBlockTypes.Recreation;
		Chore chore = new WorkChore<ArcadeMachineWorkable>(relax, workable2, null, null, true, null, null, new Action<Chore>(this.OnSocialChoreEnd), false, recreation, false, true, null, false, true, false, PriorityScreen.PriorityClass.high, 0, false);
		chore.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, workable);
		return chore;
	}

	private void OnSocialChoreEnd(Chore chore)
	{
		if (base.gameObject.HasTag(GameTags.Operational))
		{
			this.UpdateChores(true);
		}
	}

	public void UpdateChores(bool update = true)
	{
		for (int i = 0; i < this.choreOffsets.Length; i++)
		{
			Chore chore = this.chores[i];
			if (update)
			{
				if (chore == null || chore.isComplete)
				{
					this.chores[i] = this.CreateChore(i);
				}
			}
			else if (chore != null)
			{
				chore.Cancel("locator invalidated");
				this.chores[i] = null;
			}
		}
	}

	public void AddWorker(Worker player)
	{
		this.players.Add(player);
		base.smi.sm.playerCount.Set(this.players.Count, base.smi);
	}

	public void RemoveWorker(Worker player)
	{
		this.players.Remove(player);
		base.smi.sm.playerCount.Set(this.players.Count, base.smi);
	}

	public CellOffset[] choreOffsets = new CellOffset[]
	{
		new CellOffset(-1, 0),
		new CellOffset(1, 0)
	};

	private ArcadeMachineWorkable[] workables;

	private Chore[] chores;

	private HashSet<Worker> players = new HashSet<Worker>();

	public KAnimFile[][] overrideAnims = new KAnimFile[][]
	{
		new KAnimFile[] { Assets.GetAnim("anim_interacts_arcade_cabinet_playerone_kanim") },
		new KAnimFile[] { Assets.GetAnim("anim_interacts_arcade_cabinet_playertwo_kanim") }
	};

	public HashedString[][] workAnims = new HashedString[][]
	{
		new HashedString[] { "working_pre", "working_loop_one_p" },
		new HashedString[] { "working_pre", "working_loop_two_p" }
	};

	public class States : GameStateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.unoperational;
			this.unoperational.Enter(delegate(ArcadeMachine.StatesInstance smi)
			{
				smi.SetActive(false);
			}).TagTransition(GameTags.Operational, this.operational, false).PlayAnim("off");
			this.operational.TagTransition(GameTags.Operational, this.unoperational, true).Enter("CreateChore", delegate(ArcadeMachine.StatesInstance smi)
			{
				smi.master.UpdateChores(true);
			}).Exit("CancelChore", delegate(ArcadeMachine.StatesInstance smi)
			{
				smi.master.UpdateChores(false);
			})
				.DefaultState(this.operational.stopped);
			this.operational.stopped.Enter(delegate(ArcadeMachine.StatesInstance smi)
			{
				smi.SetActive(false);
			}).PlayAnim("on").ParamTransition<int>(this.playerCount, this.operational.pre, (ArcadeMachine.StatesInstance smi, int p) => p > 0);
			this.operational.pre.Enter(delegate(ArcadeMachine.StatesInstance smi)
			{
				smi.SetActive(true);
			}).PlayAnim("working_pre").OnAnimQueueComplete(this.operational.player_transition);
			this.operational.player_transition.ParamTransition<int>(this.playerCount, this.operational.post, (ArcadeMachine.StatesInstance smi, int p) => p == 0).ParamTransition<int>(this.playerCount, this.operational.one_player, (ArcadeMachine.StatesInstance smi, int p) => p == 1).ParamTransition<int>(this.playerCount, this.operational.two_player, (ArcadeMachine.StatesInstance smi, int p) => p == 2);
			this.operational.one_player.PlayAnim("working_loop_one_p", KAnim.PlayMode.Loop).ParamTransition<int>(this.playerCount, this.operational.post, (ArcadeMachine.StatesInstance smi, int p) => p == 0).ParamTransition<int>(this.playerCount, this.operational.two_player, (ArcadeMachine.StatesInstance smi, int p) => p == 2);
			this.operational.two_player.PlayAnim("working_loop_two_p", KAnim.PlayMode.Loop).ParamTransition<int>(this.playerCount, this.operational.post, (ArcadeMachine.StatesInstance smi, int p) => p == 0).ParamTransition<int>(this.playerCount, this.operational.one_player, (ArcadeMachine.StatesInstance smi, int p) => p == 1);
			this.operational.post.PlayAnim("working_pst").OnAnimQueueComplete(this.operational.stopped);
		}

		public StateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.IntParameter playerCount;

		public GameStateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.State unoperational;

		public ArcadeMachine.States.OperationalStates operational;

		public class OperationalStates : GameStateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.State
		{
			public GameStateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.State stopped;

			public GameStateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.State pre;

			public GameStateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.State player_transition;

			public GameStateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.State one_player;

			public GameStateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.State two_player;

			public GameStateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.State post;
		}
	}

	public class StatesInstance : GameStateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.GameInstance
	{
		public StatesInstance(ArcadeMachine smi)
			: base(smi)
		{
			this.operational = base.master.GetComponent<Operational>();
		}

		public void SetActive(bool active)
		{
			this.operational.SetActive(this.operational.IsOperational && active, false);
		}

		private Operational operational;
	}
}
