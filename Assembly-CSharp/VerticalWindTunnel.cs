using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class VerticalWindTunnel : StateMachineComponent<VerticalWindTunnel.StatesInstance>, IEffectDescriptor
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		ElementConsumer[] components = base.GetComponents<ElementConsumer>();
		this.bottomConsumer = components[0];
		this.bottomConsumer.EnableConsumption(false);
		this.bottomConsumer.OnElementConsumed += delegate(Sim.ConsumedMassInfo info)
		{
			this.OnElementConsumed(false, info);
		};
		this.topConsumer = components[1];
		this.topConsumer.EnableConsumption(false);
		this.topConsumer.OnElementConsumed += delegate(Sim.ConsumedMassInfo info)
		{
			this.OnElementConsumed(true, info);
		};
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GameScheduler.Instance.Schedule("Scheduling Tutorial", 2f, delegate(object obj)
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Schedule, true);
		}, null, null);
		this.workables = new VerticalWindTunnelWorkable[this.choreOffsets.Length];
		this.chores = new Chore[this.choreOffsets.Length];
		for (int i = 0; i < this.workables.Length; i++)
		{
			int num = Grid.OffsetCell(Grid.PosToCell(this), this.choreOffsets[i]);
			Vector3 vector = Grid.CellToPosCBC(num, Grid.SceneLayer.Move);
			GameObject gameObject = ChoreHelpers.CreateLocator("VerticalWindTunnelWorkable", vector);
			KSelectable kselectable = gameObject.AddOrGet<KSelectable>();
			kselectable.SetName(this.GetProperName());
			kselectable.IsSelectable = false;
			VerticalWindTunnelWorkable verticalWindTunnelWorkable = gameObject.AddOrGet<VerticalWindTunnelWorkable>();
			int player_index = i;
			VerticalWindTunnelWorkable verticalWindTunnelWorkable2 = verticalWindTunnelWorkable;
			verticalWindTunnelWorkable2.OnWorkableEventCB = (Action<Workable.WorkableEvent>)Delegate.Combine(verticalWindTunnelWorkable2.OnWorkableEventCB, new Action<Workable.WorkableEvent>(delegate(Workable.WorkableEvent ev)
			{
				this.OnWorkableEvent(player_index, ev);
			}));
			verticalWindTunnelWorkable.overrideAnims = this.overrideAnims[i];
			this.workables[i] = verticalWindTunnelWorkable;
			this.workables[i].windTunnel = this;
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
		Chore chore = new WorkChore<VerticalWindTunnelWorkable>(relax, workable2, null, true, null, null, new Action<Chore>(this.OnSocialChoreEnd), false, recreation, false, true, null, false, true, false, PriorityScreen.PriorityClass.high, 5, false, true);
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

	public void SetGasWalls(bool set)
	{
		Building component = base.GetComponent<Building>();
		Sim.Cell.Properties properties = (Sim.Cell.Properties)3;
		Vector3 position = base.transform.GetPosition();
		for (int i = 0; i < component.Def.HeightInCells; i++)
		{
			int num = Grid.XYToCell(Mathf.FloorToInt(position.x) - 2, Mathf.FloorToInt(position.y) + i);
			int num2 = Grid.XYToCell(Mathf.FloorToInt(position.x) + 2, Mathf.FloorToInt(position.y) + i);
			if (set)
			{
				SimMessages.SetCellProperties(num, (byte)properties);
				SimMessages.SetCellProperties(num2, (byte)properties);
			}
			else
			{
				SimMessages.ClearCellProperties(num, (byte)properties);
				SimMessages.ClearCellProperties(num2, (byte)properties);
			}
		}
	}

	private void OnElementConsumed(bool isTop, Sim.ConsumedMassInfo info)
	{
		Building component = base.GetComponent<Building>();
		Vector3 position = base.transform.GetPosition();
		CellOffset cellOffset = ((!isTop) ? new CellOffset(0, 0) : new CellOffset(0, component.Def.HeightInCells + 1));
		int num = Grid.OffsetCell(Grid.XYToCell((int)position.x, (int)position.y), cellOffset);
		SimMessages.AddRemoveSubstance(num, (int)info.removedElemIdx, CellEventLogger.Instance.ElementEmitted, info.mass, info.temperature, info.diseaseIdx, info.diseaseCount, true, -1);
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
		Effect.AddModifierDescriptions(base.gameObject, list, this.specificEffect, true);
		return list;
	}

	public string specificEffect;

	public string trackingEffect;

	public int basePriority;

	public CellOffset[] choreOffsets = new CellOffset[]
	{
		new CellOffset(0, 0),
		new CellOffset(-1, 0),
		new CellOffset(1, 0)
	};

	private VerticalWindTunnelWorkable[] workables;

	private Chore[] chores;

	private ElementConsumer bottomConsumer;

	private ElementConsumer topConsumer;

	public HashSet<int> players = new HashSet<int>();

	public KAnimFile[][] overrideAnims = new KAnimFile[][]
	{
		new KAnimFile[] { Assets.GetAnim("anim_interacts_windtunnel_center_kanim") },
		new KAnimFile[] { Assets.GetAnim("anim_interacts_windtunnel_left_kanim") },
		new KAnimFile[] { Assets.GetAnim("anim_interacts_windtunnel_right_kanim") }
	};

	public HashedString[][] workAnims = new HashedString[][]
	{
		new HashedString[] { "weak_working_pre", "weak_working_loop" },
		new HashedString[] { "medium_working_pre", "medium_working_loop" },
		new HashedString[] { "strong_working_pre", "strong_working_loop" }
	};

	public HashedString[][] workPstAnims = new HashedString[][]
	{
		new HashedString[] { "weak_working_pst" },
		new HashedString[] { "medium_working_pst" },
		new HashedString[] { "strong_working_pst" }
	};

	public class States : GameStateMachine<VerticalWindTunnel.States, VerticalWindTunnel.StatesInstance, VerticalWindTunnel>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.unoperational;
			this.unoperational.Enter(delegate(VerticalWindTunnel.StatesInstance smi)
			{
				smi.SetActive(false);
			}).TagTransition(GameTags.Operational, this.operational, false).PlayAnim("off");
			this.operational.TagTransition(GameTags.Operational, this.unoperational, true).Enter("CreateChore", delegate(VerticalWindTunnel.StatesInstance smi)
			{
				smi.master.UpdateChores(true);
			}).Exit("CancelChore", delegate(VerticalWindTunnel.StatesInstance smi)
			{
				smi.master.UpdateChores(false);
			})
				.DefaultState(this.operational.stopped);
			this.operational.stopped.PlayAnim("off").ParamTransition<int>(this.playerCount, this.operational.pre, (VerticalWindTunnel.StatesInstance smi, int p) => p > 0);
			this.operational.pre.PlayAnim("working_pre").OnAnimQueueComplete(this.operational.playing);
			this.operational.playing.PlayAnim("working_loop", KAnim.PlayMode.Loop).Enter(delegate(VerticalWindTunnel.StatesInstance smi)
			{
				smi.SetActive(true);
			}).Exit(delegate(VerticalWindTunnel.StatesInstance smi)
			{
				smi.SetActive(false);
			})
				.ParamTransition<int>(this.playerCount, this.operational.post, (VerticalWindTunnel.StatesInstance smi, int p) => p == 0)
				.Enter("GasWalls", delegate(VerticalWindTunnel.StatesInstance smi)
				{
					smi.master.SetGasWalls(true);
				})
				.Exit("GasWalls", delegate(VerticalWindTunnel.StatesInstance smi)
				{
					smi.master.SetGasWalls(false);
				});
			this.operational.post.PlayAnim("working_pst").QueueAnim("off_pre", false, null).OnAnimQueueComplete(this.operational.stopped);
		}

		public StateMachine<VerticalWindTunnel.States, VerticalWindTunnel.StatesInstance, VerticalWindTunnel, object>.IntParameter playerCount;

		public GameStateMachine<VerticalWindTunnel.States, VerticalWindTunnel.StatesInstance, VerticalWindTunnel, object>.State unoperational;

		public VerticalWindTunnel.States.OperationalStates operational;

		public class OperationalStates : GameStateMachine<VerticalWindTunnel.States, VerticalWindTunnel.StatesInstance, VerticalWindTunnel, object>.State
		{
			public GameStateMachine<VerticalWindTunnel.States, VerticalWindTunnel.StatesInstance, VerticalWindTunnel, object>.State stopped;

			public GameStateMachine<VerticalWindTunnel.States, VerticalWindTunnel.StatesInstance, VerticalWindTunnel, object>.State pre;

			public GameStateMachine<VerticalWindTunnel.States, VerticalWindTunnel.StatesInstance, VerticalWindTunnel, object>.State playing;

			public GameStateMachine<VerticalWindTunnel.States, VerticalWindTunnel.StatesInstance, VerticalWindTunnel, object>.State post;
		}
	}

	public class StatesInstance : GameStateMachine<VerticalWindTunnel.States, VerticalWindTunnel.StatesInstance, VerticalWindTunnel, object>.GameInstance
	{
		public StatesInstance(VerticalWindTunnel smi)
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
