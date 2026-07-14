using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class UnstableEntombDefense : GameStateMachine<UnstableEntombDefense, UnstableEntombDefense.Instance, IStateMachineTarget, UnstableEntombDefense.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.disabled;
		this.disabled.EventTransition(GameHashes.Died, this.dead, null).ParamTransition<bool>(this.Active, this.active, GameStateMachine<UnstableEntombDefense, UnstableEntombDefense.Instance, IStateMachineTarget, UnstableEntombDefense.Def>.IsTrue);
		this.active.EventTransition(GameHashes.Died, this.dead, null).ParamTransition<bool>(this.Active, this.disabled, GameStateMachine<UnstableEntombDefense, UnstableEntombDefense.Instance, IStateMachineTarget, UnstableEntombDefense.Def>.IsFalse).DefaultState(this.active.safe);
		this.active.safe.DefaultState(this.active.safe.idle);
		this.active.safe.idle.ParamTransition<float>(this.TimeBeforeNextReaction, this.active.threatened, (UnstableEntombDefense.Instance smi, float p) => GameStateMachine<UnstableEntombDefense, UnstableEntombDefense.Instance, IStateMachineTarget, UnstableEntombDefense.Def>.IsGTZero(smi, p) && UnstableEntombDefense.IsEntombedByUnstable(smi)).EventTransition(GameHashes.EntombedChanged, this.active.safe.newThreat, new StateMachine<UnstableEntombDefense, UnstableEntombDefense.Instance, IStateMachineTarget, UnstableEntombDefense.Def>.Transition.ConditionCallback(UnstableEntombDefense.IsEntombedByUnstable));
		this.active.safe.newThreat.Enter(new StateMachine<UnstableEntombDefense, UnstableEntombDefense.Instance, IStateMachineTarget, UnstableEntombDefense.Def>.State.Callback(UnstableEntombDefense.ResetCooldown)).GoTo(this.active.threatened);
		this.active.threatened.EventTransition(GameHashes.Died, this.dead, null).Exit(new StateMachine<UnstableEntombDefense, UnstableEntombDefense.Instance, IStateMachineTarget, UnstableEntombDefense.Def>.State.Callback(UnstableEntombDefense.ResetCooldown)).EventTransition(GameHashes.EntombedChanged, this.active.safe, GameStateMachine<UnstableEntombDefense, UnstableEntombDefense.Instance, IStateMachineTarget, UnstableEntombDefense.Def>.Not(new StateMachine<UnstableEntombDefense, UnstableEntombDefense.Instance, IStateMachineTarget, UnstableEntombDefense.Def>.Transition.ConditionCallback(UnstableEntombDefense.IsEntombedByUnstable)))
			.DefaultState(this.active.threatened.inCooldown);
		this.active.threatened.inCooldown.ParamTransition<float>(this.TimeBeforeNextReaction, this.active.threatened.react, GameStateMachine<UnstableEntombDefense, UnstableEntombDefense.Instance, IStateMachineTarget, UnstableEntombDefense.Def>.IsLTEZero).Update(new Action<UnstableEntombDefense.Instance, float>(UnstableEntombDefense.CooldownTick), UpdateRate.SIM_200ms, false);
		this.active.threatened.react.TriggerOnEnter(GameHashes.EntombDefenseReactionBegins, null).PlayAnim((UnstableEntombDefense.Instance smi) => smi.UnentombAnimName, KAnim.PlayMode.Once).OnAnimQueueComplete(this.active.threatened.complete)
			.ScheduleGoTo(2f, this.active.threatened.complete);
		this.active.threatened.complete.TriggerOnEnter(GameHashes.EntombDefenseReact, null).Enter(new StateMachine<UnstableEntombDefense, UnstableEntombDefense.Instance, IStateMachineTarget, UnstableEntombDefense.Def>.State.Callback(UnstableEntombDefense.AttemptToBreakFree)).Enter(new StateMachine<UnstableEntombDefense, UnstableEntombDefense.Instance, IStateMachineTarget, UnstableEntombDefense.Def>.State.Callback(UnstableEntombDefense.ResetCooldown))
			.GoTo(this.active.threatened.inCooldown);
		this.dead.DoNothing();
	}

	public static void ResetCooldown(UnstableEntombDefense.Instance smi)
	{
		smi.sm.TimeBeforeNextReaction.Set(smi.def.Cooldown, smi, false);
	}

	public static bool IsEntombedByUnstable(UnstableEntombDefense.Instance smi)
	{
		return smi.IsEntombed && smi.IsInPressenceOfUnstableSolids();
	}

	public static void AttemptToBreakFree(UnstableEntombDefense.Instance smi)
	{
		smi.AttackUnstableCells();
	}

	public static void CooldownTick(UnstableEntombDefense.Instance smi, float dt)
	{
		float num = smi.RemainingCooldown - dt;
		smi.sm.TimeBeforeNextReaction.Set(num, smi, false);
	}

	public UnstableEntombDefense.ActiveState active;

	public GameStateMachine<UnstableEntombDefense, UnstableEntombDefense.Instance, IStateMachineTarget, UnstableEntombDefense.Def>.State disabled;

	public GameStateMachine<UnstableEntombDefense, UnstableEntombDefense.Instance, IStateMachineTarget, UnstableEntombDefense.Def>.State dead;

	public StateMachine<UnstableEntombDefense, UnstableEntombDefense.Instance, IStateMachineTarget, UnstableEntombDefense.Def>.FloatParameter TimeBeforeNextReaction;

	public StateMachine<UnstableEntombDefense, UnstableEntombDefense.Instance, IStateMachineTarget, UnstableEntombDefense.Def>.BoolParameter Active = new StateMachine<UnstableEntombDefense, UnstableEntombDefense.Instance, IStateMachineTarget, UnstableEntombDefense.Def>.BoolParameter(true);

	public class Def : StateMachine.BaseDef, IGameObjectEffectDescriptor
	{
		public List<Descriptor> GetDescriptors(GameObject go)
		{
			List<Descriptor> list = new List<Descriptor>();
			UnstableEntombDefense.Instance smi = go.GetSMI<UnstableEntombDefense.Instance>();
			if (smi != null)
			{
				Descriptor stateDescriptor = smi.GetStateDescriptor();
				if (stateDescriptor.type == Descriptor.DescriptorType.Effect)
				{
					list.Add(stateDescriptor);
				}
			}
			return list;
		}

		public float Cooldown = 5f;

		public string defaultAnimName = "";
	}

	public class SafeStates : GameStateMachine<UnstableEntombDefense, UnstableEntombDefense.Instance, IStateMachineTarget, UnstableEntombDefense.Def>.State
	{
		public GameStateMachine<UnstableEntombDefense, UnstableEntombDefense.Instance, IStateMachineTarget, UnstableEntombDefense.Def>.State idle;

		public GameStateMachine<UnstableEntombDefense, UnstableEntombDefense.Instance, IStateMachineTarget, UnstableEntombDefense.Def>.State newThreat;
	}

	public class ThreatenedStates : GameStateMachine<UnstableEntombDefense, UnstableEntombDefense.Instance, IStateMachineTarget, UnstableEntombDefense.Def>.State
	{
		public GameStateMachine<UnstableEntombDefense, UnstableEntombDefense.Instance, IStateMachineTarget, UnstableEntombDefense.Def>.State inCooldown;

		public GameStateMachine<UnstableEntombDefense, UnstableEntombDefense.Instance, IStateMachineTarget, UnstableEntombDefense.Def>.State react;

		public GameStateMachine<UnstableEntombDefense, UnstableEntombDefense.Instance, IStateMachineTarget, UnstableEntombDefense.Def>.State complete;
	}

	public class ActiveState : GameStateMachine<UnstableEntombDefense, UnstableEntombDefense.Instance, IStateMachineTarget, UnstableEntombDefense.Def>.State
	{
		public UnstableEntombDefense.SafeStates safe;

		public UnstableEntombDefense.ThreatenedStates threatened;
	}

	public new class Instance : GameStateMachine<UnstableEntombDefense, UnstableEntombDefense.Instance, IStateMachineTarget, UnstableEntombDefense.Def>.GameInstance
	{
		public float RemainingCooldown
		{
			get
			{
				return base.sm.TimeBeforeNextReaction.Get(this);
			}
		}

		public bool IsEntombed
		{
			get
			{
				return this.entombVulnerable.GetEntombed;
			}
		}

		public bool IsActive
		{
			get
			{
				return base.sm.Active.Get(this);
			}
		}

		public Instance(IStateMachineTarget master, UnstableEntombDefense.Def def)
			: base(master, def)
		{
			this.UnentombAnimName = ((this.UnentombAnimName == null) ? def.defaultAnimName : this.UnentombAnimName);
		}

		public bool IsInPressenceOfUnstableSolids()
		{
			int num = Grid.PosToCell(this);
			CellOffset[] occupiedCellsOffsets = this.occupyArea.OccupiedCellsOffsets;
			for (int i = 0; i < occupiedCellsOffsets.Length; i++)
			{
				int num2 = Grid.OffsetCell(num, occupiedCellsOffsets[i]);
				if (Grid.IsValidCell(num2) && Grid.Solid[num2] && Grid.Element[num2].IsUnstable)
				{
					return true;
				}
			}
			return false;
		}

		public void AttackUnstableCells()
		{
			int num = Grid.PosToCell(this);
			CellOffset[] occupiedCellsOffsets = this.occupyArea.OccupiedCellsOffsets;
			for (int i = 0; i < occupiedCellsOffsets.Length; i++)
			{
				int num2 = Grid.OffsetCell(num, occupiedCellsOffsets[i]);
				if (Grid.IsValidCell(num2) && Grid.Solid[num2] && Grid.Element[num2].IsUnstable)
				{
					SimMessages.Dig(num2, -1, false, false);
				}
			}
		}

		public void SetActive(bool active)
		{
			base.sm.Active.Set(active, this, false);
		}

		public Descriptor GetStateDescriptor()
		{
			if (base.IsInsideState(base.sm.disabled))
			{
				return new Descriptor(UI.BUILDINGEFFECTS.UNSTABLEENTOMBDEFENSEOFF, UI.BUILDINGEFFECTS.TOOLTIPS.UNSTABLEENTOMBDEFENSEOFF, Descriptor.DescriptorType.Effect, false);
			}
			if (base.IsInsideState(base.sm.active.safe))
			{
				return new Descriptor(UI.BUILDINGEFFECTS.UNSTABLEENTOMBDEFENSEREADY, UI.BUILDINGEFFECTS.TOOLTIPS.UNSTABLEENTOMBDEFENSEREADY, Descriptor.DescriptorType.Effect, false);
			}
			if (base.IsInsideState(base.sm.active.threatened.inCooldown))
			{
				return new Descriptor(UI.BUILDINGEFFECTS.UNSTABLEENTOMBDEFENSETHREATENED, UI.BUILDINGEFFECTS.TOOLTIPS.UNSTABLEENTOMBDEFENSETHREATENED, Descriptor.DescriptorType.Effect, false);
			}
			if (base.IsInsideState(base.sm.active.threatened.react))
			{
				return new Descriptor(UI.BUILDINGEFFECTS.UNSTABLEENTOMBDEFENSEREACTING, UI.BUILDINGEFFECTS.TOOLTIPS.UNSTABLEENTOMBDEFENSEREACTING, Descriptor.DescriptorType.Effect, false);
			}
			return new Descriptor
			{
				type = Descriptor.DescriptorType.Detail
			};
		}

		public string UnentombAnimName;

		[MyCmpGet]
		private EntombVulnerable entombVulnerable;

		[MyCmpGet]
		private OccupyArea occupyArea;
	}
}
