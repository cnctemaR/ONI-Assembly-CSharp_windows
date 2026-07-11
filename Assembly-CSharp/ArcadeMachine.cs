using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ArcadeMachine : StateMachineComponent<ArcadeMachine.StatesInstance>, IEffectDescriptor
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		GameScheduler.Instance.Schedule("Scheduling Tutorial", 2f, delegate(object obj)
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Schedule);
		}, null, null);
		this.workables = new ArcadeMachineWorkable[this.choreOffsets.Length];
		this.chores = new Chore[this.choreOffsets.Length];
		for (int i = 0; i < this.workables.Length; i++)
		{
			int num = Grid.OffsetCell(Grid.PosToCell(this), this.choreOffsets[i]);
			Vector3 vector = Grid.CellToPosCBC(num, Grid.SceneLayer.Move);
			GameObject gameObject = ChoreHelpers.CreateLocator("ArcadeMachineWorkable", vector);
			ArcadeMachineWorkable arcadeMachineWorkable = gameObject.AddOrGet<ArcadeMachineWorkable>();
			arcadeMachineWorkable.SetWorkerStatusItem(Db.Get().DuplicantStatusItems.Gaming);
			int player_index = i;
			ArcadeMachineWorkable arcadeMachineWorkable2 = arcadeMachineWorkable;
			arcadeMachineWorkable2.OnWorkableEventCB = (Action<Workable.WorkableEvent>)Delegate.Combine(arcadeMachineWorkable2.OnWorkableEventCB, new Action<Workable.WorkableEvent>(delegate(Workable.WorkableEvent ev)
			{
				this.OnWorkableEvent(player_index, ev);
			}));
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

	public void OnWorkableEvent(int player, Workable.WorkableEvent ev)
	{
		bool flag = ev == Workable.WorkableEvent.WorkStarted;
		if (flag)
		{
			this.players.Add(player);
		}
		else
		{
			this.players.Remove(player);
		}
		base.smi.sm.playerCount.Set(this.players.Count, base.smi);
	}

	List<Descriptor> IEffectDescriptor.GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(UI.BUILDINGEFFECTS.RECREATION, UI.BUILDINGEFFECTS.TOOLTIPS.RECREATION, Descriptor.DescriptorType.Effect);
		list.Add(descriptor);
		Effect.AddModifierDescriptions(base.gameObject, list, "PlayedArcade", true);
		return list;
	}

	public const string SPECIFIC_EFFECT = "PlayedArcade";

	public const string TRACKING_EFFECT = "RecentlyPlayedArcade";

	public CellOffset[] choreOffsets = new CellOffset[]
	{
		new CellOffset(-1, 0),
		new CellOffset(1, 0)
	};

	private ArcadeMachineWorkable[] workables;

	private Chore[] chores;

	public HashSet<int> players = new HashSet<int>();

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
			}).PlayAnim("working_pre").OnAnimQueueComplete(this.operational.playing);
			this.operational.playing.PlayAnim(new Func<ArcadeMachine.StatesInstance, string>(this.GetPlayingAnim), KAnim.PlayMode.Loop).ParamTransition<int>(this.playerCount, this.operational.post, (ArcadeMachine.StatesInstance smi, int p) => p == 0).ParamTransition<int>(this.playerCount, this.operational.playing_coop, (ArcadeMachine.StatesInstance smi, int p) => p > 1);
			this.operational.playing_coop.PlayAnim(new Func<ArcadeMachine.StatesInstance, string>(this.GetPlayingAnim), KAnim.PlayMode.Loop).ParamTransition<int>(this.playerCount, this.operational.post, (ArcadeMachine.StatesInstance smi, int p) => p == 0).ParamTransition<int>(this.playerCount, this.operational.playing, (ArcadeMachine.StatesInstance smi, int p) => p == 1);
			this.operational.post.PlayAnim("working_pst").OnAnimQueueComplete(this.operational.stopped);
		}

		private string GetPlayingAnim(ArcadeMachine.StatesInstance smi)
		{
			bool flag = smi.master.players.Contains(0);
			bool flag2 = smi.master.players.Contains(1);
			if (flag && !flag2)
			{
				return "working_loop_one_p";
			}
			if (flag2 && !flag)
			{
				return "working_loop_two_p";
			}
			return "working_loop_coop_p";
		}

		public StateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.IntParameter playerCount;

		public GameStateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.State unoperational;

		public ArcadeMachine.States.OperationalStates operational;

		public class OperationalStates : GameStateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.State
		{
			public GameStateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.State stopped;

			public GameStateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.State pre;

			public GameStateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.State playing;

			public GameStateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.State playing_coop;

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
