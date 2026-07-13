using System;
using UnityEngine;

public class AliveEntityPoker : GameStateMachine<AliveEntityPoker, AliveEntityPoker.Instance, IStateMachineTarget, AliveEntityPoker.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.Never;
		default_state = this.approach;
		this.root.Enter(new StateMachine<AliveEntityPoker, AliveEntityPoker.Instance, IStateMachineTarget, AliveEntityPoker.Def>.State.Callback(AliveEntityPoker.RefreshTarget)).TagTransition(AliveEntityPoker.BehaviourTag, null, true);
		this.approach.InitializeStates(this.poker, this.victim, (AliveEntityPoker.Instance smi) => smi.VictimPokeOffsets, this.poke, this.failed, null).ToggleMainStatusItem(new Func<AliveEntityPoker.Instance, StatusItem>(AliveEntityPoker.GetGoingToPokeStatusItem), null);
		this.poke.ToggleAnims((AliveEntityPoker.Instance smi) => smi.def.PokeAnimFileName).OnTargetLost(this.victim, null).DefaultState(this.poke.pre)
			.ToggleMainStatusItem(new Func<AliveEntityPoker.Instance, StatusItem>(AliveEntityPoker.GetPokingStatusItem), null);
		this.poke.pre.PlayAnim((AliveEntityPoker.Instance smi) => smi.def.PokeAnim_Pre, KAnim.PlayMode.Once).OnAnimQueueComplete(this.poke.loop);
		this.poke.loop.PlayAnim((AliveEntityPoker.Instance smi) => smi.def.PokeAnim_Loop, KAnim.PlayMode.Once).OnAnimQueueComplete(this.poke.pst);
		this.poke.pst.PlayAnim((AliveEntityPoker.Instance smi) => smi.def.PokeAnim_Pst, KAnim.PlayMode.Once).OnAnimQueueComplete(this.complete);
		this.complete.TriggerOnEnter(GameHashes.EntityPoked, (AliveEntityPoker.Instance smi) => smi.CurrentVictim).BehaviourComplete(AliveEntityPoker.BehaviourTag, false);
		this.failed.Target(this.poker).TriggerOnEnter(GameHashes.TargetLost, null).EnterGoTo(null);
	}

	public static StatusItem GetGoingToPokeStatusItem(AliveEntityPoker.Instance smi)
	{
		return AliveEntityPoker.GetStatusItem(smi, smi.def.statusItemSTR_goingToPoke);
	}

	public static StatusItem GetPokingStatusItem(AliveEntityPoker.Instance smi)
	{
		return AliveEntityPoker.GetStatusItem(smi, smi.def.statusItemSTR_poking);
	}

	private static StatusItem GetStatusItem(AliveEntityPoker.Instance smi, string address)
	{
		string text = Strings.Get(address + ".NAME");
		string text2 = Strings.Get(address + ".TOOLTIP");
		return new StatusItem(smi.GetCurrentState().longName, text, text2, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, true, null);
	}

	public static void ClearPreviousVictim(AliveEntityPoker.Instance smi)
	{
		smi.sm.victim.Set(null, smi);
	}

	public static void RefreshTarget(AliveEntityPoker.Instance smi)
	{
		PokeMonitor.Instance smi2 = smi.GetSMI<PokeMonitor.Instance>();
		smi.sm.victim.Set(smi2.Target, smi, false);
		smi.VictimPokeOffsets = smi2.TargetOffsets;
	}

	public static readonly Tag BehaviourTag = GameTags.Creatures.UrgeToPoke;

	public GameStateMachine<AliveEntityPoker, AliveEntityPoker.Instance, IStateMachineTarget, AliveEntityPoker.Def>.ApproachSubState<Pickupable> approach;

	public AliveEntityPoker.PokeStates poke;

	public GameStateMachine<AliveEntityPoker, AliveEntityPoker.Instance, IStateMachineTarget, AliveEntityPoker.Def>.State complete;

	public GameStateMachine<AliveEntityPoker, AliveEntityPoker.Instance, IStateMachineTarget, AliveEntityPoker.Def>.State failed;

	public StateMachine<AliveEntityPoker, AliveEntityPoker.Instance, IStateMachineTarget, AliveEntityPoker.Def>.TargetParameter poker;

	public StateMachine<AliveEntityPoker, AliveEntityPoker.Instance, IStateMachineTarget, AliveEntityPoker.Def>.TargetParameter victim;

	public class Def : StateMachine.BaseDef
	{
		public string PokeAnimFileName;

		public string PokeAnim_Pre;

		public string PokeAnim_Loop;

		public string PokeAnim_Pst;

		public string statusItemSTR_goingToPoke;

		public string statusItemSTR_poking;
	}

	public class PokeStates : GameStateMachine<AliveEntityPoker, AliveEntityPoker.Instance, IStateMachineTarget, AliveEntityPoker.Def>.State
	{
		public GameStateMachine<AliveEntityPoker, AliveEntityPoker.Instance, IStateMachineTarget, AliveEntityPoker.Def>.State pre;

		public GameStateMachine<AliveEntityPoker, AliveEntityPoker.Instance, IStateMachineTarget, AliveEntityPoker.Def>.State loop;

		public GameStateMachine<AliveEntityPoker, AliveEntityPoker.Instance, IStateMachineTarget, AliveEntityPoker.Def>.State pst;
	}

	public new class Instance : GameStateMachine<AliveEntityPoker, AliveEntityPoker.Instance, IStateMachineTarget, AliveEntityPoker.Def>.GameInstance
	{
		public GameObject CurrentVictim
		{
			get
			{
				return base.sm.victim.Get(this);
			}
		}

		public Instance(Chore<AliveEntityPoker.Instance> chore, AliveEntityPoker.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.UrgeToPoke);
			base.sm.poker.Set(base.smi.gameObject, base.smi, false);
		}

		public CellOffset[] VictimPokeOffsets;
	}
}
