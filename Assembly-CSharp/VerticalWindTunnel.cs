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
			Vector3 vector = Grid.CellToPosCBC(Grid.OffsetCell(Grid.PosToCell(this), this.choreOffsets[i]), Grid.SceneLayer.Move);
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
			verticalWindTunnelWorkable.overrideAnim = this.overrideAnims[i];
			verticalWindTunnelWorkable.preAnims = this.workPreAnims[i];
			verticalWindTunnelWorkable.loopAnim = this.workAnims[i];
			verticalWindTunnelWorkable.pstAnims = this.workPstAnims[i];
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
		IStateMachineTarget stateMachineTarget = workable;
		ChoreProvider choreProvider = null;
		bool flag = true;
		Action<Chore> action = null;
		Action<Chore> action2 = null;
		ScheduleBlockType recreation = Db.Get().ScheduleBlockTypes.Recreation;
		WorkChore<VerticalWindTunnelWorkable> workChore = new WorkChore<VerticalWindTunnelWorkable>(relax, stateMachineTarget, choreProvider, flag, action, action2, new Action<Chore>(this.OnSocialChoreEnd), false, recreation, false, true, null, false, true, false, PriorityScreen.PriorityClass.high, 5, false, true);
		workChore.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, workable);
		return workChore;
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
		CellOffset cellOffset = (isTop ? new CellOffset(0, component.Def.HeightInCells + 1) : new CellOffset(0, 0));
		SimMessages.AddRemoveSubstance(Grid.OffsetCell(Grid.XYToCell((int)position.x, (int)position.y), cellOffset), (int)info.removedElemIdx, CellEventLogger.Instance.ElementEmitted, info.mass, info.temperature, info.diseaseIdx, info.diseaseCount, true, -1);
	}

	public void OnWorkableEvent(int player, Workable.WorkableEvent ev)
	{
		if (ev == Workable.WorkableEvent.WorkStarted)
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
		list.Add(new Descriptor(BUILDINGS.PREFABS.VERTICALWINDTUNNEL.DISPLACEMENTEFFECT.Replace("{amount}", GameUtil.GetFormattedMass(this.displacementAmount_DescriptorOnly, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), BUILDINGS.PREFABS.VERTICALWINDTUNNEL.DISPLACEMENTEFFECT_TOOLTIP.Replace("{amount}", GameUtil.GetFormattedMass(this.displacementAmount_DescriptorOnly, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), Descriptor.DescriptorType.Requirement, false));
		list.Add(new Descriptor(UI.BUILDINGEFFECTS.RECREATION, UI.BUILDINGEFFECTS.TOOLTIPS.RECREATION, Descriptor.DescriptorType.Effect, false));
		Effect.AddModifierDescriptions(base.gameObject, list, this.specificEffect, true);
		return list;
	}

	public string specificEffect;

	public string trackingEffect;

	public int basePriority;

	public float displacementAmount_DescriptorOnly;

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

	public HashedString[] overrideAnims = new HashedString[] { "anim_interacts_windtunnel_center_kanim", "anim_interacts_windtunnel_left_kanim", "anim_interacts_windtunnel_right_kanim" };

	public string[][] workPreAnims = new string[][]
	{
		new string[] { "weak_working_front_pre", "weak_working_back_pre" },
		new string[] { "medium_working_front_pre", "medium_working_back_pre" },
		new string[] { "strong_working_front_pre", "strong_working_back_pre" }
	};

	public string[] workAnims = new string[] { "weak_working_loop", "medium_working_loop", "strong_working_loop" };

	public string[][] workPstAnims = new string[][]
	{
		new string[] { "weak_working_back_pst", "weak_working_front_pst" },
		new string[] { "medium_working_back_pst", "medium_working_front_pst" },
		new string[] { "strong_working_back_pst", "strong_working_front_pst" }
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
